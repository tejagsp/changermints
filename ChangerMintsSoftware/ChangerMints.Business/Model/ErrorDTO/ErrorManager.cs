namespace ChangerMints.Business {

    public class ChangerMintsError : System.Exception {
        public ChangerMintsError() { }

        public ChangerMintsError(int ErrNumber) {
            ErrorNumber = ErrNumber;
            ErrorMessage = System.Configuration.ConfigurationManager.AppSettings[System.Convert.ToString(ErrNumber)];
        }

        public string ErrorMessage { get; set; }
        public int ErrorNumber { get; set; }
    }

    // Manages Error Responses
    public class ChangerMintsErrorManager : IChangerMintsErrorManager {
        public string ErrorMessage { get; set; }
        public int ErrorNumber { get; set; }
    }
}