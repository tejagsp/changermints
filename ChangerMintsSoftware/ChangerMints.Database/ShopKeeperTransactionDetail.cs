//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ChangerMints.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class ShopKeeperTransactionDetail
    {
        public long ShopKeeperAccountNumber { get; set; }
        public Nullable<decimal> SenderBalance { get; set; }
        public Nullable<decimal> ReceiverBalance { get; set; }
        public long ReceivedFrom { get; set; }
        public long PaidTo { get; set; }
        public decimal AmountTransfered { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public int TransactionStatus { get; set; }
        public long ShopKeeperTransactionID { get; set; }
        public string TerminalIMEINumber { get; set; }
        public string TransactionID { get; set; }
    
        public virtual OTP OTP { get; set; }
        public virtual Status Status { get; set; }
    }
}
