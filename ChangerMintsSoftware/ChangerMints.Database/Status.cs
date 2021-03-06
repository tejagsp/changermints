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
    
    public partial class Status
    {
        public Status()
        {
            this.CustomerDetails = new HashSet<CustomerDetail>();
            this.CustomerNFCTagDetails = new HashSet<CustomerNFCTagDetail>();
            this.CustomerTransactionDetails = new HashSet<CustomerTransactionDetail>();
            this.ShopKeeperAccountNumbers = new HashSet<ShopKeeperAccountNumber>();
            this.ShopKeeperDetails = new HashSet<ShopKeeperDetail>();
            this.ShopKeeperSmartCardMasters = new HashSet<ShopKeeperSmartCardMaster>();
            this.ShopKeeperTerminalDetails = new HashSet<ShopKeeperTerminalDetail>();
            this.ShopKeeperTransactionDetails = new HashSet<ShopKeeperTransactionDetail>();
        }
    
        public int StatusID { get; set; }
        public string Status1 { get; set; }
        public string Comments { get; set; }
    
        public virtual ICollection<CustomerDetail> CustomerDetails { get; set; }
        public virtual ICollection<CustomerNFCTagDetail> CustomerNFCTagDetails { get; set; }
        public virtual ICollection<CustomerTransactionDetail> CustomerTransactionDetails { get; set; }
        public virtual ICollection<ShopKeeperAccountNumber> ShopKeeperAccountNumbers { get; set; }
        public virtual ICollection<ShopKeeperDetail> ShopKeeperDetails { get; set; }
        public virtual ICollection<ShopKeeperSmartCardMaster> ShopKeeperSmartCardMasters { get; set; }
        public virtual ICollection<ShopKeeperTerminalDetail> ShopKeeperTerminalDetails { get; set; }
        public virtual ICollection<ShopKeeperTransactionDetail> ShopKeeperTransactionDetails { get; set; }
    }
}
