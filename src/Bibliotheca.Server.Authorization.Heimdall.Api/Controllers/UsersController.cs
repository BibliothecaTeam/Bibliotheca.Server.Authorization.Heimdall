using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Authorization.Heimdall.Core.DataTransferObjects;
using Bibliotheca.Server.Authorization.Heimdall.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheca.Server.Authorization.Heimdall.Api.Controllers
{
    /// <summary>
    /// Controller for managing users.
    /// </summary>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/users")]
    public class UsersController : Controller
    {
        private readonly IUsersService _usersService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="usersService">Users service.</param>
        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        /// <summary>
        /// Get users lists.
        /// </summary>
        /// <remarks>
        /// Endpoint which return list of users stored in document db.
        /// </remarks>
        /// <returns>List of users.</returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IList<UserDto>))]
        public async Task<IList<UserDto>> Get()
        {
            return await _usersService.GetAsync();
        }

        /// <summary>
        /// Get user by his id.
        /// </summary>
        /// <remarks>
        /// Endpoint for getting user by his id.
        /// </remarks>
        /// <param name="id">User id.</param>
        /// <returns>User data.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(UserDto))]
        public async Task<UserDto> Get(string id)
        {
            return await _usersService.GetAsync(id);
        }

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <remarks>
        /// Endpoint for creating new user in document db.
        /// </remarks>
        /// <param name="user">User data.</param>
        /// <returns>If created successfully endpoint returns 201 (Created).</returns>
        [HttpPost]
        [ProducesResponseType(201)]
        public async Task Post([FromBody] UserDto user)
        {
            await _usersService.CreateAsync(user);
        }

        /// <summary>
        /// Update user data.
        /// </summary>
        /// <remarks>
        /// Endpoint for updating user information in document db.
        /// </remarks>
        /// <param name="id">User id.</param>
        /// <param name="user">User data.</param>
        /// <returns>If updated successfully endpoint returns 200 (Ok).</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        public async Task Put(string id, [FromBody] UserDto user)
        {
            await _usersService.UpdateAsync(id, user);
        }

        /// <summary>
        /// Delete user.
        /// </summary>
        /// <remarks>
        /// Endpoint for deleteing user from document db.
        /// </remarks>
        /// <param name="id">User id.</param>
        /// <returns>If deleted successfully endpoint returns 200 (Ok).</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        public async Task Delete(string id)
        {
            await _usersService.DeleteAsync(id);
        }

        /// <summary>
        /// Refresh access token.
        /// </summary>
        /// <remarks>
        /// Endpoint is resposible for saving new access token for user.
        /// </remarks>
        /// <param name="id">User id.</param>
        /// <param name="accessToken">New access token.</param>
        /// <returns>If token saved successfully endpoint returns 200 (Ok).</returns>
        [HttpPut("{id}/refreshToken")]
        [ProducesResponseType(200)]
        public async Task RefreshToken(string id, [FromBody] AccessTokenDto accessToken)
        {
            await _usersService.RefreshTokenAsync(id, accessToken.AccessToken);
        }
    }
}
