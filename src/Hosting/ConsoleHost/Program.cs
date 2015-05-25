using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleHost.Initializers;

namespace ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Initialize(args);

            Console.WriteLine("\nPress <Enter> to terminate the Host application.");
            Console.WriteLine();
            Console.ReadLine();
        }
        
        private static T PrepareInstanceInNewAppDomain<T>(string appDomainName)
        {
            AppDomain regDomain = AppDomain.CreateDomain(appDomainName);
            return ((T)regDomain.CreateInstanceAndUnwrap(typeof(T).Assembly.GetName().Name, typeof(T).FullName));
        }

        private static void Initialize(string[] cmdLineStrings)
        {
            var cmdLine = new List<string>(cmdLineStrings);

            var initializer = PrepareInstanceInNewAppDomain<ConsoleBookServiceInitializer>("BookService");
            initializer.Initialize();
            Console.WriteLine("BookService service initialized.");
        }
    }
}
