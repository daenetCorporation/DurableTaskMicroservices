using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.Entities
{
    /// <summary>
    /// Defies the result of execution of chain of rules.
    /// </summary>
    public class RulesResult
    {
        public RulesResult()
        {
            this.Results = new Dictionary<string, object>();
        }



        /// <summary>
        /// Dictionary of results of execution of all rules in a chain.
        /// </summary>
        public Dictionary<string, object> Results { get; set; }
    }
}
