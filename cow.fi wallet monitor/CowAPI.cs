using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace cow.fi_wallet_monitor
{
    internal class CowAPI
    {
        public class Check
        {
            /// <summary>
            /// wallet ve limit değişkenlerini tanımlayın
            /// </summary>
            /// <param name="wallet">trump: 0x5be9a4959308a0d0c7bc0870e319314d8d957dbb</param>
            /// <param name="limit">page</param>
            /// <returns>31</returns>
            public async static Task<List<walletRes>> Wallet(string wallet, int limit)
            {
                List<walletRes> responsejson = new List<walletRes>();
                try
                {

                    using (HttpClient client = new HttpClient())
                    {
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"https://api.cow.fi/mainnet/api/v1/account/{wallet}/orders?offset=0&limit={limit}");

                        request.Headers.Add("accept", "application/json");
                        request.Headers.Add("accept-language", "en,tr-TR;q=0.9,tr;q=0.8,en-US;q=0.7");
                        request.Headers.Add("origin", "https://explorer.cow.fi");
                        request.Headers.Add("priority", "u=1, i");
                        request.Headers.Add("referer", "https://explorer.cow.fi/");
                        request.Headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"131\", \"Chromium\";v=\"131\", \"Not_A Brand\";v=\"24\"");
                        request.Headers.Add("sec-ch-ua-mobile", "?0");
                        request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                        request.Headers.Add("sec-fetch-dest", "empty");
                        request.Headers.Add("sec-fetch-mode", "cors");
                        request.Headers.Add("sec-fetch-site", "same-site");
                        request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36");

                        request.Content = new StringContent("");
                        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        HttpResponseMessage response = await client.SendAsync(request);
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();

                        var orders = JsonConvert.DeserializeObject<JArray>(responseBody);

                        foreach (var order in orders)
                        {
                            responsejson.Add(new walletRes
                            {
                                status = order["status"]?.ToString(),
                                sellToken = order["sellToken"]?.ToString(),
                                buyToken = order["buyToken"]?.ToString(),
                                sellAmount = order["sellAmount"]?.ToString(),
                                buyAmount = order["buyAmount"]?.ToString(),
                                kind = order["kind"]?.ToString(),
                                date = order["creationDate"]?.ToString()
                            });
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"********************************\nAPI Sorgusunda sorun oluştu\nHata mesajı: {ex.Message}");
                }
                return responsejson;

            }
        }
        public class Get
        {
            public async static Task<List<tokens>> Wallets()
            {
                List<tokens> responsejson = new List<tokens>();
                HttpClient client = new HttpClient();

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://tokens.coingecko.com/uniswap/all.json");

                request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36");

                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                try
                {
                    var jsonResponse = JsonConvert.DeserializeObject<JObject>(responseBody);
                    var tokens = jsonResponse["tokens"] as JArray;

                    foreach (var token in tokens)
                    {
                        responsejson.Add(new tokens
                        {
                            symbol = token["symbol"]?.ToString(),
                            adress = token["address"]?.ToString()
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Token listesi alınırken hata oluştu: {ex.Message}");
                }

                return responsejson;
            }
        }
        public class tokens
        {
            public string symbol { get; set; }
            public string adress { get; set; }
        }
        public class walletRes
        {
            public string status { get; set; }
            public string sellToken { get; set; }
            public string buyToken { get; set; }
            public string sellAmount { get; set; }
            public string buyAmount { get; set; }
            public string kind { get; set; }
            public string date { get; set; }
        }
    }
}
