using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangerMints.Business {
    // Response for Customer Registration with NFC tag
    public class CustomerNFCCardRegistrationResponse : ErrorManager {
        public long Accountnumber { get; set; }
        public long PIN { get; set; }
    }
}
