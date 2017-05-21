using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangerMints.Business {
    // Manages Error Responses
    public interface IErrorManager {
        int ErrorNumber { get; set; }
        string ErrorMessage { get; set; }
    }
}