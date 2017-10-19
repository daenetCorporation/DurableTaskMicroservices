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
using System.Threading;
using System.Linq;
using DurableTask.Tracking;
using DurableTask;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using System.Reflection;

namespace Daenet.DurableTask.Microservices
{
    public class ServiceHost
    {
        #region Private Members

        /// <summary>
        /// Holds the list of dictionaries of service and activity configurations.
        /// </summary>
        private static Dictionary<string, Dictionary<string, object>> m_SvcConfigs = new Dictionary<string, Dictionary<string, object>>();

        private TaskHubClient m_HubClient;
        private TaskHubWorker m_TaskHubWorker;

        private string m_ServiceBusConnectionString;
        private string m_StorageConnectionString;
        private string m_TaskHubName;

        private Dictionary<string, object> m_Services;

        #endregion

        #region Initialization Code

        public ServiceHost(string sbConnStr, string storageConnStr, string hubName, bool resetHub = false,
            Dictionary<string, object> services = null)
        {
            this.m_ServiceBusConnectionString = sbConnStr;
            this.m_StorageConnectionString = storageConnStr;
            this.m_TaskHubName = hubName;
            this.m_Services = services;
            if (m_Services == null)
                m_Services = new Dictionary<string, object>();

            initHubs(resetHub);
        }

        public ServiceHost(string sbConnStr, string hubName, bool resetHub = false,
            Dictionary<string, object> services = null)
        {
            this.m_ServiceBusConnectionString = sbConnStr;
            this.m_TaskHubName = hubName;
            this.m_Services = services;
            if (m_Services == null)
                m_Services = new Dictionary<string, object>();

            initHubs(resetHub);
        }


        private void initHubs(bool resetHub = false)
        {
            bool createInstanceStore = true;

            if (String.IsNullOrEmpty(m_StorageConnectionString))
                createInstanceStore = false;

            m_HubClient = createTaskHubClient(createInstanceStore);

            m_TaskHubWorker = CreateTaskHubWorker(createInstanceStore);

            if (resetHub)
                m_TaskHubWorker.DeleteHub();

            m_TaskHubWorker.CreateHubIfNotExists();
        }
        

        public TaskHubClient createTaskHubClient(bool createInstanceStore = true)
        {
            var settings = getDefaultHubClientSettings();
            //TODO? settings.Services = m_Services;

            //var stateProviderSvc = m_Services[TaskHubWorker.StateProviderKeyName] as IStateProvider;
            //if (stateProviderSvc == null)startorches
            //{

            //}     
            //else
            //{
            //    var stateProviderSvc = settings.Services[TaskHubWorker.StateProviderKeyName] as IStateProvider;
            //    if (stateProviderSvc == null)
            //        throw new ArgumentException("Please specify the IStateProviderService.");
            //}
            //if (createInstanceStore)
            //{
            //    return new TaskHubClient(m_TaskHubName, m_ServiceBusConnectionString, m_StorageConnectionString, settings);
            //}

            if (createInstanceStore)
            {
                return new TaskHubClient(m_TaskHubName, m_ServiceBusConnectionString, m_StorageConnectionString, settings);
            }
            else
                return new TaskHubClient(m_TaskHubName, m_ServiceBusConnectionString);
        }

        public TaskHubWorker CreateTaskHubWorker(bool createInstanceStore = true)
        {
            TaskHubWorkerSettings workerSettings = getDefaultHubWorkerSettings();

            if (createInstanceStore)
            {
                return new TaskHubWorker(m_TaskHubName, m_ServiceBusConnectionString, m_StorageConnectionString, workerSettings);
            }

            return new TaskHubWorker(m_TaskHubName, m_ServiceBusConnectionString, workerSettings);
        }

        private static TaskHubClientSettings getDefaultHubClientSettings()
        {
            var settings = new TaskHubClientSettings();
            settings.MessageCompressionSettings = new CompressionSettings
            {
                Style = CompressionStyle.Never,
                ThresholdInBytes = 1024
            };

            return settings;
        }

        private TaskHubWorkerSettings getDefaultHubWorkerSettings(CompressionStyle style = CompressionStyle.Threshold)
        {
            var settings = new TaskHubWorkerSettings();
            settings.TaskOrchestrationDispatcherSettings.CompressOrchestrationState = true;
            settings.MessageCompressionSettings = new CompressionSettings { Style = style, ThresholdInBytes = 1024 };

            // TODO? :
            //if (m_Services != null)
            //    settings.Services = m_Services;

            return settings;
        }
        #endregion


        #region State Transition Listener Implementation

        private static Dictionary<string, OrchestrationState> m_Instances = new Dictionary<string, OrchestrationState>();

        private static OrchestrationStateQuery m_Query;

        public void RegisterStatusChanges(string instanceId, Action<OrchestrationState> state)
        {


            // List<string>
        }

        public void RegisterStatusChanges(Type taskOrchestration, Action<OrchestrationState> state)
        {
            throw new NotImplementedException();
        }

        public void RegisterStatusChanges(Action<OrchestrationState> state)
        {
            throw new NotImplementedException();
        }

        private void startQuery(OrchestrationStateQuery query)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((obj) =>
            {
                while (true)
                {
                    Thread.Sleep(5000);

                }
            }));
        }


        /// <summary>
        /// Checks if at least one orchestration instance is already running.
        /// </summary>
        /// <param name="orchestrationFullName">Full qualified name of orchestration. i.e.: Type.FullName</param>
        /// <param name="runningInstances">Running instances of specified microservice.</param>
        /// <returns>TRUE one or more instances of orchestration are running.</returns>
        private bool loadRunningInstances(string orchestrationFullName, out List<MicroserviceInstance> runningInstances)
        {
            var oStates = loadStates(orchestrationFullName, true);

            var cntRunning = oStates.Count(o => o.OrchestrationStatus == OrchestrationStatus.Running);

            runningInstances = oStates.Where(o => o.OrchestrationStatus == OrchestrationStatus.Running).
                Select(i => i.OrchestrationInstance).Select(oi => new MicroserviceInstance()
                {
                    OrchestrationInstance = oi
                }).ToList();


            return cntRunning > 0;
        }

        private IEnumerable<OrchestrationState> loadStates(string orchestrationFullName, bool runningOnly = true)
        {
            var byNameQuery = new OrchestrationStateQuery();
            byNameQuery.AddNameVersionFilter(orchestrationFullName);
            if (runningOnly)
                byNameQuery.AddStatusFilter(OrchestrationStatus.Running);
            var oStates = m_HubClient.QueryOrchestrationStates(byNameQuery);

            return oStates;
        }

        #endregion

        [Obsolete("Please use LoadServices method", true)]
        public void LoadOrchestrations(Microservice[] descriptors)
        {
            foreach (var oDesc in descriptors)
            {
                LoadService(oDesc);
            }
        }



        private static void loadTypesFromQualifiedNames(ICollection<Microservice> services)
        {
            loadOrchestrationTypes(services);
            loadServiceActivityTypes(services);
        }


        /// <summary>
        /// Loads activity types from Assembly Qualified names.
        /// </summary>
        /// <param name="services">List of services which have to be initialized.</param>
        private static void loadServiceActivityTypes(ICollection<Microservice> services)
        {
            foreach (var svc in services)
            {
                if (svc.Activities == null)
                {
                    var activities = new List<Type>();

                    foreach (var act in svc.ActivityQNames)
                    {
                        activities.Add(Type.GetType(act));
                    }

                    svc.Activities = activities.ToArray();
                }
            }
        }

        /// <summary>
        /// Loads activity types from Assembly Qualified names.
        /// </summary>
        /// <param name="services">List of services which have to be initialized.</param>
        private static void loadOrchestrationTypes(ICollection<Microservice> services)
        {
            foreach (var svc in services)
            {
                if (svc.OrchestrationQName != null)
                {
                    svc.Orchestration = Type.GetType(svc.OrchestrationQName);
                }
            }
        }

//        private static Dictionary<Type, object> m_SvcConfigs = new Dictionary<Type, object>();


        /// <summary>
        /// Gets the key, which uniquely identifies the orchestration (service).
        /// </summary>
        ///<param name="config">Specifies the instance of activity (task) type.</param>
        /// <returns></returns>
        //private static string getConfigKey(IConfig config)
        //{
        //    return getConfigKey(config.Name, config.Type);
        //}


        /// <summary>
        /// Gets the key, which uniquely identifies the orchestration (service).
        /// </summary>
        ///<param name="typeName">Specifies the type of activity (task) type instance.</param>
        //////<param name="name">Specifies the name of activity (task) type instance.</param>
        /// <returns></returns>
        private static string getConfigKey(string name, object typeName)
        {
            return String.Format("{0}-{1}", name, typeName);
        }

        /// <summary>
        /// Adds or updates the service configuration.
        /// </summary>
        /// <param name="service"></param>
        protected void RegisterServiceConfiguration(Microservice service)
        {
            lock (m_SvcConfigs)
            {
                string svcKey = getConfigKey(String.Empty, service.Orchestration);

                if (!m_SvcConfigs.ContainsKey(svcKey))
                {
                    var svcDict = new Dictionary<string, object>();
                    
                    svcDict.Add(svcKey, service);

                    if (service.ActivityConfiguration == null)
                        service.ActivityConfiguration = new Dictionary<string, object>();
                    
                    foreach (var activityConfig in service.ActivityConfiguration)
                    {
                        Type actTp = Type.GetType(activityConfig.Key);
                        if (actTp == null)
                            throw new InvalidOperationException(String.Format("Type '{0}' of Activity (Task) cannot be loaded.", actTp.AssemblyQualifiedName));

                        string actKey = getConfigKey(String.Empty, actTp);

                        if (m_SvcConfigs.ContainsKey(actKey))
                        {
                            throw new InvalidOperationException(String.Format("Activity {0} is already registered.", actKey));
                        }
                        else
                            svcDict.Add(actKey, activityConfig.Value);                       
                    }

                    m_SvcConfigs.Add(svcKey, svcDict);
                }
            }
        }

        ///// <summary>
        ///// Adds or updates the service configuration.
        ///// </summary>
        ///// <param name="activity">Type of activity (task) for which configuration is registering.</param>
        ///// <param name="activityConfig">The configuration of activity.</param>
        //protected void RegisterActivityConfiguration(Type activity, object activityConfig)
        //{
        //    lock (m_SvcConfigs)
        //    {
        //        if (m_SvcConfigs.ContainsKey(activity))
        //        {
        //            m_SvcConfigs[activity] = activityConfig;
                  
        //        }
        //        else
        //            m_SvcConfigs.Add(activity, activityConfig);
        //    }
        //}

        /// <summary>
        /// Gets the configuration of the given microservice (orchestration).
        /// </summary>
        /// <param name="serviceInstanceName">Name of the type instance. Every activity is identified by its type and its name.</param>
        /// <param name="typeName">Type of the activity (task).</param>
        /// <returns></returns>
        public static Microservice GetServiceConfiguration(string serviceInstanceName, Type typeName)
        {
            lock (m_SvcConfigs)
            {
                string key = getConfigKey(serviceInstanceName, typeName);
                if (m_SvcConfigs.ContainsKey(key))
                {
                    var dict = m_SvcConfigs[key];
                    Microservice svcConfig = dict[key] as Microservice;
                    if (svcConfig == null)
                        throw new ArgumentException("Specified microservice (orchestration) type is not registered.");
                    else
                        return svcConfig;
                }
                else
                    throw new ArgumentException("Specified microservice (orchestration) type is not registered.");
            }
        }


        /// <summary>
        /// Gets configuration of activity (task).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">Name of the type instance. Every activity is identified by its type and its name.</param>
        /// <param name="typeName">Type of the activity (task).</param>
        /// <returns>Untyped instance of configuration.</returns>
        public static object GetActivityConfiguration(string name, Type typeName)
        {
            string activityKey = getConfigKey(String.Empty, typeName);
            string orchestrationKey = getConfigKey(String.Empty, name);

            //var actCfg = m_SvcConfigs.SelectMany(i=>i.Value.Where(k=>k.Key == activityKey)).FirstOrDefault();
            var svc = m_SvcConfigs.FirstOrDefault(i => i.Key == orchestrationKey);

            object actCfg = null;
            
            if (svc.Value != null)
            {
                //svc.Value;
               var keyValCfg = svc.Value.FirstOrDefault(i => i.Key == activityKey);

                actCfg = keyValCfg.Value;
            }

            return actCfg;
        }


        /// <summary>
        /// Registers the collection of services.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public ICollection<MicroserviceInstance> LoadServices(ICollection<Microservice> services)
        {
            loadTypesFromQualifiedNames(services);

            m_TaskHubWorker.AddTaskOrchestrations(services.Select(s => s.Orchestration).ToArray());

            List<Type> taskActivities = new List<Type>();
            List<Type> interfaceActivities = new List<Type>();
            foreach (var task in services.SelectMany(s => s.Activities))
            {
                if (taskActivities.Contains(task) == false)
                {
                    if (typeof(TaskActivity).IsAssignableFrom(task))
                        taskActivities.Add(task);
                    else
                        interfaceActivities.Add(task);
                }
            }

            // Interface based tasks (do not implement TaskActivity)
            if (interfaceActivities.Count > 0)
                m_TaskHubWorker.AddTaskActivitiesFromInterface(interfaceActivities);

            // Classic task activities.
            m_TaskHubWorker.AddTaskActivities(taskActivities.ToArray());

            List<MicroserviceInstance> allRunningInstances = new List<MicroserviceInstance>();

            foreach (var svc in services)
            {
                List<MicroserviceInstance> runningInstances;
                bool isRunning = loadRunningInstances(svc.OrchestrationQName, out runningInstances);
                allRunningInstances.AddRange(runningInstances);
                RegisterServiceConfiguration(svc);
            }

            return allRunningInstances;
        }

        /// <summary>
        /// Loads MicroService from JSON file and add it to the TaskHub
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public ICollection<MicroserviceInstance> LoadServiceFromJson(string filePath, out Microservice microservice)
        {
            var jsonText = File.ReadAllText(filePath);
            microservice = Newtonsoft.Json.JsonConvert.DeserializeObject<Microservice>(jsonText);

            return LoadService(microservice);
        }

        /// <summary>
        /// Loads MicroService from XML file and add it to the TaskHub
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public ICollection<MicroserviceInstance> LoadServiceFromXml(string filePath, IEnumerable<Type> knownTypes, out Microservice microservice)
        {
            microservice = deserializeService(filePath, knownTypes);

            return LoadService(microservice);
        }

        /// <summary>
        /// Loads MicroService from XML file and add it to the TaskHub
        /// </summary>
        /// <param name="configFiles">List of configuration files for services.</param>
        /// <returns></returns>
        public ICollection<MicroserviceInstance> LoadServicesFromXml(string[] configFiles, IEnumerable<Type> knownTypes, out ICollection<Microservice> microservices)
        {
            microservices = new List<Microservice>();
            foreach (var filePath in configFiles)
            {
                Microservice microservice = deserializeService(filePath, knownTypes);
                microservices.Add(microservice);
            }

            return LoadServices(microservices);
        }

        /// <summary>
        /// Adds the service orchestration to task hub worker.
        /// </summary>
        /// <param name="serviceConfiguraton">The configuration of the service.</param>
        /// <returns></returns>
        public ICollection<MicroserviceInstance> LoadService(Microservice serviceConfiguraton)
        {
            loadTypesFromQualifiedNames(new List<Microservice> { serviceConfiguraton });

            m_TaskHubWorker.AddTaskOrchestrations(serviceConfiguraton.Orchestration);
            if (serviceConfiguraton.Activities != null && serviceConfiguraton.Activities.Length > 0)
                m_TaskHubWorker.AddTaskActivities(serviceConfiguraton.Activities);

            RegisterServiceConfiguration(serviceConfiguraton);

            List<MicroserviceInstance> runningInstances;

            bool isRunning = loadRunningInstances(serviceConfiguraton.OrchestrationQName, out runningInstances);

            return runningInstances;
        }


        /// <summary>
        /// Restarts an eventually running instance of the microservice service (orchestration).
        /// </summary>
        /// <param name="orchestrationFullQualifiedName"></param>
        /// <param name="inputArgs">Input arguments.</param>
        /// <returns></returns>
        public MicroserviceInstance RestartService(string orchestrationFullQualifiedName, object inputArgs)
        {
            StopService(orchestrationFullQualifiedName);

            return createServiceInstance(orchestrationFullQualifiedName, inputArgs);
        }


        /// <summary>
        /// Stops the running service instance.
        /// </summary>
        /// <param name="orchestrationFullQualifiedName"></param>
        public void StopService(string orchestrationFullQualifiedName)
        {
            var tp = Type.GetType(orchestrationFullQualifiedName);

            List<MicroserviceInstance> runningInstances;

            if (loadRunningInstances(tp.FullName, out runningInstances))
            {
                int cnt = runningInstances.Count;

                foreach (var inst in runningInstances)
                {
                    this.Terminate(inst);
                }

                while (loadRunningInstances(tp.FullName, out runningInstances))
                {
                    Thread.Sleep(500);
                }
            }
        }


        public void WaitOnStatus(List<MicroserviceInstance> runningInstances, OrchestrationState state)
        {
            foreach (var inst in runningInstances)
            {
                this.Terminate(inst);
            }
        }



        /// <summary>
        /// Starts the new instance of the microservice by passing input arguments.
        /// This method will start the new instance of orchestration
        /// </summary>
        /// <param name="orchestrationQualifiedName">The full qualified name of orchestration to be started.</param>
        /// <param name="inputArgs">Input arguments.</param>
        /// <returns></returns>
        public MicroserviceInstance StartService(string orchestrationQualifiedName, object inputArgs)
        {
            return StartService(Type.GetType(orchestrationQualifiedName), inputArgs);
        }


        /// <summary>
        /// Starts the new instance of the microservice by passing input arguments.
        /// This method will start the new instance of orchestration
        /// </summary>
        /// <param name="orchestration">The type of orchestration to be started.</param>
        /// <param name="inputArgs">Input arguments.</param>
        /// <returns></returns>
        public MicroserviceInstance StartService(Type orchestration, object inputArgs)
        {
            return createServiceInstance(orchestration, inputArgs);
        }


        private MicroserviceInstance createServiceInstance(string orchestrationQualifiedName, object inputArgs)
        {
            return createServiceInstance(Type.GetType(orchestrationQualifiedName), inputArgs);
        }

        private MicroserviceInstance createServiceInstance(Type orchestration, object inputArgs)
        {  
            var ms = new MicroserviceInstance()
            {
                OrchestrationInstance = m_HubClient.CreateOrchestrationInstance(orchestration, inputArgs),
            };
            return ms;
        }

        private Microservice deserializeService(string configFile, IEnumerable<Type> knownTypes)
        {
            try
            {
                using (XmlReader writer = XmlReader.Create(configFile))
                {
                    DataContractSerializer ser = new DataContractSerializer(typeof(Microservice), knownTypes);
                    object svc = ser.ReadObject(writer);
                    return svc as Microservice;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to deserialize file: {configFile}", ex);
            }
        }


        /// <summary>
        /// Gets the number of running instances.
        /// </summary>
        /// <param name="microservice"></param>
        /// <returns>Gets the number of running instances of the service.</returns>
        public int GetNumOfRunningInstances(Microservice microservice)
        {
            var byNameQuery = new OrchestrationStateQuery();
            byNameQuery.AddStatusFilter(OrchestrationStatus.Running);
            byNameQuery.AddNameVersionFilter(microservice.Orchestration.FullName);

            var result = m_HubClient.QueryOrchestrationStates(byNameQuery);

            return result.Count();
        }


        /// <summary>
        /// Gets the list of services and their states in runtime repository.
        /// </summary>
        /// <param name="microserviceStateQuery"></param>
        /// <returns>List of microservice states.</returns>
        public IEnumerable<MicroserviceState> QueryServices(MicroserviceStateQuery microserviceStateQuery)
        {
            var result = m_HubClient.QueryOrchestrationStates(microserviceStateQuery.Query)
                .Select(s => new MicroserviceState() { OrchestrationState = s });
            return result;
        }


        /// <summary>
        /// Opens the hosts channels and start execution. 
        /// </summary>
        /// <param name="resetHub"></param>
        public void Open(bool resetHub = false)
        {
            m_TaskHubWorker.Start();

            // Includes details of inner exceptions.
            // throw new Ex() from task will be attached as serialized Details property of exception.
            m_TaskHubWorker.TaskActivityDispatcher.IncludeDetails = true;
        }


        /// <summary>
        /// Closes (stops) the hub.
        /// </summary>
        /// <param name="resetHub"></param>
        public void Cleanup(bool resetHub = false)
        {
            if (resetHub)
                m_TaskHubWorker.DeleteHub();

            m_TaskHubWorker.Stop();
        }


        public void Terminate(MicroserviceInstance svcInst, string reason = "Terminated by host.")
        {
            m_HubClient.TerminateInstance(svcInst.OrchestrationInstance, reason);
        }

        public async void TerminateAasync(MicroserviceInstance svcInst, string reason = "Terminated by host.")
        {
            await m_HubClient.TerminateInstanceAsync(svcInst.OrchestrationInstance, reason);
        }
        

        public void RaiseEvent(MicroserviceInstance instance, string eventName, string data)
        {
            m_HubClient.RaiseEvent(instance.OrchestrationInstance, eventName, data);
        }
    }
}
