using System;

namespace HipChatClient.Model
{

    public class MessageFile
    {
        public string url { get; set; }
    }
    public class MessageSender
    {
        public string name { get; set; }
    }
    public class MessageMessage
    {
        public string message { get; set; }
        public MessageSender from { get; set; }
        public MessageFile file { get; set; }
    }

    public class MessageRoom
    {
        public Int64 id { get; set; }
    }

    public class MessageItem
    {
        public MessageMessage message { get; set; }
        public MessageRoom room { get; set; }
    }
    public class MessagePosting
    {
        public MessageItem item { get; set; }
    }
}