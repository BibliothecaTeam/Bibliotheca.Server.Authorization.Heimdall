using Bibliotheca.Server.Authorization.Heimdall.Core.Exceptions;
using Bibliotheca.Server.Authorization.Heimdall.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheca.Server.Authorization.Heimdall.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/accessToken")]
    public class AccessTokenController : Controller
    {
        private readonly IUsersService _usersService;

        public AccessTokenController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            if(!HttpContext.Request.Headers.ContainsKey("UserToken"))
            {
                throw new AccessTokenHeaderNotSpecifiedException();
            }

            string accessToken = HttpContext.Request.Headers["UserToken"];
            var user = _usersService.GetUserByToken(accessToken);
            if(user != null)
            {
                return new ObjectResult(user);
            }

            return NotFound();
        }
    }
}
