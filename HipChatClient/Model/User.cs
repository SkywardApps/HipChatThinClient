using System;
using Microsoft.CSharp.RuntimeBinder;

namespace HipChatClient.Model
{
    public interface IRestrictedUser
    {
        IUser Fetch();
        Int64 Id { get; }
        string Name { get; }
        string MentionName { get; }
    }

    public interface IUser
    {
        Int64 Id { get; }
        string Name { get; }
        string MentionName { get; }
        string Email { get; }
        bool IsOnline { get; }
        string StatusMessage { get; }
        string Status { get; }
        Int64 Idle { get; }
        string XmppJid { get; }
    }

    public class User : ISeedable, IUser, IRestrictedUser
    {
        public User() { }
        public User(dynamic seed, Client client)
        {
            Seed(seed, client);
        }
        private Client Client { get; set; }

        public void Seed(dynamic seed, Client client)
        {
            Client = client;
            MentionName = seed.mention_name;
            Id = seed.id;
            Name = seed.name;

            // We split up the optional attributes so we can catch the exception if they aren't included
            try
            {
                Email = seed.email;
                IsOnline = seed.presence.is_online;
                StatusMessage = seed.presence.status;
                Status = seed.presence.show;
                Idle = seed.presence.idle;
                XmppJid = seed.xmpp_jid;
            }
            catch (RuntimeBinderException ex)
            { }
            catch (InvalidOperationException ex)
            {}
            catch (NullReferenceException ex)
            {}
        }

        public IUser Fetch()
        {
            dynamic data = Client.GetFromHipChat(String.Format("https://api.hipchat.com/v2/user/{0}", Id));
            Seed(data, Client);
            return this;
        }

        public Int64 Id { get; private set; }
        public string Name { get; private set; }
        public string MentionName { get; private set; }

        // NYI
        public string Email { get; private set; }
        public bool IsOnline { get; private set; }
        public string StatusMessage { get; private set; }
        public string Status { get; private set; }
        public Int64 Idle { get; private set; }
        public string XmppJid { get; private set; }
    }
}