namespace ChangerMints.Business {
    public class CrossTransactionResponse : ErrorManager {
        public decimal? ShopKeeperSenderBalance { get; set; }
        public decimal? ShopKeeperReceiverBalance { get; set; }
        public decimal? CustomerBalance { get; set; }
    }

    public class ShopkeeperSenderAndReceiverBalanceResponse : ErrorManager {
        public long ShopkeeperAccountNumber { get; set; }
        public decimal? SenderBalance { get; set; }
        public decimal? ReceiverBalance { get; set; }
    }

    public class InterTransactionResponse : ErrorManager {
        public long ShopkeeperAccountNumber { get; set; }
        public decimal? SenderBalance { get; set; }
        public decimal? ReceiverBalance { get; set; }
    }
}