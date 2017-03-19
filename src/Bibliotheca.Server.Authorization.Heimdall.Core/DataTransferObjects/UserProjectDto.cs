using Newtonsoft.Json;

namespace Bibliotheca.Server.Authorization.Heimdall.Core.DataTransferObjects
{
    public class UserProjectDto
    {
        [JsonProperty(PropertyName = "projectId")]
        public string ProjectId { get; set; }

        [JsonProperty(PropertyName = "guid")]
        public string Guid { get; set; }
    }
}