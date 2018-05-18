using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTask.SqlInstanceStoreProvider
{
    internal class SqlInstanceStoreConstants
    {
        public const string ExecIdTemplate = "ExecutionId = @ExecutionId{0}";
        public const string InstanceIdTemplate = "InstanceId = @InstanceId{0}";
        public const string ExecInstIdTemplate = "(ExecutionId = @ExecutionId{0} AND InstanceId = @InstanceId{1})";
    }
}
