using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace TerritoryTools.IntegrationTestFramework
{
    public class Assert
    {
        public static List<Assembly> Assemblies = new List<Assembly> ();
        public static List<Test> Tests = new List<Test> ();

        public static void Fail (string message)
        {
            throw new AssertFailedException (message);
        }

        public static void AreEqual(object expected, object actual)
        {
            if (!expected.Equals(actual))
                throw new AssertFailedException (expected, actual);
        }

        public static void AreEqual (object expected, object actual, string message)
        {
            if (!expected.Equals (actual))
                throw new AssertFailedException (expected, actual, message);
        }

        public static void AreEqual (double expected, double actual, double precision, string message)
        {
            double difference = expected = actual;
            if (difference > actual)
                throw new AssertFailedException (expected, actual, message);
        }

        public static void IsNotNull (object actual)
        {
            if (actual == null)
                throw new AssertFailedException (null, actual);
        }

        public static void IsInstanceOf<T> (object obj)
        {
            Type expected = typeof (T);
            Type actual = obj.GetType ();
            if (actual != expected)
                throw new AssertFailedException (expected, actual);
        }

        public static void SummarizeFindResults ()
        {
            Console.WriteLine (Tests.Count + " Tests Found");
        }

        public static int SummarizeTestResults ()
        {
            Console.WriteLine ("Test Result Summary:");
            Console.WriteLine ("  Test Count: " + Tests.Count);

            int passed = Tests.Count (t => t.Passed);
            Console.WriteLine ("  Tests Passed: " + passed);

            int failed = Tests.Count (t => !t.Passed);
            Console.WriteLine ("  Tests Failed: " + failed);

            Console.Write ("  Overall Result: ");
            if (failed == 0) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write ("PASS");
            } else {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("FAIL");
            }

            Console.ResetColor ();
            Console.WriteLine ();

            return failed;
        }

        public static void AddTestsFrom (Assembly assembly)
        {
            if (Assemblies.Contains (assembly))
                throw new Exception ("Assembly '" + assembly.FullName + "' already added.");
            
            Assemblies.Add (assembly);
        }

        public static void FindTests() {
            foreach (var assembly in Assemblies)
                FindTestsOn (assembly);
        }

        public static void FindTestsOn (Assembly assembly) {
             foreach (var module in assembly.Modules) {
                foreach (var type in module.GetTypes ()) {
                    foreach (var method in type.GetMethods ()) {
                        foreach (var attribute in method.CustomAttributes) {
                            if (attribute.AttributeType == typeof (TestAttribute)) {
                                Tests.Add (new Test { 
                                    Method = method, 
                                    Name = method.Name, 
                                    Type = type });
                            }
                        }
                    }
                }
            }
        }

        public static void RunTests ()
        {
            foreach (var test in Tests) {
                test.Run ();
                Console.ForegroundColor = test.Passed ? ConsoleColor.Green : ConsoleColor.Red;
                Console.Write (test.Passed ? "PASS " : "FAIL ");
                Console.ResetColor ();
                Console.Write (test.Name);
                Console.WriteLine ();
                if(!string.IsNullOrWhiteSpace(test.ErrorMessage))
                    Console.WriteLine ("    " + test.ErrorMessage);
            }
        }
    }
}
