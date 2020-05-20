// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

using nsCDEngine.Communication;
using nsCDEngine.Engines.ThingService;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

using CDMyOPCUAClient.Contracts;
using nsCDEngine.BaseClasses;

namespace TheOPCUAClientAPI
{

    internal class ThingCreationParameters
    {
        public bool CreateIfNotExist { get; set; }
        public bool Hidden { get; set; }
        public bool ReadOnly { get; set; }
        public string InstanceId { get; set; }
        public string FriendlyName { get; set; }

        public ThingCreationParameters()
        {
            CreateIfNotExist = true;
            Hidden = true;
        }
        public ThingCreationParameters(ThingCreationParameters p)
        {
            CreateIfNotExist = p.CreateIfNotExist;
            Hidden = p.Hidden;
            ReadOnly = p.ReadOnly;
            InstanceId = p.InstanceId;
            FriendlyName = p.FriendlyName;
        }
    }

    internal class OPCUAParameters
    {
        public bool DisableSecurity { get; set; }
        public string PreferredLocales { get; set; }
        public bool DisableDomainCheck { get; set; }
        public bool AutoConnect { get; set; }
        public bool AcceptUntrustedCertificate { get; set; }
        public bool AcceptInvalidCertificate { get; set; }
        public string AppCertSubjectName { get; set; }
        public int ReconnectPeriod { get; set; } = 5000;
        public int ReconnectCount { get; set; } = 0;
        public int SessionTimeout { get; set; } = 60000;
        public int KeepAliveInterval { get; set; } = 5000;
        public int KeepAliveTimeout { get; set; } = 10000;
        public int OperationTimeout { get; set; } = 60000;
        public bool? SendStatusCode { get; set; }
        public bool? SendServerTimestamp { get; set; }
        public bool? SendPicoSeconds { get; set; }
        public bool? SendSequenceNumber { get; set; }
        public bool? UseSequenceNumberForThingSequence { get; set; }
        public bool? DoNotUsePropsOfProps { get; set; }
        public bool? DoNotWriteArrayElementsAsProperties { get; set; }
        public bool? EnableOPCDataLogging { get; set; }

        public OPCUAParameters() { }
        public OPCUAParameters(OPCUAParameters p)
        {
            DisableSecurity = p.DisableSecurity;
            PreferredLocales = p.PreferredLocales;
            DisableDomainCheck = p.DisableDomainCheck;
            AutoConnect = p.AutoConnect;
            AcceptUntrustedCertificate = p.AcceptUntrustedCertificate;
            AcceptInvalidCertificate = p.AcceptInvalidCertificate;
            AppCertSubjectName = p.AppCertSubjectName;
            ReconnectPeriod = p.ReconnectPeriod;
            ReconnectCount = p.ReconnectCount;
            SessionTimeout = p.SessionTimeout;
            KeepAliveInterval = p.KeepAliveInterval;
            KeepAliveTimeout = p.KeepAliveTimeout;
            OperationTimeout = p.OperationTimeout;
            SendStatusCode = p.SendStatusCode;
            SendServerTimestamp = p.SendServerTimestamp;
            SendPicoSeconds = p.SendPicoSeconds;
            SendSequenceNumber = p.SendSequenceNumber;
            UseSequenceNumberForThingSequence = p.UseSequenceNumberForThingSequence;
            DoNotUsePropsOfProps = p.DoNotUsePropsOfProps;
            DoNotWriteArrayElementsAsProperties = p.DoNotWriteArrayElementsAsProperties;
            EnableOPCDataLogging = p.EnableOPCDataLogging;
        }
    }

    internal class OPCUAConnectParameters
    {
        public string UserName;
        public string Password;

        public OPCUAConnectParameters() { }
        public OPCUAConnectParameters(OPCUAConnectParameters p)
        {
            if (p != null)
            {
                UserName = p.UserName;
                Password = p.Password;
            }
        }
    }

    internal class RegistrationError
    {
        public string PropertyName;
        public string NodeId;
        public string MethodId;
        public string Error;
        public override string ToString()
        {
            if (PropertyName != null)
            {
                return string.Format("Property {0} NodeId {1} : {2}", PropertyName, NodeId, Error);
            }
            if (!String.IsNullOrEmpty(MethodId))
            {
                return string.Format("Method {0}: {1}", MethodId, Error);
            }
            return Error;
        }
    }

    internal class TheOPCUAClient
    {
        public static async Task<TheOPCUAClient> CreateAndInitAsync(TheMessageAddress owner, string serverAddress, OPCUAParameters opcUAParameters = null, OPCUAConnectParameters connectParameters = null, ThingCreationParameters thingCreationParameters = null)
        {
            var client = new TheOPCUAClient(owner, serverAddress, connectParameters);
            if (await client.Initialize(opcUAParameters, thingCreationParameters))
            {
                return client;
            }
            return null;
        }

        public Task<MsgOPCUAReadTagsResponse> ReadTagsAsync(List<string> nodeIds)
        {
            return ReadTagsAsync(nodeIds, this.ReadTagCallTimeout);
        }
        public async Task<MsgOPCUAReadTagsResponse> ReadTagsAsync(List<string> nodeIds, TimeSpan callTimeout)
        {
            var response = await TheCommRequestResponse.PublishRequestJSonAsync<MsgOPCUAReadTags, MsgOPCUAReadTagsResponse>(OwnerAddress, OpcThingAddress, new MsgOPCUAReadTags
            {
                Tags = nodeIds.Select( (n) => new MsgOPCUAReadTags.TagInfo { NodeId = n }).ToList()
            }, callTimeout);
            return response;
        }

        //public static void Browse() { }

        //public async Task<string> RegisterAndReadTagAsync(cdeP property, string nodeId, int samplingInterval = 1000, ChangeTrigger changeTrigger = ChangeTrigger.Value, double deadbandvalue = 0) { return false; }

        public bool RegisterTag(cdeP property, string nodeId, int samplingInterval = 1000, ChangeTrigger changeTrigger = ChangeTrigger.Value, double deadbandvalue = 0)
        {
            if (property.GetThing()?.GetBaseThing() == null)
            {
                return false;
            }
            return _TagRegistrations.TryAdd(property.Name, new TagRegistration
            {
                PropertyName = property.Name,
                TagInfo = new MsgOPCUACreateTags.TagInfo
                {
                    DisplayName = property.Name,
                    NodeId = nodeId,
                    SamplingInterval = samplingInterval,
                    ChangeTrigger = changeTrigger,
                    DeadbandValue = deadbandvalue,
                    HostThingAddress = property.GetThing().GetBaseThing(),
                },
                Error = "Not subscribed",
            });
        }
        public bool UnregisterTag(cdeP property)
        {
            TagRegistration registration;
            return _TagRegistrations.TryRemove(property.Name, out registration);
        }

        public bool RegisterEvent(cdeP property, string nodeId, TheEventSubscription subscriptionInfo)
        {
            if (property.GetThing()?.GetBaseThing() == null)
            {
                return false;
            }
            return _EventRegistrations.TryAdd(property.Name, new EventRegistration
            {
                PropertyName = property.Name,
                EventInfo = new MsgOPCUACreateTags.EventInfo
                {
                    DisplayName = property.Name,
                    NodeId = nodeId,
                    Subscription = subscriptionInfo.Clone(),
                    HostThing = property.GetThing().GetBaseThing(),
                },
                Error = "Not subscribed",
            });
        }
        public bool UnregisterEvent(cdeP property)
        {
            EventRegistration registration;
            return _EventRegistrations.TryRemove(property.Name, out registration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reRegisterAll"></param>
        /// <returns></returns>
        public async Task<List<RegistrationError>> UpdateRegistrationsAsync(bool reRegisterAll = false, bool bulkApply = false)
        {
            var tags = new List<TagRegistration>();
            var methods = new List<MethodRegistration>();
            var events = new List<EventRegistration>();

            foreach (var tagRegistration in _TagRegistrations.Values)
            {
                if (reRegisterAll || !String.IsNullOrEmpty(tagRegistration.Error))
                {
                    tagRegistration.Error = "pending";
                    tags.Add(tagRegistration);
                }
            }
            foreach (var methodRegistration in _MethodRegistrations.Values)
            {
                if (reRegisterAll || !String.IsNullOrEmpty(methodRegistration.Error))
                {
                    methodRegistration.Error = "pending";
                    methods.Add(methodRegistration);
                }
            }

            foreach (var eventRegistration in _EventRegistrations.Values)
            {
                if (reRegisterAll || !String.IsNullOrEmpty(eventRegistration.Error))
                {
                    eventRegistration.Error = "pending";
                    events.Add(eventRegistration);
                }
            }

            if (tags.Count == 0 && methods.Count == 0 && events.Count == 0)
            {
                return new List<RegistrationError>();
            }

            var response = await TheCommRequestResponse.PublishRequestJSonAsync<MsgOPCUACreateTags, MsgOPCUACreateTagsResponse>(OwnerAddress, OpcThingAddress, new MsgOPCUACreateTags
            {
                Tags = tags.Count > 0 ? tags.Select((t) => t.TagInfo).ToList() : null,
                Methods = methods.Count > 0 ? methods.Select((m) => m.MethodInfo).ToList() : null,
                Events = events.Count > 0 ? events.Select(e => e.EventInfo).ToList() : null,
                BulkApply = bulkApply
            });

            if (response == null)
            {
                return null;
            }
            if (response.Results == null)
            {
                return new List<RegistrationError> { new RegistrationError { Error = response.Error, } };
            }

            if (response.Results.Count != tags.Count + methods.Count + events.Count)
            {
                return new List<RegistrationError> { new RegistrationError { Error = String.Format("Internal error (result count mismatch: {0}, expected {1}", response.Results.Count, tags.Count + methods.Count), } };
            }

            var registrationErrors = new List<RegistrationError>();

            int resultIndex = 0;
            var results = response.Results;
            foreach (var tag in tags)
            {
                var error = results[resultIndex].Error;
                if (String.IsNullOrEmpty(error))
                {
                    tag.Error = null;
                }
                else
                { 
                    tag.Error = error;
                    registrationErrors.Add(new RegistrationError
                    {
                        PropertyName = tag.PropertyName,
                        NodeId = tag.TagInfo.NodeId,
                        Error = error,
                    });
                }
                resultIndex++;
            }
            foreach(var method in methods)
            {
                var error = results[resultIndex].Error;
                if (String.IsNullOrEmpty(error))
                {
                    method.Error = null;
                    method.MethodThingAddress = results[resultIndex].MethodThingAddress;
                }
                else
                { 
                    method.Error = error;
                    registrationErrors.Add(new RegistrationError
                    {
                        MethodId = method.MethodId,
                        Error = error,
                    });
                }
                resultIndex++;
            }
            foreach (var eventRegistration in events)
            {
                var error = results[resultIndex].Error;
                if (String.IsNullOrEmpty(error))
                {
                    eventRegistration.Error = null;
                }
                else
                {
                    eventRegistration.Error = error;
                    registrationErrors.Add(new RegistrationError
                    {
                        PropertyName = eventRegistration.PropertyName,
                        NodeId = eventRegistration.EventInfo.NodeId,
                        Error = error,
                    });
                }
                resultIndex++;
            }

            return registrationErrors;
        }

        public List<TagRegistration> GetTagRegistrations()
        {
            return _TagRegistrations.Select(t => t.Value).ToList();
        }

        public List<EventRegistration> GetEventRegistrations()
        {
            return _EventRegistrations.Select(t => t.Value).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <returns>Empty string if tag is properly registered. null if tag is not known at all. Error string if registration has failed or not occurred.</returns>
        public string CheckTagStatus(cdeP property)
        {
            TagRegistration registration;
            if (!_TagRegistrations.TryGetValue(property.Name, out registration))
            {
                return null;
            }
            if (String.IsNullOrEmpty(registration.Error))
            {
                return "";
            }
            return registration.Error;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="methodId">null = use nodeid as methodId</param>
        /// <param name="calltimeout"></param>
        /// <returns></returns>
        public bool RegisterMethod(string objectId, string nodeId, string methodId = null, int calltimeout = 60000)
        {
            if (string.IsNullOrEmpty(nodeId))
            {
                return false;
            }
            if (methodId == null)
            {
                methodId = nodeId;
            }
            return _MethodRegistrations.TryAdd(methodId, new MethodRegistration
            {
                MethodId = methodId,
                Error = "not subscribed",
                MethodInfo = new MsgOPCUACreateTags.MethodInfo
                {
                    ObjectId = objectId,
                    NodeId = nodeId,
                    CallTimeout = calltimeout,
                },
            });
        }

        public bool UnregisterMethod(string methodId)
        {
            MethodRegistration registration;
            return _MethodRegistrations.TryRemove(methodId, out registration);
        }

        public async Task<MsgOPCUAMethodCallResponse> CallMethodAsync(string methodId, Dictionary<string, object> parameters, bool returnRawJSON = false)
        {
            MethodRegistration methodRegistration;
            if (!_MethodRegistrations.TryGetValue(methodId, out methodRegistration))
            {
                return null;
            }
            if (methodRegistration.MethodThingAddress == null)
            {
                await UpdateRegistrationsAsync(false);
            }
            if (methodRegistration.MethodThingAddress == null)
            {
                return null;
            }

            var methodCall = new MsgOPCUAMethodCall { Arguments = parameters, ReturnRawJSON = returnRawJSON };
            MsgOPCUAMethodCallResponse response;
            if (methodRegistration.MethodInfo.CallTimeout > 0)
            {
                response = await TheCommRequestResponse.PublishRequestJSonAsync<MsgOPCUAMethodCall, MsgOPCUAMethodCallResponse>(OwnerAddress, methodRegistration.MethodThingAddress,
                                methodCall, new TimeSpan(0, 0, 0, 0, methodRegistration.MethodInfo.CallTimeout + 15));
            }
            else
            {
                response = await TheCommRequestResponse.PublishRequestJSonAsync<MsgOPCUAMethodCall, MsgOPCUAMethodCallResponse>(OwnerAddress, methodRegistration.MethodThingAddress,
                                methodCall);
            }
            return response;
        }

        public void Connect(OPCUAConnectParameters connectParams = null, Action<ICDEThing, object> callback = null, bool logEssentialOnly = false)
        {
            if (callback != null)
            {
                ConnectAsync(connectParams, logEssentialOnly).ContinueWith((t) => callback(null, t.Result));
            }
            else
            {
                try
                {
                    var taskIgnoreOutcome = ConnectAsync(connectParams, logEssentialOnly);
                }
                catch { }
            }
        }

        public async Task<string> ConnectAsync(OPCUAConnectParameters connectParams = null, bool logEssentialOnly = false)
        {
            if (connectParams != null)
            {
                ConnectParameters = new OPCUAConnectParameters(connectParams);
            }

            if (ConnectParameters != null)
            { 
                // TODO make this work for remote things as well
                var tThing = GetOpcThing();
                TheThing.SetSafePropertyString(tThing, "UserName", ConnectParameters.UserName);
                TheThing.SetSafePropertyString(tThing, "Password", ConnectParameters.Password);
            }

            var response = await TheCommRequestResponse.PublishRequestJSonAsync<MsgOPCUAConnect, MsgOPCUAConnectResponse>(OwnerAddress, OpcThingAddress, new MsgOPCUAConnect { LogEssentialOnly = logEssentialOnly });
            if (response == null)
            {
                return "Error sending connect message";
            }
            return response.Error;
        }

        public void Disconnect(Action<ICDEThing, object> callback = null)
        {
            if (callback != null)
            {
                DisconnectAsync().ContinueWith((t) => callback(null, t.Result));
            }
            else
            {
                var taskNotWaited = DisconnectAsync();
            }

        }
        public async Task<string> DisconnectAsync()
        {
            var response = await TheCommRequestResponse.PublishRequestJSonAsync<MsgOPCUADisconnect, MsgOPCUADisconnectResponse>(OwnerAddress, OpcThingAddress, new MsgOPCUADisconnect());
            if (response == null)
            {
                return "Error sending disconnect message";
            }
            return response.Error;
        }

        public void Dispose()
        {
            _TagRegistrations.Clear();
            _MethodRegistrations.Clear();
        }

        public async Task<bool> DeleteAsync()
        {
            return await TheThingRegistry.DeleteOwnedThingAsync(OpcThingAddress);
        }

        public void RegisterEvent(string pEventName, Action<ICDEThing, object> pCallback)
        {
            // TODO Make this cross-node capable
            GetOpcThing()?.RegisterEvent(pEventName, pCallback);
        }

        public void UnregisterEvent(string pEventName, Action<ICDEThing, object> pCallback)
        {
            GetOpcThing()?.UnregisterEvent(pEventName, pCallback);
        }

        private TheOPCUAClient(TheMessageAddress owner, string serverAddress, OPCUAConnectParameters connectParameters)
        {
            ServerAddress = serverAddress;
            OwnerAddress = owner;
            ConnectParameters = new OPCUAConnectParameters(connectParameters);
            //OpcUAParameters = new OPCUACreationParameters(opcUAParameters);
        }
        private async Task<bool> Initialize(OPCUAParameters opcUAParameters, ThingCreationParameters thingCreation)
        {
            if (thingCreation == null)
            {
                thingCreation = new ThingCreationParameters();
            }

            Dictionary<string,object> properties;

            if (opcUAParameters != null)
            {
                try
                {
                    properties = TheCommonUtils.DeserializeJSONStringToObject<Dictionary<string, object>>(TheCommonUtils.SerializeObjectToJSONString(opcUAParameters));
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                properties = new Dictionary<string, object>();
            }

            var tTaskForThing = TheThingRegistry.CreateOwnedThingAsync(new TheThingRegistry.MsgCreateThingRequestV1
            {
                EngineName = TheOPCUAClientEngines.EngineName,
                DeviceType = eOPCUAClientDeviceTypes.RemoteServer,
                Address = ServerAddress,
                CreateIfNotExist = thingCreation.CreateIfNotExist,
                FriendlyName = thingCreation.FriendlyName,
                Hidden = thingCreation.Hidden,
                ReadOnly = thingCreation.ReadOnly,
                InstanceId = thingCreation.InstanceId,
                OwnerAddress = OwnerAddress,
                Properties = properties,
            }, new TheMessageAddress { Node = TheBaseAssets.MyServiceHostInfo.MyDeviceInfo.DeviceID }, new TimeSpan(0,0,30));
            var tThing = await tTaskForThing;
            if (tThing == null)
            {
                return false;
            }

            OpcThingAddress = tThing;
            _opcThing = tThing; 
            RegisterEvent(eOPCUAClientEvents.ConnectComplete, sinkConnected);
            RegisterEvent(eOPCUAClientEvents.ConnectFailed, sinkConnectFailure);
            RegisterEvent(eOPCUAClientEvents.DisconnectComplete, sinkDisconnected);
            return true;
        }

        #region Initialization parameters
        TheMessageAddress OwnerAddress;
        TheMessageAddress OpcThingAddress;
        private string ServerAddress;
        private OPCUAConnectParameters ConnectParameters;
        #endregion

        // TODO eliminate the last dependencies on the (local) OPC thing: 
        // - Events
        // - property data from OPC to owner thing
        // - username/password property set (encrypted)
        TheThing _opcThing;
        TheThing GetOpcThing()
        {
            if (_opcThing == null)
            {
                _opcThing = TheThingRegistry.GetThingByMID(OpcThingAddress.EngineName, OpcThingAddress.ThingMID);
            }
            return _opcThing;
        }

        public class TagRegistration
        {
            public string PropertyName { get; internal set; }
            public MsgOPCUACreateTags.TagInfo TagInfo;
            public string Error;
        }

        public class EventRegistration
        {
            public string PropertyName { get; internal set; }
            public MsgOPCUACreateTags.EventInfo EventInfo;
            public string Error;
        }


        ConcurrentDictionary<string, TagRegistration> _TagRegistrations = new ConcurrentDictionary<string, TagRegistration>();

        ConcurrentDictionary<string, EventRegistration> _EventRegistrations = new ConcurrentDictionary<string, EventRegistration>();

        class MethodRegistration
        {
            public string MethodId;
            public MsgOPCUACreateTags.MethodInfo MethodInfo;
            public TheMessageAddress MethodThingAddress;
            public string Error;

        }
        ConcurrentDictionary<string, MethodRegistration> _MethodRegistrations = new ConcurrentDictionary<string, MethodRegistration>();

        public bool IsConnected { get; private set; }
        public bool IsReconnecting { get; internal set; } // TODO wire this up
        public TimeSpan ReadTagCallTimeout { get; set; } = new TimeSpan(0, 1, 0);

        public string GetLastMessage()
        {
            return GetOpcThing().LastMessage;
        }
        public int GetStatusLevel()
        {
            return GetOpcThing().StatusLevel;
        }
        public DateTimeOffset GetLastDataReceivedTime()
        {
            return TheThing.GetSafePropertyDate(GetOpcThing(), "LastDataReceivedTime");
        }
        public long GetDataReceivedCount()
        {
            return (long)TheThing.GetSafePropertyNumber(GetOpcThing(), "DataReceivedCount");
        }

        void sinkConnected(ICDEThing tThing, object param)
        {
            IsConnected = true;
        }

        void sinkConnectFailure(ICDEThing tThing, object param)
        {
            IsConnected = false;
        }

        void sinkDisconnected(ICDEThing tThing, object param)
        {
            IsConnected = false;
        }

        public static string GetErrorsAsString(List<RegistrationError> errors)
        {
            if (errors == null)
            {
                return "";
            }
            var errorString = new StringBuilder();
            bool first = true;
            foreach (var error in errors)
            {
                if (!first)
                {
                    errorString.Append(";");
                }
                else
                {
                    first = false;
                }
                errorString.Append(error.ToString());
            }
            return errorString.ToString();
        }

    }

}