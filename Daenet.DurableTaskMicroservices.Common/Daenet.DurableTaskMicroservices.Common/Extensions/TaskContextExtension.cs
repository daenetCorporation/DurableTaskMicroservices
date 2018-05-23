//  ----------------------------------------------------------------------------------
//  Copyright daenet Gesellschaft für Informationstechnologie mbH
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  http://www.apache.org/licenses/LICENSE-2.0
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//  ----------------------------------------------------------------------------------
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
