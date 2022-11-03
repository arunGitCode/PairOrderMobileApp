using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZebuPairOrder.Helper;
using ZebuPairOrder.Models.Request;
using ZebuPairOrder.Models.Response;

namespace ZebuPairOrder.Services
{
    internal class OrderService
    {
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

                var symbolIdCE = GetSymbolToken(bnfCeStrike, Constants.USER_ID, sessionId);
                var symbolIdPE = GetSymbolToken(bnfPeStrike, Constants.USER_ID, sessionId);

                var response = PlaceOrder(bnfCeStrike, bnfPeStrike, symbolIdCE, symbolIdPE, Constants.USER_ID, sessionId);


                return new Tuple<bool, string>(true, $"Order Placed successfully and the Response is {response}");
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(false, $"{ex.Message}");
            }


        }
        public string GetEncrptedKey()
        {
            try
            {
                using (var _httpClient = new HttpClient())
                {
                    _httpClient.BaseAddress = new Uri(Constants.BASE_URL);
                    _httpClient.DefaultRequestHeaders.Accept.Clear();
                    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var data = new StringContent(JsonConvert.SerializeObject(new GetEncryptionModel { userId = Constants.USER_ID }), Encoding.UTF8, "application/json");

                    //POST Method
                    HttpResponseMessage response = _httpClient.PostAsync(Constants.GET_ENCRYPTION_URL, data).Result;

                    return JsonConvert.DeserializeObject<EncrytedKeyResponse>(response.Content.ReadAsStringAsync().Result).encKey;
                }
            }
            catch (Exception ex)
            {
                Helper.Helper.LogExceptiontoConsole(ex);
                throw;
            }
        }

        public string GetSessionId(string encryptedKey)
        {
            try
            {
                using (var _httpClient = new HttpClient())
                {
                    _httpClient.BaseAddress = new Uri(Constants.BASE_URL);
                    _httpClient.DefaultRequestHeaders.Accept.Clear();
                    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var hashedUserData = Helper.Helper.ComputeSha256Hash($"{Constants.USER_ID}{Constants.API_KEY}{encryptedKey}");
                    var sessionBody = new GetSessionIdModel { userId = Constants.USER_ID, userData = hashedUserData };
                    var data = new StringContent(JsonConvert.SerializeObject(sessionBody), Encoding.UTF8, "application/json");
                    //POST Method
                    HttpResponseMessage response = _httpClient.PostAsync(Constants.GET_SESSION_URL, data).Result;
                    return JsonConvert.DeserializeObject<GetSessionIdResponse>(response.Content.ReadAsStringAsync().Result).sessionID;
                }
            }
            catch (Exception ex)
            {
                Helper.Helper.LogExceptiontoConsole(ex);
                throw;
            }
        }

        public string GetSymbolToken(string scriptWithStrike, string userId, string sessionId)
        {
            try
            {
                using (var _httpClient = new HttpClient())
                {
                    _httpClient.BaseAddress = new Uri(Constants.V1_URL_GETSCRIT);
                    _httpClient.DefaultRequestHeaders.Accept.Clear();
                    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{userId}{sessionId}");
                    var body = new GetScripModel { symbol = scriptWithStrike, exchange = new List<string> { "NFO" } };

                    var data = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                    //POST Method
                    HttpResponseMessage response = _httpClient.PostAsync(Constants.V1_URL_GETSCRIT, data).Result;
                    return JsonConvert.DeserializeObject<List<GetScriptToken>>(response.Content.ReadAsStringAsync().Result)[0].token;
                }

            }
            catch (Exception ex)
            {
                Helper.Helper.LogExceptiontoConsole(ex);
                throw;
            }
        }

        public string PlaceOrder(string ceStrike, string peStrike, string symbolIdCE, string symbolIdPE, string userId, string sessionId)
        {
            try
            {
                int quantitySize;
                string straddleType = Application.Current.Properties["OrderType"] as string;
                if (string.IsNullOrEmpty(straddleType))
                    straddleType = "SELL";
                if (ceStrike.Contains("BANK"))
                    quantitySize = Constants.BNF_QTY;
                else
                    quantitySize = Constants.NF_QTY;

                using (var _httpClient = new HttpClient())
                {
                    _httpClient.BaseAddress = new Uri(Constants.BASE_URL);
                    _httpClient.DefaultRequestHeaders.Accept.Clear();
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{userId}{sessionId}");
                    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    _httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");

                    var orderBodyCE = OrderPayload(ceStrike, symbolIdCE, quantitySize, straddleType);
                    var orderBodyPE = OrderPayload(peStrike, symbolIdPE, quantitySize, straddleType);

                    var listPayload = new List<PlaceOrder> { orderBodyCE, orderBodyPE };

                    var data = new StringContent(JsonConvert.SerializeObject(listPayload), Encoding.UTF8, "application/json");
                    //POST Method
                    HttpResponseMessage response = _httpClient.PostAsync(Constants.PLACEORDER_URL, data).Result;
                    return response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception ex)
            {

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

    }
}
