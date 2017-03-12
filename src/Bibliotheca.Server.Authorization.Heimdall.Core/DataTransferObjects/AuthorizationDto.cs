using System.Collections.Generic;

namespace Bibliotheca.Server.Authorization.Heimdall.Core.DataTransferObjects
{
    public class AuthorizationDto
    {
        public IList<string> Roles { get; set; }    
    }
}