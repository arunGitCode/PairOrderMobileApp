using System;
using System.Collections.Generic;
using System.Text;

namespace ZebuPairOrder.Helper
{
    internal class Constants
    {
        public const string SCRIPT_NF = "NIFTY";
        public const string API_KEY = "";
        public const string SCRIPT_BNF = "BANKNIFTY";
        public const int BNF_QTY = 25;
        public const int NF_QTY = 50;
        public const string USER_ID = "";
        public const string BASE_URL = @"https://api.zebull.in/rest/V2MobullService/api/";
        public const string V1_URL_GETSCRIT = @"https://zebull.in/rest/MobullService/exchange/getScripForSearch";
        public const string GET_QOUTE = @"ScripDetails/getScripQuoteDetails";
        public const string GET_ENCRYPTION_URL = @"customer/getAPIEncpkey";
        public const string GET_SESSION_URL = @"customer/getUserSID";
        public const string PLACEORDER_URL = @"placeOrder/executePlaceOrder";
        public const string STRADDLE_TYPE = "SELL";
    }
}
