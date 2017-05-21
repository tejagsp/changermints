namespace ChangerMints.Business {

    // Customer new NFC Tag info
    // When customer loses his NFC tag and wants to update his old tag with new one
    public class CustomerNewNFCCardinfo {
        public long CustomerAccountNumber { get; set; }
        public long PIN { get; set; }
    }

    // Customer Registration with NFC tag
    public class CustomerNFCCardRegistration {
        public string CalledPhoneNumber { get; set; }
        public string PhoneNumber { get; set; }
        public long PIN { get; set; }
    }

    // Customer Registration with NFC tag
    public class CustomerNFCCardRegistrationPush {
        public string CallDuration { get; set; }
        public string Date { get; set; }
        public string EndTime { get; set; }
        public string PhoneNumber { get; set; }
        public long PIN { get; set; }
        public string StartTime { get; set; }
        public string Time { get; set; }
    }
}