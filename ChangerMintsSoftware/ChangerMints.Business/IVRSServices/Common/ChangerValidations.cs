//Written by Vamsi Krushna Lingala

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ChangerMints.Business
{
    public class ChangerValidations {
        private const int CUSTOMER_ACCOUNT_NUMBER_LENGTH = 10;
        private const int CUSTOMER_NFC_TAG_SERIAL_NUMBER_LENGTH = 14;
        private const int CUSTOMER_NFC_TAG_SERIAL_ID_LENGTH= 10;
        private const string PHONENUMBER_REGEX = @"[a-z]+|\@|#|\$|\%|\*|\.|\=|\{|\}|\[|\]";
        private const int SHOP_KEEPER_ACCOUNT_NUMBER_LENGTH = 10;
        private const int SMART_CARD_SERIAL_NUMBER_LENGTH = 16;
        private const int Terminal_IMEI_Number_LENGTH = 10;
        private const string TerminalIMEI_REGEX = @"[a-z]+|\@|#|\$|\%|\*|\=|\{|\}|\[|\]|\+";

        public static bool GetInputParametersStatus<T, U>(IList<ModelResponses<T, U>> _errorMessagers, out ModelResponses<T, U> errorCreator)
            where T : new()
            where U : new() {
            errorCreator = _errorMessagers.FirstOrDefault(mc => !(mc.IsValid(new U())));
            return !(errorCreator == null) ? errorCreator.IsValid(new U()) : true;
        }

        public bool IsAmountValid(decimal amount) {
            return (amount > 0) && (amount % 1 == 0) ? true : false;
        }

        public bool IsCustomerAccountValid(long account) {
            return (Convert.ToString(account).Length == CUSTOMER_ACCOUNT_NUMBER_LENGTH) ? true : false;
        }

        public static bool IsCustomerNFCTagValid(long NFCTag) {
            return (Convert.ToString(NFCTag).Length == CUSTOMER_NFC_TAG_SERIAL_NUMBER_LENGTH) ? true : false;
        }

        public bool IsCustomerNFCTagIDValid(string NFCTagID)
        {
            return (Convert.ToString(NFCTagID).Length == CUSTOMER_NFC_TAG_SERIAL_ID_LENGTH) ? true : false;
        }

        public static bool IsPhoneNumberValid(string phoneNumber) {

            //Return false if Phone number has characters
            var match = Regex.Match(phoneNumber, PHONENUMBER_REGEX, RegexOptions.IgnoreCase);
            if (match.Success) return false;
            var numberOfPlus = phoneNumber.Count(x => x == '+');
            if (numberOfPlus  > 1) return false;
            if (numberOfPlus == 1) {
                if (phoneNumber[0] != '+') return false;
            }
            return true;
        }

        public bool IsShopKeeperAccountValid(long account) {
            return (Convert.ToString(account).Length == SHOP_KEEPER_ACCOUNT_NUMBER_LENGTH) ? true : false;
        }
        internal static bool IsShopKeeperSerialNumberValid(long SmartCardSerialNumber) {
            return (Convert.ToString(SmartCardSerialNumber).Length == SMART_CARD_SERIAL_NUMBER_LENGTH) ? true : false;
        }

        internal static bool IsTerminalIMEINumberValid(string TerminalIMEINumber) {
            if (Convert.ToString(TerminalIMEINumber).Length != Terminal_IMEI_Number_LENGTH) return false;
            var match = Regex.Match(TerminalIMEINumber, TerminalIMEI_REGEX, RegexOptions.IgnoreCase);
            if (match.Success) return false;
            return true;
        }
    }
}
