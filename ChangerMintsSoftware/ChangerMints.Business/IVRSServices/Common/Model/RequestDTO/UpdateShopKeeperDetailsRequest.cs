using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangerMints.Business
{
    // Transfers details to the customer for new mobile
    public class ShopKeeperNewMobileNumeberInfo {
        public long ShopKeeperAccountNumber { get; set; }
        public string PhoneNumber { get; set; }
    }

    // Transfer new smart card details to shopkeeper for the lost terminal
    public class ShopKeeperNewSmartCardInfo {
        public long ShopKeeperAccountNumber { get; set; }
        public long SmartCardSerialNumber { get; set; }
        public long Password { get; set; }
    }

    // Transfers New Terminal information for the lost terminals
    public class ShopKeeperNewTerminalInfo {
        public long ShopKeeperAccountNumber { get; set; }
        public string TerminalIMEINumber { get; set; }
    }
}
