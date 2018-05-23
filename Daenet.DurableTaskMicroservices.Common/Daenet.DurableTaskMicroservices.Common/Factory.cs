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
using Daenet.DurableTaskMicroservices.Common.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common
{
    public interface IAdapterMapper
    {
        object Map(object input);
    }

    /// <summary>
    /// Factory for creation of mapper instances.
    /// </summary>
    public class Factory
    {
        /// <summary>
        /// Creates the instance of IAdapterMapper frim qualified name.
        /// </summary>
        /// <param name="mapperQualifiedName"></param>
        /// <returns></returns>
        public static IAdapterMapper GetAdapterMapper(string mapperQualifiedName)
        {
            var mpType = Type.GetType(mapperQualifiedName);
            if (mpType == null)
                throw new Exception(String.Format("Cannot create the type '{0}'.", mapperQualifiedName));

            IAdapterMapper mapper = Activator.CreateInstance(mpType) as IAdapterMapper;
            if (mapper == null)
                throw new Exception(String.Format("Cannot create the instance of mapper '{0}'.", mpType));

            return mapper;
        }


        /// <summary>
        /// Creates the instance of the rule from qualified name.
        /// </summary>
        /// <param name="ruleQualifiedName"></param>
        /// <returns></returns>
        public static IRule GetRule(string ruleQualifiedName)
        {
            var ruleType = Type.GetType(ruleQualifiedName);
            if (ruleType == null)
                throw new Exception(String.Format("Cannot create the type '{0}'.", ruleQualifiedName));

            IRule ruleInstance = Activator.CreateInstance(ruleType) as IRule;
            if (ruleInstance == null)
                throw new Exception(String.Format("Cannot create the instance of mapper '{0}'.", ruleType));

            return ruleInstance;
        }
    }
}
