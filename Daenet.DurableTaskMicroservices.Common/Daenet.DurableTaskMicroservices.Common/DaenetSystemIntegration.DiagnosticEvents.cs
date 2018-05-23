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
using System.Linq;
using System.Text;

namespace Daenet.DurableTaskMicroservices.Common
{
    
    public static class DiagnosticEvents
    {
        internal const int Base = 1000;
        public const int InfMethodStarted = Base + 2; // Id 1002
        public const int InfMethodCompleted = Base + 3; // Id 1003
        public const int InfCheckDataCmdTextResults = Base + 4; // Id 1004
        public const int InfFetchDataCmdTextResults = Base + 5; // Id 1005
        public const int InfTransactionRolledBack = Base + 7; // Id 1007
        public const int ErrFailedToCommitTransactionRollback = Base + 8; // Id 1008
        public const int ErryAdapterExecution = Base + 10; // Id 1010
        public const int ErrValidationRule = Base + 11; // Id 1011
        public const int ErrTaskFailedException = Base + 12; // Id 1012
        public const int ErrOrchestrationCompensationFailed = Base + 14; // Id 1014
        public const int ErrLoggingSystemFailed = Base + 15; // Id 1015
        public const int ErrOrchestrationFailed = Base + 16; // Id 1016
        public const int ErrFileReceiveAdapter = Base + 17; // Id 1017
        public const int InfStartFileReceiveAdapter = Base + 18; // Id 1018
        public const int InfEndFileReceiveAdapter = Base + 19; // Id 1019
    
    }

}