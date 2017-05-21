namespace ChangerMints.Business {
    public class ShopKeeperToCustomerTransaction {
        public long ShopKeeperAccountNumber { set; get; }
        public string NFCTagID { set; get; }
        public decimal Amount { set; get; }
    }

    public class CustomerToShopKeeperTransaction {
        public long ShopKeeperAccountNumber { set; get; }
        public string NFCTagID { set; get; }
        public int Password { set; get; }
        public decimal Amount { set; get; }
        public int? OTP { set; get; }
    }

    public class ShopKeeperReceiverToSenderTransaction {
        public long ShopKeeperAccountNumber { set; get; }
        public decimal Amount { set; get; }
    }

    public class ShopKeeperSenderAndReceiverBalance {
        public long ShopKeeperAccountNumber { set; get; }
    }

    public class ResendOTP {
        public string NFCTagID { set; get; }
        public int? OTP { set; get; }
    }
}
