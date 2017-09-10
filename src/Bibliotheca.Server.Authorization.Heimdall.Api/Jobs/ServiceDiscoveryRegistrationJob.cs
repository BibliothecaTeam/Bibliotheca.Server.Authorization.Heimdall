using System;
using System.Threading.Tasks;
using Bibliotheca.Server.Authorization.Heimdall.Core.Parameters;
using Hangfire.Server;
using Microsoft.Extensions.Options;
using Neutrino.AspNetCore.Client;
using Neutrino.Entities.Model;

namespace Bibliotheca.Server.Authorization.Heimdall.Api.Jobs
{
    /// <summary>
    /// Class for registering service in service discovery application.
    /// </summary>
    public class ServiceDiscoveryRegistrationJob : IServiceDiscoveryRegistrationJob
    {
        private readonly INeutrinoClient _neutrinoClient;

        private readonly ApplicationParameters _applicationParameters;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="neutrinoClient">Service discovery client.</param>
        /// <param name="options">Application parameters.</param>
        public ServiceDiscoveryRegistrationJob(
            INeutrinoClient neutrinoClient,
            IOptions<ApplicationParameters> options)
        {
            _neutrinoClient = neutrinoClient;
            _applicationParameters = options.Value;
        }

        /// <summary>
        /// Job for registering service in service discovery app.
        /// </summary>
        /// <param name="context">Job context.</param>
        /// <returns>Thread task.</returns>
        public async Task RegisterServiceAsync(PerformContext context)
        {
            var service = CreateServiceInformation();
            var registeredService = await _neutrinoClient.GetServiceByIdAsync(service.Id);
            if(registeredService == null)
            {
                await _neutrinoClient.AddServiceAsync(service);
            }
            else
            {
                await _neutrinoClient.UpdateServiceAsync(service.Id, service);
            }
        }

        private Service CreateServiceInformation()
        {
            var options = new Service
            {
                Id = _applicationParameters.ServiceDiscovery.ServiceId,
                ServiceType = _applicationParameters.ServiceDiscovery.ServiceType,
                Address = _applicationParameters.ServiceDiscovery.ServiceAddress,
                HealthCheck = new HealthCheck {
                    Address = _applicationParameters.ServiceDiscovery.ServiceHttpHealthCheck,
                    DeregisterCriticalServiceAfter = 240,
                    HealthCheckType = HealthCheckType.HttpRest,
                    Interval = 30
                },
                Tags = _applicationParameters.ServiceDiscovery.ServiceTags
            };

            return options;
        }
    }
}