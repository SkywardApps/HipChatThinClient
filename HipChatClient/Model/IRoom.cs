using System;
using System.Collections.Generic;

namespace HipChatClient.Model
{
    public interface IRoom
    {
        Int64 Id { get; }
        string Name { get; }
        IDictionary<Int64, IRestrictedUser> Participants { get; }
        Int64 AddWebhook(string name, string trigger, string pattern, string callback);
        void ClearWebhooks(string name);
        void SendNotification(string message, string from, string color = "purple", string format = "text");
        void SendUrl(string message, string urlToShare);
    }
}