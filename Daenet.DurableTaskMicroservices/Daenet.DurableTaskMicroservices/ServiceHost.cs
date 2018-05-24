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


using DurableTask.Core;
using DurableTask.Core.Exceptions;
using DurableTask.Core.Tracing;
using Microsoft.Extensions.Logging;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Daenet.DurableTaskMicroservices.Core
{
    /// <summary>
    /// Implements set of host functionalities for microservicese based on top of
    /// Durable Task Framework.
    /// </summary>
    public class ServiceHost : MicroserviceBase
    {
        #region Private Members


        /// <summary>
        /// Holds the list of dictionaries of service and activity configurations.
        /// </summary>
        private static Dictionary<string, Dictionary<string, object>> m_SvcConfigs = new Dictionary<string, Dictionary<string, object>>();


        private TaskHubWorker m_TaskHubWorker;
        private IOrchestrationServiceInstanceStore m_InstanceStoreService;
        private ILogger m_Logger;
        private static ILoggerFactory m_LoggerFactory;

        #endregion

        #region Initialization Code


        public ServiceHost(IOrchestrationService orchestrationService,
            IOrchestrationServiceClient orchestrationClient,
            IOrchestrationServiceInstanceStore instanceStore,
            bool resetHub = false,
            ILoggerFactory loggerFactory = null)
        {
            m_HubClient = new TaskHubClient(orchestrationClient);
            this.m_TaskHubWorker = new TaskHubWorker(orchestrationService);
            this.m_InstanceStoreService = instanceStore;

            if (loggerFactory != null)
            {
                m_LoggerFactory = loggerFactory;
                m_Logger = m_LoggerFactory.CreateLogger<ServiceHost>();
            }


            if (resetHub)
                orchestrationService.DeleteAsync().Wait();

            int n = 10;
            while (--n > 0)
            {
                try
                {
                    orchestrationService.CreateIfNotExistsAsync().Wait();
                    break;
                }
                catch (AggregateException aggEx)
                {
                    if (n <= 0)
                        throw;

                    if (aggEx.InnerException.Message.Contains("409"))
                    {
                        Thread.Sleep(10000);
                    }
                }
            }
        }

        #region Removed
        //private void startQuery(OrchestrationStateQuery query)
        //{
        //    ThreadPool.QueueUserWorkItem(new WaitCallback((obj) =>
        //    {
        //        while (true)
        //        {
        //            Thread.Sleep(5000);

        //        }
        //    }));
        //}

        //public TaskHubClient createTaskHubClient(bool createInstanceStore = true)
        //{
        //    var settings = GetDefaultHubClientSettings();
        //    //TODO? settings.Services = m_Services;

        //    //var stateProviderSvc = m_Services[TaskHubWorker.StateProviderKeyName] as IStateProvider;
        //    //if (stateProviderSvc == null)startorches
        //    //{

        //    //}     
        //    //else
        //    //{
        //    //    var stateProviderSvc = settings.Services[TaskHubWorker.StateProviderKeyName] as IStateProvider;
        //    //    if (stateProviderSvc == null)
        //    //        throw new ArgumentException("Please specify the IStateProviderService.");
        //    //}
        //    //if (createInstanceStore)
        //    //{
        //    //    return new TaskHubClient(m_TaskHubName, m_ServiceBusConnectionString, m_StorageConnectionString, settings);
        //    //}

        //    if (createInstanceStore)
        //    {
        //        return new TaskHubClient(m_TaskHubName, m_ServiceBusConnectionString, m_StorageConnectionString, settings);
        //    }
        //    else
        //        return new TaskHubClient(m_TaskHubName, m_ServiceBusConnectionString);
        //}

        //public TaskHubWorker CreateTaskHubWorker(IOrchestrationService orchestrationService)
        //{
        //    return new TaskHubWorker(orchestrationService);
        //}

        //internal static TaskHubClientSettings GetDefaultHubClientSettings()
        //{
        //    var settings = new TaskHubClientSettings();
        //    settings.MessageCompressionSettings = new CompressionSettings
        //    {
        //        Style = CompressionStyle.Never,
        //        ThresholdInBytes = 1024
        //    };

        //    return settings;
        //}

        //private TaskHubWorkerSettings getDefaultHubWorkerSettings(CompressionStyle style = CompressionStyle.Threshold)
        //{
        //    var settings = new TaskHubWorkerSettings();
        //    settings.TaskOrchestrationDispatcherSettings.CompressOrchestrationState = true;
        //    settings.MessageCompressionSettings = new CompressionSettings { Style = style, ThresholdInBytes = 1024 };

        //    // TODO? :
        //    //if (m_Services != null)
        //    //    settings.Services = m_Services;

        //    return settings;
        //}
        #endregion

        #endregion

        #region Public Members
        /// <summary>
        /// Restarts an eventually running instance of the microservice service (orchestration).
        /// </summary>
        /// <param name="orchestrationFullQualifiedName"></param>
        /// <param name="inputArgs">Input arguments.</param>
        /// <returns></returns>
        public async Task<MicroserviceInstance> RestartServiceAsync(string orchestrationFullQualifiedName, object inputArgs, Dictionary<string, object> context)
        {
            await StopServiceAsync(orchestrationFullQualifiedName);

            return await StartServiceAsync(orchestrationFullQualifiedName, inputArgs, context);
        }


        /// <summary>
        /// Stops the running service instances.
        /// </summary>
        /// <param name="orchestrationFullQualifiedName"></param>
        public async Task StopServiceAsync(string orchestrationFullQualifiedName)
        {
            var tp = Type.GetType(orchestrationFullQualifiedName);

            List<MicroserviceInstance> runningInstances;

            runningInstances = await loadRunningInstancesAsync(tp.FullName);


            foreach (var inst in runningInstances)
            {
                await this.m_HubClient.TerminateInstanceAsync(inst.OrchestrationInstance);
            }

            while (true)
            {
                runningInstances = await loadRunningInstancesAsync(tp.FullName);
                if (runningInstances.Count == 0)
                    break;
            }
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
        /// Gets the logger instance.
        /// </summary>
        /// <param name="type">Type which defines logger category.</param>
        /// <param name="scopeId">Activity identifier.</param>
        /// <returns></returns>
        public static ILogger GetLogger(Type type, string scopeId = null)
        {
            lock (m_LoggerFactory)
            {
                if (m_LoggerFactory != null)
                {
                    var logger = m_LoggerFactory.CreateLogger(type.FullName);
                    if (scopeId != null)
                        logger.BeginScope(scopeId);

                    return logger;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets the activity identifier from input argument.
        /// </summary>
        /// <param name="orchestrationInput"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static string GetActivityIdFromContext(Dictionary<string, object> context)
        {
            string activityId = Guid.NewGuid().ToString();

            if (context.ContainsKey(MicroserviceBase.cActivityIdCtxName))
            {
                activityId = context[MicroserviceBase.cActivityIdCtxName] as string;
            }
            else
                context.Add(MicroserviceBase.cActivityIdCtxName, activityId);

            return activityId;
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
        /// Registers the collection of services, but it does not start hub worker.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public void LoadServices(ICollection<Microservice> services)
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
                // TODO. Waiting on Microsoft DTF team to provide QueryStates on interface.
                //List<MicroserviceInstance> runningInstances;
                //bool isRunning = loadRunningInstances(svc.OrchestrationQName, out runningInstances);
                //if(runningInstances != null)
                //    allRunningInstances.AddRange(runningInstances);

                RegisterServiceConfiguration(svc);
            }

        }


        /// <summary>
        /// Starts the MicroService Host.
        /// </summary>
        /// <param name="directory">Directory where to search for *.config.xml, *.config.json and assemblies</param>
        /// <param name="searchPattern">Search pattern for configuration files.</param>
        /// <param name="runningInstances">List of currently running instances. In the future ServiceHost will be able to grab the list 
        /// of running instances. Waiting on DTF team.</param>
        public async Task<List<MicroserviceInstance>> StartServiceHostAsync(string directory = null, string searchPattern = "*.config.xml", ICollection<OrchestrationState> runningInstances = null, Dictionary<string, object> context = null)
        {
            try
            {
                m_Logger?.LogInformation(EventIds.ServiceHost.ServiceStarted, "Service started. Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString());

                if (String.IsNullOrEmpty(directory))
                    directory = Environment.CurrentDirectory;

                string[] configFiles = loadConfigFiles(directory, searchPattern);

                List<MicroserviceInstance> instances = new List<MicroserviceInstance>();

                if (configFiles.Length > 0)
                {
                    m_Logger?.LogInformation(EventIds.ServiceHost.ConfigLoaded, "Loaded {0} configuration files.", configFiles.Length);

                    List<Type> knownTypes = new List<Type>();

                    knownTypes.AddRange(loadKnownTypes(directory));
                    if (directory != AppContext.BaseDirectory)
                        knownTypes.AddRange(loadKnownTypes(AppContext.BaseDirectory));

                    List<Microservice> microServices = new List<Microservice>();

                    if (searchPattern.ToLower().EndsWith(".xml"))
                        microServices = LoadServicesFromXml(configFiles, knownTypes);
                    else
                        throw new NotSupportedException("JSON not yet supported!");
                    //instances = LoadServicesFromJson(configFiles, knownTypes, out microServices);


                    foreach (var svc in microServices)
                    {
                        if (svc.AutoStart == false)
                            continue;

                        if (runningInstances == null ||
                            runningInstances.FirstOrDefault(s => s.Name == svc.Orchestration.FullName) == null)
                        {
                            var newInst = StartServiceAsync(svc.OrchestrationQName, svc.InputArgument, context).Result;

                            instances.Add(newInst);

                            m_Logger?.LogInformation(EventIds.ServiceHost.InstanceCreated, "New instance with InstanceId '{instanceId}' created", newInst.OrchestrationInstance.InstanceId);
                        }
                        else
                        {
                            var alreadyRunningInstance = runningInstances.FirstOrDefault(s => s.Name == svc.Orchestration.FullName);

                            instances.Add(new MicroserviceInstance() { OrchestrationInstance = alreadyRunningInstance.OrchestrationInstance });

                            m_Logger?.LogInformation(EventIds.ServiceHost.InstanceCreationFailed, "Service instance of '{orchestration}' not started, because it is singleton and it is running already under instanceId '{instanceId}'.", svc.OrchestrationQName, alreadyRunningInstance.OrchestrationInstance.InstanceId);
                        }
                    }
                    await OpenAsync();

                    m_Logger?.LogInformation(EventIds.ServiceHost.HostCreated, "Host created successfully.");
                }
                else
                {
                    m_Logger?.LogWarning(EventIds.ServiceHost.NoConfigFilesFound, "No {searchPattern} files found in folder: {folder}.", searchPattern, directory);
                    //throw new Exception(String.Format("No {0} files found in folder: {1}.", searchPattern, directory));
                }

                return instances;
            }
            catch (Exception ex)
            {
                m_Logger?.LogError(EventIds.ServiceHost.HostFailed, ex, "Failed to start the Host.");

                throw;
            }
        }


        /// <summary>
        /// Loads MicroService from JSON file and add it to the TaskHub
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public Microservice LoadServiceFromJson(string filePath)
        {
            Microservice microservice;
            var jsonText = File.ReadAllText(filePath);
            microservice = Newtonsoft.Json.JsonConvert.DeserializeObject<Microservice>(jsonText);

            LoadServices(new Microservice[] { microservice });

            return microservice;
        }


        /// <summary>
        /// Loads MicroService from XML file and add it to the TaskHub
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public Microservice LoadServiceFromXml(string filePath, IEnumerable<Type> knownTypes)
        {
            Microservice microservice = deserializeService(filePath, knownTypes);

            LoadServices(new Microservice[] { microservice });

            return microservice;
        }


        /// <summary>
        /// Loads MicroService from XML file and add it to the TaskHub
        /// </summary>
        /// <param name="configFiles">List of configuration files for services.</param>
        /// <returns></returns>
        public List<Microservice> LoadServicesFromXml(string[] configFiles, IEnumerable<Type> knownTypes)
        {
            List<Microservice> microservices = new List<Microservice>();
            foreach (var filePath in configFiles)
            {
                Microservice microservice = deserializeService(filePath, knownTypes);
                microservices.Add(microservice);
            }

            LoadServices(microservices);

            return microservices;
        }


        /// <summary>
        /// Adds the service orchestration to task hub worker.
        /// </summary>
        /// <param name="serviceConfiguraton">The configuration of the service.</param>
        /// <returns></returns>
        public void LoadService(Microservice serviceConfiguraton)
        {
            loadTypesFromQualifiedNames(new List<Microservice> { serviceConfiguraton });

            m_TaskHubWorker.AddTaskOrchestrations(serviceConfiguraton.Orchestration);
            if (serviceConfiguraton.Activities != null && serviceConfiguraton.Activities.Length > 0)
                m_TaskHubWorker.AddTaskActivities(serviceConfiguraton.Activities);

            RegisterServiceConfiguration(serviceConfiguraton);
        }



        public void WaitOnStatus(List<MicroserviceInstance> runningInstances, OrchestrationState state)
        {
            foreach (var inst in runningInstances)
            {
                m_HubClient.TerminateInstanceAsync(inst.OrchestrationInstance);
            }
        }


        /// <summary>
        /// Gets the number of running instances.
        /// </summary>
        /// <param name="microservice"></param>
        /// <returns>Gets the number of running instances of the service.
        /// If m_StorageConnectionString is not confiogured this method returns 0.</returns>
        public async Task<int> GetNumOfRunningInstancesAsync(Microservice microservice)
        {
            if (m_InstanceStoreService == null)
                return -1;

            //var byNameQuery = new OrchestrationStateQuery();
            //byNameQuery.AddStatusFilter(OrchestrationStatus.Running);
            //byNameQuery.AddNameVersionFilter(microservice.Orchestration.FullName);

            //var result = m_HubClient.QueryOrchestrationStates(byNameQuery);

            var res = await loadRunningInstancesAsync(microservice.OrchestrationQName);

            return res.Count;
        }



        /// <summary>
        /// Gets the list of services and their states in runtime repository.
        /// </summary>
        /// <param name="microserviceStateQuery"></param>
        /// <returns>List of microservice states.</returns>
        //public IEnumerable<MicroserviceState> QueryServices(MicroserviceStateQuery microserviceStateQuery)
        //{
        //    var result = m_HubClient.QueryOrchestrationStates(microserviceStateQuery.Query)
        //        .Select(s => new MicroserviceState() { OrchestrationState = s });
        //    return result;
        //}


        /// <summary>
        /// Starts HubWorker. 
        /// </summary>
        /// <param name="resetHub"></param>
        public async Task OpenAsync(bool resetHub = false)
        {
            // Includes details of inner exceptions.
            // throw new Ex() from task will be attached as serialized Details property of exception.
            //m_TaskHubWorker.TaskActivityDispatcher?.IncludeDetails = true;

            await m_TaskHubWorker.StartAsync();
        }

        /// <summary>
        /// Instance of event receiver.
        /// </summary>
        ObservableEventListener m_EventListener;

        /// <summary>
        /// Subscribes receiver for DTF internal trace events. The caller can define a callback function,
        /// which will receive all internally created events.
        /// </summary>
        /// <param name="eventLevel">Level of events to be used.</param>
        /// <param name="onEvent">Action to be registered as event subscriber.</param>
        /// <param name="filter">If null, then all events will be received.
        /// If you want to recieve errors, only pass string "errors" in this parameter.</param>
        public void SubscribeEvents(EventLevel eventLevel, Action<string> onEvent, object filter)
        {
            m_EventListener = new ObservableEventListener();
            m_EventListener.Subscribe(new TraceEventReceiver(onEvent, filter));
            m_EventListener.EnableEvents(DefaultEventSource.Log, eventLevel);
        }

        /// <summary>
        /// Closes (stops) the hub.
        /// </summary>
        /// <param name="resetHub"></param>
        /// <param name="deletInstanceStore"></param>
        public async Task Cleanup(bool resetHub = false, bool deletInstanceStore = true)
        {
            if (resetHub)
                await m_TaskHubWorker.orchestrationService.DeleteAsync(deletInstanceStore);

            await m_TaskHubWorker.StopAsync();
        }


        /// <summary>
        /// Wait on multiple instances to enter one of terminal states.
        /// Instance is running if it is in one of Pending, ContinuedAsNew or Running states.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="microservices">The instancea of the service to wait on.</param>
        public void WaitOnInstances(ServiceHost host, List<MicroserviceInstance> microservices)
        {
            List<Task> waitingTasks = new List<Task>();

            foreach (var microservice in microservices)
            {
                waitingTasks.Add(host.WaitOnInstanceAsync(microservice));
            }

            Task.WaitAll(waitingTasks.ToArray());
        }
        #endregion

        #region Private Members
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

        private static void loadTypesFromQualifiedNames(ICollection<Microservice> services)
        {
            loadOrchestrationTypes(services);
            loadServiceActivityTypes(services);
        }


        /// <summary>
        /// Get all files which matches to *.config.xml
        /// </summary>
        /// <returns></returns>
        private string[] loadConfigFiles(string directory, string searchPattern = "*.config.xml")
        {
            List<string> configFiles = new List<string>();

            foreach (var cfgFile in Directory.GetFiles(directory, searchPattern, SearchOption.AllDirectories))
            {
                configFiles.Add(cfgFile);
            }

            return configFiles.ToArray();
        }


        /// <summary>
        /// Loads all assemblies from specified folder with specified extension and
        /// custom attribute IntegrationAssemblyAttribute.
        /// </summary>
        /// <param name="directory">Folder from which assemblies have to be loaded.</param>
        /// <returns>List of types, which will be used fro deserialization.</returns>
        private Type[] loadKnownTypes(string directory)
        {
            List<Type> types = new List<Type>();

            foreach (var assemblyFile in Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).Where(f => f.ToLower().EndsWith(".dll") || f.ToLower().EndsWith(".exe")))
            {
                try
                {
                    Assembly asm = Assembly.LoadFile(assemblyFile);
                    var attr = asm.GetCustomAttribute(typeof(IntegrationAssemblyAttribute));
                    if (attr != null)
                    {
                        foreach (var type in asm.GetTypes())
                        {
                            if (type.GetCustomAttributes(typeof(DataContractAttribute)).Count() > 0)
                            {
                                types.Add(type);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    m_Logger.LogWarning(EventIds.ServiceHost.AssemblyLoadingFailed, ex, "Failed to load Assembly: {assembly}", assemblyFile);
                }
            }

            return types.ToArray();
        }


        /// <summary>
        /// Checks if at least one orchestration instance is already running.
        /// </summary>
        /// <remarks>If the instance store is NOT configured this method will always return FALSE.</remarks>
        /// <param name="orchestrationFullName">Full qualified name of orchestration. i.e.: Type.FullName</param>
        /// <returns>List of running instances.</returns>
        private async Task<List<MicroserviceInstance>> loadRunningInstancesAsync(string orchestrationFullName)
        {
            List<MicroserviceInstance> runningInstances = new List<MicroserviceInstance>();

            if (m_InstanceStoreService != null)
            {
                var oStates = await loadStatesAsync(orchestrationFullName, true);

                runningInstances = oStates.Where(o => o.OrchestrationStatus == OrchestrationStatus.Running ||
                o.OrchestrationStatus == OrchestrationStatus.ContinuedAsNew ||
                o.OrchestrationStatus == OrchestrationStatus.Pending).
                    Select(i => i.OrchestrationInstance).Select(oi => new MicroserviceInstance()
                    {
                        OrchestrationInstance = oi
                    }).ToList();
            }

            return runningInstances;
        }

        private async Task<IEnumerable<OrchestrationState>> loadStatesAsync(string orchestrationFullName, bool runningOnly = true)
        {
            List<OrchestrationState> list = new List<OrchestrationState>();

            if (m_InstanceStoreService != null)
            {
                var records = await m_InstanceStoreService.GetJumpStartEntitiesAsync(10000);
                foreach (var item in records)
                {
                    list.Add(item.State);
                }
                //var byNameQuery = new OrchestrationStateQuery();
                //byNameQuery.AddNameVersionFilter(orchestrationFullName);
                //if (runningOnly)
                //    byNameQuery.AddStatusFilter(OrchestrationStatus.Running);

                //var oStates = await m_HubClient.hiGetOrchestrationStateAsync("TODO");

                return list;
            }
            else
                return list;
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
                    if (svc.Orchestration == null)
                        throw new TypeMissingException($"The type {svc.OrchestrationQName} cannot be found. Please be sure that AssemblyQualifiedName is correctlly specified in the configuration file.");
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
        #endregion
    }
}
