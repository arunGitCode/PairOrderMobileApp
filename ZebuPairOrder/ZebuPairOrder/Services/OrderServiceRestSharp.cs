using RestSharp;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZebuPairOrder.Helper;
using ZebuPairOrder.Models.Request;
using ZebuPairOrder.Models.Response;

namespace ZebuPairOrder.Services
{
    internal class OrderServiceRestSharp
    {
        private RestClient restClient;
        private string _userId;
        private string _apiKey;
        private int _quantitySize;
        public OrderServiceRestSharp()
        {
            restClient = new RestClient();

            if (string.IsNullOrEmpty(Constants.API_KEY))
                _apiKey = Preferences.Get("ApiKey", "");
            else
                _apiKey = Constants.API_KEY;

            if (string.IsNullOrEmpty(Constants.API_KEY))
                _userId = Preferences.Get("UserId", "")?.ToUpper();
            else
                _userId = Constants.USER_ID;

            var qty = Preferences.Get("Quantity", "");
            if (!string.IsNullOrEmpty(qty))
                int.TryParse(qty, out _quantitySize);

        }
        public Tuple<bool, string> PlaceOrder(string strike, string orderType)
        {
            try
            {
                string encryptedKey = GetEncrptedKey();
                string sessionId = GetSessionId(encryptedKey);

                string bnfCeStrike = $"{Helper.Helper.GetBankNiftyExpiryScript(Constants.SCRIPT_BNF)}{Application.Current.Properties["StrikePrice"] as string}CE";
                string bnfPeStrike = $"{ Helper.Helper.GetBankNiftyExpiryScript(Constants.SCRIPT_BNF)}{ Application.Current.Properties["StrikePrice"] as string}PE";

                if (string.IsNullOrEmpty(Application.Current.Properties["StrikePrice"] as string))
                    throw new Exception("Enter the Strike Price");

                string symbolIdCE = GetSymbolToken(bnfCeStrike, _userId, sessionId);
                string symbolIdPE = GetSymbolToken(bnfPeStrike, _userId, sessionId);

                var response = ExecuteOrder(bnfCeStrike, bnfPeStrike, symbolIdCE, symbolIdPE, _userId, sessionId);

                if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
                    return new Tuple<bool, string>(true, $"Order Placed successfully and the Response is {response.Content}");
                else
                    return new Tuple<bool, string>(false, $"Problem with Request and the Response is {response.Content}");

            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(false, $"Error - and the exception is {ex.Message}");
            }
        }

        private RestResponse ExecuteOrder(string ceStrike, string peStrike, string symbolIdCE, string symbolIdPE, string userId, string sessionId)
        {
            try
            {
                int quantitySize;
                string straddleType = Application.Current.Properties["OrderType"] as string;
                if (string.IsNullOrEmpty(straddleType))
                    straddleType = "SELL";

                if (_quantitySize.Equals(default(int)))
                {
                    if (ceStrike.Contains("BANK"))
                        quantitySize = Constants.BNF_QTY;
                    else
                        quantitySize = Constants.NF_QTY;
                }
                else
                    quantitySize = _quantitySize;
                var orderRequest = new RestRequest(Constants.BASE_URL + Constants.PLACEORDER_URL, Method.Post);
                orderRequest.AddHeader("Authorization", $"Bearer {userId} {sessionId}");
                orderRequest.AddHeader("Content-Type", "application/json");

                PlaceOrder orderBodyCE = OrderPayload(ceStrike, symbolIdCE, quantitySize, straddleType);
                PlaceOrder orderBodyPE = OrderPayload(peStrike, symbolIdPE, quantitySize, straddleType);

                var listPayload = new List<PlaceOrder> { orderBodyCE, orderBodyPE };

                #region samplePayload
                //var scripBody = "[\r\n  {\r\n   \"complexty\": \"REGULAR\",\r\n    \"discqty\": \"0\",\r\n    \"exch\": \"NFO\",\r\n    \"pCode\": \"NRML\",\r\n    \"prctyp\": \"MKT\",\r\n    \"price\": \"0\",\r\n    \"qty\": 50,\r\n    \"ret\": \"DAY\",\r\n    \"symbol_id\": \"52591\",\r\n    \"trading_symbol\": \"NIFTY22SEP2217000PE\",\r\n    \"transtype\": \"SELL\",\r\n    \"trigPrice\": \"\"\r\n  }\r\n]";
                #endregion

                orderRequest.AddParameter("application/json", JsonSerializer.Serialize(listPayload), ParameterType.RequestBody);
                var response = restClient.Execute(orderRequest);
                return response;
            }
            catch (Exception ex)
            {
                Helper.Helper.LogExceptiontoConsole(ex);
                throw;
            }
        }

        private PlaceOrder OrderPayload(string symbol, string symbolId, int quantitySize, string straddleType)
        {
            return new PlaceOrder
            {
                complexty = "REGULAR",
                discqty = "0",
                exch = "NFO",
                pCode = "NRML",
                prctyp = "MKT",
                price = "",
                qty = quantitySize,
                ret = "DAY",
                symbol_id = symbolId,
                trading_symbol = symbol,
                transtype = straddleType,
                trigPrice = ""
            };
        }


        private string GetEncrptedKey()
        {
            try
            {

                var restRequest = new RestRequest(Constants.BASE_URL + Constants.GET_ENCRYPTION_URL, Method.Post);
                restRequest.AddHeader("Content-Type", "application/json");
                var body = new GetEncryptionModel { userId = _userId };
                restRequest.AddParameter("application/json", JsonSerializer.Serialize(body), ParameterType.RequestBody);
                return JsonSerializer.Deserialize<EncrytedKeyResponse>(restClient.Execute(restRequest).Content).encKey;
            }
            catch (Exception ex)
            {
                Helper.Helper.LogExceptiontoConsole(ex);
                throw;
            }
        }

        private string GetSessionId(string encryptedKey)
        {
            try
            {
                var sessionRequest = new RestRequest(Constants.BASE_URL + Constants.GET_SESSION_URL, Method.Post);
                sessionRequest.AddHeader("Content-Type", "application/json");
                var hashedUserData = Helper.Helper.ComputeSha256Hash(_userId + _apiKey + encryptedKey);
                var sessionBody = new GetSessionIdModel { userId = _userId, userData = hashedUserData };
                sessionRequest.AddParameter("application/json", JsonSerializer.Serialize(sessionBody), ParameterType.RequestBody);
                return JsonSerializer.Deserialize<GetSessionIdResponse>(restClient.Execute(sessionRequest).Content).sessionID;
            }
            catch (Exception ex)
            {
                Helper.Helper.LogExceptiontoConsole(ex);
                throw;
            }
        }

        private string GetSymbolToken(string scriptWithStrike, string uSER_ID, string sessionId)
        {
            try
            {
                var scriptSearchRequest = new RestRequest(Constants.V1_URL_GETSCRIT, Method.Post);
                scriptSearchRequest.AddHeader("Content-Type", "application/json");
                scriptSearchRequest.AddHeader("Authorization", $"Bearer {_userId} {sessionId}");

                var scripBody = new GetScripModel { symbol = scriptWithStrike, exchange = new List<string> { "NFO" } };
                scriptSearchRequest.AddParameter("application/json", JsonSerializer.Serialize(scripBody), ParameterType.RequestBody);

                var response = restClient.Execute(scriptSearchRequest);
                return JsonSerializer.Deserialize<List<GetScriptToken>>(response.Content)[0].token;
            }
            catch (Exception ex)
            {
                Helper.Helper.LogExceptiontoConsole(ex);
                throw;
            }
        }

    }
}
