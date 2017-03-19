using System;

namespace Bibliotheca.Server.Authorization.Heimdall.Core.Exceptions
{
    public class UserWasNotCreatedException : Exception
    {
        public UserWasNotCreatedException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}