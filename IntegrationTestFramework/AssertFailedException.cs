using System;

namespace TerritoryTools.IntegrationTestFramework
{
    [Serializable]
    internal class AssertFailedException : Exception
    {
        Exception innerException;
        object actual;
        object expected;
        string message = string.Empty;

        public AssertFailedException (Exception innerException)
        {
            this.innerException = innerException; 
        }

        public AssertFailedException (string message)
        {
            this.message = message;
        }

        public AssertFailedException(object expected, object actual)
        {
            this.expected = expected;
            this.actual = actual;
        }

        public AssertFailedException (object expected, object actual, string message)
        {
            this.expected = expected;
            this.actual = actual;
        }

        public override string Message
        {
            get
            {
                if (innerException != null)
                    return "Exception: " + innerException.Message;

                return "Expected: " + expected
                    + Environment.NewLine + "    Actual: " + actual
                    + (string.IsNullOrWhiteSpace (message) ? string.Empty : (Environment.NewLine + "    " + message));
            }
        }
    }
}