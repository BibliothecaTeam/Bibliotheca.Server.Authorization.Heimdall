namespace Bibliotheca.Server.Authorization.Heimdall.Core.Parameters
{
    public class ApplicationParameters
    {
        public string SecurityToken { get; set; }
        public string OAuthAuthority { get; set; }
        public string OAuthAudience { get; set; }
    }
}