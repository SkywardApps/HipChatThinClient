using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Script.Serialization;
using HipChatClient.Model;
using HipChatClient.Utility;

namespace HipChatClient
{
    public class Client
    {
        public string OAuthToken { get; private set; }
        
        public Client(string oAuthToken)
        {
            OAuthToken = oAuthToken;
        }

        public static OAuthToken GetAuthenticatedTokenResponse(string oauthId, string oauthSecret, string scope)
        {
            const string tokenUrl = "https://api.hipchat.com/v2/oauth/token";
            const string grantType = "client_credentials";

            JavaScriptSerializer js = new JavaScriptSerializer();

            var json = js.Serialize(new
            {
                grant_type = grantType,
                scope = scope,
            });

            var payload = new StringContent(json, Encoding.UTF8, "application/json");
            var client = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes(oauthId + ":" + oauthSecret);
            var header = new AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(byteArray));
            client.DefaultRequestHeaders.Authorization = header;

            var result = client.PostAsync(tokenUrl, payload).Result;
            if (!result.IsSuccessStatusCode)
            {
                return null;
            }

            string tokenResult = result.Content.ReadAsStringAsync().Result;
            if(String.IsNullOrWhiteSpace(tokenResult))
            {
                return null;
            }

            var deserializer = new JavaScriptSerializer();
            deserializer.RegisterConverters(new[] { new DynamicJsonConverter() });
            dynamic tokenData = deserializer.Deserialize(tokenResult, typeof(object));
            if (tokenData == null)
            {
                return null;
            }

            return new OAuthToken(tokenData);
        }

        public IDictionary<Int64, User> Users
        {
            get
            {
                dynamic data = GetFromHipChat("https://api.hipchat.com/v2/user");
                IDictionary<Int64, User> users = HipChatClient.Client.ToDictionary<User>(data.items, this);
                foreach (var user in users.Values)
                {
                    user.Fetch();
                }
                return users;
            }
        }

        public IDictionary<Int64, Room> Rooms
        {
            get
            {
                dynamic data = GetFromHipChat(String.Format("https://api.hipchat.com/v2/room?format=json&auth_token={0}", OAuthToken));
                return HipChatClient.Client.ToDictionary<Room>(data.items, this);
            }
        }

        public dynamic GetFromHipChat(string url)
        {
            const string method = "GET";
            return FromHipChat(url, method);
        }

        public string DeleteFromHipChat(string url)
        {
            const string method = "DELETE";
            return FromHipChat(url, method);
        }

        public dynamic PostToHipChat<T>(Dictionary<string, T> options, string url) 
        {
            var serializer = new JavaScriptSerializer();
            string dataString = serializer.Serialize(options);
            return PostToHipChat(dataString, url);
        }

        public dynamic PostToHipChat(string dataString, string url)
        {
            url = AuthenticateUrl(url);

            var dataBytes = Encoding.UTF8.GetBytes(dataString);

            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.ContentType = "application/json";
            request.Method = "POST";
            request.ContentLength = dataBytes.Length;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(dataBytes, 0, dataBytes.Length);
            }

            try
            {
                var response = request.GetResponse();
                using (var responseStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        string result = reader.ReadToEnd();
                        Console.WriteLine(result);
                        return DynamicDeserialize(result);
                    }
                }
            }
            catch (WebException ex)
            {
                var response = ex.Response;
                using (var responseStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        string result = reader.ReadToEnd();
                        Console.WriteLine(result);
                        return null;
                    }
                }
            }
        }

        private string AuthenticateUrl(string url)
        {
            string append = String.Format("format=json&auth_token={0}", OAuthToken);
            if (url.Contains('?'))
            {
                url += '&';
            }
            else
            {
                url += '?';
            }
            url += append;
            return url;
        }

        private static dynamic DynamicDeserialize(string result)
        {
            var deserializer = new JavaScriptSerializer();
            deserializer.RegisterConverters(new[] { new DynamicJsonConverter() });
            dynamic data = deserializer.Deserialize(result, typeof(object));
            return data;
        }

        private dynamic FromHipChat(string url, string method)
        {
            url = AuthenticateUrl(url);

            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Method = method;
            try
            {
                var response = request.GetResponse();
                using (var responseStream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(responseStream))
                    {
                        string result = reader.ReadToEnd();
                        Console.WriteLine(result);
                        return DynamicDeserialize(result);
                    }
                }
            }
            catch (WebException ex)
            {
                var response = ex.Response;
                using (var responseStream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(responseStream))
                    {
                        string result = reader.ReadToEnd();
                        throw new WebException(result, ex);
                    }
                }
            }

        }

        public static IDictionary<Int64, T> ToDictionary<T>(dynamic list, Client client) where T:ISeedable,new()
        {
            if (list == null)
            {
                return new Dictionary<Int64, T>();
            }
            var enumeration = (list as IEnumerable<object>);
            if (enumeration == null)
            {
                return new Dictionary<Int64, T>();
            }
            return enumeration.Select(i =>
                                      {
                                          var t = new T();
                                          t.Seed(i, client);
                                          return t;
                                      }).ToDictionary(i => i.Id, i => i); ;
        }
    }
}