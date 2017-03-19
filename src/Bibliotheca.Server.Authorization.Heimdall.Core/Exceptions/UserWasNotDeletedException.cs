using System;

namespace Bibliotheca.Server.Authorization.Heimdall.Core.Exceptions
{
    public class UserWasNotDeletedException : Exception
    {
        public UserWasNotDeletedException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}