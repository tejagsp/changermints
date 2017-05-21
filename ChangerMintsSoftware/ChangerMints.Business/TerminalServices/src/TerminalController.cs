using System;
using System.Configuration;

namespace ChangerMints.Business {

    /// <summary>
    ///  This class performs the terminal activities
    /// </summary>
    public class TerminalController {
        private CrossTransactionResponse _crossTransactionResponse = null;
        private CustomerToShopKeeperTransaction _customerToShopkeeper = null;
        private InterTransactionResponse _interTransactionResponse = null;
        private LogFiles _logerr;
        private ShopKeeperSenderAndReceiverBalance _shopkeeperBalance = null;
        private ShopkeeperSenderAndReceiverBalanceResponse _shopkeeperBalanceResponse = null;
        private ShopKeeperToCustomerTransaction _shopkeeperToCustomer = null;
        private ShopKeeperReceiverToSenderTransaction _shopkeeperToShopkeeper = null;
        private CrossTransactionResponse _terminalResponse = null;
        private TerminalServices _terminalService = null;
        private bool IsRequestDTOSuccess;

        public TerminalController() {
            _terminalResponse = new CrossTransactionResponse();
            _terminalService = new TerminalServices();
            _logerr = new LogFiles();
        }

        // Customer to Shopkeeper Transaction
        public string CustomerToShopkeeperTransaction(string ChangerQS) {
            _customerToShopkeeper = ChangerUtils.URLSplitter<CustomerToShopKeeperTransaction>(ChangerQS, out IsRequestDTOSuccess);
            _crossTransactionResponse = new CrossTransactionResponse();

            try {
                if (IsRequestDTOSuccess)
                    _crossTransactionResponse = _terminalService.CustomerToShopKeeperTransaction(_customerToShopkeeper);
                else {
                    _crossTransactionResponse.ErrorNumber = 1303;
                    _crossTransactionResponse.ErrorMessage = ConfigurationManager.AppSettings[_crossTransactionResponse.ErrorNumber.ToString()];
                }
            } catch (ChangerMintsError Er) {
                _logerr.LogError(Er.Message, Er, LogType.Error);
                return ChangerUtils.ResponseConcatenater<ChangerMintsError>(Er, true);
            } catch (Exception ex) {
                _crossTransactionResponse.ErrorNumber = 2001;
                _crossTransactionResponse.ErrorMessage = ConfigurationManager.AppSettings[_crossTransactionResponse.ErrorNumber.ToString()];
            }
            return ChangerUtils.ResponseConcatenater<CrossTransactionResponse>(_crossTransactionResponse, true);
        }

        // Shopkeeper Balance
        public string ShopkeeperBalance(string ChangerQS) {
            _shopkeeperBalance = ChangerUtils.URLSplitter<ShopKeeperSenderAndReceiverBalance>(ChangerQS, out IsRequestDTOSuccess);
            _shopkeeperBalanceResponse = new ShopkeeperSenderAndReceiverBalanceResponse();
            try {
                if (IsRequestDTOSuccess)
                    _shopkeeperBalanceResponse = _terminalService.GetShopKeeperBalance(_shopkeeperBalance);
                else {
                    _shopkeeperBalanceResponse.ErrorNumber = 1303;
                    _shopkeeperBalanceResponse.ErrorMessage = ConfigurationManager.AppSettings[_shopkeeperBalanceResponse.ErrorNumber.ToString()];
                }
            } catch (ChangerMintsError Er) {
                _logerr.LogError(Er.Message, Er, LogType.Error);
                return ChangerUtils.ResponseConcatenater<ChangerMintsError>(Er, true);
            } catch (Exception ex) {
                string message = string.Format("Class Name:{0},Method Name:{1},Error:{2}", this.GetType().Name, "ShopkeeperBalance", ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                _logerr.LogError(message, ex, LogType.Error);

                _shopkeeperBalanceResponse.ErrorNumber = 2001;
                _shopkeeperBalanceResponse.ErrorMessage = ConfigurationManager.AppSettings[_shopkeeperBalanceResponse.ErrorNumber.ToString()];
            }
            return ChangerUtils.ResponseConcatenater<ShopkeeperSenderAndReceiverBalanceResponse>(_shopkeeperBalanceResponse, true);
        }

        // Shopkeeper to Customer Transaction
        public string ShopkeepertoCustomerTransaction(string ChangerQS) {
            _shopkeeperToCustomer = ChangerUtils.URLSplitter<ShopKeeperToCustomerTransaction>(ChangerQS, out IsRequestDTOSuccess);
            _crossTransactionResponse = new CrossTransactionResponse();

            try {
                if (IsRequestDTOSuccess)
                    _crossTransactionResponse = _terminalService.ShopkeeperToCustomerTransaction(_shopkeeperToCustomer);
                else {
                    _crossTransactionResponse.ErrorNumber = 1303;
                    _crossTransactionResponse.ErrorMessage = ConfigurationManager.AppSettings[_crossTransactionResponse.ErrorNumber.ToString()];
                }
            } catch (ChangerMintsError Er) {
                _logerr.LogError(Er.Message, Er, LogType.Error);
                return ChangerUtils.ResponseConcatenater<ChangerMintsError>(Er, true);
            } catch (Exception ex) {
                _crossTransactionResponse.ErrorNumber = 2001;
                _crossTransactionResponse.ErrorMessage = ConfigurationManager.AppSettings[_crossTransactionResponse.ErrorNumber.ToString()];
            }
            return ChangerUtils.ResponseConcatenater<CrossTransactionResponse>(_crossTransactionResponse, true);
        }

        // Shopkeeper Receiver to Sender Transaction
        public string ShopkeeperToShopkeeperTransaction(string ChangerQS) {
            _shopkeeperToShopkeeper = ChangerUtils.URLSplitter<ShopKeeperReceiverToSenderTransaction>(ChangerQS, out IsRequestDTOSuccess);
            _interTransactionResponse = new InterTransactionResponse();

            try {
                if (IsRequestDTOSuccess)
                    _interTransactionResponse = _terminalService.ShopKeeperToShopKeeperTransaction(_shopkeeperToShopkeeper);
                else {
                    _interTransactionResponse.ErrorNumber = 1303;
                    _interTransactionResponse.ErrorMessage = ConfigurationManager.AppSettings[_interTransactionResponse.ErrorNumber.ToString()];
                }
            } catch (ChangerMintsError Er) {
                _logerr.LogError(Er.Message, Er, LogType.Error);
                return ChangerUtils.ResponseConcatenater<ChangerMintsError>(Er, true);
            } catch (Exception ex) {
                _interTransactionResponse.ErrorNumber = 2001;
                _interTransactionResponse.ErrorMessage = ConfigurationManager.AppSettings[_interTransactionResponse.ErrorNumber.ToString()];
            }
            return ChangerUtils.ResponseConcatenater<InterTransactionResponse>(_interTransactionResponse, true);
        }
    }
}