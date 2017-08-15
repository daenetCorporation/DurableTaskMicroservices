using Daenet.DurableTaskMicroservices.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.Exceptions
{
    [Serializable]
    public class ValidationRuleException : Exception, ISerializable
    {
        public ValidatorRulesResult ValidationResult { get; set; }

        /// <summary>
        /// The object which didn;t pass validation.
        /// </summary>
        public object ValidatingInstance { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result">The validation result.</param>
        /// <param name="reason">The message which describes the general problem which caused exception to be thrown.</param>
        public ValidationRuleException(ValidatorRulesResult result, string reason, object validatingInstance)
        {
            this.ValidationResult = result;
            this.ValidatingInstance = validatingInstance;
        }

        protected ValidationRuleException(SerializationInfo info, StreamingContext context) : base(info, context) 
        {
            this.ValidatingInstance = info.GetValue("ValidatingInstance", typeof(object));
            this.ValidationResult = (ValidatorRulesResult) info.GetValue("ValidationResult", typeof(ValidatorRulesResult));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            info.AddValue("ValidatingInstance", this.ValidatingInstance);
            info.AddValue("ValidationResult", this.ValidationResult);

            base.GetObjectData(info, context);
        }


    }
}
