using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using HipChatClient.Utility;

namespace HipChatClient.Model
{
    public class Room : ISeedable
    {
        public Room() { }
        public Room(dynamic seed, Client client)
        {
            Seed(seed, client);
        }
        private Client Client { get; set; }

        public void Seed(dynamic seed, Client client)
        {
            Client = client;
            Id = seed.id;
            Name = seed.name;
        }

        public Int64 Id { get; private set; }
        public string Name { get; private set; }

        public IDictionary<Int64, User> Participants
        {
            get
            {
                try
                {
                    dynamic data = Client.GetFromHipChat(String.Format(
                                                     "https://api.hipchat.com/v2/room/{0}/participant",
                                                     Id));
                    return HipChatClient.Client.ToDictionary<User>(data.items, Client);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw;
                }
            }
        } 

        public Int64 AddWebhook(string name, string trigger, string pattern, string callback)
        {
            try
            {
                string url = String.Format("https://api.hipchat.com/v2/room/{0}/webhook", Id);
                var options = new Dictionary<string, string>
                {
                    {"url", callback},
                    {"pattern", pattern},
                    {"event", trigger},
                    {"name", name},
                };

                dynamic data = Client.PostToHipChat(options, url);
                Int64 id = data.id;
                return id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public void ClearWebhooks(string name)
        {
            try
            {
                dynamic data = Client.GetFromHipChat(String.Format("https://api.hipchat.com/v2/room/{0}/webhook", Id));

                foreach (dynamic item in data.items)
                {
                    if (item.name == name)
                    {
                        Client.DeleteFromHipChat(String.Format("https://api.hipchat.com/v2/room/{0}/webhook/{1}", Id, item.id));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public void SendNotification(string message, string from, string color = "purple", string format = "text")
        {
            try
            {
                string url =
                    String.Format("https://api.hipchat.com/v2/room/{0}/notification",Id);

                var options = new Dictionary<string, string>
                {
                    {"message", message},
                    {"color", color},
                    {"message_format", format},
                    {"from", @from}
                };
                Client.PostToHipChat(options, url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public void SendUrl(string message, string urlToShare)
        {
            try
            {
                string url = String.Format("https://api.hipchat.com/v2/room/{0}/share/link", Id);

                var options = new Dictionary<string, string>
                {
                    {"message", message},
                    {"link", urlToShare}
                };

                string result = Client.PostToHipChat(options, url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}