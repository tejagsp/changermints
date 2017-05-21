using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ChangerMints.Database;

namespace ChangerMints.Business {

    public class TerminalServices : ITerminalServices {
        private const decimal COMMISSION_TO_CHANGER = 0.025m;
        private const int MAXIMUM_TRANSACTION_LIMIT = 100;
        private const decimal TRANSFERABLE_PERCENTAGE_AFTER_COMMISION = 1 - COMMISSION_TO_CHANGER;
        private ShopkeeperSenderAndReceiverBalanceResponse _balanceResponse = null;
        private UnitOfWork _changerMintsUOW = null;
        private CrossTransactionResponse _crossTransactionResponse = null;
        private InterTransactionResponse _interTransactionResponse = null;

        public TerminalServices() {
            _changerMintsUOW = new UnitOfWork();
        }

        // Customer to Shopkeeper Transaction Service
        public CrossTransactionResponse CustomerToShopKeeperTransaction(CustomerToShopKeeperTransaction CustomerToShopkeeper) {
            _crossTransactionResponse = new CrossTransactionResponse();

            try {
                var messageCreators = new List<ModelResponses<CrossTransactionResponse, ChangerValidations>> {
                    // Check if the Shopkeeper Account is of Valid length
                    new ModelResponses<CrossTransactionResponse, ChangerValidations>(model => model.IsShopKeeperAccountValid(
                        CustomerToShopkeeper.ShopKeeperAccountNumber), 1101),
                    // Check if the Customer Account is of Valid length
                    new ModelResponses<CrossTransactionResponse, ChangerValidations>(model => model.IsCustomerNFCTagIDValid(
                        CustomerToShopkeeper.NFCTagID), 1202),
                    // Check if the Amount is of >0
                    new ModelResponses<CrossTransactionResponse, ChangerValidations>(model => model.IsAmountValid(
                        CustomerToShopkeeper.Amount), 1401),
                };

                ModelResponses<CrossTransactionResponse, ChangerValidations> errorCreator = null;
                if (!ChangerValidations.GetInputParametersStatus<CrossTransactionResponse, ChangerValidations>(messageCreators, out errorCreator))
                    return errorCreator.FillErrorDTO<CrossTransactionResponse>();

                // Get current customer details
                var currentCustomerDetails = _changerMintsUOW.Repository<CustomerDetail>().Query().Filter(q => q.NFCTagID ==
                    CustomerToShopkeeper.NFCTagID).Get().FirstOrDefault();

                // Verify if the customer is registered to the changer
                if (currentCustomerDetails == null) {
                    //Transaction Fail
                    _crossTransactionResponse.ErrorNumber = 1208;
                    _crossTransactionResponse.ErrorMessage = ConfigurationManager.AppSettings[_crossTransactionResponse.ErrorNumber.ToString()];

                    return _crossTransactionResponse;
                }

                // Verify if the password matches
                if (!(currentCustomerDetails.Password == CustomerToShopkeeper.Password)) {
                    _crossTransactionResponse.ErrorNumber = 1209;
                    _crossTransactionResponse.ErrorMessage = ConfigurationManager.AppSettings[_crossTransactionResponse.ErrorNumber.ToString()];

                    return _crossTransactionResponse;
                }

                var currentCustomerNFCDetails = _changerMintsUOW.Repository<CustomerNFCTagDetail>().Query().Filter(q => q.NFCTagID ==
                                 CustomerToShopkeeper.NFCTagID).Get().FirstOrDefault();

                if (currentCustomerNFCDetails.NFCTagUID != CustomerToShopkeeper.NFCTagUID) {
                    //Transaction Fail - NFCUID mis match
                    _crossTransactionResponse.ErrorNumber = 1210;
                    _crossTransactionResponse.ErrorMessage = ConfigurationManager.AppSettings[_crossTransactionResponse.ErrorNumber.ToString()];

                    return _crossTransactionResponse;
                }

                // Get current Customer Account Details
                var currentCustomerAccountDetails = _changerMintsUOW.Repository<CustomerAccountDetail>().Query().Filter(q => q.CustomerAccountNumber ==
                    currentCustomerDetails.CustomerAccountNumber).Get().FirstOrDefault();

                // Verify if the customer has account in our system
                if (currentCustomerAccountDetails == null) {
                    _crossTransactionResponse.ErrorNumber = 1208;
                    _crossTransactionResponse.ErrorMessage = ConfigurationManager.AppSettings[_crossTransactionResponse.ErrorNumber.ToString()];

                    return _crossTransactionResponse;
                }

                // Get current Shopkeeper Account Details
                var currentShopkeeperAccountDetails = _changerMintsUOW.Repository<ShopKeeperAccountDetail>().Query().Filter(q => q.ShopKeeperAccountNumber ==
                    CustomerToShopkeeper.ShopKeeperAccountNumber).Get().FirstOrDefault();

                // Verify if the Shopkeeper has account in our system
                if (currentShopkeeperAccountDetails == null) {
                    _crossTransactionResponse.ErrorNumber = 1107;
                    _crossTransactionResponse.ErrorMessage = ConfigurationManager.AppSettings[_crossTransactionResponse.ErrorNumber.ToString()];

                    return _crossTransactionResponse;
                }

                if (currentCustomerAccountDetails.Balance > 0 && currentCustomerAccountDetails.Balance > CustomerToShopkeeper.Amount) {
                    var transactionID = GenerateTransactionID();
                    bool IsOTPRequired = (CustomerToShopkeeper.Amount >= MAXIMUM_TRANSACTION_LIMIT) ? true : false;

                    if (IsOTPRequired) {
                        if (CustomerToShopkeeper.OTP > 0) {
                            // OTP is already sent, please validate the records & continue the transaction
                            bool validateOTPStatus = ValidateOTP(CustomerToShopkeeper);
                            if (validateOTPStatus) {
                                // Start Transaction
                                currentShopkeeperAccountDetails.ReceiverBalance += CustomerToShopkeeper.Amount;
                                currentCustomerAccountDetails.Balance -= CustomerToShopkeeper.Amount;
                                _changerMintsUOW.Repository<ShopKeeperAccountDetail>().Update(currentShopkeeperAccountDetails);
                                _changerMintsUOW.Repository<CustomerAccountDetail>().Update(currentCustomerAccountDetails);
                                _changerMintsUOW.Save();
                            } else {
                                _crossTransactionResponse.ErrorNumber = 1500;
                                _crossTransactionResponse.ErrorMessage = ConfigurationManager.AppSettings[_crossTransactionResponse.ErrorNumber.ToString()];
                                return _crossTransactionResponse;
                            }
                        } else {
                            // Generate OTP and send it as SMS to customer
                            GenerateOTP(transactionID);

                            // Send the response for Terminal
                            _crossTransactionResponse.TransactionID = transactionID;
                            return _crossTransactionResponse;
                        }
                    } else {
                        // Start Transaction
                        currentShopkeeperAccountDetails.ReceiverBalance += CustomerToShopkeeper.Amount;
                        currentCustomerAccountDetails.Balance -= CustomerToShopkeeper.Amount;
                        _changerMintsUOW.Repository<ShopKeeperAccountDetail>().Update(currentShopkeeperAccountDetails);
                        _changerMintsUOW.Repository<CustomerAccountDetail>().Update(currentCustomerAccountDetails);
                        _changerMintsUOW.Save();
                    }

                    // getting updated details after inserting values into database after transaction success
                    var getShopkeeperAccountDetails = _changerMintsUOW.Repository<ShopKeeperAccountDetail>().Query().Filter(q => q.ShopKeeperAccountNumber ==
                        CustomerToShopkeeper.ShopKeeperAccountNumber).Get().FirstOrDefault();
                    var getCustomerAccountDetails = _changerMintsUOW.Repository<CustomerAccountDetail>().Query().Filter(q => q.CustomerAccountNumber ==
                        currentCustomerDetails.CustomerAccountNumber).Get().FirstOrDefault();

                    //updating details in transaction table after transaction sucess
                    UpdateTransactionDetails<CustomerToShopKeeperTransaction>(CustomerToShopkeeper);

                    // Fill the Response DTO & send back to the controller
                    _crossTransactionResponse.ShopKeeperSenderBalance = Convert.ToDecimal(getShopkeeperAccountDetails.SenderBalance);
                    _crossTransactionResponse.ShopKeeperReceiverBalance = Convert.ToDecimal(getShopkeeperAccountDetails.ReceiverBalance);
                    _crossTransactionResponse.ErrorNumber = 1302;
                    _crossTransactionResponse.ErrorMessage = ConfigurationManager.AppSettings[_crossTransactionResponse.ErrorNumber.ToString()];
                } else {
                    _crossTransactionResponse.ErrorNumber = 1301;
                    _crossTransactionResponse.ErrorMessage = ConfigurationManager.AppSettings[_crossTransactionResponse.ErrorNumber.ToString()];

                    return _crossTransactionResponse;
                }
            } catch (Exception e) {
                throw e;
            }
            return _crossTransactionResponse;
        }

        // Generate OTP
        public void GenerateOTP(string transactionID) {
            try {
                OTP otpEntity = new OTP();
                Random r = new Random();
                var OTPNumber = r.Next(1000000, 99999999);
                var transactionExists = _changerMintsUOW.Repository<OTP>().Query().Filter(q => q.TransactionID ==
                    transactionID).Get().FirstOrDefault();
                otpEntity.ValidFrom = DateTime.Now;
                otpEntity.ValidTo = DateTime.Now.AddMinutes(3);

                if (transactionExists != null) {
                    transactionExists.OTPNumber = OTPNumber;
                    transactionExists.Hits++;
                } else {
                    otpEntity.OTPNumber = OTPNumber;
                    otpEntity.IsActive = true;
                    otpEntity.Hits = 0;
                    _changerMintsUOW.Repository<OTP>().Insert(otpEntity);
                }
                _changerMintsUOW.Save();
            } catch (Exception e) {
                throw e;
            }
        }

        // Get the Shopkeeper available balance both in Sender & Receiver
        public ShopkeeperSenderAndReceiverBalanceResponse GetShopKeeperBalance(ShopKeeperSenderAndReceiverBalance ShopkeeperBalance) {
            _balanceResponse = new ShopkeeperSenderAndReceiverBalanceResponse();

            try {
                var messageCreators = new List<ModelResponses<ShopkeeperSenderAndReceiverBalanceResponse, ChangerValidations>> {
                    // Check if the Shopkeeper Account is of Valid length
                    new ModelResponses<ShopkeeperSenderAndReceiverBalanceResponse, ChangerValidations>(model => model.IsShopKeeperAccountValid(
                        ShopkeeperBalance.ShopKeeperAccountNumber), 1101)
                };

                ModelResponses<ShopkeeperSenderAndReceiverBalanceResponse, ChangerValidations> errorCreator = null;
                if (!ChangerValidations.GetInputParametersStatus<ShopkeeperSenderAndReceiverBalanceResponse, ChangerValidations>(messageCreators, out errorCreator))
                    return errorCreator.FillErrorDTO<ShopkeeperSenderAndReceiverBalanceResponse>();

                // Get current shopkeeper account details
                var currentShopkeeperAccountDetails = _changerMintsUOW.Repository<ShopKeeperAccountDetail>().Query().Filter(q => q.ShopKeeperAccountNumber ==
                    ShopkeeperBalance.ShopKeeperAccountNumber).Get().FirstOrDefault();

                if (currentShopkeeperAccountDetails != null) {
                    _balanceResponse.ErrorNumber = 1;
                    _balanceResponse.ErrorMessage = ConfigurationManager.AppSettings[_balanceResponse.ErrorNumber.ToString()];
                    _balanceResponse.SenderBalance = currentShopkeeperAccountDetails.SenderBalance;
                    _balanceResponse.ReceiverBalance = currentShopkeeperAccountDetails.ReceiverBalance;
                } else {
                    _balanceResponse.ErrorNumber = 1050;
                    _balanceResponse.ErrorMessage = ConfigurationManager.AppSettings[_balanceResponse.ErrorNumber.ToString()];
                }
            } catch (Exception ex) {
                throw ex;
            }
            return _balanceResponse;
        }

        public CrossTransactionResponse RunTransactionWithOTP(RunTransactionWithOTP OTPTransactiondetails) {
            var transactionExists = _changerMintsUOW.Repository<OTP>().Query().Filter(q => q.TransactionID ==
                OTPTransactiondetails.TransactionID).Get().FirstOrDefault();
            if (transactionExists.OTPNumber == OTPTransactiondetails.OTP) {
            }
            return null;
        }

        // Shopkeeper to Customer Transaction Service
        public CrossTransactionResponse ShopkeeperToCustomerTransaction(ShopKeeperToCustomerTransaction ShopkeeperToCustomer) {
            _crossTransactionResponse = new CrossTransactionResponse();

            try {
                var messageCreators = new List<ModelResponses<CrossTransactionResponse, ChangerValidations>> {
                    // Check if the Shopkeeper Account is of Valid length
                    new ModelResponses<CrossTransactionResponse, ChangerValidations>(model => model.IsShopKeeperAccountValid(
                        ShopkeeperToCustomer.ShopKeeperAccountNumber), 1101),
                    // Check if the Customer Account is of Valid length
                    new ModelResponses<CrossTransactionResponse, ChangerValidations>(model => model.IsCustomerNFCTagIDValid(
                        ShopkeeperToCustomer.NFCTagID), 1202),
                    // Check if the Amount is of >0
                    new ModelResponses<CrossTransactionResponse, ChangerValidations>(model => model.IsAmountValid(
                        ShopkeeperToCustomer.Amount), 1401),
                };

                ModelResponses<CrossTransactionResponse, ChangerValidations> errorCreator = null;
                if (!ChangerValidations.GetInputParametersStatus<CrossTransactionResponse, ChangerValidations>(messageCreators, out errorCreator))
                    return errorCreator.FillErrorDTO<CrossTransactionResponse>();

                // Get current customer details
                var currentCustomerDetails = _changerMintsUOW.Repository<CustomerDetail>().Query().Filter(q => q.NFCTagID ==
                    ShopkeeperToCustomer.NFCTagID).Get().FirstOrDefault();

                // Verify if the customer is registered to the changer
                if (currentCustomerDetails == null) {
                    //Transaction Fail
                    _crossTransactionResponse.ErrorNumber = 1208;
                    _crossTransactionResponse.ErrorMessage = ConfigurationManager.AppSettings[_crossTransactionResponse.ErrorNumber.ToString()];

                    return _crossTransactionResponse;
                }

                var currentCustomerNFCDetails = _changerMintsUOW.Repository<CustomerNFCTagDetail>().Query().Filter(q => q.NFCTagID ==
                                                 ShopkeeperToCustomer.NFCTagID).Get().FirstOrDefault();

                if (currentCustomerNFCDetails.NFCTagUID != ShopkeeperToCustomer.NFCTagUID) {
                    //Transaction Fail - NFCUID mis match
                    _crossTransactionResponse.ErrorNumber = 1210;
                    _crossTransactionResponse.ErrorMessage = ConfigurationManager.AppSettings[_crossTransactionResponse.ErrorNumber.ToString()];

                    return _crossTransactionResponse;
                }

                // Retrieve particulars from the database for given Shopkeeper and Customer
                var currentShopkeeperAccountDetails = _changerMintsUOW.Repository<ShopKeeperAccountDetail>().Query().Filter(q => q.ShopKeeperAccountNumber ==
                    ShopkeeperToCustomer.ShopKeeperAccountNumber).Get().FirstOrDefault();
                var currentCustomerAccountDetails = _changerMintsUOW.Repository<CustomerAccountDetail>().Query().Filter(q => q.CustomerAccountNumber ==
                    currentCustomerDetails.CustomerAccountNumber).Get().FirstOrDefault();

                // Verify if the Shopkeeper is registered to our system.
                if (currentShopkeeperAccountDetails == null) {
                    //Transaction Fail
                    _crossTransactionResponse.ErrorNumber = 1107;
                    _crossTransactionResponse.ErrorMessage = ConfigurationManager.AppSettings[_crossTransactionResponse.ErrorNumber.ToString()];

                    return _crossTransactionResponse;
                }

                // Verify if the customer is registered to the changer
                if (currentCustomerAccountDetails == null) {
                    //Transaction Fail
                    _crossTransactionResponse.ErrorNumber = 1208;
                    _crossTransactionResponse.ErrorMessage = ConfigurationManager.AppSettings[_crossTransactionResponse.ErrorNumber.ToString()];

                    return _crossTransactionResponse;
                }

                // If the balance is greater than the transferable amount, start the transaction
                if (currentShopkeeperAccountDetails.SenderBalance > ShopkeeperToCustomer.Amount &&
                    currentShopkeeperAccountDetails.SenderBalance > 0) {
                    // Start the transaction
                    currentShopkeeperAccountDetails.SenderBalance -= ShopkeeperToCustomer.Amount;
                    currentCustomerAccountDetails.Balance += ShopkeeperToCustomer.Amount;
                    _changerMintsUOW.Repository<ShopKeeperAccountDetail>().Update(currentShopkeeperAccountDetails);
                    _changerMintsUOW.Repository<CustomerAccountDetail>().Update(currentCustomerAccountDetails);
                    _changerMintsUOW.Save();

                    // Update Shopkeeper & Customer Transaction Details
                    var getShopkeeperAccountDetails = _changerMintsUOW.Repository<ShopKeeperAccountDetail>().Query().Filter(q => q.ShopKeeperAccountNumber ==
                        ShopkeeperToCustomer.ShopKeeperAccountNumber).Get().FirstOrDefault();
                    var getCustomerAccountDetails = _changerMintsUOW.Repository<CustomerAccountDetail>().Query().Filter(q => q.CustomerAccountNumber ==
                        currentCustomerDetails.CustomerAccountNumber).Get().FirstOrDefault();

                    var query1 = from s in _changerMintsUOW.Repository<ShopKeeperAccountDetail>().Query().Filter(q => q.ShopKeeperAccountNumber ==
                                 ShopkeeperToCustomer.ShopKeeperAccountNumber).Get()
                                 select new CrossTransactionResponse {
                                     ShopKeeperSenderBalance = s.SenderBalance,
                                     ShopKeeperReceiverBalance = s.ReceiverBalance
                                 };

                    UpdateTransactionDetails<ShopKeeperToCustomerTransaction>(ShopkeeperToCustomer);

                    // Fill the Response DTO & send back to the controller
                    _crossTransactionResponse.ErrorNumber = 1302;
                    _crossTransactionResponse.ErrorMessage = ConfigurationManager.AppSettings[_crossTransactionResponse.ErrorNumber.ToString()];
                    _crossTransactionResponse.ShopKeeperSenderBalance = Convert.ToDecimal(getShopkeeperAccountDetails.SenderBalance);
                    _crossTransactionResponse.ShopKeeperReceiverBalance = Convert.ToDecimal(getShopkeeperAccountDetails.ReceiverBalance);
                } else {
                    _crossTransactionResponse.ErrorNumber = 1301;
                    _crossTransactionResponse.ErrorMessage = ConfigurationManager.AppSettings[_crossTransactionResponse.ErrorNumber.ToString()];
                }
            } catch (Exception e) {
                throw e;
            }
            return _crossTransactionResponse;
        }

        // Receiver balance to sender balance transaction service
        public InterTransactionResponse ShopKeeperToShopKeeperTransaction(ShopKeeperReceiverToSenderTransaction ShopkeeperToShopkeeper) {
            _interTransactionResponse = new InterTransactionResponse();

            try {
                var messageCreators = new List<ModelResponses<InterTransactionResponse, ChangerValidations>> {
                    // Check if the Shopkeeper Account is of Valid length
                    new ModelResponses<InterTransactionResponse, ChangerValidations>(model => model.IsShopKeeperAccountValid(
                        ShopkeeperToShopkeeper.ShopKeeperAccountNumber), 1101),
                    // Check if the Amount is of >0
                    new ModelResponses<InterTransactionResponse, ChangerValidations>(model => model.IsAmountValid(
                        ShopkeeperToShopkeeper.Amount), 1401),
                };

                ModelResponses<InterTransactionResponse, ChangerValidations> errorCreator = null;
                if (!ChangerValidations.GetInputParametersStatus<InterTransactionResponse, ChangerValidations>(messageCreators, out errorCreator))
                    return errorCreator.FillErrorDTO<InterTransactionResponse>();

                // Get current shopkeeper account details
                var currentShopkeeperAccountDetails = _changerMintsUOW.Repository<ShopKeeperAccountDetail>().Query().Filter(q => q.ShopKeeperAccountNumber ==
                    ShopkeeperToShopkeeper.ShopKeeperAccountNumber).Get().FirstOrDefault();

                if (currentShopkeeperAccountDetails == null) {
                    _interTransactionResponse.ErrorNumber = 1107;
                    _interTransactionResponse.ErrorMessage = ConfigurationManager.AppSettings[_interTransactionResponse.ErrorNumber.ToString()];

                    return _interTransactionResponse;
                }

                if (currentShopkeeperAccountDetails.ReceiverBalance > ShopkeeperToShopkeeper.Amount &&
                    currentShopkeeperAccountDetails.ReceiverBalance > 0) {
                    // Start Transaction
                    decimal TransferableAmountToShopkeeper = TRANSFERABLE_PERCENTAGE_AFTER_COMMISION * ShopkeeperToShopkeeper.Amount;
                    decimal TransferableAmountToChanger = COMMISSION_TO_CHANGER * ShopkeeperToShopkeeper.Amount;

                    currentShopkeeperAccountDetails.ReceiverBalance -= ShopkeeperToShopkeeper.Amount;
                    currentShopkeeperAccountDetails.SenderBalance += TransferableAmountToShopkeeper;
                    _changerMintsUOW.Repository<ShopKeeperAccountDetail>().Update(currentShopkeeperAccountDetails);
                    _changerMintsUOW.Save();

                    // Update the Transaction details
                    var getShopkeeperAccountDetails = _changerMintsUOW.Repository<ShopKeeperAccountDetail>().Query().Filter(q => q.ShopKeeperAccountNumber ==
                        ShopkeeperToShopkeeper.ShopKeeperAccountNumber).Get().FirstOrDefault();

                    // Update the transaction table for Shopkeeper
                    var shopKeeperTransactionDetail = new ShopKeeperTransactionDetail() {
                        ShopKeeperAccountNumber = getShopkeeperAccountDetails.ShopKeeperAccountNumber, SenderBalance = getShopkeeperAccountDetails.SenderBalance,
                        ReceiverBalance = getShopkeeperAccountDetails.ReceiverBalance, ReceivedFrom = 0, PaidTo = getShopkeeperAccountDetails.ShopKeeperAccountNumber,
                        AmountTransfered = ShopkeeperToShopkeeper.Amount, TransactionDate = DateTime.Now, TransactionStatus = 1001, TerminalIMEINumber = ShopkeeperToShopkeeper.TerminalIMEINumber,
                        TransactionID = GenerateTransactionID()
                    };
                    _changerMintsUOW.Repository<ShopKeeperTransactionDetail>().Insert(shopKeeperTransactionDetail);
                    _changerMintsUOW.Save();

                    // success message
                    _interTransactionResponse.ErrorNumber = 1302;
                    _interTransactionResponse.ErrorMessage = ConfigurationManager.AppSettings[_interTransactionResponse.ErrorNumber.ToString()];
                    _interTransactionResponse.SenderBalance = Convert.ToDecimal(currentShopkeeperAccountDetails.SenderBalance);
                    _interTransactionResponse.ReceiverBalance = Convert.ToDecimal(currentShopkeeperAccountDetails.ReceiverBalance);
                } else
                    _interTransactionResponse.ErrorNumber = 1301;
                _interTransactionResponse.ErrorMessage = ConfigurationManager.AppSettings[_interTransactionResponse.ErrorNumber.ToString()];
            } catch (Exception e) {
                throw e;
            }
            return _interTransactionResponse;
        }

        private string GenerateTransactionID() {
            var r = new Random();
            var digits = r.Next(1001, 999999);
            string transactionID = "CM" + digits.ToString();
            return transactionID;
        }

        private bool StartTransaction(string trasactionID) {
            return false;
        }

        // Update transaction table
        private void UpdateTransactionDetails<T>(T ShopkeeperToCustomer) {
            var currentShopkeeperAccountNumber = Convert.ToInt64(ShopkeeperToCustomer.GetType().GetProperty("ShopKeeperAccountNumber").GetValue(ShopkeeperToCustomer, null));
            var terminalIMEINumber = Convert.ToString(ShopkeeperToCustomer.GetType().GetProperty("IMEINumber").GetValue(ShopkeeperToCustomer, null));

            var currentCustomerNFCTagID = long.Parse(ShopkeeperToCustomer.GetType().GetProperty("NFCTagID").GetValue(ShopkeeperToCustomer, null).ToString());

            // Get current customer details
            var currentCustomerDetails = _changerMintsUOW.Repository<CustomerDetail>().Query().Filter(q => q.NFCTagID ==
                currentCustomerNFCTagID).Get().FirstOrDefault();

            var currentAmount = Convert.ToDecimal(ShopkeeperToCustomer.GetType().GetProperty("Amount").GetValue(ShopkeeperToCustomer, null));

            // Update the transaction table for Shopkeeper & Customer
            var getShopkeeperAccountDetails = _changerMintsUOW.Repository<ShopKeeperAccountDetail>().Query().Filter(q => q.ShopKeeperAccountNumber ==
                currentShopkeeperAccountNumber).Get().FirstOrDefault();
            var getCustomerAccountDetails = _changerMintsUOW.Repository<CustomerAccountDetail>().Query().Filter(q => q.CustomerAccountNumber ==
                currentCustomerDetails.CustomerAccountNumber).Get().FirstOrDefault();

            var transactionID = GenerateTransactionID();
            // Update the transaction table for Shopkeeper
            var shopKeeperTransactionDetail = new ShopKeeperTransactionDetail() {
                ShopKeeperAccountNumber = getShopkeeperAccountDetails.ShopKeeperAccountNumber, SenderBalance = getShopkeeperAccountDetails.SenderBalance,
                ReceiverBalance = getShopkeeperAccountDetails.ReceiverBalance, ReceivedFrom = 0, PaidTo = getCustomerAccountDetails.CustomerAccountNumber,
                AmountTransfered = currentAmount,
                TransactionDate = DateTime.Now,
                TransactionStatus = 1001,
                TerminalIMEINumber = terminalIMEINumber,
                TransactionID = transactionID
            };
            _changerMintsUOW.Repository<ShopKeeperTransactionDetail>().Insert(shopKeeperTransactionDetail);

            // Update the transaction table for Customer
            var customerTransactionDetail = new CustomerTransactionDetail() {
                CustomerAccountNumber = getCustomerAccountDetails.CustomerAccountNumber, Balance = getCustomerAccountDetails.Balance,
                ReceivedFrom = getShopkeeperAccountDetails.ShopKeeperAccountNumber, PaidTo = 0, AmountTransfered = currentAmount,
                TransactionDate = DateTime.Now, TransactionStatus = 1001,
                TerminalIMEINumber = terminalIMEINumber,
                TransactionID = transactionID
            };
            _changerMintsUOW.Repository<CustomerTransactionDetail>().Insert(customerTransactionDetail);

            _changerMintsUOW.Save();
        }

        // Validate the OTP
        private bool ValidateOTP(CustomerToShopKeeperTransaction CtoS) {
            // Get current customer details
            var currentCustomerDetails = _changerMintsUOW.Repository<CustomerDetail>().Query().Filter(q => q.NFCTagID ==
                CtoS.NFCTagID).Get().FirstOrDefault();

            // Get the current customer entity from OTP records
            var currentCustomerOTPDetails = _changerMintsUOW.Repository<OTP>().Query().Filter(q => q.CustomerAccountnumber ==
                 currentCustomerDetails.CustomerAccountNumber && q.OTPNumber == CtoS.OTP).Get().FirstOrDefault();

            if (currentCustomerOTPDetails == null) return false;

            // Check if the OTP duration is still valid
            bool IsOTPDurationValid = DateTime.Compare(currentCustomerOTPDetails.ValidTo, currentCustomerOTPDetails.ValidFrom) > 0 && currentCustomerOTPDetails.IsActive == true ? true : false;

            if (IsOTPDurationValid) {
                if (currentCustomerOTPDetails.Hits < int.Parse(ConfigurationManager.AppSettings["OTPExpireTime"])) {
                    // Add the hit counter to 1
                    currentCustomerOTPDetails.Hits++;
                    currentCustomerOTPDetails.IsActive = false;
                    _changerMintsUOW.Save();

                    // Check if the OTP PIN is valid
                    if (currentCustomerOTPDetails.OTPNumber == CtoS.OTP) return true;
                } else {
                    return false;
                }
            }
            return false;
        }
    }
}