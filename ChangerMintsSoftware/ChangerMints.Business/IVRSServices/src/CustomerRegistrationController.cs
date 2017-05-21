using System;

namespace ChangerMints.Business {

    public class CustomerRegistrationController {
        // Country Code
        private string COUNTRY_CODE_INDIA = "91";

        // Member variables
        private const int NO_OF_PARAMETERS_FOR_REGISTRATION = 3;

        private LogFiles _logerr;

        /*---------------------------------------------------------------------*/
        // DTO for receiving the parameters
        // Interface for Registration Service
        // DTO for sending the output
        /*---------------------------------------------------------------------*/
        private CustomerNFCCardRegistration _newNFCCardRegistration = null;
        private CustomerNFCCardRegistrationPush _newNFCCardRegistrationPush = null;
        private ICustomerRegistrationService _registerCustomerToChanger = null;
        private CustomerNFCCardRegistrationResponse _registrationResponse = null;
        private bool IsRequestDTOSuccess;
        private bool IsResponseDTOSuccess;

        // Constructor to create objects for Service & Response
        # region Constructors

        public CustomerRegistrationController() {
            _registerCustomerToChanger = new CustomerRegistrationService();
            _registrationResponse = new CustomerNFCCardRegistrationResponse();
            _logerr = new LogFiles();
        }

        #endregion

        // Register the customer using NFC Card Reader
        #region Methods

        // 2 Parameters
        //----------------------------------------------------------------------
        // 1 - Customer phone number
        // 2 - PIN
        public string NFCCardRegistration(string registerToChanger, bool saveChangesToDB = false) {
            try {
                // Split the Query String
                if (saveChangesToDB)
                    _newNFCCardRegistration = ChangerUtils.URLSplitter<CustomerNFCCardRegistration>(registerToChanger, out IsRequestDTOSuccess);
                else {
                    _newNFCCardRegistrationPush = ChangerUtils.URLSplitter<CustomerNFCCardRegistrationPush>(registerToChanger, out IsRequestDTOSuccess);

                    _newNFCCardRegistration = new CustomerNFCCardRegistration();
                    _newNFCCardRegistration.PhoneNumber = _newNFCCardRegistrationPush.PhoneNumber;
                    _newNFCCardRegistration.PIN = _newNFCCardRegistrationPush.PIN;
                }

                // Check the number of parameters and proceed for calling the service
                if (IsRequestDTOSuccess) {
                    if (saveChangesToDB) {
                        // Call the Service with the Request DTO & get the Response DTO
                        _registrationResponse = _registerCustomerToChanger.RegisterToChanger(_newNFCCardRegistration);

                        //For successful registration
                        if (_registrationResponse.ErrorNumber == 1151) {
                            //Send SMS to customer with account number and password
                            //Send success number to IVR
                            var countryCode = _registrationResponse.PhoneNumber.Length > 10 && _registrationResponse.PhoneNumber.Substring(0, 2) == COUNTRY_CODE_INDIA ? 
                                String.Empty : COUNTRY_CODE_INDIA;
                            string myCustomMessage = "Hi, " + countryCode + _registrationResponse.PhoneNumber + "+.Thanks+for+registering+with+Changer.";
                            string parameters = "username=prashanth5429&password=36253035&source=SenderID&dmobile=" + countryCode +
                                _registrationResponse.PhoneNumber + "&message=" + myCustomMessage;
                            ChangerUtils.CreateGetRequest(parameters);
                        }
                    } else
                        _registrationResponse.ErrorNumber = 1;
                } else {
                    if (saveChangesToDB)
                        //Send ErrorNumber to IVR - Invalid Number of Arguments
                        _registrationResponse.ErrorNumber = 1303;
                    else
                        _registrationResponse.ErrorNumber = 0;
                }

                // Create the XML File as response to IVR
                if (saveChangesToDB)
                    return ChangerUtils.CreateXMLDoc(_registrationResponse.ErrorNumber);
                else {
                    string status = (_registrationResponse.ErrorNumber == 1) ? "Success" : "Failure";
                    return ChangerUtils.CreateXMLDoc(status);
                }
            } catch (ChangerMintsError Er) {
                return ChangerUtils.CreateXMLDoc(Er.ErrorNumber);
            } catch (Exception ex) {
                // catch in log file
                string message = string.Format("Class Name{0},Method Name:{1},Error{2}", "CustomerRegistrationController", "NFCCardRegistration", ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                _logerr.LogError(message, ex, LogType.Error);

                //Unexpected error
                _registrationResponse.ErrorNumber = 2001;
                return ChangerUtils.CreateXMLDoc(_registrationResponse.ErrorNumber);
            }
        }

        #endregion
    }
}