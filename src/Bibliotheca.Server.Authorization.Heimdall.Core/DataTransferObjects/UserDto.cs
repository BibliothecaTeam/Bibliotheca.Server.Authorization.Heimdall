using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Bibliotheca.Server.Authorization.Heimdall.Core.DataTransferObjects
{
    public class UserDto
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "role")]
        public RoleEnumDto Role { get; set; }

        [JsonProperty(PropertyName = "accessToken")]
        public string AccessToken { get; set;}

        [JsonProperty(PropertyName = "projects", NullValueHandling = NullValueHandling.Ignore)]
        public IList<string> Projects { get; set; }
    }
}