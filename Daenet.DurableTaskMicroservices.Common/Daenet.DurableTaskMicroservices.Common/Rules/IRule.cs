using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.Rules
{
    public interface IRule
    {
        /// <summary>
        /// Checks if the rule can operate on specified instance.
        /// </summary>
        /// <param name="entityInstance"></param>
        /// <returns></returns>
        bool CanExecute(object entityInstance);

        /// <summary>
        /// Executes the rule.
        /// </summary>
        /// <param name="entityInstance">The instance of the rule.</param>
        /// <returns>Result of the rule.</returns>
        object Execute(object entityInstance);
    }
}
