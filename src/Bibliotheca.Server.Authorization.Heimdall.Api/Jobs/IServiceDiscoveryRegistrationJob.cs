using System.Threading.Tasks;
using Hangfire.Server;

namespace Bibliotheca.Server.Authorization.Heimdall.Api.Jobs
{
    /// <summary>
    /// Service discovery registration jobs.
    /// </summary>
    public interface IServiceDiscoveryRegistrationJob
    {
        /// <summary>
        /// Job for registering service in service discovery app.
        /// </summary>
        /// <param name="context">Job context.</param>
        /// <returns>Thread task.</returns>
        Task RegisterServiceAsync(PerformContext context);
    }
}