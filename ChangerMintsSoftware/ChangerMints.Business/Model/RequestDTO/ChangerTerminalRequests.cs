namespace ChangerMints.Business {

    // Requests during customer to shopkeeper transaction
    public class CustomerToShopKeeperTransaction {
        public string IMEINumber{ set; get; }
        public decimal Amount { set; get; }
        public long NFCTagID { set; get; }
        public string NFCTagUID { set; get; }
        public int? OTP { set; get; }
        public int Password { set; get; }
        public long ShopKeeperAccountNumber { set; get; }
    }

    // Request during resend of an OTP
    public class ResendOTP {
        public long NFCTagID { set; get; }
        public int? OTP { set; get; }
    }

    // Requests during shopkeeper to shopkeeper transaction
    public class ShopKeeperReceiverToSenderTransaction {
        public string TerminalIMEINumber{ set; get; }
        public decimal Amount { set; get; }
        public long ShopKeeperAccountNumber { set; get; }
    }

    // Request when shop keeper sends the balance query
    public class ShopKeeperSenderAndReceiverBalance {
        public string TerminalIMEINumber { set; get; }
        public long ShopKeeperAccountNumber { set; get; }
    }

    // Requests during shopkeeper to customer transaction
    public class ShopKeeperToCustomerTransaction {
        public string IMEINumber{ set; get; }
        public decimal Amount { set; get; }
        public long NFCTagID { set; get; }
        public string NFCTagUID { set; get; }
        public long ShopKeeperAccountNumber { set; get; }
    }

    // Request when Resend OTP
    public class RunTransactionWithOTP {
        public string TransactionID { set; get; }
        public int? OTP { set; get; }
    }
}