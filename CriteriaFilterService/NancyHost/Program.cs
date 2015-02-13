using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy.Hosting.Self;
using System.Diagnostics;

namespace NancyHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var hostConfig = new HostConfiguration() { UrlReservations = new UrlReservations(){CreateAutomatically = true}};

            var nancyHost = new Nancy.Hosting.Self.NancyHost(new Uri("http://localhost:8888/"), new Nancy.DefaultNancyBootstrapper(), hostConfig);
            nancyHost.Start();
            
            Console.WriteLine("Nancy now listening - navigating to http://localhost:8888/. Press enter to stop");
            Process.Start("http://localhost:8888/");
            Console.ReadKey();

            nancyHost.Stop();

            Console.WriteLine("Stopped. Good bye!");
        }
    }
}
