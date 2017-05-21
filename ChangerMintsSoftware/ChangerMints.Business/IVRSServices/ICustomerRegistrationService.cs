using ChangerMints.Business;

namespace ChangerMints.Business {
   public interface ICustomerRegistrationService {
       CustomerNFCCardRegistrationResponse RegisterToChanger(CustomerNFCCardRegistration registrationWithNFC);
   }
}
