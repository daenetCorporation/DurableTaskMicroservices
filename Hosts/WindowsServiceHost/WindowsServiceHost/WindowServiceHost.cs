using Daenet.DurableTask.Microservices;
using Daenet.DurableTaskMicroservices.Host;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WindowsServiceHost
{
    public partial class WindowServiceHost : ServiceBase
    {

        #region Private Member variables

        private EventLog m_ELog;

        #endregion

        #region Public/Protected Methods

        public WindowServiceHost()
        {
            InitializeComponent();
        }


        protected override void OnStart(string[] args)
        {
            RunService();
        }

        protected override void OnStop()
        {
            m_ELog?.WriteEntry("Stopped", EventLogEntryType.Information, 1);
        }

        internal void RunService()
        {
            try
            {
#if !DEVELOPMENT
            m_ELog = new EventLog("System", ".", "Daenet.DurableTask.Microservices");
#endif

                m_ELog?.WriteEntry("Started", EventLogEntryType.Information, 1);

                Host host = new Host();
                host.StartServiceHost(Environment.CurrentDirectory);
            }
            catch (Exception ex)
            {
                m_ELog?.WriteEntry($"Error: {ex.ToString()}", EventLogEntryType.Error, 1);
                this.Stop();
            }
        }

        #endregion
    }
}
