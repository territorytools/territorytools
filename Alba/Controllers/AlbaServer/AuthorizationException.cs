using System;

namespace TerritoryTools.Alba.Controllers.AlbaServer
{
    public class AuthorizationException : Exception
    {
        public AuthorizationException(string message) : base(message)
        {
        }
    }
}
