using System;

namespace Bibliotheca.Server.Authorization.Heimdall.Core.Exceptions
{
    public class UserWasNotUpdatedException : Exception
    {
        public UserWasNotUpdatedException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}