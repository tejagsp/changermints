namespace ChangerMints.Business {

    // Transaction response between Shopkeeper & customer
    public class CrossTransactionResponse : ChangerMintsErrorManager {
        public decimal? ShopKeeperReceiverBalance { get; set; }
        public decimal? ShopKeeperSenderBalance { get; set; }
        public string TransactionID { get; set; }
    }

    // Transaction response between Shopkeeper & Shopkeeper
    public class InterTransactionResponse : ChangerMintsErrorManager {
        public decimal? ReceiverBalance { get; set; }
        public decimal? SenderBalance { get; set; }
        public string TransactionID { get; set; }
    }

    // Response for Shopkeeper Balance
    public class ShopkeeperSenderAndReceiverBalanceResponse : ChangerMintsErrorManager {
        public decimal? ReceiverBalance { get; set; }
        public decimal? SenderBalance { get; set; }
    }
}