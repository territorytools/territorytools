using System;

namespace Alba.Controllers.AlbaServer
{
    public class AuthorizationException : Exception
    {
        public AuthorizationException(string message) : base(message)
        {
        }
    }
}
