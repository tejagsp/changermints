namespace ChangerMints.Business {

    // Response for Customer Registration with NFC tag
    // SMS message to customer
    public class CustomerNFCCardRegistrationResponse : ChangerMintsErrorManager {
        public string PhoneNumber { get; set; }
        public long Accountnumber { get; set; }
        public long PIN { get; set; }
    }
}