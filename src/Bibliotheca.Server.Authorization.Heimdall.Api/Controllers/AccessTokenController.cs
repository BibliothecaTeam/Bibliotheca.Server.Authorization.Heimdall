using Bibliotheca.Server.Authorization.Heimdall.Core.DataTransferObjects;
using Bibliotheca.Server.Authorization.Heimdall.Core.Exceptions;
using Bibliotheca.Server.Authorization.Heimdall.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheca.Server.Authorization.Heimdall.Api.Controllers
{
    /// <summary>
    /// Controller which manages user's access tokens.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/accessToken")]
    public class AccessTokenController : Controller
    {
        private readonly IUsersService _usersService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="usersService">User service.</param>
        public AccessTokenController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        /// <summary>
        /// Returns user information based on user token which is send in headers (UserToken: [TOKEN]).
        /// </summary>
        /// <remarks>
        /// This endpoint is accessibe also for anonymous users. It's exposed for veryfing user's tokens.
        /// </remarks>
        /// <returns>User information when user with specified token exists, if not 404 NotFound.</returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(UserDto))]
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
