using System;
using System.Threading.Tasks;
using Bibliotheca.Server.Authorization.Heimdall.Core.DataTransferObjects;
using Bibliotheca.Server.Authorization.Heimdall.Core.Parameters;
using Microsoft.Extensions.Options;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System.Collections.Generic;
using System.Net;
using Bibliotheca.Server.Authorization.Heimdall.Core.Exceptions;
using System.Linq;

namespace Bibliotheca.Server.Authorization.Heimdall.Core.Services
{
    public class UsersService : IUsersService
    {
        private readonly ApplicationParameters _applicationParameters;
        private static readonly ConnectionPolicy connectionPolicy = new ConnectionPolicy { UserAgentSuffix = " heimdall/1.0" };

        public UsersService(IOptions<ApplicationParameters> applicationParameters)
        {
            _applicationParameters = applicationParameters.Value;
        }

        public async Task<IList<UserDto>> GetAsync()
        {
            using (DocumentClient client = new DocumentClient(new Uri(_applicationParameters.EndpointUrl), _applicationParameters.AuthorizationKey, connectionPolicy))
            {
                var collectionLink = UriFactory.CreateDocumentCollectionUri(_applicationParameters.DatabaseId, _applicationParameters.CollectionId);
                var docs = await client.ReadDocumentFeedAsync(collectionLink, new FeedOptions { MaxItemCount = 1000 });

                var users = new List<UserDto>();
                foreach (var doc in docs)
                {
                    var user = (UserDto)doc;
                    ClearAccessToken(user);
                    users.Add(user);
                }

                return users;
            }
        }

        public async Task<UserDto> GetAsync(string id)
        {
            var user = await GetUserAsync(id);
            ClearAccessToken(user);

            return user;
        }

        public async Task CreateAsync(UserDto user)
        {
            try
            {
                using (DocumentClient client = new DocumentClient(new Uri(_applicationParameters.EndpointUrl), _applicationParameters.AuthorizationKey, connectionPolicy))
                {
                    var collectionLink = UriFactory.CreateDocumentCollectionUri(_applicationParameters.DatabaseId, _applicationParameters.CollectionId);
                    await client.CreateDocumentAsync(collectionLink, user);
                }
            }
            catch (DocumentClientException documentClientException)
            {
                throw new UserWasNotCreatedException($"There was an error during creating user '{user.Id}'.", documentClientException);
            }
        }

        public async Task UpdateAsync(string id, UserDto user)
        {
            try
            {
                var existingUser = await GetUserAsync(id);
                if(existingUser != null)
                {
                    user.AccessToken = existingUser.AccessToken;
                }

                using (DocumentClient client = new DocumentClient(new Uri(_applicationParameters.EndpointUrl), _applicationParameters.AuthorizationKey, connectionPolicy))
                {
                    await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_applicationParameters.DatabaseId, _applicationParameters.CollectionId, id), user);
                }
            }
            catch (DocumentClientException documentClientException)
            {
                if (documentClientException.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new UserNotFoundException();
                }

                throw new UserWasNotUpdatedException($"There was an error during updating user '{id}'.", documentClientException);
            }
        }

        public async Task RefreshTokenAsync(string id, string accessToken)
        {
            try
            {
                var user = await GetUserAsync(id);
                if(user != null)
                {
                    user.AccessToken = accessToken;              

                    using (DocumentClient client = new DocumentClient(new Uri(_applicationParameters.EndpointUrl), _applicationParameters.AuthorizationKey, connectionPolicy))
                    {
                        await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_applicationParameters.DatabaseId, _applicationParameters.CollectionId, id), user);
                    }
                }
            }
            catch (DocumentClientException documentClientException)
            {
                if (documentClientException.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new UserNotFoundException();
                }

                throw new UserWasNotUpdatedException($"There was an error during refreshing token for user '{id}'.", documentClientException);
            }
        }

        public async Task DeleteAsync(string id)
        {
            try
            {
                using (DocumentClient client = new DocumentClient(new Uri(_applicationParameters.EndpointUrl), _applicationParameters.AuthorizationKey, connectionPolicy))
                {
                    await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_applicationParameters.DatabaseId, _applicationParameters.CollectionId, id));
                }
            }
            catch (DocumentClientException documentClientException)
            {
                if (documentClientException.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new UserNotFoundException();
                }
                else 
                {
                    throw new UserWasNotDeletedException($"There was an error during deleting user '{id}'.", documentClientException);
                }
            }
        }

        public UserDto GetUserByToken(string accessToken)
        {
            using (DocumentClient client = new DocumentClient(new Uri(_applicationParameters.EndpointUrl), _applicationParameters.AuthorizationKey, connectionPolicy))
            {
                var collectionLink = UriFactory.CreateDocumentCollectionUri(_applicationParameters.DatabaseId, _applicationParameters.CollectionId);

                UserDto user = client.CreateDocumentQuery<UserDto>(collectionLink)
                    .Where(x => x.AccessToken == accessToken).AsEnumerable().FirstOrDefault();

                return user;
            }
        }

        private async Task<UserDto> GetUserAsync(string id)
        {
            try
            {
                using (DocumentClient client = new DocumentClient(new Uri(_applicationParameters.EndpointUrl), _applicationParameters.AuthorizationKey, connectionPolicy))
                {
                    var response = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_applicationParameters.DatabaseId, _applicationParameters.CollectionId, id.ToLower()));

                    UserDto user = (UserDto)(dynamic)response.Resource;
                    return user;
                }
            }
            catch (DocumentClientException documentClientException)
            {
                if (documentClientException.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new UserNotFoundException();
                }

                throw;
            }
        }

        private static void ClearAccessToken(UserDto user)
        {
            if (!string.IsNullOrWhiteSpace(user.AccessToken))
            {
                user.AccessToken = "********-*****-****-****-************";
            }
        }
    }
}