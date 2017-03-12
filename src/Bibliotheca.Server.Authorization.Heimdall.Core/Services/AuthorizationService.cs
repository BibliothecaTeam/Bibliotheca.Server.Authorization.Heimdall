using System;
using System.Threading.Tasks;
using Bibliotheca.Server.Authorization.Heimdall.Core.DataTransferObjects;
using Bibliotheca.Server.Authorization.Heimdall.Core.Parameters;
using Microsoft.Extensions.Options;

namespace Bibliotheca.Server.Authorization.Heimdall.Core.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IOptions<ApplicationParameters> _applicationParameters;

        public AuthorizationService(IOptions<ApplicationParameters> applicationParameters)
        {
            _applicationParameters = applicationParameters;
        }

        public Task<AuthorizationDto> GetAsync(string key)
        {
            throw new NotImplementedException();
        }
    }
}