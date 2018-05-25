using System;
using System.Collections.Generic;
using System.Text;

namespace Daenet.DurableTaskMicroservices.Core
{
    /// <summary>
    /// All EventIds used in the DurableTaskMicroServices
    /// </summary>
    public static class EventIds
    {
        /// <summary>
        /// EventIds used for ServiceHost.
        /// </summary>
        public static class ServiceHost
        {
            private const int Base = 1;

            /// <summary>
            /// Logged when <see cref="ServiceHost.StartServiceHostAsync"/> is called.
            /// </summary>
            public const int ServiceStarted = Base;

            /// <summary>
            /// Logged in <see cref="ServiceHost.StartServiceHostAsync"/> after config.xml files 
            /// were successfuly loaded.
            /// </summary>
            public const int ConfigLoaded = Base + 1;

            /// <summary>
            /// Logged in <see cref="ServiceHost.StartServiceHostAsync"/> every time a 
            /// new MicroService gets started.
            /// </summary>
            public const int InstanceCreated = Base + 2;

            /// <summary>
            /// Logged in <see cref="ServiceHost.StartServiceHostAsync"/> every time a MicroService fails to start.
            /// </summary>
            public const int InstanceCreationFailed = Base + 3;

            /// <summary>
            /// Logged in <see cref="ServiceHost.StartServiceHostAsync"/> after all MicroServices were started and the TaskHubWorker is ready.
            /// </summary>
            public const int HostCreated = Base + 4;

            /// <summary>
            /// Logged in <see cref="ServiceHost.StartServiceHostAsync"/> if no config.xml file was found.
            /// </summary>
            public const int NoConfigFilesFound = Base + 5;

            /// <summary>
            /// Logged in <see cref="ServiceHost.StartServiceHostAsync"/> if something unexpected happens.
            /// </summary>
            public const int HostFailed = Base + 6;

            /// <summary>
            /// Logged in <see cref="ServiceHost.loadKnownTypes"/> if loading of an Assembly failed.
            /// If the Assembly isn't needed, this log message is not critical.
            /// Some Assemblies like System.Net are not loadable.
            /// </summary>
            public const int AssemblyLoadingFailed = Base + 7;
        }

        /// <summary>
        /// EventIds used for OrchestrationBase.
        /// </summary>
        public static class OrchestrationBase
        {
            private const int Base = 100;

            /// <summary>
            /// Logged in OrchestrationBase on Orchestration started.
            /// </summary>
            public const int OrchestrationStarted = Base;

            /// <summary>
            /// Logged in OrchestrationBase on Orchestration ended.
            /// </summary>
            public const int OrchestrationEnded = Base + 2;

            /// <summary>
            /// Logged in OrchestrationBase on Orchestration failed.
            /// </summary>
            public const int OrchestrationFailed = Base + 3;
        }

        /// <summary>
        /// EventIds used for TaskBase.
        /// </summary>
        public static class TaskBase
        {
            private const int Base = 200;

            /// <summary>
            /// Logged in TaskBase on Task started.
            /// </summary>
            public const int TaskStarted = Base;

            /// <summary>
            /// Logged in TaskBase on Task ended.
            /// </summary>
            public const int TaskEnded = Base + 2;

            /// <summary>
            /// Logged in TaskBase on Task failed.
            /// </summary>
            public const int TaskFailed = Base + 3;
        }
    }
}
