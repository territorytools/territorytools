using System;
using System.Collections.Generic;
using System.Text;

namespace AlbaConsole
{
    public class NormalException : Exception
    {
        public NormalException(string message) : base(message)
        {
        }

        public NormalException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
