using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangerMints.Business {
    // Customer Registration with NFC tag
    public class CustomerNFCCardRegistration {
        public string IVRNumber { get; set; }
        public string PhoneNumber { get; set; }
        public long PIN { get; set; }
    }

    // Customer new NFC Tag info
    // When customer loses his NFC tag and wants to update his old tag with new one
    public class CustomerNewNFCCarinfo {
        public long CustomerAccountNumber { get; set; }
        public long PIN { get; set; }
    }

}
