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
    
    public partial class ShopKeeperTerminalDetail
    {
        public string Password { get; set; }
        public int TerminalStatusID { get; set; }
        public long TerminalIMEINumber { get; set; }
        public string Username { get; set; }
        public long AccountNumber { get; set; }
    
        public virtual ShopKeeperAccountNumber ShopKeeperAccountNumber { get; set; }
        public virtual Status Status { get; set; }
    }
}
