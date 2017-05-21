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
    
    public partial class ShopKeeperDetail
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Locality { get; set; }
        public long PinCode { get; set; }
        public int KYCNormsNumber { get; set; }
        public long ShopKeeperAccountNumber { get; set; }
        public int AccountStatus { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public int AddressProofID { get; set; }
        public string AddressProofIDNumber { get; set; }
        public string BankAccountNumber { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<long> NFCTagID { get; set; }
    
        public virtual AddressProof AddressProof { get; set; }
        public virtual ShopKeeperAccountNumber ShopKeeperAccountNumber1 { get; set; }
        public virtual Status Status { get; set; }
    }
}