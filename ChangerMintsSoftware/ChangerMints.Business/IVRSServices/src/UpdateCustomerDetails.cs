using System;
using System.Linq;
using ChangerMints.Database;

namespace ChangerMints.Business {

    internal class UpdateCustomerDetails {
        private UnitOfWork _changerMintsUOW = null;

        public UpdateCustomerDetails() {
            _changerMintsUOW = new UnitOfWork();
        }

        public int UpdateCustomerNFCTag(CustomerNewNFCCardinfo registrationNewNFC) {
            try {
                // GenerateCustomerAccountNumber();
                if (!ChangerValidations.IsCustomerNFCTagValid(registrationNewNFC.PIN)) return 1202;

                //Get the OLD NFC tag for the given customer account and update the status as Lost
                var CustomerDetails = _changerMintsUOW.Repository<CustomerDetail>().Query().Filter(q => q.CustomerAccountNumber == registrationNewNFC.CustomerAccountNumber).Get();
                if (CustomerDetails.FirstOrDefault() == null) return 1201;

                var OLDNFCTag = CustomerDetails.FirstOrDefault().NFCTagID.ToString();

                //Add the updated date
                CustomerDetails.FirstOrDefault().UpdatedDate = DateTime.Now;

                //Update old NFCTag status as Lost
                var CustomerOldNFCDetails = _changerMintsUOW.Repository<CustomerNFCTagDetail>().Query().Filter(q => q.NFCTagID.ToString() == OLDNFCTag).Get();
                CustomerOldNFCDetails.FirstOrDefault().NFCStatus = 1007;

                //Insert new NFC tag in CustomerNFCTagDetail and update the status as ACTIVE
                var CustomerNewNFCDetails = _changerMintsUOW.Repository<CustomerNFCTagDetail>().Query().Filter(q => q.PIN == registrationNewNFC.PIN).Get();
                if (CustomerNewNFCDetails.FirstOrDefault() == null) return 1202;
                CustomerNewNFCDetails.FirstOrDefault().NFCStatus = 1001;

                //Update the new NFC Tag for the given account Number
                CustomerDetails.FirstOrDefault().NFCTagID = CustomerNewNFCDetails.FirstOrDefault().NFCTagID;

                //Maintain previous history in the ChangerUpdatedDetails table
                var updatedDetails = new ChangerUpdatedDetail() {
                    CustomerAccountNumber = registrationNewNFC.CustomerAccountNumber, ShopKeeperAccountNumber = null,
                    UpdatedField = "NFCTagID", UpdatedDate = DateTime.Now, PreviousValue = OLDNFCTag
                };
                return 1251;
            } catch (Exception ex) {
                if (ex.InnerException == null) throw ex;
                else throw ex.InnerException;
            }
        }
    }
}