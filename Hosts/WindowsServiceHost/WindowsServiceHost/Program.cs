using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.WindowsServiceHost
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if DEVELOPMENT
            var svc =  new WindowServiceHost();
            svc.RunService();
            System.Threading.Thread.Sleep(int.MaxValue);
            
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new WindowServiceHost()
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
