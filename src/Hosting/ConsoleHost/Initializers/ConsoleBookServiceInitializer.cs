using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleHost.Initializers
{
    public class ConsoleBookServiceInitializer : MarshalByRefObject, IDisposable
    {
        private  ServiceHost booksSrvHostHost;
        
        public void Dispose()
        {
            booksSrvHostHost.Close();
        }

        public void Initialize()
        {
            booksSrvHostHost = new ServiceHost(typeof(BookService.ServiceImplementation.BookService));
            booksSrvHostHost.Open();

            var initializer = new BookServiceInitializer.BookServiceInitializer("BookService");
            initializer.Initialize();
        }
    }
}
