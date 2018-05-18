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

using System;
using System.Collections.Generic;
using System.Text;

namespace Daenet.DurableTaskMicroservices.Core
{
    /// <summary>
    /// Put this attribute in assembly which implements integration Microservices.
    /// It is required by host loader to load all integration relevant classes from this assembly.
    /// If you don't put this attribute, no any type can be used in serialization process.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class IntegrationAssemblyAttribute : Attribute
    {

    }
}
