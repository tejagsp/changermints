namespace ChangerMints.Business {

    // Manages Error Responses
    interface IChangerMintsErrorManager {
        int ErrorNumber { get; set; }
        string ErrorMessage { get; set; }
    }
}