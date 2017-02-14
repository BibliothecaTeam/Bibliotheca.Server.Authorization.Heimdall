using System.Threading.Tasks;
using Bibliotheca.Server.Authorization.Abstractions;
using Bibliotheca.Server.Authorization.Abstractions.DataTransferObjects;
using Bibliotheca.Server.Authorization.Heimdall.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheca.Server.Authorization.Heimdall.Api.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    [ApiVersion("1.0")]
    [Route("api/authorization")]
    public class AuthorizationController : Controller, IAuthorizationController
    {
        private readonly IAuthorizationService _authorizationService;

        public AuthorizationController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        [HttpGet("{key}")]
        public async Task<AuthorizationDto> Get(string key)
        {
            return await _authorizationService.GetAsync(key);
        }
    }
}
