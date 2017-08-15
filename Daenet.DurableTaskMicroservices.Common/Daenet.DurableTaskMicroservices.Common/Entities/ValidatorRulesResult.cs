using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.Entities
{
    public class ValidatorRulesResult : RulesResult
    {

        /// <summary>
        /// Returns true if at leas one validator has failed.
        /// </summary>
        public bool HasFailed
        {
            get
            {   
                foreach (var keyPair in this.Results)
                {
                    if ((bool)keyPair.Value)
                        return true;
                }

                return false;
            }
        }
    }
}
