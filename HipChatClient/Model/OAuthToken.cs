using System;

namespace HipChatClient.Model
{
    public class OAuthToken
    {
        public OAuthToken(dynamic seed)
        {
            AccessToken = seed.access_token;
            ExpiresIn = seed.expires_in;
            Name = seed.group_name;
            TokenType = seed.token_type;
            Scope = seed.scope;
            GroupId = seed.group_id;
        }
        
        public string AccessToken { get; private set; }
        public Int64 ExpiresIn { get; private set; }
        public string Name { get; private set; }
        public string TokenType { get; private set; }
        public string Scope { get; private set; }
        public Int64 GroupId { get; private set; }
    }
}