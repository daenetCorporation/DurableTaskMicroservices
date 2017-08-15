using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.Rules
{
    public abstract class RuleBase : IRule
    {
        /// <summary>
        /// Checks if the rule can operate on specified instance.
        /// </summary>
        /// <param name="entityInstance"></param>
        /// <returns></returns>
        public abstract bool CanExecute(object entityInstance);

        
        public abstract object Execute(object entityInstance);        
    }
}
