using System.Linq;
using Bibliotheca.Server.Authorization.Heimdall.Core.Parameters;
using Bibliotheca.Server.Mvc.Middleware.Authorization.UserTokenAuthentication;
using Flurl;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neutrino.AspNetCore.Client;

namespace Bibliotheca.Server.Authorization.Heimdall.Api.UserTokenAuthorization
{
    /// <summary>
    /// Class for user token configuration (retrieves user token authorization URL).
    /// </summary>
    public class UserTokenConfiguration : IUserTokenConfiguration
    {
        private readonly ILogger<UserTokenConfiguration> _logger;

        private readonly INeutrinoClient _neutrinoClient;

        IOptions<ApplicationParameters> _applicationParameters;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="neutrinoClient">Service discovery query.</param>
        /// <param name="applicationParameters">Application parameters.</param>
        public UserTokenConfiguration(
            ILogger<UserTokenConfiguration> logger, 
            INeutrinoClient neutrinoClient, 
            IOptions<ApplicationParameters> applicationParameters)
        {
            _logger = logger;
            _neutrinoClient = neutrinoClient;
            _applicationParameters = applicationParameters;
        }

        /// <summary>
        /// Getting authorization URL.
        /// </summary>
        /// <returns>Returns authorization URL (url to authorization service).</returns>
        public string GetAuthorizationUrl()
        {
            _logger.LogInformation("Retrieving authorization url...");

            var services = _neutrinoClient.GetServicesByServiceTypeAsync("authorization").GetAwaiter().GetResult();
            if (services != null && services.Count > 0)
            {
                var instance = services.FirstOrDefault();
                var address = instance.Address.AppendPathSegment("api/");
                _logger.LogInformation($"Authorization url was retrieved ({address}).");

                return address;
            }

            _logger.LogInformation($"Authorization url was not retrieved.");
            return null;
        }
    }
}