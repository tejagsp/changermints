using System;

namespace ChangerMints.Business {

    public class RunTerminalTransactions : IRunTerminalTransactions {
        private TerminalController _terminalController;
        private char _functSplitter = '?';
        private char _inputSplitter = '#';

        private enum TerminalRequest {
            ShopKeeperBalance = 5001,
            ShopKeeperToCustomer = 5002,
            CustomerToShopKeeper = 5003,
            ShopKeeperToShopKeeper = 5004
        }

        public RunTerminalTransactions() {
            _terminalController = null;
        }

        public string RunTransaction(string queryString) {
            _terminalController = new TerminalController();
            var input = queryString.Split(_functSplitter);
            var inputRequest = Convert.ToInt64(input[0]);
            var transactionInputs = input[1].Split(_inputSplitter);

            TerminalRequest terminalRequest = (TerminalRequest)inputRequest;

            switch (terminalRequest) {
                case TerminalRequest.ShopKeeperBalance:
                    return _terminalController.ShopkeeperBalance(transactionInputs[0]);

                case TerminalRequest.ShopKeeperToCustomer:
                    return _terminalController.ShopkeepertoCustomerTransaction(transactionInputs[0]);

                case TerminalRequest.CustomerToShopKeeper:
                    return _terminalController.CustomerToShopkeeperTransaction(transactionInputs[0]);

                case TerminalRequest.ShopKeeperToShopKeeper:
                    return _terminalController.ShopkeeperToShopkeeperTransaction(transactionInputs[0]);

                default:
                    throw new NotImplementedException("");
            }
        }
    }
}