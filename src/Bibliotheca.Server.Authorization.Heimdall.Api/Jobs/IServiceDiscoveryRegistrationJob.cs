using System.Threading.Tasks;
using Hangfire.Server;

namespace Bibliotheca.Server.Authorization.Heimdall.Api.Jobs
{
    public interface IServiceDiscoveryRegistrationJob
    {
        Task RegisterServiceAsync(PerformContext context);
    }
}