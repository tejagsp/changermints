using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangerMints.Business {
    public class ShopkeeperMobileNumberUpdateResponse : ErrorManager {
        public string NewPhoneNumber { get; set; }
        public long ShopkeeperAccountNumber { get; set; }
    }

    public class ShopkeeperTerminalNumberUpdateResponse : ErrorManager {
        public string NewTerminalNumber { get; set; }
        public long ShopkeeperAccountNumber { get; set; }
        public decimal? SenderBalance { get; set; }
        public decimal? ReceiverBalance { get; set; }
    }

    public class ShopkeeperSmartCardNumberUpdateResponse : ErrorManager {
        public long NewSmartCardSerialNumber { get; set; }
        public long ShopkeeperAccountNumber { get; set; }
    }
}
