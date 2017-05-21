namespace ChangerMints.Business {

    internal interface ITerminalServices {

        CrossTransactionResponse CustomerToShopKeeperTransaction(CustomerToShopKeeperTransaction CustomerToShopkeeper);

        CrossTransactionResponse ShopkeeperToCustomerTransaction(ShopKeeperToCustomerTransaction ShopkeeperToCustomer);

        InterTransactionResponse ShopKeeperToShopKeeperTransaction(ShopKeeperReceiverToSenderTransaction ShopkeeperToShopkeeper);
    }
}