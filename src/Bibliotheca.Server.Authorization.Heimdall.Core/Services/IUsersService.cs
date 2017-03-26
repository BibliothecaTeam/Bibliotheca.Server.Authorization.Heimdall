using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Authorization.Heimdall.Core.DataTransferObjects;

namespace Bibliotheca.Server.Authorization.Heimdall.Core.Services
{
    public interface IUsersService
    {
        Task<IList<UserDto>> GetAsync();
        
        Task<UserDto> GetAsync(string id);

        Task CreateAsync(UserDto user);

        Task UpdateAsync(string id, UserDto user);

        Task DeleteAsync(string id);

        Task RefreshTokenAsync(string id, string accessToken);

        UserDto GetUserByToken(string accessToken);
    }
}