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
    
    public partial class CustomerNFCTagDetail
    {
        public long NFCTagID { get; set; }
        public string NFCTagUID { get; set; }
        public long PIN { get; set; }
        public string NFCSerialNumber { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int NFCStatus { get; set; }
        public string KYCFormNumber { get; set; }
        public string NFCKitSerialNumber { get; set; }
    
        public virtual Status Status { get; set; }
    }
}
