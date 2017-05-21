using System;
using System.Xml.Linq;

namespace ChangerMints.Business {
    interface ICustomerRegistration {
        long IVRKey { get; set; }

        bool ValidateIVRKey(long IVRKey);
        XDocument UpdateIVRKeyStatus();
    }
}
