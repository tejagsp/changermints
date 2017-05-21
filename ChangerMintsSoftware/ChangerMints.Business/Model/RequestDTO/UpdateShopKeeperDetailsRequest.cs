namespace ChangerMints.Business {

    // Transfers details to the customer for new mobile
    public class ShopKeeperNewMobileNumeberInfo {
        public string PhoneNumber { get; set; }
        public long ShopKeeperAccountNumber { get; set; }
    }

    // Transfer new smart card details to shopkeeper for the lost terminal
    public class ShopKeeperNewSmartCardInfo {
        public long Password { get; set; }
        public long ShopKeeperAccountNumber { get; set; }
        public long SmartCardSerialNumber { get; set; }
    }

    // Transfers New Terminal information for the lost terminals
    public class ShopKeeperNewTerminalInfo {
        public long ShopKeeperAccountNumber { get; set; }
        public string TerminalIMEINumber { get; set; }
    }
}