using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using ChangerMints.Business;

namespace ChangerMints.Business
{
    // Manages Error Responses
    public class ErrorManager : IErrorManager
    {
        public int ErrorNumber { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class Error : Exception
    {
        public Error() { }
        public Error(int ErrNumber)  {
            ErrorNumber = ErrNumber;
            ErrorMessage = System.Configuration.ConfigurationManager.AppSettings[Convert.ToString(ErrNumber)];
        }
        public int ErrorNumber { get; set; }
        public string ErrorMessage { get; set; }
    }
}