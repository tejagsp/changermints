using System;
using System.Linq;

using ChangerMints.Database;

namespace ChangerMints.Business {

    public class CustomerRegistrationService : ICustomerRegistrationService {
        /*---------------------------------------------------------------------*/
        // Unit of Work
        // Response DTO for sending to controller
        /*---------------------------------------------------------------------*/
        private UnitOfWork _changerMintsUOW = null;
        private ChangerMintsError _changerMintsError = null;
        private const int BLOCKEDCUSTOMER = 20;
        private const int SEMIBLOCKEDCUSTOMER = 10;
        private const string CALLED_PHONE_NUMBER = "9019202632";

        public CustomerRegistrationService() {
            _changerMintsUOW = new UnitOfWork();
            _changerMintsError = new ChangerMintsError();
        }

        // Register customer to changer
        public CustomerNFCCardRegistrationResponse RegisterToChanger(CustomerNFCCardRegistration registrationWithNFC) {
            try {
                // If the phone number called is invalid
                if (registrationWithNFC.CalledPhoneNumber != CALLED_PHONE_NUMBER) {
                    _changerMintsError.ErrorNumber = 1200;
                    throw _changerMintsError;
                }

                // If Phone Number already registered
                var currentCustomer = _changerMintsUOW.Repository<CustomerDetail>().Query().Filter(q =>
                    q.PhoneNumber == registrationWithNFC.PhoneNumber).Get().FirstOrDefault();
                if (currentCustomer != null) {
                    _changerMintsError.ErrorNumber = 1207;
                    throw _changerMintsError;
                }

                // Check if the NFC Tag ID is valid
                if (!ChangerValidations.IsCustomerNFCTagValid(registrationWithNFC.PIN)) {
                    _changerMintsError.ErrorNumber = 1202;
                    throw _changerMintsError;
                }

                // Block the phone number if it is an invalid NFC Tag PIN
                var customerNFCDetails = _changerMintsUOW.Repository<CustomerNFCTagDetail>().Query().Filter(q =>
                    q.PIN == registrationWithNFC.PIN).Get().FirstOrDefault();
                if (customerNFCDetails == null) {
                    var blockedCustomer = _changerMintsUOW.Repository<ChangerBlockList>().Query().Filter(q =>
                                                            q.MobileNumber == registrationWithNFC.PhoneNumber).Get().FirstOrDefault();
                    if (blockedCustomer == null) {
                        var blockedCustomerDetail = new ChangerBlockList() {
                            MobileNumber = registrationWithNFC.PhoneNumber,
                            Attempts = 1,
                            Blocked = false,
                            SemiBlocked = false,
                        };
                        _changerMintsUOW.Repository<ChangerBlockList>().Insert(blockedCustomerDetail);
                    } else {
                        blockedCustomer.Attempts++;
                        if (blockedCustomer.Attempts >= BLOCKEDCUSTOMER) {
                            blockedCustomer.Blocked = true;
                            var wrongNFCEntries = new ChangerWrongNFCEntry() {
                                MobileNumber = registrationWithNFC.PhoneNumber,
                                NFC = registrationWithNFC.PIN
                            };
                        } else if (blockedCustomer.Attempts >= SEMIBLOCKEDCUSTOMER) {
                            blockedCustomer.SemiBlocked = true;
                            var wrongNFCEntries = new ChangerWrongNFCEntry() {
                                MobileNumber = registrationWithNFC.PhoneNumber,
                                NFC = registrationWithNFC.PIN
                            };
                        }
                    }
                    _changerMintsUOW.Save();

                    _changerMintsError.ErrorNumber = 1202;
                    throw _changerMintsError;
                }

                // If customer NFC status is NOTACTIVE, then NFC is already registered.
                if (customerNFCDetails.NFCStatus != 1002) {
                    _changerMintsError.ErrorNumber = 1206;
                    throw _changerMintsError;
                }

                customerNFCDetails.NFCStatus = 1001;

                // If everything succeeds, register the customer to changer
                var customerDetail = new CustomerDetail() {
                    CustomerAccountNumber = GenerateCustomerAccountNumber(registrationWithNFC.PhoneNumber),
                    NFCTagID = customerNFCDetails.NFCTagID,
                    PhoneNumber = registrationWithNFC.PhoneNumber.ToString(),
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    Password = GeneratePassword(),
                    AccountStatus = 1001
                };

                _changerMintsUOW.Repository<CustomerDetail>().Insert(customerDetail);

                //If customer registers, add his account details.
                var customerAccountDetails = new CustomerAccountDetail() {
                    CustomerAccountNumber = customerDetail.CustomerAccountNumber,
                    Balance = 0.00m
                };

                _changerMintsUOW.Repository<CustomerDetail>().Insert(customerDetail);
                _changerMintsUOW.Repository<CustomerAccountDetail>().Insert(customerAccountDetails);

                //save changes = false then it is just a response no need to save the details
                _changerMintsUOW.Save();

                var registrationResponse = (from s in _changerMintsUOW.Repository<CustomerDetail>().Query().Filter(q => q.CustomerAccountNumber == customerDetail.CustomerAccountNumber).Get()
                                            select new CustomerNFCCardRegistrationResponse {
                                                PhoneNumber = s.PhoneNumber,
                                                Accountnumber = s.CustomerAccountNumber,
                                                PIN = s.Password,
                                                ErrorNumber = 1151,
                                                ErrorMessage = System.Configuration.ConfigurationManager.AppSettings[Convert.ToString(1151)]
                                            }).FirstOrDefault();
                return registrationResponse;
            } catch (ChangerMintsError Er) {
                throw new ChangerMintsError(Er.ErrorNumber);
            } catch (Exception ex) {
                if (ex.InnerException == null) throw ex;
                else throw ex.InnerException;
            }
        }

        // Generate account number for registering the customer to changer
        private long GenerateCustomerAccountNumber(string phoneNumber) {
            var r = new Random();
            var lastFourDigits = r.Next(1001, 9999);

            var subString = phoneNumber.Substring(phoneNumber.Length - 4);
            var accountNumber = long.Parse(phoneNumber.Replace(subString, lastFourDigits.ToString()));
            // If Phone Number already registered
            while (true) {
                var customer = _changerMintsUOW.Repository<CustomerDetail>().Query().Filter(q =>
                q.CustomerAccountNumber == accountNumber).Get().FirstOrDefault();
                if (customer == null) break;
                accountNumber = long.Parse(phoneNumber.Replace(subString, lastFourDigits.ToString()));
            }
            return accountNumber;
        }

        // TODO : Implement an algorithm to generate password
        private int GeneratePassword() {
            var r = new Random();
            var fourDigits = r.Next(1001, 9999);
            return fourDigits;
        }
    }
}