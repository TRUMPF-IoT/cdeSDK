// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿/*********************************************************************
*
* Project Name" 185-SenderBase
*
* Description: 
*
* Date of creation: 
*
* Author: 
*
* NOTES:
*               "FldOrder" for UX 10 to 
*********************************************************************/
//#define TESTDIRECTUPDATES
using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using System.Threading;

using nsCDEngine.BaseClasses;
using nsCDEngine.ViewModels;
using nsCDEngine.Engines.ThingService;

using nsTheThingToPublish;

namespace nsTheSenderBase
{

    public class TheSenderThing : TheDataBase, nsTheEventConverters.IThingToConvert
    {
        public string ThingMID { get { return _thingMid; } set { _thingMid = value; _thing = null; ThingMidAsGuid = TheCommonUtils.CGuid(value); } }
        string _thingMid;
        public bool ContinueMatching { get; set; }
        public string EventFormat { get; set; }
        public bool Disable { get; set; }
        public bool ChangeNaNToNull { get; set; }
        public string PartitionKey { get; set; }
        public string TargetType { get; set; }  //NEW: allows to set a target type for the service data is supposed to be sent to. Ignored if a TheSenderBase override does not use it
        public string TargetName { get; set; }  //NEW: allows to set a different target name for the service data is supposed to be sent to. Ignored if a TheSenderBase override does not use it
        public string TargetUnit { get; set; }  //NEW: allows to set a different target units for the service data is supposed to be sent to. Ignored if a TheSenderBase override does not use it

        public static string[] PartitionKeyChoices = new string[] { "None", "Address", "ThingId", "Fixed" };
        public string GetPartitionKeyValue(TheThingStore thingEvent)
        {
            if (PartitionKey == null || PartitionKey == PartitionKeyChoices[0])
            {
                return null;
            }
            if (PartitionKey == PartitionKeyChoices[1])
            {
                return GetThing()?.GetBaseThing()?.Address;
            }
            if (PartitionKey == PartitionKeyChoices[2])
            {
                return ThingMID;
            }
            return "0";
        }

        TheThing _thing;

        [NonSerialized]
        public Guid ThingMidAsGuid;
        public TheThing GetThing()
        {
            if (_thing == null)
            {
                _thing = TheThingRegistry.GetThingByMID("*", TheCommonUtils.CGuid(this.ThingMID), true);
            }
            return _thing;
        }
        
        public bool IsTemplate()
        {
            return TheCommonUtils.CGuid(ThingMID) == Guid.Empty;
        }

        // Override if new properties in a derived class need to be considered in the comparison
        internal virtual bool IsEqual(TheSenderThing senderThingToAdd)
        {
            return
                senderThingToAdd != null
                //&& cdeMID == senderThingToAdd.cdeMID
                && ChangeBufferLatency == senderThingToAdd.ChangeBufferLatency
                && ChangeBufferTimeBucketSize == senderThingToAdd.ChangeBufferTimeBucketSize
                && ChangeNaNToNull == senderThingToAdd.ChangeNaNToNull
                && Disable == senderThingToAdd.Disable
                && PartitionKey == senderThingToAdd.PartitionKey
                && RetryLastValueOnly == senderThingToAdd.RetryLastValueOnly
                && EventFormat == senderThingToAdd.EventFormat
                && StaticProperties == senderThingToAdd.StaticProperties
                && AddThingIdentity == senderThingToAdd.AddThingIdentity
                && TheCommonUtils.CGuid(ThingMID) == TheCommonUtils.CGuid(senderThingToAdd.ThingMID)
                && PropertiesIncluded == senderThingToAdd.PropertiesIncluded
                && PropertiesExcluded == senderThingToAdd.PropertiesExcluded
                && PreserveOrder == senderThingToAdd.PreserveOrder
                && SendUnchangedValue == senderThingToAdd.SendUnchangedValue
                && SendInitialValues == senderThingToAdd.SendInitialValues
                && IgnoreExistingHistory== senderThingToAdd.IgnoreExistingHistory
                && TokenExpirationInHours == senderThingToAdd.TokenExpirationInHours
                && KeepDurableHistory == senderThingToAdd.KeepDurableHistory
                && MaxHistoryCount == senderThingToAdd.MaxHistoryCount
                && MaxHistoryTime == senderThingToAdd.MaxHistoryTime
                && IgnorePartialFailure == senderThingToAdd.IgnorePartialFailure
                && EngineName == senderThingToAdd.EngineName
                && FriendlyName == senderThingToAdd.FriendlyName
                && DeviceType == senderThingToAdd.DeviceType
                && PropertiesToMatch == senderThingToAdd.PropertiesToMatch
                && ContinueMatching == senderThingToAdd.ContinueMatching
                ;
        }

        /// <summary>
        /// Determines if the history parameters for two sender things can be satisfied by the same historian request, only differing in the properties requested
        /// </summary>
        /// <param name="senderThing"></param>
        /// <returns></returns>
        internal bool IsMatching(TheSenderThing senderThing)
        {
            return ThingMidAsGuid == senderThing.ThingMidAsGuid
                && StaticProperties == senderThing.StaticProperties
                && SendUnchangedValue == senderThing.SendUnchangedValue
                && SendInitialValues == senderThing.SendInitialValues
                && IgnoreExistingHistory == senderThing.IgnoreExistingHistory
                && TokenExpirationInHours == senderThing.TokenExpirationInHours
                && RetryLastValueOnly == senderThing.RetryLastValueOnly
                && PreserveOrder == senderThing.PreserveOrder
                && MaxHistoryTime == senderThing.MaxHistoryTime
                && MaxHistoryCount == senderThing.MaxHistoryCount
                && KeepDurableHistory == senderThing.KeepDurableHistory
                && IgnorePartialFailure == senderThing.IgnorePartialFailure
                && EventFormat == senderThing.EventFormat
                //&& Disable == senderThing.Disable
                && ChangeNaNToNull == senderThing.ChangeNaNToNull
                && ChangeBufferLatency == senderThing.ChangeBufferLatency
                && ChangeBufferTimeBucketSize == senderThing.ChangeBufferTimeBucketSize
                && AddThingIdentity == senderThing.AddThingIdentity
                ;
        }

        public bool PreserveOrder { get; set; }
        public uint ChangeBufferLatency { get; set; } // CooldownPeriod: keep old name for storage mirror compatibility
        public uint ChangeBufferTimeBucketSize { get; set; } // SamplingWindow: keep old name for storage mirror compatibility
        public bool RetryLastValueOnly { get; set; }
        public bool SendUnchangedValue { get; set; }
        public bool? SendInitialValues { get; set; }
        public bool? IgnoreExistingHistory { get; set; }
        /// <summary>
        /// Indicates the time of inactivity on the history token after which the token should be removed automatically. 0 or null indicate infinite lifetime (explicit unregistration required).
        /// </summary>
        public uint? TokenExpirationInHours { get; set; }

        public Guid HistoryToken { get; set; }

        public string PropertiesIncluded { get; set; }
        public string PropertiesExcluded { get; set; }
        /// <summary>
        /// List of properties and values that are to be added to each thing update that gets sent. Format: propName=propValue,propName2=propValue2.
        /// </summary>
        public string StaticProperties
        {
            get
            {
                if (_staticProperties == null)
                {
                    return null;
                }
                return CDictToString(_staticProperties);
            }
            set
            {
                if (value == null)
                {
                    _staticProperties = null;
                }
                else
                {
                    _staticProperties = CStringToDict(value);
                }
            }
        }
        Dictionary<string, object> _staticProperties;

        public bool AddThingIdentity { get; set; }
        public bool KeepDurableHistory { get; set; }
        public int MaxHistoryCount { get; set; }
        public uint MaxHistoryTime { get; set; }
        public bool IgnorePartialFailure { get; set; }

        /// <summary>
        /// List of properties and values that are to be used to find a matching thing when no ThingMID is specified. Format: propName=propValue,propName2=propValue2.
        /// </summary>
        public string PropertiesToMatch
        {
            get
            {
                if (_propertiesToMatch == null)
                {
                    return null;
                }
                return CDictToString(_propertiesToMatch);
            }
            set
            {
                if (value == null)
                {
                    _propertiesToMatch = null;
                }
                else
                {
                    _propertiesToMatch = CStringToDict(value);
                }
            }
        }
        Dictionary<string, object> _propertiesToMatch;
        public Dictionary<string, object> GetPropertiesToMatch()
        {
            var props = _propertiesToMatch;
            if (!string.IsNullOrEmpty(EngineName))
            {
                if (props == null) props = new Dictionary<string, object>();
                props[nameof(EngineName)] = EngineName;
            }
            if (!string.IsNullOrEmpty(DeviceType))
            {
                if (props == null) props = new Dictionary<string, object>();
                props[nameof(DeviceType)] = DeviceType;
            }
            if (!string.IsNullOrEmpty(FriendlyName))
            {
                if (props == null) props = new Dictionary<string, object>();
                props[nameof(FriendlyName)] = FriendlyName;
            }
            return props;
        }

        public string EngineName { get; set; } // Used by some senders to indicate all things of a certain engine name/devicetype - ThingMid must be null/empty
        public string FriendlyName { get; set; }
        public string DeviceType { get; set; }// Used by some senders to indicate all things of a certain engine name/devicetype - ThingMid must be null/empty

        public List<string> GetPropsIncluded()
        {
            return GetPropertiesAsList(PropertiesIncluded).Select(p => p.Split(new char[] { ';' }, 2)[0]).ToList();
        }
        public List<string[]> GetPropsAndParams()
        {
            return GetPropertiesAsList(PropertiesIncluded).Select(p => p.Split(new char[] { ';' })).ToList();
        }

        public List<string> GetPropsExcluded() { return GetPropertiesAsList(PropertiesExcluded); }
        public Dictionary<string, object> GetStaticProps() { return _staticProperties ?? new Dictionary<string, object>(); }
        private static List<string> GetPropertiesAsList(string propertiesCommaSeparated)
        {
            var propertiesList = new List<string>();
            if (!string.IsNullOrEmpty(propertiesCommaSeparated))
            {
                propertiesList.AddRange(propertiesCommaSeparated.Split(','));
            }
            return propertiesList;
        }

        /// <summary>
        /// Determines if the sender thing will return all the properties requested by the other sender thing
        /// </summary>
        /// <param name="senderThing"></param>
        /// <returns></returns>
        internal bool DoesInclude(TheSenderThing senderThing)
        {
            var result =
                   IsMatching(senderThing)
                && CanSatisfy(this.GetHistoryParameters(), senderThing.GetHistoryParameters());
            return result;
        }

        public TheHistoryParameters GetHistoryParameters()
        {
            var historyParams = new TheHistoryParameters
            {
                TokenExpiration = this.TokenExpirationInHours.HasValue ? new TimeSpan?(new TimeSpan((int) this.TokenExpirationInHours, 0, 0)) : null,
                MaxCount = this.MaxHistoryCount,
                MaxAge = new TimeSpan(0, 0, 0, 0, (int)this.MaxHistoryTime),
                Properties = this.GetPropsIncluded(),
                PropertiesToExclude = this.GetPropsExcluded(),
                SamplingWindow = new TimeSpan(0, 0, 0, 0, (int)this.ChangeBufferTimeBucketSize),
                CooldownPeriod = new TimeSpan(0, 0, 0, 0, (int)this.ChangeBufferLatency),
                ReportUnchangedProperties = this.SendUnchangedValue,
                ReportInitialValues = this.SendInitialValues,
                Persistent = this.KeepDurableHistory,
                IgnoreExistingHistory = this.IgnoreExistingHistory,
                //MaintainHistoryStore = false,
            };
            return historyParams;
        }

        public bool IsHistoryTokenCurrent()
        {
            if (HistoryToken == Guid.Empty)
            {
                return false;
            }
            var tThing = GetThing();
            if (tThing == null)
            {
                return false;
            }
            var historyParams = GetHistoryParameters();
            var tokenParams = tThing.GetHistoryParameters(HistoryToken);
            if (tokenParams == null)
            {
                return false;
            }
            var result = CanSatisfy(tokenParams, historyParams);
            return result;
        }

        public static bool CanSatisfy(TheHistoryParameters param, TheHistoryParameters param2)
        {
            var result =
                   (param.ComputeAvg == false || param.ComputeAvg == param2.ComputeAvg)
                && (param.ComputeMax == false || param.ComputeMax == param2.ComputeMax)
                && (param.ComputeN == false || param.ComputeN == param2.ComputeN)
                && param.CooldownPeriod == param2.CooldownPeriod
                && param.MaintainHistoryStore == param2.MaintainHistoryStore
                && param.ExternalHistoryStore == param2.ExternalHistoryStore
                && param.MaxAge == param2.MaxAge
                && param.MaxCount == param2.MaxCount
                && param.Persistent == param2.Persistent
                && (param.Properties == null || (param2.Properties != null && param2.Properties.Intersect(param.Properties).Count() == param2.Properties.Count()))
                && (param.PropertiesToExclude == null || param2.PropertiesToExclude == null || param2.PropertiesToExclude.Intersect(param.PropertiesToExclude).Count() == 0)
                && param.ReportUnchangedProperties == param2.ReportUnchangedProperties
                && param.SamplingWindow == param2.SamplingWindow
                ;
            return result;
        }

        public TheSenderThing() : base()
        {
        }

        // Override if new properties in a derived class need to be initialized from a TheThingToPublish
        internal virtual void Initialize(TheThingToPublish senderThingToAdd)
        {
            cdeMID = senderThingToAdd.cdeMID;
            ChangeBufferLatency = 0;
            if (senderThingToAdd.SamplingWindow.HasValue)
            {
                ChangeBufferTimeBucketSize = senderThingToAdd.SamplingWindow.Value;
            }
            else
            {
#pragma warning disable CS0618 // Type or member is obsolete
                ChangeBufferTimeBucketSize = senderThingToAdd.ChangeBufferTimeBucketSize; // Using obsolete member for compatibility
#pragma warning restore CS0618 // Type or member is obsolete
            }
            if (senderThingToAdd.CooldownPeriod.HasValue)
            {
                ChangeBufferLatency = senderThingToAdd.CooldownPeriod.Value;
            }
            ChangeNaNToNull = false;
            Disable = false;
            PartitionKey = senderThingToAdd.PartitionKey;
            RetryLastValueOnly = false;
            EventFormat = senderThingToAdd.EventFormat;
            if (!string.IsNullOrEmpty(senderThingToAdd.ThingMID))
            {
                ThingMID = senderThingToAdd.ThingMID;
            }
            //else if (senderThingToAdd.ThingReference != null)
            //{
            //    this.ThingMID = senderThingToAdd.ThingReference.ThingMID?.ToString(); // TODO use thing finder etc.
            //}
            PropertiesIncluded = senderThingToAdd.PropertiesIncluded != null ? TheCommonUtils.CListToString(senderThingToAdd.PropertiesIncluded, ",") : null;
            PropertiesExcluded = senderThingToAdd.PropertiesExcluded != null ? TheCommonUtils.CListToString(senderThingToAdd.PropertiesExcluded, ",") : null;
            PropertiesToMatch = CDictToString(senderThingToAdd.PropertiesToMatch);
            AddThingIdentity = senderThingToAdd.AddThingIdentity;
            StaticProperties = CDictToString(senderThingToAdd.StaticProperties);
            PreserveOrder = false;
            SendUnchangedValue = senderThingToAdd.SendUnchangedValue;
            SendInitialValues = senderThingToAdd.SendInitialValues;
            IgnoreExistingHistory = senderThingToAdd.IgnoreExistingHistory;
            TokenExpirationInHours = senderThingToAdd.TokenExpirationInHours;
            KeepDurableHistory = senderThingToAdd.KeepDurableHistory;
            MaxHistoryCount = senderThingToAdd.MaxHistoryCount;
            MaxHistoryTime = senderThingToAdd.MaxHistoryTime;
            IgnorePartialFailure = senderThingToAdd.IgnorePartialFailure;
            PreserveOrder = senderThingToAdd.PreserveOrder;
            TargetName = senderThingToAdd.TargetName;
            TargetType = senderThingToAdd.TargetType;
            TargetUnit = senderThingToAdd.TargetUnit;
            EngineName = senderThingToAdd.EngineName;
            FriendlyName = senderThingToAdd.FriendlyName;
            DeviceType = senderThingToAdd.DeviceType;
            ContinueMatching = (senderThingToAdd.ContinueMatching == true);
        }

        // Override if new properties need to be considered when merging things or creating sender things from templates
        internal virtual void Initialize(TheSenderThing senderThing)
        {
            AddThingIdentity = senderThing.AddThingIdentity;
            ChangeBufferLatency = senderThing.ChangeBufferLatency;
            ChangeBufferTimeBucketSize = senderThing.ChangeBufferTimeBucketSize;
            ChangeNaNToNull = senderThing.ChangeNaNToNull;
            Disable = senderThing.Disable;
            EventFormat = senderThing.EventFormat;
            HistoryToken = senderThing.HistoryToken;
            IgnorePartialFailure = senderThing.IgnorePartialFailure;
            KeepDurableHistory = senderThing.KeepDurableHistory;
            MaxHistoryCount = senderThing.MaxHistoryCount;
            MaxHistoryTime = senderThing.MaxHistoryTime;
            PartitionKey = senderThing.PartitionKey;
            PreserveOrder = senderThing.PreserveOrder;
            PropertiesExcluded = senderThing.PropertiesExcluded;
            PropertiesIncluded = senderThing.PropertiesIncluded;
            RetryLastValueOnly = senderThing.RetryLastValueOnly;
            SendUnchangedValue = senderThing.SendUnchangedValue;
            SendInitialValues = senderThing.SendInitialValues;
            IgnoreExistingHistory = senderThing.IgnoreExistingHistory;
            TokenExpirationInHours = senderThing.TokenExpirationInHours;
            StaticProperties = senderThing.StaticProperties;
            ThingMID = senderThing.ThingMID;
            DeviceType = senderThing.DeviceType;
            EngineName = senderThing.EngineName;
            TargetName = senderThing.TargetName;
            TargetType = senderThing.TargetType;
            TargetUnit = senderThing.TargetUnit;
            senderThing.CloneBase(this);
        }

        static string CDictToString(Dictionary<string, object> dict)
        {
            if (dict == null) return null;

            var dictString = dict.Aggregate("", (s, kv) => $"{s},{kv.Key}={TheCommonUtils.CStr(kv.Value)}");
            if (dictString.Length >0)
            {
                dictString = dictString.Substring(1);
            }
            return dictString;
        }

        internal static Dictionary<string, object> CStringToDict(string propBag)
        {
            if (propBag == null)
            {
                return null;
            }
            Dictionary<string, object> dict = new Dictionary<string, object>();
            if (!String.IsNullOrEmpty(propBag))
            {
                foreach (var p in propBag.Split(','))
                {
                    var parts = p.Split(new char[] { '=' }, 2);
                    if (parts.Length >= 2)
                    {
                        dict.Add(parts[0], parts[1]);
                    }
                }
            }
            return dict;
        }

        public void CancelSender(TheThing myBaseThingForLogging, CancellationToken? masterToken = null, bool triggerCancelOnly = false)
        {
            StopThingMatcher();
            SenderTaskInfo previousTaskInfo = null;
            if (pendingHistoryLoops.TryGetValue(cdeMID, out previousTaskInfo))
            {
                try
                {
                    if (!masterToken.HasValue)
                    {
                        masterToken = TheBaseAssets.MasterSwitchCancelationToken;
                    }
                    try
                    {
                        previousTaskInfo.cancelSource.Cancel();
                    }
                    catch { }
                    if (triggerCancelOnly)
                    {
                        return;
                    }
                    int count = 0;
                    const int timePerLoop = 1000;
                    const int timeout = 1 * 60;
                    while (!masterToken.Value.IsCancellationRequested 
                        && previousTaskInfo.task.Status != TaskStatus.Canceled
                        && previousTaskInfo.task.Status != TaskStatus.Faulted
                        && previousTaskInfo.task.Status != TaskStatus.RanToCompletion
                        && count < timeout
                        )
                    {
                        try
                        {
                            previousTaskInfo.task.Wait(timePerLoop, masterToken.Value);
                        }
                        catch (OperationCanceledException) { }
                        count++;
                        if (count >= timeout)
                        {
                            TheBaseAssets.MySYSLOG.WriteToLog(95275, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(myBaseThingForLogging.EngineName, "Connect", eMsgLevel.l2_Warning, $"Waiting on cancel previous send loop for {myBaseThingForLogging.FriendlyName} - {cdeMID}: timeout after {count * timePerLoop} ms. Retrying."));
                            count = 0;
                        }
                    }
                    if (count >= timeout)
                    {
                        TheBaseAssets.MySYSLOG.WriteToLog(95275, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(myBaseThingForLogging.EngineName, "Connect", eMsgLevel.l2_Warning, $"Unable to cancel previous send loop for {myBaseThingForLogging.FriendlyName} - {cdeMID}: timeout after {count * timePerLoop} ms"));
                    }
                    previousTaskInfo.cancelSource?.Dispose();
                    pendingHistoryLoops.RemoveNoCare(cdeMID);
                }
                catch (Exception e)
                {
                    TheBaseAssets.MySYSLOG.WriteToLog(95257, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(myBaseThingForLogging.EngineName, "Connect", eMsgLevel.l6_Debug, String.Format("Exception while cancelling previous send loop for thing {0}: {1}", cdeMID, TheCommonUtils.GetAggregateExceptionMessage(e))));
                }
            }
        }

        // We are using the TSenderThing in the storage mirror to carry per-item state that is not under user control
        // To ensure that these items don't get overwritten/lost when the user edits an item via NMI, we need to explicity move this state over
        internal virtual void TransferInternalState<TSenderThing>(TSenderThing activeSenderThing) where TSenderThing : TheSenderThing, new()
        {
            this.HistoryToken = activeSenderThing.HistoryToken;
            this._thingMatcherSubscription = activeSenderThing._thingMatcherSubscription;
        }

        public bool HasThingMatcher()
        {
            return _thingMatcherSubscription != null;
        }
        public void SetThingMatcher(IDisposable subscription)
        {
            _thingMatcherSubscription = subscription;
        }
        public void StopThingMatcher()
        {
            var thingMatcherSubscription = _thingMatcherSubscription;
            _thingMatcherSubscription = null;
            try
            {
                thingMatcherSubscription?.Dispose();
            }
            catch { }
        }

        IDisposable _thingMatcherSubscription;
        public bool StartSender(TheThing myBaseThing, TheSenderThing senderClone, Func<object,Task> senderLoop, CancellationToken masterToken)
        {
            if (pendingHistoryLoops.TryGetValue(cdeMID, out var previousTaskInfo))
            {
                if (previousTaskInfo.senderThing.HistoryToken == this.HistoryToken && previousTaskInfo.senderThing.IsEqual(this) && previousTaskInfo.senderThing.IsHistoryTokenCurrent() && !previousTaskInfo.task.IsCompleted && !previousTaskInfo.cancelSource.IsCancellationRequested)
                {
                    return true;
                }
            }
            CancelSender(myBaseThing, masterToken);
            var cancelTokenSource = CancellationTokenSource.CreateLinkedTokenSource(masterToken, TheBaseAssets.MasterSwitchCancelationToken);
            var senderTaskInfo = new SenderTaskInfo { owner = myBaseThing, senderThing = senderClone, cancelSource = cancelTokenSource };
            var task = TheCommonUtils.cdeRunTaskChainAsync("HistorySenderLoop", senderLoop, senderTaskInfo, true);
            //var task = senderLoop(senderTaskInfo);

            senderTaskInfo.task = task;
            pendingHistoryLoops[this.cdeMID] = senderTaskInfo;
            return true;
        }

        static cdeConcurrentDictionary<Guid, SenderTaskInfo> pendingHistoryLoops = new cdeConcurrentDictionary<Guid, SenderTaskInfo>();

        static internal void CancelPendingSenders(TheThing myBaseThing)
        {
            var loops = pendingHistoryLoops.GetDynamicEnumerable().Where(l => l.Value.owner == myBaseThing);

            foreach (var loop in loops)
            {
                loop.Value.senderThing.CancelSender(myBaseThing, null, true);
            }
            foreach (var loop in loops)
            {
                loop.Value.senderThing.CancelSender(myBaseThing);
            }
        }

        //static internal TheSenderThing GetSenderThingForToken(Guid historyToken)
        //{
        //    var senderTaskInfoKV = pendingHistoryLoops?.FirstOrDefault(kv => kv.Value.senderThing.HistoryToken == historyToken);
        //    return senderTaskInfoKV?.Value?.senderThing;
        //}
        //static internal TheSenderThing GetMatchingSenderThing(TheSenderThing senderThing)
        //{
        //    var senderTaskInfoKV = pendingHistoryLoops?.FirstOrDefault(kv => kv.Value.senderThing.IsMatching(senderThing));
        //    return senderTaskInfoKV?.Value?.senderThing;
        //}
        static internal IEnumerable<TheSenderThing> GetRunningSenderThings(TheThing ownerThing)
        {
            return pendingHistoryLoops?.Where(kv => kv.Value.owner == ownerThing).Select(kv => kv.Value.senderThing).ToList() ?? new List<TheSenderThing>();
        }

        public HashSet<string> GetPropertiesToSend()
        {
            HashSet<string> propertiesToSend;
            if (String.IsNullOrEmpty(PropertiesIncluded))
            {
                propertiesToSend = new HashSet<string>(GetThing().GetBaseThing().GetAllProperties(10).Select(p => p.Name));
            }
            else
            {
                propertiesToSend = new HashSet<string>(GetPropsIncluded());
            }
            if (!string.IsNullOrEmpty(PropertiesExcluded))
            {
                foreach (var propName in GetPropsExcluded())
                {
                    propertiesToSend.Remove(propName);
                }
            }
            return propertiesToSend;
        }

        //public bool IsSending()
        //{

        //    return _senderLoopCancel != null && !_senderLoopCancel.IsCancellationRequested;
        //}
    }

    class SenderTaskInfo
    {
        public Task task;
        public TheThing owner; // Sender that owns the senderThing
        public TheSenderThing senderThing;
        public CancellationTokenSource cancelSource;
    }

}
