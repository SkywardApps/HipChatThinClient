using System.Collections.Generic;
using HipChatClient.Model;

namespace HipChatClient
{
    public interface IHipChatClient
    {
        IDictionary<long, IUser> Users { get; }
        IDictionary<long, IRoom> Rooms { get; }
    }
}