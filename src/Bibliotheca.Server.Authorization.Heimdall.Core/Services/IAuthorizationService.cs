using System.Threading.Tasks;
using Bibliotheca.Server.Authorization.Heimdall.Core.DataTransferObjects;

namespace Bibliotheca.Server.Authorization.Heimdall.Core.Services
{
    public interface IAuthorizationService
    {
        Task<AuthorizationDto> GetAsync(string key);
    }
}