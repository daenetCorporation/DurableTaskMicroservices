using System;
using System.Collections.Generic;
using System.Text;

namespace Daenet.DurableTaskMicroservices.Core
{
    public static class EventIds
    {
        public static class ServiceHost
        {
            private const int Base = 1;

            public const int ServiceStarted = Base;
            public const int ConfigLoaded = Base + 1;
            public const int InstanceCreated = Base + 2;
            public const int InstanceCreationFailed = Base + 3;
            public const int HostCreated = Base + 4;
            public const int NoConfigFilesFound = Base + 5;
            public const int HostFailed = Base + 6;
            public const int AssemblyLoadingFailed = Base + 7;
        }

        public static class OrchestrationBase
        {
            private const int Base = 100;

            public const int OrchestrationStarted = Base;
            public const int OrchestrationEnded = Base + 2;
            public const int OrchestrationFailed = Base + 3;
        }

        public static class TaskBase
        {
            private const int Base = 200;

            public const int TaskStarted = Base;
            public const int TaskEnded = Base + 2;
            public const int TaskFailed = Base + 3;
        }
    }
}
