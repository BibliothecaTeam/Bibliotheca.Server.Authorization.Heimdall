namespace Bibliotheca.Server.Authorization.Heimdall.Core.Parameters
{
    public class ApplicationParameters
    {
        public string SecurityToken { get; set; }
        public string OAuthAuthority { get; set; }
        public string OAuthAudience { get; set; }

        public string EndpointUrl  { get; set; }
        public string AuthorizationKey  { get; set; }
        public string DatabaseId  { get; set; }
        public string CollectionId  { get; set; }

        public ServiceDiscovery ServiceDiscovery { get; set; }
    }
}