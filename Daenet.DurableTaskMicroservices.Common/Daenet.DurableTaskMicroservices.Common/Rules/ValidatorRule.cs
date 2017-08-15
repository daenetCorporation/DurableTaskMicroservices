using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.Rules
{

    public abstract class ValidatorRule : RuleBase
    {
        /// <summary>
        /// Executes the rule.
        /// </summary>
        /// <param name="entityInstance">The instance of the rule.</param>
        /// <returns>Result of the rule. TRUE if rule has failed. FALSE if rule has passed validation successfully.</returns>
        public abstract bool Validate(object entityInstance);

        public override object Execute(object entityInstance)
        {
            return Validate(entityInstance);    
        }
    }
}
