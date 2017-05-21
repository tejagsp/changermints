using System;

namespace ChangerMints.Business {

    public class RunCustomerRegistration : IRunCustomerRegistration {
        private CustomerRegistrationController _customerController = null;
        private char _functSplitter = '?';
        private char _inputSplitter = '#';

        private enum CustomerRegistrationRequest {
            ShopKeeperToCustomer = 5100,
            ShopKeeperToCustomerPush = 5101
        }

        public RunCustomerRegistration() {
            _customerController = new CustomerRegistrationController();
        }

        public string NFCCustomerRegistration(string queryString) {
            var input = queryString.Split(_functSplitter);
            var inputRequest = Convert.ToInt64(input[0]);
            var registrationInputs = input[1].Split(_inputSplitter);

            CustomerRegistrationRequest registrationRequest = (CustomerRegistrationRequest)inputRequest;

            switch (registrationRequest) {
                case CustomerRegistrationRequest.ShopKeeperToCustomer:
                    return _customerController.NFCCardRegistration(registrationInputs[0], true);

                case CustomerRegistrationRequest.ShopKeeperToCustomerPush:
                    return _customerController.NFCCardRegistration(registrationInputs[0]);

                default:
                    return null;
            }
        }
    }
}