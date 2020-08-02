using System;

namespace TerritoryTools.Alba.Controllers.AlbaServer
{
    public class UserException : Exception
    {
        public UserException(string message) : base(message)
        {
        }
    }
}
