using Daenet.Common.Logging;
using DurableTask.Core;
using System.Collections.Generic;

namespace Daenet.DurableTaskMicroservices.Common.Extensions
{
    public static class TaskContextExtension
    {
        private static Dictionary<TaskContext, LoggingContext> m_LogManagers = new Dictionary<TaskContext, LoggingContext>();

        internal static void SetLoggingContext(this TaskContext context, LoggingContext logMgr)
        {
            lock (m_LogManagers)
            {
                if (m_LogManagers.ContainsKey(context) == false)
                    m_LogManagers.Add(context, logMgr);
            }
        }

        public static LoggingContext GetLoggingContext(this TaskContext context)
        {
            lock (m_LogManagers)
            {
                if (m_LogManagers.ContainsKey(context))
                    return m_LogManagers[context];
                else
                    return null;
            }
        }
    }
}
