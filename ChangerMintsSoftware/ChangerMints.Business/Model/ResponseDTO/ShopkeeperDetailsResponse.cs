namespace ChangerMints.Business {

    // Response for Updated shopkeeper mobile number
    public class ShopkeeperMobileNumberUpdateResponse : ChangerMintsErrorManager {
        public string NewPhoneNumber { get; set; }
        public long ShopkeeperAccountNumber { get; set; }
    }

    public class ShopkeeperSmartCardNumberUpdateResponse : ChangerMintsErrorManager {
        public long NewSmartCardSerialNumber { get; set; }
        public long ShopkeeperAccountNumber { get; set; }
    }

    public class ShopkeeperTerminalNumberUpdateResponse : ChangerMintsErrorManager {
        public string NewTerminalNumber { get; set; }
        public decimal? ReceiverBalance { get; set; }
        public decimal? SenderBalance { get; set; }
        public long ShopkeeperAccountNumber { get; set; }
    }
}