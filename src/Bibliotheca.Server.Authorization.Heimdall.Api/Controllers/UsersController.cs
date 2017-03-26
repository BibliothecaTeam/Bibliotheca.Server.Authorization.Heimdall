using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Authorization.Heimdall.Core.DataTransferObjects;
using Bibliotheca.Server.Authorization.Heimdall.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheca.Server.Authorization.Heimdall.Api.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/users")]
    public class UsersController : Controller
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet]
        public async Task<IList<UserDto>> Get()
        {
            return await _usersService.GetAsync();
        }

        [HttpGet("{id}")]
        public async Task<UserDto> Get(string id)
        {
            return await _usersService.GetAsync(id);
        }

        [HttpPost]
        public async Task Post([FromBody] UserDto user)
        {
            await _usersService.CreateAsync(user);
        }

        [HttpPut("{id}")]
        public async Task Put(string id, [FromBody] UserDto user)
        {
            await _usersService.UpdateAsync(id, user);
        }

        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            await _usersService.DeleteAsync(id);
        }

        [HttpPut("{id}/refreshToken")]
        public async Task RefreshToken(string id, [FromBody] AccessTokenDto accessToken)
        {
            await _usersService.RefreshTokenAsync(id, accessToken.AccessToken);
        }
    }
}
