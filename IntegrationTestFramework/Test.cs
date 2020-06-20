using System;
using System.Reflection;

namespace TerritoryTools.IntegrationTestFramework
{
    public class Test
    {
        public Type Type;
        public MethodInfo Method;
        public string Name;
        public bool Passed;
        public string ErrorMessage;

        public void Run () {
            try {
                object obj = Activator.CreateInstance (Type);
                Method.Invoke (obj, new object [] { });
                Passed = true;
            } catch (Exception e) {
                Passed = false;
                ErrorMessage = e.InnerException.Message;
            }
        }
    }
}

