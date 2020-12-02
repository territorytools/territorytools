using System;

namespace TerritoryTools.Alba.Controllers.AlbaServer
{
    public class AuthorizationException : UserException
    {
        public AuthorizationException(string message) : base(message)
        {
        }
    }
}
