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
using System.Text;

using System.Threading.Tasks;
using System.Threading;

using nsCDEngine.Engines;
using nsCDEngine.BaseClasses;
using nsCDEngine.Communication;
using nsCDEngine.ViewModels;
using nsCDEngine.Engines.StorageService;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.ThingService;
using nsTheConnectionBase;
using TheCommonMessageContracts;
using System.Text.RegularExpressions;

using nsTheEventConverters;
using nsTheThingToPublish;
using System.Globalization;


namespace nsTheSenderBase
{

    public abstract partial class TheSenderBase : TheSenderBase<TheSenderThing, TheSenderTSM>
    {
        public TheSenderBase(TheThing pThing, ICDEPlugin pPluginBase) : base(pThing, pPluginBase)
        {
        }
    }

    public abstract partial class TheSenderBase<TSenderThing, TSenderTSM> : TheConnectionBase
        where TSenderThing : TheSenderThing, new()
        where TSenderTSM : TheSenderTSM, new()
    {

        #region ThingProperties

        public bool PreserveSendOrderAllThings
        {
            get { return TheThing.GetSafePropertyBool(MyBaseThing, nameof(PreserveSendOrderAllThings)); }
            set { TheThing.SetSafePropertyBool(MyBaseThing, nameof(PreserveSendOrderAllThings), value); }
        }

        [ConfigProperty(DefaultValue = true)]
        public bool MergeSenderThings
        {
            get { return TheThing.GetSafePropertyBool(MyBaseThing, nameof(MergeSenderThings)); }
            set { TheThing.SetSafePropertyBool(MyBaseThing, nameof(MergeSenderThings), value); }
        }

        // KPIs in UI

        public long PropertiesSent
        {
            get { return (long)TheThing.GetSafePropertyNumber(MyBaseThing, "PropertiesSent"); }
            set { TheThing.SetSafePropertyNumber(MyBaseThing, "PropertiesSent", value); }
        }
        public long PropertiesSentSinceStart
        {
            get { return (long)TheThing.GetSafePropertyNumber(MyBaseThing, "PropertiesSentSinceStart"); }
            set { TheThing.SetSafePropertyNumber(MyBaseThing, "PropertiesSentSinceStart", value); }
        }

        public double PropertiesPerSecond
        {
            get { return TheThing.GetSafePropertyNumber(MyBaseThing, "PropertiesPerSecond"); }
            set
            {
                TheThing.SetSafePropertyNumber(MyBaseThing, "PropertiesPerSecond", value);
                TheThing.SetSafePropertyNumber(MyBaseThing, "QValue", value);
            }
        }

        public double DataSentKBytesPerSecond
        {
            get { return TheThing.GetSafePropertyNumber(MyBaseThing, "DataSentKBytesPerSecond"); }
            set { TheThing.SetSafePropertyNumber(MyBaseThing, "DataSentKBytesPerSecond", value); }
        }
        public long DataSentKBytesSinceStart
        {
            get { return (long)TheThing.GetSafePropertyNumber(MyBaseThing, "DataSentKBytesSinceStart"); }
            set { TheThing.SetSafePropertyNumber(MyBaseThing, "DataSentKBytesSinceStart", value); }
        }
        public double EventsPerSecond
        {
            get { return TheThing.GetSafePropertyNumber(MyBaseThing, "EventsPerSecond"); }
            set { TheThing.SetSafePropertyNumber(MyBaseThing, "EventsPerSecond", value); }
        }
        public long EventsSent
        {
            get { return (long)TheThing.GetSafePropertyNumber(MyBaseThing, "EventsSent"); }
            set
            {
                TheThing.SetSafePropertyNumber(MyBaseThing, "EventsSent", value);

            }
        }
        public long EventsSentSinceStart
        {
            get { return (long)TheThing.GetSafePropertyNumber(MyBaseThing, "EventsSentSinceStart"); }
            set
            {
                TheThing.SetSafePropertyNumber(MyBaseThing, "EventsSentSinceStart", value);
            }
        }
        public long EventsSentErrorCountSinceStart
        {
            get { return (long)TheThing.GetSafePropertyNumber(MyBaseThing, "EventsSentErrorCountSinceStart"); }
            set { TheThing.SetSafePropertyNumber(MyBaseThing, "EventsSentErrorCountSinceStart", value); }
        }

        public long PendingEventsSentSinceStart
        {
            get { return (long)TheThing.GetSafePropertyNumber(MyBaseThing, "PendingEventsSentSinceStart"); }
            set { TheThing.SetSafePropertyNumber(MyBaseThing, "PendingEventsSentSinceStart", value); }
        }

        public long PendingEvents
        {
            get { return (long)TheThing.GetSafePropertyNumber(MyBaseThing, "PendingEvents"); }
            set { TheThing.SetSafePropertyNumber(MyBaseThing, "PendingEvents", value); }
        }
        public DateTimeOffset KPITime
        {
            get { return TheThing.GetSafePropertyDate(MyBaseThing, "KPITime"); }
            set { TheThing.SetSafePropertyDate(MyBaseThing, "KPITime", value); }
        }

        public DateTimeOffset LastSendTime
        {
            get { return TheThing.GetSafePropertyDate(MyBaseThing, nameof(LastSendTime)); }
            set { TheThing.SetSafePropertyDate(MyBaseThing, nameof(LastSendTime), value); }
        }

        public DateTimeOffset LastSendAttemptTime
        {
            get { return TheThing.GetSafePropertyDate(MyBaseThing, nameof(LastSendAttemptTime)); }
            set { TheThing.SetSafePropertyDate(MyBaseThing, nameof(LastSendAttemptTime), value); }
        }

        public int PoisonEventRetryCount
        {
            get { return (int)TheThing.GetSafePropertyNumber(MyBaseThing, nameof(PoisonEventRetryCount)); }
            set { TheThing.SetSafePropertyNumber(MyBaseThing, nameof(PoisonEventRetryCount), value); }
        }

        public int SenderUpdatesPerBatch
        {
            get { return (int)TheThing.GetSafePropertyNumber(MyBaseThing, nameof(SenderUpdatesPerBatch)); }
            set { TheThing.SetSafePropertyNumber(MyBaseThing, nameof(SenderUpdatesPerBatch), value); }
        }
        public int SenderRetryPeriod
        {
            get { return (int)TheThing.GetSafePropertyNumber(MyBaseThing, nameof(SenderRetryPeriod)); }
            set { TheThing.SetSafePropertyNumber(MyBaseThing, nameof(SenderRetryPeriod), value); }
        }

        // Set in derived class to not get senderbase KPIs in the NMI
        public bool DoNotAddSenderBaseKPIs { get; private set; }

        #endregion

        public override async void HandleMessage(ICDEThing sender, object pIncoming)
        {
            TheProcessMessage pMsg = pIncoming as TheProcessMessage;
            if (pMsg == null || pMsg.Message == null) return;

            var cmd = TheCommonUtils.cdeSplit(pMsg.Message.TXT, ":", false, false);

            switch (cmd[0])
            {
                case "RUREADY":
                    if (cmd.Length > 1 && cmd[1] == TheCommonUtils.cdeGuidToString(MyBaseThing.cdeMID))
                    {
                        TheCommCore.PublishToOriginator(pMsg.Message, new TSM(pMsg.Message.ENG, "IS_READY:" + TheCommonUtils.cdeGuidToString(MyBaseThing.cdeMID), mIsInitialized.ToString()) { FLG = 8 }, true);
                    }
                    break;
                case nameof(MsgAddThingsToPublish):
                    {
                        var responseMsg = new MsgAddThingsToPublishResponse();
                        try
                        {

                        var request = TheCommRequestResponse.ParseRequestMessageJSON<MsgAddThingsToPublish>(pMsg.Message);
                        if (request == null)
                        {
                            responseMsg.Error = "Error parsing request message";
                        }
                        else
                        {
                            foreach (var thingToAdd in request.Things)
                            {
                                var thingStatus = AddThingToPublish(thingToAdd);
                                responseMsg.ThingStatus.Add(thingStatus);
                            }
                        }
                        RegisterSenderThingsForSend();
                        }
                        catch (Exception ex)
                        {
                            responseMsg.Error = $"Internal error: {ex.Message}";
                        }
                        TheCommRequestResponse.PublishResponseMessageJson(pMsg.Message, responseMsg);
                        break;
                    }

                case nameof(TheThing.MsgSubscribeToThings):
                    {
                        var responseMsg = new TheThing.MsgSubscribeToThingsResponse();
                        try
                        {

                            var request = TheCommRequestResponse.ParseRequestMessageJSON<TheThing.MsgSubscribeToThings>(pMsg.Message);
                            if (request == null)
                            {
                                responseMsg.Error = "Error parsing request message";
                            }
                            else
                            {
                                responseMsg.SubscriptionStatus = new List<TheThing.TheThingSubscriptionStatus>();
                                foreach (var subscription in request.SubscriptionRequests)
                                {
                                    await AddOrUpdateThingSubscription(responseMsg, subscription);
                                }
                            }
                            RegisterSenderThingsForSend();
                        }
                        catch (Exception ex)
                        {
                            responseMsg.Error = $"Internal error: {ex.Message}";
                        }
                        TheCommRequestResponse.PublishResponseMessageJson(pMsg.Message, responseMsg);
                        break;
                    }


                case nameof(TheThing.MsgGetThingSubscriptions):
                    {
                        var response = new TheThing.MsgGetThingSubscriptionsResponse();
                        try
                        {
                            var request = TheCommRequestResponse.ParseRequestMessageJSON<TheThing.MsgGetThingSubscriptions>(pMsg.Message);
                            if (request == null)
                            {
                                response.Error = "Error parsing request message";
                            }
                            else
                            {
                                response.ThingSubscriptions = GetThingSubscriptions(request.Generalize);
                            }
                        }
                        catch (Exception ex)
                        {
                            response.Error = $"Internal error: {ex.Message}";
                        }
                        TheCommRequestResponse.PublishResponseMessageJson(pMsg.Message, response);
                        break;
                    }
                case nameof(MsgAddTSMsToPublish):
                    {
                        var request = TheCommRequestResponse.ParseRequestMessageJSON<MsgAddTSMsToPublish>(pMsg.Message);
                        var responseMsg = new MsgAddTSMsToPublishResponse();
                        if (request == null)
                        {
                            responseMsg.Error = "Error parsing request message";
                        }
                        else
                        {
                            foreach (var tsmToAdd in request.TSMs)
                            {
                                var thingStatus = AddTSMToPublish(tsmToAdd);
                                responseMsg.ThingStatus.Add(thingStatus);
                            }
                        }
                        TheCommRequestResponse.PublishResponseMessageJson(pMsg.Message, responseMsg);
                        break;
                    }
                case nameof(TheThing.MsgUnsubscribeFromThings):
                    {
                        DoHandleMessage<TheThing.MsgUnsubscribeFromThings, TheThing.MsgUnsubscribeFromThingsResponse>(pMsg.Message, HandleUnsubscribeFromThingsMessage);

                        //var request = TheCommRequestResponse.ParseRequestMessageJSON<MsgDeletePublishedThing>(pMsg.Message);
                        //var response = new MsgDeletePublishedThingResponse();
                        //if (request == null)
                        //{
                        //    response.Error = "Error parsing request message";
                        //}
                        //else
                        //{
                        //    HandleDeleteMessage(request, response);
                        //}
                        //TheCommRequestResponse.PublishResponseMessageJson(pMsg.Message, response);
                        break;
                    }
                default:
                    HandleTSMToSend(sender, pMsg);
                    base.HandleMessage(sender, pIncoming);
                    break;
            }
        }

        protected virtual List<TheThing.TheThingSubscription> GetThingSubscriptions(bool? bGeneralize)
        {
            return MySenderThings.TheValues.Select(st => GetSubscriptionInfoFromSenderThing(st, bGeneralize)).ToList();
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        protected virtual async Task AddOrUpdateThingSubscription(TheThing.MsgSubscribeToThingsResponse responseMsg, TheThing.TheThingSubscription subscription)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            // TODO Use TheThingSubscription as the primary structure and keep TheThingToPublish only for backwards compat
            var thingToAdd = new TheThingToPublish
            {
                AddThingIdentity = subscription.AddThingIdentity ?? false,
                cdeMID = subscription.SubscriptionId ?? Guid.Empty,
                SamplingWindow = subscription.SamplingWindow,
                ContinueMatching = subscription.ContinueMatching ?? false,
                CooldownPeriod = subscription.CooldownPeriod,
                EngineName = subscription.ThingReference?.EngineName,
                DeviceType = subscription.ThingReference?.DeviceType,
                FriendlyName = subscription.ThingReference?.FriendlyName,
                EventFormat = subscription.EventFormat,
                ThingMID = subscription.ThingReference?.ThingMID?.ToString(),
                TargetName = subscription.TargetName,
                TargetType = subscription.TargetType,
                TargetUnit = subscription.TargetUnit,
                StaticProperties = subscription.StaticProperties,
                PropertiesExcluded = subscription.PropertiesExcluded,
                PropertiesIncluded = subscription.PropertiesIncluded,
                PropertiesToMatch = subscription.ThingReference?.PropertiesToMatch,
                ForceAllProperties = subscription.ForceAllProperties,
                IgnoreExistingHistory = subscription.IgnoreExistingHistory,
                IgnorePartialFailure = subscription.IgnorePartialFailure ?? false,
                KeepDurableHistory = subscription.KeepDurableHistory ?? false,
                MaxHistoryCount = subscription.MaxHistoryCount ?? 0,
                MaxHistoryTime = subscription.MaxHistoryTime ?? 0,
                PreserveOrder = subscription.PreserveOrder ?? false,
                PartitionKey = subscription.PartitionKey,
                ReplaceExistingThing = subscription.ReplaceExistingThing ?? false,
                SendInitialValues = subscription.SendInitialValues,
                SendUnchangedValue = subscription.SendUnchangedValue ?? false,
                TokenExpirationInHours = subscription.TokenExpirationInHours,
                //ExtensionData = subscription.ExtensionData,
            };
            var thingStatus = AddThingToPublish(thingToAdd);
            subscription.SubscriptionId = thingStatus.cdeMid;
            responseMsg.SubscriptionStatus.Add(new TheThing.TheThingSubscriptionStatus { Error = thingStatus.Error, Subscription = subscription });
        }

        virtual protected TheThing.TheThingSubscription GetSubscriptionInfoFromSenderThing(TSenderThing st, bool? bGeneralize)
        {
            var sub = new TheThing.TheThingSubscription
            {
                SubscriptionId = st.cdeMID,
                AddThingIdentity = st.AddThingIdentity,
                SamplingWindow = st.ChangeBufferTimeBucketSize,
                ContinueMatching = st.ContinueMatching,
                CooldownPeriod = st.ChangeBufferLatency,
                ThingReference = new TheThingReference
                {
                    ThingMID = TheCommonUtils.CGuid(st.ThingMID) != Guid.Empty ? (Guid?)TheCommonUtils.CGuid(st.ThingMID) : null,
                    EngineName = st.EngineName,
                    DeviceType = st.DeviceType,
                    FriendlyName = st.FriendlyName,
                    PropertiesToMatch = TheSenderThing.CStringToDict(st.PropertiesToMatch),
                },

                EventFormat = st.EventFormat,
                // ForceAllProperties = st.ForceAllProperties, // TODO: add the logic and field to TheSenderThing
                IgnoreExistingHistory = st.IgnoreExistingHistory,
                IgnorePartialFailure = st.IgnorePartialFailure,
                KeepDurableHistory = st.KeepDurableHistory,
                MaxHistoryCount = st.MaxHistoryCount,
                MaxHistoryTime = st.MaxHistoryTime,
                PartitionKey = st.PartitionKey,
                PreserveOrder = st.PreserveOrder,
                PropertiesExcluded = TheCommonUtils.CStringToList(st.PropertiesExcluded, ','),
                PropertiesIncluded = TheCommonUtils.CStringToList(st.PropertiesIncluded, ','),
                SendInitialValues = st.SendInitialValues,
                SendUnchangedValue = st.SendUnchangedValue,
                StaticProperties = TheSenderThing.CStringToDict(st.StaticProperties),
                TargetName = st.TargetName,
                TargetType = st.TargetType,
                TargetUnit = st.TargetUnit,
                TokenExpirationInHours = st.TokenExpirationInHours,
            };
            return sub;
        }

        private void DoHandleMessage<requestT, responseT>(TSM requestTSM, Action<requestT, responseT> handler) 
            where requestT : class, new()
            where responseT : class, TheThing.IMsgResponse, new()
        {
            var response = new responseT();
            try
            {
                var request = TheCommRequestResponse.ParseRequestMessageJSON<requestT>(requestTSM);
                if (request == null)
                {
                    response.Error = "Error parsing request message";
                }
                else
                {
                    handler(request, response);
                }
            }
            catch (Exception ex)
            {
                response.Error = $"Internal error: {ex.Message}";
            }
            TheCommRequestResponse.PublishResponseMessageJson(requestTSM, response);
        }

        private void HandleUnsubscribeFromThingsMessage(TheThing.MsgUnsubscribeFromThings request, TheThing.MsgUnsubscribeFromThingsResponse response)
        {
            foreach (var subscriptionId in request.SubscriptionIds)
            {
                var thingStatus = new TheAddThingStatus { cdeMid = subscriptionId };
                var deletedItem = MySenderThings.GetEntryByID(subscriptionId);
                string error = null;
                if (deletedItem == null)
                {
                    thingStatus.Error = "Thing to delete could not be found";
                    continue;
                }
                try
                {
                    OnSenderThingDelete(deletedItem);
                    MySenderThings.MyRecordsRWLock.RunUnderUpgradeableReadLock(() =>
                    {
                        MySenderThings.RemoveAnItem(deletedItem, (sr) => { });
                    }); // TODO: Check if it's ok.
                }
                catch (Exception e)
                {
                    error = e.Message;
                }
                thingStatus.Error = error;

                if (!string.IsNullOrEmpty(error))
                {
                    var status = new TheThing.TheThingSubscriptionStatus
                    {
                        Subscription = new TheThing.TheThingSubscription { SubscriptionId = deletedItem.cdeMID },
                        Error = error,
                    };
                    response.Failed.Add(status);
                }
            }
        }

        // TODO: Test if it already supports update scenario
        private TheAddThingStatus AddThingToPublish(TheThingToPublish thingToPublish)
        {
            var status = new TheAddThingStatus { cdeMid = thingToPublish.cdeMID };

            TSenderThing currentSenderThing = null;
            if (thingToPublish.ReplaceExistingThing == true)
            {
                var thingMidToPublish = TheCommonUtils.CGuid(thingToPublish.ThingMID);
                if (thingMidToPublish != Guid.Empty)
                {
                    var existingThings = MySenderThings.MyMirrorCache.GetEntriesByFunc(s => TheCommonUtils.CGuid(s.ThingMID) == thingMidToPublish);
                    if (existingThings.Count > 1)
                    {
                        status.Error = MsgAddThingsToPublishResponse.strErrorMoreThanOneMatchingThingFound;
                    }
                    else
                    {
                        currentSenderThing = existingThings.FirstOrDefault();
                    }
                }
            }
            else
            {
                currentSenderThing = MySenderThings.MyMirrorCache.GetEntryByID(thingToPublish.cdeMID);
            }

            if (String.IsNullOrEmpty(status.Error))
            {
                var newSenderThing = new TSenderThing();
                newSenderThing.Initialize(thingToPublish);
                if (!newSenderThing.IsEqual(currentSenderThing))
                {
                    bool wasConnected = IsConnected;
                    if (currentSenderThing != null && thingToPublish.DoNotCreate) // CODE REVIEW (note to self): what exactly are the DoNotCreate semantics?
                    {
                        MySenderThings.RemoveAnItem(currentSenderThing, null);

                        try
                        {
                            if (currentSenderThing.HistoryToken != Guid.Empty)
                            {
                                currentSenderThing.GetThing().GetBaseThing().UnregisterUpdateHistory(currentSenderThing.HistoryToken);
                            }
                        }
                        catch { }
                    }
                    MySenderThings.AddAnItem(newSenderThing);
                    status.cdeMid = newSenderThing.cdeMID;
                }
            }
            return status;
        }

        private TheAddThingStatus AddTSMToPublish(TheTSMToPublish tsmToPublish)
        {
            var status = new TheAddThingStatus { cdeMid = tsmToPublish.cdeMID };

            TSenderTSM currentSenderTSM = null;
            currentSenderTSM = MySenderTSMs.MyMirrorCache.GetEntryByID(tsmToPublish.cdeMID);

            if (String.IsNullOrEmpty(status.Error))
            {
                var newSenderThing = new TSenderTSM();
                newSenderThing.Initialize(tsmToPublish);
                if (!newSenderThing.IsEqual(currentSenderTSM))
                {
                    MySenderTSMs.RemoveAnItem(currentSenderTSM, null);
                    MySenderTSMs.AddAnItem(newSenderThing);
                    UpdateEnginesWithTSMsToSend(IsConnected);
                }
            }
            return status;
        }

        public TheSenderBase(TheThing pThing, ICDEPlugin pPluginBase) : base(pThing, pPluginBase)
        {
            _kpiUpdateInterval = _defaultKPIInterval;
        }

        protected bool InitBase(string deviceType)
        {
            if (mIsInitCalled) return false;
            mIsInitCalled = true;

            MyBaseThing.AddCapability(eThingCaps.SensorConsumer);

            if (TheCommonUtils.CGuid(MyBaseThing.ID) == Guid.Empty)
            {
                MyBaseThing.ID = Guid.NewGuid().ToString();
                if (string.IsNullOrEmpty(MyBaseThing.FriendlyName))
                    MyBaseThing.FriendlyName = "Sender:" + MyBaseThing.ID;
                PoisonEventRetryCount = 3;
            }

            MyBaseThing.LastUpdate = DateTimeOffset.Now;
            TheThing.SetSafePropertyString(MyBaseThing, "StateSensorValueName", "Events Sent");
            MyBaseThing.EngineName = MyBaseEngine.GetEngineName();
            MyBaseThing.DeviceType = deviceType;

            if (MyBaseThing.GetProperty(nameof(KPIUpdateInterval)) == null)
            {
                KPIUpdateInterval = _defaultKPIInterval;
            }

            if (MyBaseThing.GetProperty(nameof(SenderRetryPeriod)) == null)
            {
                SenderRetryPeriod = 30000;
            }
            if (MyBaseThing.GetProperty(nameof(SenderUpdatesPerBatch)) == null)
            {
                SenderUpdatesPerBatch = 100;
            }

            MyBaseThing.RegisterOnChange(nameof(KPIUpdateInterval),
                (p) => SetKpiIntervalInternal(new TimeSpan(0, 0, 0, 0, TheCommonUtils.CInt(p))));

            TheBaseEngine.WaitForStorageReadiness((pThing, pReady) =>
           {
               if (pReady != null)
               {
                   {
                       var senderThingsStore = new TheStorageMirror<TSenderThing>(TheCDEngines.MyIStorageService);
                       senderThingsStore.CacheTableName = "SenderThings" + TheThing.GetSafeThingGuid(MyBaseThing, "SThings");
                       senderThingsStore.IsRAMStore = true;
                       senderThingsStore.CacheStoreInterval = 60;
                       senderThingsStore.IsStoreIntervalInSeconds = true;
                       senderThingsStore.IsCachePersistent = true;
                       senderThingsStore.UseSafeSave = true;
                       senderThingsStore.AllowFireUpdates = true;
                       senderThingsStore.RegisterEvent(eStoreEvents.UpdateRequested, OnSenderThingUpdateObj);
                       senderThingsStore.RegisterEvent(eStoreEvents.InsertRequested, OnSenderThingInsertObj);
                       senderThingsStore.RegisterEvent(eStoreEvents.DeleteRequested, OnSenderThingDeleteObj);
                       senderThingsStore.RegisterEvent(eStoreEvents.StoreReady, (args) =>
                           {
                               var result = args.Para as TSM;
                               if (result != null && result.LVL == eMsgLevel.l1_Error)
                               {
                                   MyBaseThing.SetStatus(3, "Error loading sender things");
                                   TheBaseAssets.MySYSLOG.WriteToLog(95201, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, "Initialization", eMsgLevel.l6_Debug, $"Error loading sender things for connection {MyBaseThing.FriendlyName} {this.GetBaseThing().Address}"));
                               }
                               else
                               {
                                   MySenderThings = senderThingsStore;
                                   if (tSenderThingsForm != null)
                                   {
                                       tSenderThingsForm.defDataSource = MySenderThings.StoreMID.ToString();
                                   }
                                   TheBaseAssets.MySYSLOG.WriteToLog(95202, TSM.L(eDEBUG_LEVELS.FULLVERBOSE) ? null : new TSM(MyBaseThing.EngineName, "Initialization", eMsgLevel.l6_Debug, $"Sender things loaded for connection {MyBaseThing.FriendlyName} {this.GetBaseThing().Address}"));
                                   CheckInitialized();
                               }
                           }
                       );
                       senderThingsStore.InitializeStore(false, false);
                   }

                   {
                       var senderTSMsStore = new TheStorageMirror<TSenderTSM>(TheCDEngines.MyIStorageService)
                       {
                           CacheTableName = "SenderTSMs" + TheThing.GetSafeThingGuid(MyBaseThing, "STSMs"),
                           IsRAMStore = true,
                           CacheStoreInterval = 1,
                           IsStoreIntervalInSeconds = true,
                           IsCachePersistent = true,
                           UseSafeSave = true,
                           AllowFireUpdates = true,
                       };
                       senderTSMsStore.RegisterEvent(eStoreEvents.UpdateRequested, OnSenderTSMUpdate);
                       senderTSMsStore.RegisterEvent(eStoreEvents.InsertRequested, OnSenderTSMInsert);
                       senderTSMsStore.RegisterEvent(eStoreEvents.DeleteRequested, OnSenderTSMDelete);
                       senderTSMsStore.RegisterEvent(eStoreEvents.StoreReady, (args) =>
                       {
                           var result = args.Para as TSM;
                           if (result != null && result.LVL == eMsgLevel.l1_Error)
                           {
                               MyBaseThing.SetStatus(3, "Error loading sender TSMs");
                               TheBaseAssets.MySYSLOG.WriteToLog(95269, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, "Initialization", eMsgLevel.l6_Debug, $"Error loading sender TSMs for connection {MyBaseThing.FriendlyName} {this.GetBaseThing().Address}"));
                           }
                           else
                           {
                               MySenderTSMs = senderTSMsStore;
                               if (tSenderTSMsForm != null)
                               {
                                   tSenderTSMsForm.defDataSource = MySenderTSMs.StoreMID.ToString();
                               }
                               UpdateEnginesWithTSMsToSend(null);
                               TheBaseAssets.MySYSLOG.WriteToLog(95270, TSM.L(eDEBUG_LEVELS.FULLVERBOSE) ? null : new TSM(MyBaseThing.EngineName, "Initialization", eMsgLevel.l6_Debug, $"Sender TSMs loaded for connection {MyBaseThing.FriendlyName} {this.GetBaseThing().Address}"));
                               CheckInitialized();
                           }
                       }
                       );
                       senderTSMsStore.InitializeStore(false, false);
                   }

               }
           }, true);
            return mIsInitialized;
        }

        protected void CheckInitialized()
        {
            if (MySenderThings == null || MySenderTSMs == null)
            {
                // Not fully initialized yet
                return;
            }

            if (!OnCheckInitialized())
            {
                return;
            }

            if (AutoConnect)
            {
                if (!MyBaseThing.cdePendingConfig)
                {
                    Connect();
                }
                if (!IsConnected)
                {
                    TheCommonUtils.cdeRunTaskAsync("senderAutoConnect", async o =>
                    {
                        await TheCommonUtils.TaskDelayOneEye(30000, 100).ConfigureAwait(false); ;
                        while (!IsConnected && AutoConnect && TheBaseAssets.MasterSwitch)
                        {
                            
                            if (!MyBaseThing.cdePendingConfig)
                            {
                                Connect();
                            }
                            await TheCommonUtils.TaskDelayOneEye(30000, 100).ConfigureAwait(false); ;
                        }
                    }).ContinueWith(t => t.Exception);
                }
            }
            else
            {
                if (MyBaseThing.StatusLevel == 4)
                {
                    MyBaseThing.SetStatus(0, "Initialized.");
                }
            }
            mIsInitialized = true;
            FireEvent(eThingEvents.Initialized, this, true, true);
            FireEvent("ServerInit", this, IsConnected.ToString(), true);
        }

        protected virtual bool OnCheckInitialized()
        {
            return true;
        }
        public override bool OnDelete()
        {
            Disconnect(true);
            StopAllSenderLoops(true);
            MySenderThings.Reset();
            MySenderTSMs.Reset();
            return base.OnDelete();
        }

        private void OnSenderThingInsertObj(object param)
        {
            if (param == null || !(param is StoreEventArgs) || ((StoreEventArgs)param).Para == null)
            {
                return;
            }
            var senderThing = ((StoreEventArgs)param).Para as TSenderThing;
            OnSenderThingInsert(senderThing);
        }

        protected virtual void OnSenderThingInsert(TSenderThing senderThing)
        {
            if (IsConnected && senderThing != null && !senderThing.Disable)
            {
                RegisterSenderThingForSend(senderThing);
            }
        }

        private void OnSenderThingUpdateObj(object objStoreArgs)
        {
            if (objStoreArgs == null || !(objStoreArgs is StoreEventArgs) || ((StoreEventArgs)objStoreArgs).Para == null)
            {
                return;
            }
            var senderThing = ((objStoreArgs as StoreEventArgs)?.Para) as TSenderThing;
            OnSenderThingUpdate(senderThing);
        }
        protected virtual void OnSenderThingUpdate(TSenderThing senderThing)
        {
            if (senderThing == null)
            {
                return;
            }

            //CODE-REVIEW: This is causing weird behavior in the IoTSender as this fires when a Type or name is changed. Better is to run through this path only if Disabled has changed right?
            // => Any changes here could result in the sender loops requiring updates: RegisterSenderThingForSend performs a diff and only applies the required changes (or none if insignificant)
            if (this.IsConnected)
            {
                RegisterSenderThingForSend(senderThing);
            }
        }

        private void OnSenderThingDeleteObj(object objStoreArgs)
        {
            Guid? itemId = null;
            if (objStoreArgs is StoreEventArgs)
            {
                itemId = ((StoreEventArgs)objStoreArgs).Para as Guid?;
            }
            else
            {
                itemId = objStoreArgs as Guid?;
            }
            if (itemId.HasValue)
            {
                var deletedItem = MySenderThings.GetEntryByID(itemId.Value);
                OnSenderThingDelete(deletedItem);

            }
        }
        protected virtual void OnSenderThingDelete(TSenderThing deletedSenderThing)
        {
            if (deletedSenderThing != null)
            {
                bool isTokenUnique = !MySenderThings.TheValues.Any(st => st.HistoryToken == deletedSenderThing.HistoryToken && st.cdeMID != deletedSenderThing.cdeMID);
                deletedSenderThing.Disable = true;
                UnregisterSenderThingForSend(deletedSenderThing, isTokenUnique);
            }
            RegisterSenderThingsForSend();
        }

        private void OnSenderTSMInsert(object param)
        {
            if (param == null || !(param is StoreEventArgs) || ((StoreEventArgs)param).Para == null)
            {
                return;
            }
            var senderTSM = ((StoreEventArgs)param).Para as TSenderTSM;
            OnSenderTSMInsert(senderTSM);
        }

        protected virtual void OnSenderTSMInsert(TSenderTSM senderTSM)
        {
            TheCommonUtils.cdeRunAsync("", true, UpdateEnginesWithTSMsToSend);
        }

        protected virtual void OnSenderTSMUpdate(object objStoreArgs)
        {
            TheCommonUtils.cdeRunAsync("", true, UpdateEnginesWithTSMsToSend);
        }

        protected virtual void OnSenderTSMDelete(object objItemId)
        {
            TheCommonUtils.cdeRunAsync("", true, UpdateEnginesWithTSMsToSend);
        }

        void UpdateEnginesWithTSMsToSend(object ignored)
        {
            UpdateEnginesWithTSMsToSend(IsConnected);
        }

        bool bUpdateEngineTSMsRetryPending;
        void UpdateEnginesWithTSMsToSend(bool bRegister)
        {
            IEnumerable<string> engines;
            if (bRegister)
            {
                engines = MySenderTSMs.TheValues.Where(t => !t.Disable).Select(t => t.SourceEngineName).Distinct();
            }
            else
            {
                engines = new List<string>();
            }
            var enginesRemoved = registeredEngines.Except(engines);
            foreach (var engineName in enginesRemoved)
            {
                if (!String.IsNullOrEmpty(engineName))
                {
                    var tEngine = TheThingRegistry.GetBaseEngine(engineName);
                    tEngine?.UnregisterEvent(eEngineEvents.IncomingMessage, HandleTSMToSend);
                    var tThings = TheThingRegistry.GetThingsOfEngine(engineName);
                    if (tThings != null)
                    {
                        foreach (var tThing in tThings)
                        {
                            tThing.UnregisterEvent(eThingEvents.IncomingMessage, HandleTSMToSend);
                        }
                    }
                }
            }
            foreach (var engineName in engines)
            {
                if (!String.IsNullOrEmpty(engineName))
                {
                    var tEngine = TheThingRegistry.GetBaseEngine(engineName);
                    if (tEngine == null && !bUpdateEngineTSMsRetryPending)
                    {
                        bUpdateEngineTSMsRetryPending = true;
                        // Work around startup race conditions: 
                        TheCommonUtils.cdeRunAsync("", true, async o =>
                        {
                            await TheCommonUtils.TaskDelayOneEye(5000, 100).ConfigureAwait(false);
                            bUpdateEngineTSMsRetryPending = false;
                            UpdateEnginesWithTSMsToSend(null);
                        });
                    }
                    tEngine?.RegisterEvent(eEngineEvents.IncomingMessage, HandleTSMToSend);
                    var tThings = TheThingRegistry.GetThingsOfEngine(engineName, true, false);
                    if (tThings != null)
                    {
                        foreach (var tThing in tThings)
                        {
                            tThing.RegisterEvent(eThingEvents.IncomingMessage, HandleTSMToSend);
                        }
                    }
                }
            }
            registeredEngines = engines;
        }

        private async void HandleTSMToSend(ICDEThing arg1, object arg2)
        {
            try
            {
                var pMsg = arg2 as TheProcessMessage;
                if (pMsg == null)
                {
                    return;
                }
                if (MySenderTSMs?.Count > 0)
                {
                    LastSendAttemptTime = DateTimeOffset.Now;
                    var senderTSMs = MySenderTSMs.MyMirrorCache.GetEntriesByFunc(t => !t.Disable && pMsg.Message.ENG == t.TargetEngineName && Regex.IsMatch(pMsg.Message.TXT, t.TXTPattern));
                    var sendTasks = new List<Task>();
                    foreach (var senderTSM in senderTSMs)
                    {
                        try
                        {
                            // TODO Finish design for sending large files
                            //if (senderTSM.IsFile)
                            //{
                            //    var fileContents = new System.IO.FileStream(pMsg.Message.PLS, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                            //    SendFileAsync(GetNextConnection(), senderTSM, fileContents);
                            //}
                            //else
                            {
                                List<Task> internalSendTasks;
                                SendTSMAsync(GetNextConnection(), senderTSM, this.sendCancelationTokenSource.Token, pMsg, out internalSendTasks);
                                sendTasks.AddRange(internalSendTasks);
                            }
                            var allTasks = TheCommonUtils.TaskWhenAll(sendTasks);
                            await TheCommonUtils.TaskWaitTimeout(allTasks, new TimeSpan(0, 0, 60)).ConfigureAwait(false);

                            Interlocked.Add(ref _pendingKPIs.EventSentErrorCount, sendTasks.Count(t => t.Status != TaskStatus.RanToCompletion));
                            Interlocked.Add(ref _pendingKPIs.EventSentCount, sendTasks.Count(t => t.Status == TaskStatus.RanToCompletion));

                            if (!string.IsNullOrEmpty(senderTSM.AckTXTTemplate))
                            {
                                var ackTXT = CreateStringFromTemplate(senderTSM.AckTXTTemplate, this, senderTSM, pMsg.Message, null);
                                var ackTSM = new TSM(pMsg.Message.ENG, ackTXT);
                                if (!string.IsNullOrEmpty(senderTSM.AckPLSTemplate))
                                {
                                    ackTSM.PLS = CreateStringFromTemplate(senderTSM.AckPLSTemplate, this, senderTSM, pMsg.Message, null);
                                }
                                if (!senderTSM.AckToAll)
                                {
                                    TheCommCore.PublishToOriginator(pMsg.Message, ackTSM, true);
                                }
                                else
                                {
                                    TheCommCore.PublishCentral(ackTSM, true);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            TheBaseAssets.MySYSLOG.WriteToLog(95205, TSM.L(eDEBUG_LEVELS.VERBOSE) ? null : new TSM(MyBaseThing.EngineName, "HandleTSMToSend", eMsgLevel.l1_Error, $"Internal error processing TSM for sender {MyBaseThing.DeviceType} {MyBaseThing.FriendlyName} {MyBaseThing.Address}: {e?.ToString()}"));
                        }
                    }
                }
            }
            catch { }
        }

        IEnumerable<string> registeredEngines = new List<string>();

        protected TheFormInfo tSenderThingsForm;
        protected TheFormInfo tSenderTSMsForm;
        public override void CreateUXBase(string formTitle)
        {
            if (mIsUXInitCalled) return;
            mIsUXInitCalled = true;

            // var tFlds = TheNMIEngine.AddStandardForm(MyBaseThing, "FACEPLATE");
            var tFlds = TheNMIEngine.AddStandardForm(MyBaseThing, "FACEPLATE", null, 0, new nmiStandardForm { MaxTileWidth = 24, UseMargin = true });
            TheFormInfo tMyForm = tFlds["Form"] as TheFormInfo;
            MyForm = tMyForm;

            // Group: Device Status (Collapsible Group Field Order = 3)
            var ts = TheNMIEngine.AddStatusBlock(MyBaseThing, MyForm, 3);
            ts["Group"].SetParent(1);
            ts["FriendlyName"].Header = "Connection Name";

            // Group: KPIs (Subgroup of Device Status group)
            if (!DoNotAddSenderBaseKPIs)
            {
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.CollapsibleGroup, 202, 2, 0, "KPIs", null, new nmiCtrlCollapsibleGroup() { DoClose = true, ParentFld = 3, TileWidth = 6, IsSmall = true });//() { "TileWidth=6", "Format=Advanced Configurations", "Style=font-size:26px;text-align: left" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Number, 203, 0, 0, "Events Sent", nameof(EventsSentSinceStart), new ThePropertyBag() { "ParentFld=202", "TileHeight=1", "TileWidth=6", "FldWidth=4" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Number, 204, 0, 0, "Events Pending", nameof(PendingEvents), new ThePropertyBag() { "ParentFld=202", "TileHeight=1", "TileWidth=6", "FldWidth=4" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Number, 205, 0, 0, "Send Error Count", nameof(this.EventsSentErrorCountSinceStart), new ThePropertyBag() { "ParentFld=202", "TileHeight=1", "TileWidth=6", "FldWidth=4" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.DateTime, 206, 0, 0, "Last Send", nameof(LastSendTime), new ThePropertyBag() { "ParentFld=202", "TileHeight=1", "TileWidth=6", "FldWidth=4" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.DateTime, 207, 0, 0, "Last Send Attempt", nameof(LastSendAttemptTime), new ThePropertyBag() { "ParentFld=202", "TileHeight=1", "TileWidth=6", "FldWidth=4" });
                //TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Number, 204, 0, 0, "KBytes Sent Since Start", nameof(DataSentKBytesSinceStart), new ThePropertyBag() { "ParentFld=200", "TileHeight=1", "TileWidth=6", "FldWidth=4" });
                //TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleEnded, 208, 0, 0, "Time", nameof(KPITime), new ThePropertyBag() { "ParentFld=201", "TileHeight=1", "TileWidth=6", "FldWidth=4" });

                TheFieldInfo tReset = TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileButton, 250, 2, 0xC0, "Reset", null, new nmiCtrlTileButton() { ParentFld = 202, ClassName = "cdeBadActionButton", NoTE = true });
                tReset.RegisterUXEvent(MyBaseThing, eUXEvents.OnClick, "RESET", (pThing, pObj) =>
                {
                    TheProcessMessage pMsg = pObj as TheProcessMessage;
                    if (pMsg == null || pMsg.Message == null) return;
                    ResetKPIs();
                    TheCommCore.PublishToOriginator(pMsg.Message, new TSM(eEngineName.NMIService, "NMI_TOAST", "Reset KPIs..."));
                });

                // Group: Last 10 Seconds (Subgroup of Device Status->KPIs Group
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.CollapsibleGroup, 280, 2, 0, "Last 10 seconds:", false, null, null, new nmiCtrlCollapsibleGroup() { ParentFld = 202, DoClose = true, IsSmall = true, TileWidth = 6 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Number, 281, 0, 0, "Properties Sent", nameof(PropertiesSent), new ThePropertyBag() { "ParentFld=280", "TileHeight=1", "TileWidth=6", "FldWidth=4" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Number, 282, 0, 0, "Properties Per Second", nameof(PropertiesPerSecond), new ThePropertyBag() { "ParentFld=280", "TileHeight=1", "TileWidth=6", "FldWidth=4" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Number, 283, 0, 0, "KBytes Per Second", nameof(DataSentKBytesPerSecond), new ThePropertyBag() { "ParentFld=280", "TileHeight=1", "TileWidth=6", "FldWidth=4" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Number, 284, 0, 0, "Events Per Second", nameof(EventsPerSecond), new ThePropertyBag() { "ParentFld=280", "TileHeight=1", "TileWidth=6", "FldWidth=4" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Number, 285, 0, 0, "Events Sent", nameof(EventsSent), new ThePropertyBag() { "ParentFld=280", "TileHeight=1", "TileWidth=6", "FldWidth=4" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Number, 286, 0, 0, "Update interval", nameof(KPIUpdateInterval), new ThePropertyBag() { "ParentFld=280", "TileHeight=1", "TileWidth=6", "FldWidth=4" });
            }

            // Group: Connectivity (Collapsible Group Field Order = 20)
            var tc = TheNMIEngine.AddConnectivityBlock(MyBaseThing, MyForm, 20, (e, c) =>
            {
                if (c)
                {
                    Connect();
                }
                else
                {
                    Disconnect(true);
                }
            });
            tc["Group"].SetParent(1);

            TheFieldInfo tBut3 = TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileButton, 29, 2, 0xC0, "Force Disconnect", null, new nmiCtrlTileButton() { ParentFld = 20, ClassName = "cdeBadActionButton", NoTE = true });
            tBut3.RegisterUXEvent(MyBaseThing, eUXEvents.OnClick, "DISCONNECT_FORCE", (pThing, pObj) =>
            {
                TheProcessMessage pMsg = pObj as TheProcessMessage;
                if (pMsg == null || pMsg.Message == null) return;
                Disconnect(false);
                TheCommCore.PublishToOriginator(pMsg.Message, new TSM(eEngineName.NMIService, "NMI_TOAST", "Disconnecting..."));
            });


            // Group: Advanced Configuration (Collapsible Group Field Order = 40)
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.CollapsibleGroup, 40, 2, 0xC0, "Advanced Configurations...", null, ThePropertyBag.Create(new nmiCtrlCollapsibleGroup() { ParentFld = 1, TileWidth = 6, DoClose = true, IsSmall = true }));//() { "TileWidth=6", "Format=Advanced Configurations", "Style=font-size:26px;text-align: left" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 44, 2, 0xC0, "Preserve Order for all Things", nameof(PreserveSendOrderAllThings), new ThePropertyBag() { "ParentFld=40", "TileWidth=3", "TileHeight=1" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 80, 2, 0xC0, "Merge matching Sender Things", nameof(MergeSenderThings), new ThePropertyBag() { "ParentFld=40", "TileWidth=3", "TileHeight=1" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Number, 85, 2, 0xC0, "Max Updates per batch", nameof(SenderUpdatesPerBatch), new nmiCtrlNumber { LabelFontSize = 16, ParentFld = 40, TileHeight = 1, TileWidth = 3 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Number, 87, 2, 0xC0, "Send Retry Period (ms)", nameof(SenderRetryPeriod), new nmiCtrlNumber { LabelFontSize = 16, ParentFld = 40, TileHeight = 1, TileWidth = 3 });
            AddClearHistoryButton(tMyForm, 89, 40);

            // TheSenderThings Form: User accesses by clicking "Show Sender Thing List" button (added below)
            {
                // Form for the Things for which events are to be sent to the cloud
                string tDataSource = "TheSenderThings";
                if (MySenderThings != null)
                    tDataSource = MySenderThings.StoreMID.ToString();
                //tSenderThingsForm =                    new TheFormInfo(TheThing.GetSafeThingGuid(MyBaseThing, "SenderThings_ID"), eEngineName.NMIService, "Sender: Things to publish", tDataSource) { IsNotAutoLoading = true, AddButtonText = "Add new Thing" };
                tSenderThingsForm = TheNMIEngine.AddForm(new TheFormInfo(TheThing.GetSafeThingGuid(MyBaseThing, "SenderThings_ID"), eEngineName.NMIService, "Sender: Things to Publish", tDataSource) { AddButtonText = "Add New Thing", TileHeight = 10, TileWidth = -1 });
                // TheNMIEngine.AddFormToThingUX(MyBaseThing, tSenderThingsForm, "CMyTable", "Sender Thing List", 1, 3, 0xF0, null, null, new ThePropertyBag() { "Visibility=false" });
                TheNMIEngine.AddFields(tSenderThingsForm, new List<TheFieldInfo> {
                    new TheFieldInfo() { FldOrder=11,DataItem=nameof(TheSenderThing.Disable),Flags=2,Type=eFieldType.SingleCheck,Header="Disable",FldWidth=3,  DefaultValue="false" },
                    new TheFieldInfo() { FldOrder=12,DataItem=nameof(TheSenderThing.ThingMID),Flags=2, cdeA = 0xC0, Type=eFieldType.ThingPicker,Header="Thing to Send",FldWidth=3,  PropertyBag=new nmiCtrlThingPicker() { IncludeEngines=true, IncludeRemotes=true } },
                    new TheFieldInfo() { FldOrder=13,DataItem=nameof(TheSenderThing.EventFormat),Flags=2,Type=eFieldType.ComboBox,Header="Event Serializer",FldWidth=3,  PropertyBag=new ThePropertyBag() {"Options="+ TheEventConverters.GetDisplayNamesAsSemicolonSeperatedList() }, DefaultValue = TheEventConverters.GetDisplayName(typeof(JSonPropertyEventConverter)) },
                    new TheFieldInfo() { FldOrder=14,DataItem=nameof(TheSenderThing.ChangeNaNToNull),Flags=2,Type=eFieldType.SingleCheck,Header="Change NaN to Null for double/float",FldWidth=3,  DefaultValue="false" },
                    new TheFieldInfo() { FldOrder=15,DataItem=nameof(TheSenderThing.PreserveOrder),Flags=2,Type=eFieldType.SingleCheck,Header="Preserve Order Per Thing",FldWidth=3,  DefaultValue="false" },
                    new TheFieldInfo() { FldOrder=16,DataItem=nameof(TheSenderThing.PartitionKey),Flags=2,Type=eFieldType.ComboBox,Header="Partition Key for EventHub",FldWidth=3,  PropertyBag=new ThePropertyBag() {"Options="+ TheSenderThing.PartitionKeyChoices.Aggregate ("", (a,p) => a+p+";").TrimEnd(';') } },
                    new TheFieldInfo() { FldOrder=17,DataItem=nameof(TheSenderThing.ChangeBufferTimeBucketSize),Flags=2,Type=eFieldType.Number,Header="Sampling Window(in ms)",FldWidth=3,  DefaultValue="100" },
                    new TheFieldInfo() { FldOrder=18,DataItem=nameof(TheSenderThing.ChangeBufferLatency),Flags=2,Type=eFieldType.Number,Header="Change buffer latency (in ms)",FldWidth=3,  DefaultValue="100" },
                    new TheFieldInfo() { FldOrder=20,DataItem=nameof(TheSenderThing.RetryLastValueOnly),Flags=2,Type=eFieldType.SingleCheck,Header="Drop old values on error",FldWidth=3,  DefaultValue="false" },
                    new TheFieldInfo() { FldOrder=22,DataItem=nameof(TheSenderThing.KeepDurableHistory),Flags=2,Type=eFieldType.SingleCheck,Header="Durable Update History",FldWidth=3,  DefaultValue="true" },
                    new TheFieldInfo() { FldOrder=23,DataItem=nameof(TheSenderThing.MaxHistoryCount),Flags=2,Type=eFieldType.Number,Header="Max History Item Count",FldWidth=3,  DefaultValue="0" },
                    new TheFieldInfo() { FldOrder=24,DataItem=nameof(TheSenderThing.MaxHistoryTime),Flags=2,Type=eFieldType.Number,Header="Max History Time (ms)",FldWidth=3,  DefaultValue="0" },
                    new TheFieldInfo() { FldOrder=25,DataItem=nameof(TheSenderThing.PropertiesIncluded),Flags=2,Type=eFieldType.PropertyPicker,Header="Properties to Include",FldWidth=3,  DefaultValue="", PropertyBag=new nmiCtrlPropertyPicker{ ThingFld=12, AllowMultiSelect=true } },
                    new TheFieldInfo() { FldOrder=26,DataItem=nameof(TheSenderThing.PropertiesExcluded),Flags=2,Type=eFieldType.PropertyPicker,Header="Properties to Exclude",FldWidth=3,  DefaultValue="",PropertyBag=new nmiCtrlPropertyPicker{ ThingFld=12, AllowMultiSelect=true } },
                    new TheFieldInfo() { FldOrder=27,DataItem=nameof(TheSenderThing.StaticProperties),Flags=2,Type=eFieldType.SingleEnded,Header="Properties to Add",FldWidth=3,  DefaultValue="" },
                    new TheFieldInfo() { FldOrder=28,DataItem=nameof(TheSenderThing.AddThingIdentity),Flags=2,Type=eFieldType.SingleCheck,Header="Send Thing Identity",FldWidth=3 },
                    new TheFieldInfo() { FldOrder=29,DataItem=nameof(TheSenderThing.SendUnchangedValue),Flags=2,Type=eFieldType.SingleCheck,Header="Send value if unchanged",FldWidth=3,  DefaultValue="false" },
                    new TheFieldInfo() { FldOrder=30,DataItem=nameof(TheSenderThing.SendInitialValues),Flags=2,Type=eFieldType.SingleCheck,Header="Send initial values",FldWidth=3,  DefaultValue="false" },
                    new TheFieldInfo() { FldOrder=31,DataItem=nameof(TheSenderThing.IgnoreExistingHistory),Flags=2,Type=eFieldType.SingleCheck,Header="Ignore existing History",FldWidth=3,  DefaultValue="false" },
                    new TheFieldInfo() { FldOrder=32,DataItem=nameof(TheSenderThing.TokenExpirationInHours),Flags=2,Type=eFieldType.Number,Header="Token Lifetime (hours)",FldWidth=3,  DefaultValue="0" },
                });

                TheNMIEngine.AddTableButtons(tSenderThingsForm, false, 100);

                // Add link to Sender Thing list to Connection form
                // TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileButton, 18, 2, 0xF0, "Show Sender Thing List", null, new nmiCtrlTileButton() { OnClick = $"TTS:{tSenderThingsForm.cdeMID}", ClassName = "cdeTransitButton", ParentFld = 1, NoTE = true });

                // Group: Sender Thing List
                TheNMIEngine.AddSmartControl(MyBaseThing, MyForm, eFieldType.CollapsibleGroup, 1180, 2, 0x80, "Sender Thing List...", null, new nmiCtrlCollapsibleGroup { TileWidth = 18, IsSmall = true, DoClose = true, ParentFld = 1, AllowHorizontalExpand = true, MaxTileWidth = 18 });
                TheNMIEngine.AddSmartControl(MyBaseThing, MyForm, eFieldType.Table, 1181, 0, 128, null, tSenderThingsForm.cdeMID.ToString(), new nmiCtrlTableView { TileWidth = -1, ParentFld = 1180, NoTE = true, TileHeight = -1, MID = TheThing.GetSafeThingGuid(MyBaseThing, "TSTMID"), MainClassName = "cdeInFormTable" });
            }

            // The Sender TSMs Form: User accesses by clicking the "Show Sender TSM List" button.
            {

                // Form for the Things for which events are to be sent to the cloud
                string tDataSource = "TheSenderTSMs";
                if (MySenderTSMs != null)
                    tDataSource = MySenderTSMs.StoreMID.ToString();
                // tSenderTSMsForm =                   new TheFormInfo(TheThing.GetSafeThingGuid(MyBaseThing, "SenderTSMs_ID"), eEngineName.NMIService, "Sender: TSMs to publish", tDataSource) { IsNotAutoLoading = true, AddButtonText = "Add new TSM" };
                tSenderTSMsForm = TheNMIEngine.AddForm(new TheFormInfo(TheThing.GetSafeThingGuid(MyBaseThing, "SenderTSMs_ID"), eEngineName.NMIService, "Sender: TSMs to Publish", tDataSource) { AddButtonText = "Add New TSM", TileHeight = 10, TileWidth = -1 });
                // TheNMIEngine.AddFormToThingUX(MyBaseThing, tSenderTSMsForm, "CMyTable", "Sender TSM List", 1, 3, 0xF0, null, null, new ThePropertyBag() { "Visibility=false" });
                TheNMIEngine.AddFields(tSenderTSMsForm, new List<TheFieldInfo> {
                    new TheFieldInfo() { FldOrder=11,DataItem="Disable",Flags=2,Type=eFieldType.SingleCheck,Header="Disable",FldWidth=3,  DefaultValue="false" },
                    new TheFieldInfo() { FldOrder=20,DataItem = nameof(TheSenderTSM.SourceEngineName),Flags=2,Type=eFieldType.SingleEnded,Header="Source Engine Name",FldWidth=3,  DefaultValue="" },
                    new TheFieldInfo() { FldOrder=22,DataItem = nameof(TheSenderTSM.TargetEngineName),Flags=2,Type=eFieldType.SingleEnded,Header="Target Engine Name",FldWidth=3,  DefaultValue="" },
                    new TheFieldInfo() { FldOrder=25,DataItem = nameof(TheSenderTSM.TXTPattern),Flags=2,Type=eFieldType.SingleEnded,Header="Pattern for TXTs Include",FldWidth=3,  DefaultValue="" },
                    new TheFieldInfo() { FldOrder=26,DataItem = nameof(TheSenderTSM.AckTXTTemplate),Flags=2,Type=eFieldType.SingleEnded,Header="Template for Ack TXT",FldWidth=3,  DefaultValue="" },
                    new TheFieldInfo() { FldOrder=27,DataItem = nameof(TheSenderTSM.AckPLSTemplate),Flags=2,Type=eFieldType.SingleEnded,Header="Template for Ack PLS",FldWidth=3,  DefaultValue="" },
                    new TheFieldInfo() { FldOrder=28,DataItem = nameof(TheSenderTSM.AckToAll),Flags=2,Type=eFieldType.SingleCheck,Header="Ack to All",FldWidth=3, },
                    new TheFieldInfo() { FldOrder=30,DataItem = nameof(TheSenderTSM.MQTTTopicTemplate),Flags=2,Type=eFieldType.SingleEnded,Header="MQTT Topic Template",FldWidth=3,  DefaultValue="" },
                    new TheFieldInfo() { FldOrder=35,DataItem = nameof(TheSenderTSM.SendAsFile),Flags=2,Type=eFieldType.SingleCheck,Header="Send as File",FldWidth=3,  DefaultValue="" },
                    new TheFieldInfo() { FldOrder=40,DataItem = nameof(TheSenderTSM.SerializeTSM),Flags=2,Type=eFieldType.SingleCheck,Header="Send entire TSM",FldWidth=3,  DefaultValue="" },
                });

                TheNMIEngine.AddTableButtons(tSenderTSMsForm, false, 100);

                //// Add link to Sender TSM list to Connection form
                //TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileButton, 19, 2, 0xF0, "Show Sender TSM List", null, new nmiCtrlTileButton() { OnClick = $"TTS:{tSenderTSMsForm.cdeMID}", ClassName = "cdeTransitButton", ParentFld = 1, NoTE = true });

                // Group: Sender TSM List
                TheNMIEngine.AddSmartControl(MyBaseThing, MyForm, eFieldType.CollapsibleGroup, 1280, 2, 0x80, "Sender TSM List...", null, new nmiCtrlCollapsibleGroup { TileWidth = 18, IsSmall = true, DoClose = true, ParentFld = 1, AllowHorizontalExpand = true, MaxTileWidth = 18 });
                TheNMIEngine.AddSmartControl(MyBaseThing, MyForm, eFieldType.Table, 1281, 0, 128, null, tSenderTSMsForm.cdeMID.ToString(), new nmiCtrlTableView { TileWidth = -1, ParentFld = 1280, NoTE = true, TileHeight = -1, MID = TheThing.GetSafeThingGuid(MyBaseThing, "TSMMID"), MainClassName = "cdeInFormTable" });
            }

            return;
        }

        protected void AddClearHistoryButton(TheFormInfo tMyForm, int fldOrder, int parentFld)
        {
            TheFieldInfo tClearAllHistory = TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileButton, fldOrder, 2, 0xC0, "Clear History", null, new nmiCtrlTileButton() { ParentFld = parentFld, ClassName = "cdeBadActionButton", AreYouSure = "Do you want to clear update buffers and thing history for all things in this sender?", NoTE = true, TileWidth = 3 });
            tClearAllHistory.RegisterUXEvent(MyBaseThing, eUXEvents.OnClick, "CLEARHISTORY", (pThing, pObj) =>
            {
                TheProcessMessage pMsg = pObj as TheProcessMessage;
                if (pMsg == null || pMsg.Message == null) return;
                var wasConnected = IsConnected;
                if (wasConnected)
                {
                    Disconnect(true);
                    if (IsConnected)
                    {
                        TheCommCore.PublishToOriginator(pMsg.Message, new TSM(eEngineName.NMIService, "NMI_TOAST", "Unable to disconnect. History NOT cleared!"));
                        return;
                    }
                }
                foreach (var senderThing in MySenderThings.TheValues)
                {
                    senderThing.GetThing()?.UnregisterUpdateHistory(senderThing.HistoryToken);
                }
                if (wasConnected)
                {
                    Connect();
                }
                TheCommCore.PublishToOriginator(pMsg.Message, new TSM(eEngineName.NMIService, "NMI_TOAST", "History cleared."));
            });
        }

        protected TheStorageMirror<TSenderThing> MySenderThings = null;
        protected bool HoldOffOnSenderLoop = false;
        public override void Connect()
        {
            if (this.Connecting)
            {
                MyBaseThing.SetStatus(MyBaseThing.StatusLevel, "Connect already in progress.");
                TheBaseAssets.MySYSLOG.WriteToLog(95203, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, "Connect", eMsgLevel.l4_Message, String.Format("Already connecting for sender {0} {1}", MyBaseThing.FriendlyName, this.GetBaseThing().Address)));
                return;
            }
            if (this.IsConnected)
            {
                MyBaseThing.SetStatus(MyBaseThing.StatusLevel, "Already Connected.");
                TheBaseAssets.MySYSLOG.WriteToLog(95203, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, "Connect", eMsgLevel.l4_Message, String.Format("Already connected for sender {0} {1}", MyBaseThing.FriendlyName, this.GetBaseThing().Address)));
                return;
            }
            try
            {
                this.Connecting = true;
                MyBaseThing.SetStatus(4, "Connecting.");

                this.reportTimer = new Timer(UpdateKPIs, null, _kpiUpdateInterval, _kpiUpdateInterval);

                if (!this.sendCancelationTokenSource.IsCancellationRequested)
                {
                    TheBaseAssets.MySYSLOG.WriteToLog(95203, TSM.L(eDEBUG_LEVELS.VERBOSE) ? null : new TSM(MyBaseThing.EngineName, "Connect", eMsgLevel.l6_Debug, String.Format("Cancelling previous send loop for sender {0} {1}", MyBaseThing.FriendlyName, this.GetBaseThing().Address)));
                    try
                    {
                        sendCancelationTokenSource.Cancel();
                    }
                    catch (Exception e)
                    {
                        TheBaseAssets.MySYSLOG.WriteToLog(95204, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, "Connect", eMsgLevel.l6_Debug, $"Exception while cancelling previous send loop for {MyBaseThing.DeviceType} connection {MyBaseThing.FriendlyName} {MyBaseThing.Address}: {TheCommonUtils.GetAggregateExceptionMessage(e)}"));
                    }
                }
                sendCancelationTokenSource = new CancellationTokenSource();
                if (!DoConnect())
                {
                    throw new Exception(""); // TODO handle error reporting correctly
                }

                TheBaseAssets.MySYSLOG.WriteToLog(95205, TSM.L(eDEBUG_LEVELS.VERBOSE) ? null : new TSM(MyBaseThing.EngineName, "Connect", eMsgLevel.l6_Debug, $"Registering things for sender {MyBaseThing.DeviceType} {MyBaseThing.FriendlyName} {MyBaseThing.Address}"));
                if (!HoldOffOnSenderLoop)
                {
                    StartAllSenderLoops();
                }

                TheBaseAssets.MySYSLOG.WriteToLog(95255, TSM.L(eDEBUG_LEVELS.VERBOSE) ? null : new TSM(MyBaseThing.EngineName, "Connect", eMsgLevel.l6_Debug, $"Registering TSMs for sender {MyBaseThing.DeviceType} {MyBaseThing.FriendlyName}  {MyBaseThing.Address}"));
                UpdateEnginesWithTSMsToSend(true);

                TheBaseAssets.MySYSLOG.WriteToLog(95001, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, "Connect", eMsgLevel.l3_ImportantMessage, $"Sender {MyBaseThing.DeviceType} connected to {MyBaseThing.FriendlyName} {MyBaseThing.Address}"));

                MyBaseThing.FireEvent("ConnectComplete", this, null, true);

                MyBaseThing.SetStatus(1, "Connected.");
                IsConnected = true; // Ensure any property change notifications are fired
                OnConnectDisconnect(true);
            }
            catch (Exception e)
            {
                TheBaseAssets.MySYSLOG.WriteToLog(95206, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, "Connect", eMsgLevel.l1_Error, $"Unable to Connect. Sender {MyBaseThing.DeviceType} {MyBaseThing.FriendlyName} {MyBaseThing.Address}. Exception: {TheCommonUtils.GetAggregateExceptionMessage(e)}"));
                MyBaseThing.SetStatus(3, "Unable to connect: " + e.Message);
            }
            finally
            {
                this.Connecting = false;
            }

        }
        /// <summary>
        /// Called after a connect or disconnect is finished
        /// </summary>
        /// <param name="isConnected"></param>
        protected virtual void OnConnectDisconnect(bool isConnected) //TODO: Check if it's possible to make this method abstract.
        {
        }

        abstract protected bool DoConnect();

        public override void Disconnect(bool bDrain)
        {
            try
            {
                this.Disconnecting = true;
                MyBaseThing.SetStatus(6, "Disconnecting.");

                TheBaseAssets.MySYSLOG.WriteToLog(95207, TSM.L(eDEBUG_LEVELS.VERBOSE) ? null : new TSM(MyBaseThing.EngineName, "Disconnect", eMsgLevel.l6_Debug, $"Unregistering things for sender {MyBaseThing.FriendlyName} {this.GetBaseThing().Address}"));
                StopAllSenderLoops();

                TheBaseAssets.MySYSLOG.WriteToLog(95271, TSM.L(eDEBUG_LEVELS.VERBOSE) ? null : new TSM(MyBaseThing.EngineName, "Disconnect", eMsgLevel.l6_Debug, $"Unregistering TSMs for sender {MyBaseThing.FriendlyName} {this.GetBaseThing().Address}"));
                UpdateEnginesWithTSMsToSend(false);

                if (!DoDisconnect(bDrain))
                {
                    throw new Exception(""); // TODO fix error handling and logging here
                }
                MyBaseThing.FireEvent("DisconnectComplete", this, null, true);

                TheBaseAssets.MySYSLOG.WriteToLog(95007, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, "Disconnect", eMsgLevel.l3_ImportantMessage, $"Disconnected from {MyBaseThing.FriendlyName} {this.GetBaseThing().Address}"));
                MyBaseThing.SetStatus(0, "Disconnected.");
                IsConnected = false; // Ensure any property change notifications are fired
                OnConnectDisconnect(false);
            }
            catch (Exception e)
            {
                TheBaseAssets.MySYSLOG.WriteToLog(95214, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, "Disconnect", eMsgLevel.l1_Error, $"Unable to disconnect. Sender {MyBaseThing.FriendlyName} {this.GetBaseThing().Address}. Exception: {TheCommonUtils.GetAggregateExceptionMessage(e)}"));
                MyBaseThing.SetStatus(2, "Unable to disconnect - " + e.Message);
            }
            finally
            {
                this.Disconnecting = false;
            }
        }

        protected abstract bool DoDisconnect(bool bDrain);

        private void RegisterSenderThingForSend(TSenderThing senderThing)
        {
            var configuredSenderThings = MySenderThings.TheValues;

            var existingThing = configuredSenderThings.FirstOrDefault(st => st.cdeMID == senderThing.cdeMID);
            if (existingThing == null)
            {
                configuredSenderThings.Add(senderThing);
            }
            else
            {
                // An update came (i.e. through the NUI) but is not yet in MySenderThings
                var existingIndex = configuredSenderThings.IndexOf(existingThing);
                senderThing.TransferInternalState(existingThing);
                configuredSenderThings[existingIndex] = senderThing;
            }
            RegisterSenderThingsForSendInternal(configuredSenderThings);
        }

        private void RegisterSenderThingsForSend()
        {
            RegisterSenderThingsForSendInternal(MySenderThings.TheValues);
        }

        bool _bInSenderThingRegistration = false;
        private void RegisterSenderThingsForSendInternal(List<TSenderThing> configuredSenderThings)
        {
            if (HoldOffOnSenderLoop)
            {
                TheBaseAssets.MySYSLOG.WriteToLog(95273, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, "RegisterSenders", eMsgLevel.l6_Debug, $"Not registering senderthing because HoldOffOnSenderLoop is set. {MyBaseThing.FriendlyName} {this.GetBaseThing().Address}"));
                return;
            }
            if (!IsConnected && !Connecting)
            {
                TheBaseAssets.MySYSLOG.WriteToLog(95273, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, "RegisterSenders", eMsgLevel.l6_Debug, $"Not registering senderthing because sender is not connected. {MyBaseThing.FriendlyName} {this.GetBaseThing().Address}"));
                return;
            }

            MySenderThings.MyRecordsRWLock.RunUnderUpgradeableReadLock(() =>
            //lock (MySenderThings.MyRecordsLock) //LOCK-REVIEW: This lock will only prevent this function from entering twice. Is this really necessary or can it use the Reader Slim Lock?  => This needs to lock out all other writers; could use the RW.EnterUpgradeableWriteLock, but the plug-in needs to run on C-DEngine < 4.106 (for now).
            {
                if (_bInSenderThingRegistration)
                {
                    // Prevent reentrancy even on the same thread (i.e. via storage mirror events) - this currently does not happen; defense in depth
                    TheBaseAssets.MySYSLOG.WriteToLog(95273, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, "RegisterSenders", eMsgLevel.l2_Warning, $"Internal error: attempted to reenter RegisterSenderThingsForSendInternal. {MyBaseThing.FriendlyName} {this.GetBaseThing().Address}"));
                    return;
                }
                try
                {
                    _bInSenderThingRegistration = true;

                    AddSenderThingsForTemplates(configuredSenderThings);

                    // Keep track of which senders are running before we do the merge/update
                    var orphanedSenders = TheSenderThing.GetRunningSenderThings(MyBaseThing).ToList();

                    while (configuredSenderThings.Any())
                    {
                        var nextSenderThing = configuredSenderThings[0];
                        if (MergeSenderThings)
                        {
                            // Merge sender things to minimize the set of history tokens/sender loops
                            DoMergeSenderThings(nextSenderThing, configuredSenderThings);

                            var sendersRemoved = orphanedSenders.RemoveAll(rs => rs.IsMatching(nextSenderThing));
                            if (sendersRemoved > 1)
                            {
                                TheBaseAssets.MySYSLOG.WriteToLog(95273, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, "RegisterSenders", eMsgLevel.l2_Warning, $"Internal error: removed {sendersRemoved} senders while merging when expecting at most 1. {MyBaseThing.FriendlyName} {this.GetBaseThing().Address}"));
                            }
                        }
                        else
                        {
                            // Start a separate history loop for each configured sender thing
                            RegisterSenderThingForSendInternal(nextSenderThing, true);

                            var sendersRemoved = orphanedSenders.RemoveAll(rs => rs.cdeMID == nextSenderThing.cdeMID);
                            if (sendersRemoved > 1)
                            {
                                TheBaseAssets.MySYSLOG.WriteToLog(95274, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, "RegisterSenders", eMsgLevel.l2_Warning, $"Internal error: removed {sendersRemoved} senders when expecting at most 1. {MyBaseThing.FriendlyName} {this.GetBaseThing().Address}"));
                            }
                        }
                        configuredSenderThings.Remove(nextSenderThing);
                    }

                    // Stop any senders that are no longer required (i.e. deleted)
                    foreach (var sender in orphanedSenders)
                    {
                        UnregisterSenderThingForSend(sender as TSenderThing, MySenderThings.TheValues.FirstOrDefault(st => st.HistoryToken == sender.HistoryToken) == null);
                    }
                }
                finally
                {
                    _bInSenderThingRegistration = false;
                }
            });
        }

        private void AddSenderThingsForTemplates(List<TSenderThing> configuredSenderThings)
        {
            // Apply all templates, so we have a final list of sender things to register
            var senderTemplates = configuredSenderThings.Where(st => st.IsTemplate()).ToList();
            foreach (var senderTemplate in senderTemplates)
            {
                configuredSenderThings.Remove(senderTemplate);
                if (!senderTemplate.HasThingMatcher() && (senderTemplate.ContinueMatching || configuredSenderThings.FirstOrDefault(ct => ct.IsMatching(senderTemplate)) == null))
                {
                    var thingMatcher = new TheThingMatcher(new TheMessageAddress { ThingMID = TheCommonUtils.CGuid(senderTemplate.ThingMID), EngineName = senderTemplate.EngineName }, senderTemplate.GetPropertiesToMatch());
                    if (senderTemplate.ContinueMatching)
                    {
                        var observer = new TheThingMatchObserver(this, senderTemplate, st => configuredSenderThings.Add(st));
                        var subscription = thingMatcher.Subscribe(observer);
                        senderTemplate.SetThingMatcher(subscription);
                        // The observer will get called during Subscribe on the same thread: the callback adds these to the configuredSenderThings instead of getting reentered
                        // After this point we turn off the callback and any further calls will come on another thread and will wait at MySenderThings.MyRecordLock
                        Interlocked.Exchange(ref observer.Callback, null);
                    }
                    else
                    {
                        foreach (var matchingThing in thingMatcher.GetMatchingThings())
                        {
                            var newSenderThing = AddSenderThingFromTemplate(senderTemplate, matchingThing);
                            if (newSenderThing != null)
                            {
                                configuredSenderThings.Add(newSenderThing);
                            }
                        }
                    }
                }
            }
        }

        protected virtual TSenderThing AddSenderThingFromTemplate(TSenderThing senderTemplate, TheThing tThing)
        {
            if (MySenderThings.TheValues.FirstOrDefault(st => TheCommonUtils.CGuid(st.ThingMID) == tThing.cdeMID) == null)
            {
                var newSenderThing = CloneSender(senderTemplate);
                newSenderThing.cdeMID = Guid.NewGuid();
                newSenderThing.ThingMID = TheCommonUtils.cdeGuidToString(tThing.cdeMID);
                MySenderThings.AddAnItem(newSenderThing);
                return newSenderThing;
            }
            return null;
        }

        class TheThingMatchObserver : IObserver<TheThing>
        {
            TheSenderBase<TSenderThing, TSenderTSM> _senderBase;
            TSenderThing _senderThing;
            public Action<TSenderThing> Callback;
            public TheThingMatchObserver(TheSenderBase<TSenderThing, TSenderTSM> senderBase, TSenderThing senderThing, Action<TSenderThing> callbackNewSender)
            {
                _senderBase = senderBase;
                _senderThing = senderThing;
                Callback = callbackNewSender;
            }

            public void OnCompleted()
            {
                _senderThing.StopThingMatcher();
            }

            public void OnError(Exception error)
            {

            }

            public void OnNext(TheThing tThing)
            {
                var newSenderThing = _senderBase.AddSenderThingFromTemplate(_senderThing, tThing);
                if (newSenderThing != null)
                {

                    var callback = Interlocked.CompareExchange(ref Callback, null, null);
                    if (callback != null)
                    {
                        callback(newSenderThing);
                    }
                    else
                    {
                        _senderBase.RegisterSenderThingForSend(newSenderThing);
                    }
                }
            }
        }

        private void DoMergeSenderThings(TSenderThing senderThing, List<TSenderThing> configuredSenderThings)
        {
            TSenderThing mergedSenderThing = null;
            var matchingThings = GetMatchingSenderThings(senderThing, configuredSenderThings).ToList();
            if (!matchingThings.Any())
            {
                RegisterSenderThingForSendInternal(senderThing, true);
            }
            else
            {
                mergedSenderThing = CloneSender(senderThing);

                if (!mergedSenderThing.IsHistoryTokenCurrent())
                {
                    // Check that the history token still matches the matchingThing: it may have been edited to now match the senderThing but no longer match it's token. 
                    var thingWithHistory = matchingThings.FirstOrDefault(s => s.IsHistoryTokenCurrent());
                    if (thingWithHistory != null)
                    {
                        mergedSenderThing.HistoryToken = thingWithHistory.HistoryToken;
                    }
                }

                var propertiesIncluded = !mergedSenderThing.Disable ? mergedSenderThing.GetPropsIncluded() : null;
                var propertiesExcluded = !mergedSenderThing.Disable ? mergedSenderThing.GetPropsExcluded() : null;
                foreach (var matchingThing in matchingThings)
                {
                    configuredSenderThings.Remove(matchingThing);
                    if (!matchingThing.Disable)
                    {
                        if (propertiesIncluded == null)
                        {
                            propertiesIncluded = matchingThing.GetPropsIncluded();
                            mergedSenderThing = matchingThing;
                        }
                        else if (propertiesIncluded.Count > 0)
                        {
                            var matchingPropsIncluded = matchingThing.GetPropsIncluded();
                            if (matchingPropsIncluded.Count > 0)
                            {
                                propertiesIncluded = propertiesIncluded.Union(matchingPropsIncluded).ToList();
                            }
                            else
                            {
                                propertiesIncluded = new List<string>();
                            }
                        }

                        var matchingPropsExcluded = matchingThing.GetPropsExcluded();
                        if (propertiesExcluded == null)
                        {
                            propertiesExcluded = matchingPropsExcluded;
                        }
                        else if (matchingPropsExcluded.Count > 0)
                        {
                            propertiesExcluded = propertiesExcluded.Union(matchingPropsExcluded).ToList();
                        }
                        else
                        {
                            propertiesExcluded = new List<string>();
                        }
                    }
                }

                var runningSender = TheSenderThing.GetRunningSenderThings(MyBaseThing).Where(rt => rt.IsMatching(senderThing)).FirstOrDefault() as TSenderThing;

                if (!mergedSenderThing.Disable)
                {
                    mergedSenderThing.PropertiesIncluded = TheCommonUtils.CListToString(propertiesIncluded.OrderBy(s => s).ToList(), ",");
                    mergedSenderThing.PropertiesExcluded = TheCommonUtils.CListToString(propertiesExcluded.OrderBy(s => s).ToList(), ",");
                    mergedSenderThing.PartitionKey = null;
                    if (!mergedSenderThing.IsEqual(runningSender) || mergedSenderThing.HistoryToken != runningSender.HistoryToken)
                    {
                        UnregisterSenderThingForSend(runningSender, false);
                        RegisterSenderThingForSendInternal(mergedSenderThing, false);
                        if (runningSender != null && runningSender.HistoryToken != mergedSenderThing.HistoryToken)
                        {
                            runningSender.GetThing().UnregisterUpdateHistory(runningSender.HistoryToken);
                        }
                    }
                }
                else
                {
                    UnregisterSenderThingForSend(runningSender, true);
                    if (mergedSenderThing.HistoryToken != Guid.Empty)
                    {
                        mergedSenderThing.GetThing()?.UnregisterUpdateHistory(mergedSenderThing.HistoryToken);
                        mergedSenderThing.HistoryToken = Guid.Empty;
                    }
                }
                senderThing.HistoryToken = mergedSenderThing.HistoryToken;
                foreach (var matchingThing in matchingThings)
                {
                    if (matchingThing.HistoryToken != mergedSenderThing.HistoryToken)
                    {
                        matchingThing.HistoryToken = mergedSenderThing.HistoryToken;
                        if (MySenderThings.ContainsID(matchingThing.cdeMID))
                        {
                            MySenderThings.UpdateItem(matchingThing);
                        }
                    }
                }

                if (MySenderThings.ContainsID(senderThing.cdeMID))
                {
                    MySenderThings.UpdateItem(senderThing);
                }
            }
        }

        /// <summary>
        /// Return all sender things that match in everything except PropertiesIncluded, PropertiesExcluded and Disable
        /// </summary>
        /// <param name="senderThing"></param>
        /// <param name="senderThings"></param>
        /// <returns></returns>
        IEnumerable<TSenderThing> GetMatchingSenderThings(TSenderThing senderThing, IEnumerable<TSenderThing> senderThings)
        {
            var matchingThings = senderThings.Where(s =>
                s != senderThing
                && s.IsMatching(senderThing)
                );
            return matchingThings;
        }

        TimeSpan _kpiUpdateInterval;
        TimeSpan _defaultKPIInterval = new TimeSpan(0, 0, 10);
        void SetKpiIntervalInternal(TimeSpan interval)
        {
            if (interval.TotalMilliseconds == 0)
            {
                _kpiUpdateInterval = _defaultKPIInterval;
            }
            else
            {
                _kpiUpdateInterval = interval;
            }
            reportTimer?.Change(_kpiUpdateInterval, _kpiUpdateInterval);
        }

        public TimeSpan KPIUpdateInterval
        {
            get
            {
                var intervalInMs = TheThing.GetSafePropertyNumber(MyBaseThing, nameof(KPIUpdateInterval));
                var interval = new TimeSpan(0, 0, 0, 0, (int)(intervalInMs >= 0 ? intervalInMs : 0));
                SetKpiIntervalInternal(interval);
                return interval;
            }
            set
            {
                SetKpiIntervalInternal(value);
                TheThing.SetSafePropertyNumber(MyBaseThing, nameof(KPIUpdateInterval), value.TotalMilliseconds);
            }
        }

        void RegisterSenderThingForSendInternal(TSenderThing senderThing, bool bUpdate)
        {
            if (senderThing.Disable)
            {
                senderThing.CancelSender(MyBaseThing);
                return;
            }

            var tThing = senderThing.GetThing();
            if (tThing == null)
            {
                TheBaseAssets.MySYSLOG.WriteToLog(95215, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, "Connect", eMsgLevel.l1_Error, String.Format("Unable to get Sender Thing {0} from ThingRegistry. Will not send data for this thing.", senderThing.ThingMID)));
                return;
            }

            var historyParams = senderThing.GetHistoryParameters();
            historyParams.OwnerThing = MyBaseThing;
            historyParams.OwnerName = MyBaseThing.FriendlyName;

            if (!tThing.RestartUpdateHistory(senderThing.HistoryToken, historyParams))
            {
                senderThing.CancelSender(MyBaseThing);
                senderThing.HistoryToken = tThing.RegisterForUpdateHistory(historyParams);
                TheBaseAssets.MySYSLOG.WriteToLog(95216, TSM.L(eDEBUG_LEVELS.ESSENTIALS) ? null : new TSM(MyBaseThing.EngineName, "RegisterSenderThingForSendInternal " + MyBaseThing.FriendlyName, eMsgLevel.l6_Debug, $"Registered for thing history. Token: {senderThing.HistoryToken}"));
                if (bUpdate && MySenderThings.ContainsID(senderThing.cdeMID))
                {
                    MySenderThings.UpdateItem(senderThing);
                }
            }

            senderThing.StartSender(MyBaseThing, CloneSender(senderThing), this.HistorySenderLoop, sendCancelationTokenSource.Token);
        }

        // Do this to avoid the need for an abstract TheSenderBase.Clone method that must to be overridden in every derived class
        // Note that TheSenderBase.Initialize should still be overridden if a derived class adds properties that must be preserved when merging things (unlikely) or creating sender things from templates (somewhat more likely)
        TSenderThing CloneSender(TSenderThing senderThing)
        {
            var senderClone = new TSenderThing();
            senderClone.Initialize(senderThing);
            return senderClone;
        }

        protected void StartAllSenderLoops()
        {
            HoldOffOnSenderLoop = false;
            if (MySenderThings.MyMirrorCache.TheValues.Count == 0)
            {
                TheBaseAssets.MySYSLOG.WriteToLog(95272, TSM.L(eDEBUG_LEVELS.FULLVERBOSE) ? null : new TSM(MyBaseThing.EngineName, "RegisterAllSenderThingsForSend " + MyBaseThing.FriendlyName, eMsgLevel.l6_Debug, String.Format("No sender things. Storage initialized: {0}. Stack: {1}", MySenderThings.IsReady, Environment.StackTrace)));
            }
            else
            {
                TheBaseAssets.MySYSLOG.WriteToLog(95254, TSM.L(eDEBUG_LEVELS.FULLVERBOSE) ? null : new TSM(MyBaseThing.EngineName, "RegisterAllSenderThingsForSend " + MyBaseThing.FriendlyName, eMsgLevel.l6_Debug, String.Format("Initializing Sender things. Storage initialized: {0}. Stack: {1}", MySenderThings.IsReady, Environment.StackTrace)));
            }

            RegisterSenderThingsForSend();
        }

        void UnregisterSenderThingForSend(TSenderThing senderThing, bool deleteHistory)
        {
            if (senderThing == null)
                return;
            senderThing.CancelSender(MyBaseThing, TheBaseAssets.MasterSwitchCancelationToken);

            if (deleteHistory)
            {
                var t = senderThing.GetThing();
                if (t != null)
                {
                    t.GetBaseThing().UnregisterUpdateHistory(senderThing.HistoryToken);
                    senderThing.HistoryToken = Guid.Empty;
                }
            }
        }

        void StopAllSenderLoops(bool deleteHistory = false)
        {
            var senderThings = this.MySenderThings?.MyMirrorCache?.TheValues;
            if (senderThings != null)
            {
                foreach (var senderThing in senderThings)
                {
                    UnregisterSenderThingForSend(senderThing, deleteHistory);
                }
            }
            TheSenderThing.CancelPendingSenders(MyBaseThing);
        }

        protected bool GetPreserveOrderForSenderThing(TSenderThing senderThing)
        {
            return this.PreserveSendOrderAllThings || senderThing.PreserveOrder;
        }

        class ThePendingThingEvent
        {
            public List<TheThingStore> ThingEvents;
            public Task Task;
        }

        public async Task<bool> HistorySenderLoop(object senderTaskInfoObj)
        {
            try
            {
                var senderInfo = senderTaskInfoObj as SenderTaskInfo;

                var senderThing = senderInfo?.senderThing as TSenderThing;
                if (senderThing == null)
                {
                    TheBaseAssets.MySYSLOG.WriteToLog(95219, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, $"{MyBaseThing.DeviceType} '{MyBaseThing.FriendlyName}': Internal Error - no sender thing", eMsgLevel.l1_Error, ""));
                    return false;
                }
                var cancelToken = senderInfo.cancelSource.Token;

                var thingToSample = senderThing.GetThing();
                Dictionary<string, object> staticProperties = senderThing.GetStaticProps();

                int pendingEventCountReported = 0;
                bool bPreviousErrorLogged = false;
                int senderLoopStatusLevel = 1;

                var thingReadyTask = OnStartSenderLoopAsync(thingToSample, cancelToken);
                await thingReadyTask;
                if (thingReadyTask.IsCanceled)
                {
                    return false;
                }

                TheBaseAssets.MySYSLOG.WriteToLog(95003, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, $"{MyBaseThing.DeviceType} '{MyBaseThing.FriendlyName}' '{thingToSample.FriendlyName}'({thingToSample.cdeMID}): Starting sender loop", eMsgLevel.l3_ImportantMessage));

                // Ensure that the initial task creation finishes quickly by forcing an async await
                try
                {
#if !NET35 && !NET40
                    await Task.Yield();
#else
                await TheCommonUtils.TaskDelayOneEye(1, 100).ConfigureAwait(false);
#endif
                }
                catch { }

                IEventConverter eventConverter = TheEventConverters.GetEventConverter(senderThing.EventFormat);

                while (TheBaseAssets.MasterSwitch && !cancelToken.IsCancellationRequested && !senderThing.Disable)
                {
                    // Use the same connection for all events in the batch to guarantee ordering (if so selected)
                    // If no ordering selected, the SendEvent method will get a new connection for each event for maximum performance.
                    var myClient = GetNextConnection();
                    try
                    {
                        var updatesPerBatch = SenderUpdatesPerBatch;
                        if (updatesPerBatch <= 0)
                        {
                            updatesPerBatch = 100;
                            SenderUpdatesPerBatch = 100;
                        }
                        var historyToken = senderThing.HistoryToken;

                        List<TheThingStore> history;
                        do
                        {
                            int currentPendingItemCount;
                            var historyResponse = await thingToSample.GetThingHistoryAsync(historyToken, updatesPerBatch, 1, new TimeSpan(0, 0, 120), cancelToken, false).ConfigureAwait(false);
                            currentPendingItemCount = (historyResponse?.PendingItemCount).GetValueOrDefault();


                            if (historyResponse?.DatalossDetected == true)
                            {
                                TheBaseAssets.MySYSLOG.WriteToLog(98261, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, $"{MyBaseThing.DeviceType} '{MyBaseThing.FriendlyName}' '{thingToSample.FriendlyName}'({thingToSample.cdeMID}): data loss detected (likely historian buffer overflow)", eMsgLevel.l1_Error));
                            }
                            history = historyResponse?.HistoryItems;
                            if (history != null && history.Count > 0)
                            {
                                var sendTasks = new List<ThePendingThingEvent>();
                                int poisonEventRetries = 0;
                                do
                                {
                                    bool bSuccess = true;
                                    long totalSizeSent = 0;
                                    int totalPropertiesSent = 0;
                                    if (sendTasks.Count == 0)
                                    {
                                        var updatesToSend = new List<TheThingStore>();
                                        foreach (var thingUpdate in history)
                                        {
                                            try
                                            {
                                                if (MyBaseThing.StatusLevel == 4)
                                                {
                                                    MyBaseThing.SetStatus(1, "Sending.");
                                                    senderLoopStatusLevel = 1;
                                                }
                                                // TODO Remove once TFS Task 607 is completed (dummy baseline items from Historian)
                                                if (thingUpdate.PB.ContainsKey("PropertyForHistorianBaselineCheck"))
                                                {
                                                    TheBaseAssets.MySYSLOG.WriteToLog(95220, TSM.L(eDEBUG_LEVELS.FULLVERBOSE) ? null : new TSM(MyBaseThing.EngineName, $"{MyBaseThing.DeviceType} '{MyBaseThing.FriendlyName}' '{thingToSample.FriendlyName}'({thingToSample.cdeMID}): Not sending thing because of baseline check", eMsgLevel.l6_Debug, $"{thingUpdate.PB.Count}"));
                                                    TheBaseAssets.MySYSLOG.WriteToLog(98263, TSM.L(eDEBUG_LEVELS.FULLVERBOSE) ? null : new TSM(MyBaseThing.EngineName, $"{MyBaseThing.DeviceType} '{MyBaseThing.FriendlyName}' '{thingToSample.FriendlyName}'({thingToSample.cdeMID}): Not Sending baseline thing", eMsgLevel.l3_ImportantMessage, GetThingStoreAsString(thingUpdate)));
                                                    continue;
                                                }
                                                TheBaseAssets.MySYSLOG.WriteToLog(95221, TSM.L(eDEBUG_LEVELS.FULLVERBOSE) ? null : new TSM(MyBaseThing.EngineName, $"{MyBaseThing.DeviceType} '{MyBaseThing.FriendlyName}' '{thingToSample.FriendlyName}'({thingToSample.cdeMID}): Sending thing", eMsgLevel.l6_Debug, $"{thingUpdate.PB.Count} - {thingUpdate.cdeCTIM}"));
                                                TheBaseAssets.MySYSLOG.WriteToLog(98264, TSM.L(eDEBUG_LEVELS.FULLVERBOSE) ? null : new TSM(MyBaseThing.EngineName, $"{MyBaseThing.DeviceType} '{MyBaseThing.FriendlyName}' '{thingToSample.FriendlyName}'({thingToSample.cdeMID}): Sending thing", eMsgLevel.l3_ImportantMessage, GetThingStoreAsString(thingUpdate)));

                                                var thingUpdateToSend = thingUpdate.CloneForThingSnapshot(null, false, null, null, true);
                                                thingUpdateToSend.cdeMID = thingToSample.cdeMID;
                                                foreach (var nv in staticProperties)
                                                {
                                                    if (!thingUpdateToSend.PB.ContainsKey(nv.Key))
                                                    {
                                                        thingUpdateToSend.PB[nv.Key] = nv.Value;
                                                    }
                                                }
                                                updatesToSend.Add(thingUpdateToSend);
                                            }
                                            catch (Exception e)
                                            {
                                                bSuccess = false;
                                                TheBaseAssets.MySYSLOG.WriteToLog(98222, TSM.L(eDEBUG_LEVELS.ESSENTIALS) ? null : new TSM(MyBaseThing.EngineName, $"{MyBaseThing.DeviceType} '{MyBaseThing.FriendlyName}' '{thingToSample.FriendlyName}'({thingToSample.cdeMID}): Internal error during history sample", eMsgLevel.l1_Error, e.ToString()));
                                            }

                                        }
                                        try
                                        {
                                            bool bRetry2 = false;
                                            do
                                            {
                                                // TODO Enable batching across sender things
                                                //i.e. var queue = new BlockingCollection<TheThingStore>(senderThing.MaxHistoryCount);
                                                //if (!queue.TryAdd(thingUpdateToSend))
                                                //{ // should neve happend
                                                //}
                                                var result = await SendThingEvents(myClient, updatesToSend, senderThing, cancelToken, eventConverter, sendTasks).ConfigureAwait(false);
                                                if (result != null)
                                                {
                                                    totalSizeSent += result.totalSizeSent;
                                                    totalPropertiesSent += result.totalPropertiesSent;
                                                    bSuccess = result.bSuccess;
                                                }
                                            } while (bRetry2 && IsConnected);   //CM: Added is connected as I have seen this loop never exiting after a Disconnect
                                        }
                                        catch (OperationCanceledException e)
                                        {
                                            bSuccess = false;
                                            if (senderInfo.cancelSource.IsCancellationRequested)
                                            {
                                                TheBaseAssets.MySYSLOG.WriteToLog(98258, TSM.L(eDEBUG_LEVELS.VERBOSE) ? null : new TSM(MyBaseThing.EngineName, $"{MyBaseThing.DeviceType} '{MyBaseThing.FriendlyName}' '{thingToSample.FriendlyName}'({thingToSample.cdeMID}): History sample canceled", eMsgLevel.l1_Error, e.ToString()));
                                            }
                                            else
                                            {
                                                TheBaseAssets.MySYSLOG.WriteToLog(98258, TSM.L(eDEBUG_LEVELS.ESSENTIALS) ? null : new TSM(MyBaseThing.EngineName, $"{MyBaseThing.DeviceType} '{MyBaseThing.FriendlyName}' '{thingToSample.FriendlyName}'({thingToSample.cdeMID}): Error during history sample", eMsgLevel.l1_Error, e.ToString()));
                                            }
                                        }
                                        catch (TimeoutException)
                                        {
                                            bSuccess = false;
                                            TheBaseAssets.MySYSLOG.WriteToLog(98258, TSM.L(eDEBUG_LEVELS.ESSENTIALS) ? null : new TSM(MyBaseThing.EngineName, $"{MyBaseThing.DeviceType} '{MyBaseThing.FriendlyName}' '{thingToSample.FriendlyName}'({thingToSample.cdeMID}): Timeout during history sample", eMsgLevel.l1_Error));

                                        }
                                        catch (Exception e)
                                        {
                                            bSuccess = false;
                                            TheBaseAssets.MySYSLOG.WriteToLog(98258, TSM.L(eDEBUG_LEVELS.ESSENTIALS) ? null : new TSM(MyBaseThing.EngineName, $"{MyBaseThing.DeviceType} '{MyBaseThing.FriendlyName}' '{thingToSample.FriendlyName}'({thingToSample.cdeMID}): Error during history sample", eMsgLevel.l1_Error, e.ToString()));
                                        }
                                    }
                                    else
                                    {
                                        var newSendTasks = new List<ThePendingThingEvent>();
                                        foreach (var pte in sendTasks.Where((pte) => pte.Task?.Status != TaskStatus.RanToCompletion))
                                        {
                                            try
                                            {
                                                bool bRetry = false;
                                                do
                                                {
                                                    var result = await SendThingEvents(myClient, pte.ThingEvents, senderThing, cancelToken, eventConverter, newSendTasks).ConfigureAwait(false); ;
                                                    if (result != null)
                                                    {
                                                        totalSizeSent += result.totalSizeSent;
                                                        totalPropertiesSent += result.totalPropertiesSent;
                                                        bSuccess = result.bSuccess;
                                                    }
                                                } while (bRetry);
                                            }
                                            catch (Exception e)
                                            {
                                                bSuccess = false;
                                                if (e is TimeoutException)
                                                {
                                                    TheBaseAssets.MySYSLOG.WriteToLog(98223, TSM.L(eDEBUG_LEVELS.ESSENTIALS) ? null : new TSM(MyBaseThing.EngineName, $"{MyBaseThing.DeviceType} '{MyBaseThing.FriendlyName}' '{thingToSample.FriendlyName}'({thingToSample.cdeMID}): Timeout/cancelation while sending", eMsgLevel.l1_Error, e.ToString()));
                                                }
                                                else
                                                {
                                                    TheBaseAssets.MySYSLOG.WriteToLog(98259, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, $"{MyBaseThing.DeviceType} '{MyBaseThing.FriendlyName}' '{thingToSample.FriendlyName}'({thingToSample.cdeMID}): Internal error while sending", eMsgLevel.l1_Error, e.ToString()));
                                                }
                                                newSendTasks.Add(pte);
                                            }
                                        }
                                        sendTasks = newSendTasks;
                                    }


                                    if (bSuccess)
                                    {
                                        var allTasksComplete = TheCommonUtils.TaskWhenAll(sendTasks.Select((pte) => pte.Task));
                                        try
                                        {
                                            TheBaseAssets.MySYSLOG.WriteToLog(95224, TSM.L(eDEBUG_LEVELS.VERBOSE) ? null : new TSM(MyBaseThing.EngineName, $"{MyBaseThing.DeviceType} '{MyBaseThing.FriendlyName}' '{thingToSample.FriendlyName}'({thingToSample.cdeMID}): Waiting for sendTasks", eMsgLevel.l4_Message));
                                            await allTasksComplete.ConfigureAwait(false);
                                        }
                                        catch (Exception e)
                                        {
                                            int errorCount = 1;
                                            if (e is AggregateException)
                                            {
                                                errorCount = (e as AggregateException).InnerExceptions.Count;
                                            }

                                            Interlocked.Add(ref _pendingKPIs.EventSentErrorCount, errorCount);
                                            // Assumption: senders log their errors already
                                            bSuccess = false;
                                            if (e is OperationCanceledException || e is TimeoutException || e is TaskCanceledException)
                                            {
                                                TheBaseAssets.MySYSLOG.WriteToLog(95004, TSM.L(eDEBUG_LEVELS.VERBOSE) ? null : new TSM(MyBaseThing.EngineName, $"{MyBaseThing.DeviceType} '{MyBaseThing.FriendlyName}' '{thingToSample.FriendlyName}'({thingToSample.cdeMID}): Error while sending: timeout or canceled", eMsgLevel.l6_Debug));
                                            }
                                            else
                                            {
                                                TheBaseAssets.MySYSLOG.WriteToLog(95005, TSM.L(eDEBUG_LEVELS.VERBOSE) ? null : new TSM(MyBaseThing.EngineName, $"{MyBaseThing.DeviceType} '{MyBaseThing.FriendlyName}' '{thingToSample.FriendlyName}'({thingToSample.cdeMID}): Error while sending: wait sendTasks (will be reported via send tasks later)", eMsgLevel.l6_Debug, TheCommonUtils.GetAggregateExceptionMessage(e, false)));
                                            }
                                        }

                                        if (bSuccess && allTasksComplete.Status == TaskStatus.RanToCompletion)
                                        {
                                            TheBaseAssets.MySYSLOG.WriteToLog(95225, TSM.L(eDEBUG_LEVELS.VERBOSE) ? null : new TSM(MyBaseThing.EngineName, $"{MyBaseThing.DeviceType} '{MyBaseThing.FriendlyName}' '{thingToSample.FriendlyName}'({thingToSample.cdeMID}): Completed sendTasks", eMsgLevel.l4_Message, $"{sendTasks.Count}"));
                                            if (bPreviousErrorLogged)
                                            {
                                                var message = $"Successfully sent {sendTasks?.Count} events after previous error.";
                                                TheBaseAssets.MySYSLOG.WriteToLog(95015, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, $"{MyBaseThing.DeviceType} '{MyBaseThing.FriendlyName}' '{thingToSample.FriendlyName}'({thingToSample.cdeMID}): {message}", eMsgLevel.l3_ImportantMessage));
                                                MyBaseThing.SetStatus(1, message);
                                                senderLoopStatusLevel = 1;
                                                bPreviousErrorLogged = false;
                                            }
                                            Interlocked.Add(ref _pendingKPIs.EventSentCount, sendTasks.Count);
                                            LastSendTime = DateTimeOffset.Now;

                                            Interlocked.Add(ref _pendingKPIs.EventSentSize, totalSizeSent);
                                            Interlocked.Add(ref _pendingKPIs.EventSentPropertyCount, totalPropertiesSent);

                                            if (pendingEventCountReported > 0 || currentPendingItemCount > 0)
                                            {
                                                Interlocked.Add(ref _pendingKPIs.PendingEventCount, -pendingEventCountReported + currentPendingItemCount);
                                                pendingEventCountReported = currentPendingItemCount;
                                            }

                                            if (senderLoopStatusLevel != 1)
                                            {
                                                MyBaseThing.SetStatus(1, "Sent data.");
                                                senderLoopStatusLevel = 1;
                                            }
                                            senderLoopStatusLevel = 1;
                                            sendTasks.Clear();
                                            thingToSample.ClearUpdateHistory(historyToken);
                                        }
                                        else
                                        {
                                            bPreviousErrorLogged = true;
                                            Interlocked.Add(ref _pendingKPIs.PendingEventCount, currentPendingItemCount + sendTasks.Count - pendingEventCountReported);
                                            pendingEventCountReported = currentPendingItemCount + sendTasks.Count;

                                            var successCount = sendTasks.Count((pte) => pte.Task.Status == TaskStatus.RanToCompletion);

                                            var message = $"Error sending {sendTasks.Count - successCount} of {sendTasks.Count} events.";
                                            var uniqueExceptions = GetUniqueExceptions(sendTasks);

                                            if (successCount > 0 && senderThing.IgnorePartialFailure)
                                            {
                                                // Some Events succeeded, so assume the others are "poison events": retry only the failed events
                                                if (poisonEventRetries + 1 >= PoisonEventRetryCount)
                                                {
                                                    message += $" Failed Events will NOT be retried due to IgnorePartialFailure setting.";
                                                    TheBaseAssets.MySYSLOG.WriteToLog(95006, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, $"{MyBaseThing.DeviceType} '{MyBaseThing.FriendlyName}' '{thingToSample.FriendlyName}'({thingToSample.cdeMID}): {message}", eMsgLevel.l1_Error, uniqueExceptions)); // Send Partial failure: NOT retrying
                                                    thingToSample.ClearUpdateHistory(historyToken);
                                                    sendTasks.Clear();
                                                }
                                                else
                                                {
                                                    message += $" Failed Events will be retried.";
                                                    TheBaseAssets.MySYSLOG.WriteToLog(95268, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, $"{MyBaseThing.DeviceType} '{MyBaseThing.FriendlyName}' '{thingToSample.FriendlyName}'({thingToSample.cdeMID}): {message}", eMsgLevel.l2_Warning, uniqueExceptions)); // Send partial failure: retrying
                                                }
                                            }
                                            else
                                            {
                                                // Re-process the entire batch, even if only one item failed to be ack'd
                                                thingToSample.RestartUpdateHistory(historyToken);
                                                message += $" Will retry all events in {SenderRetryPeriod / 1000} seconds.";
                                                TheBaseAssets.MySYSLOG.WriteToLog(95014, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, $"{MyBaseThing.DeviceType} '{MyBaseThing.FriendlyName}' '{thingToSample.FriendlyName}'({thingToSample.cdeMID}): {message}", eMsgLevel.l1_Error, uniqueExceptions)); // Send failure: retrying
                                                sendTasks.Clear();
                                            }
                                            MyBaseThing.SetStatus(2, $"{message}. {uniqueExceptions}");
                                            senderLoopStatusLevel = 2;
                                            //if (successCount == 0) // Wait on any failure, to give the receiving system time to recover, and avoid tight loops/high CPU
                                            {
                                                // If no event went through it's likely a transport/network issue or the receiver is overloaded: wait a while for things to recover
                                                try
                                                {
                                                    await TheCommonUtils.TaskDelayOneEye(SenderRetryPeriod, 100, cancelToken).ConfigureAwait(false);
                                                }
                                                catch (TaskCanceledException) { }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        bPreviousErrorLogged = true;
                                        // Re-process the entire batch, even if only one item failed to be ack'd
                                        thingToSample.RestartUpdateHistory(historyToken);
                                        var message = $"{MyBaseThing.DeviceType} '{MyBaseThing.FriendlyName}' '{thingToSample.FriendlyName}'({thingToSample.cdeMID}): Error processing all {history.Count} events. Will retry in 30 seconds.";
                                        var uniqueExceptions = GetUniqueExceptions(sendTasks);
                                        TheBaseAssets.MySYSLOG.WriteToLog(95008, TSM.L(eDEBUG_LEVELS.ESSENTIALS) ? null : new TSM(MyBaseThing.EngineName, message, eMsgLevel.l1_Error, uniqueExceptions)); // Send failure: retrying
                                        sendTasks.Clear();
                                        MyBaseThing.SetStatus(2, $"{message}. {uniqueExceptions}");
                                        senderLoopStatusLevel = 2;
                                        Interlocked.Add(ref _pendingKPIs.PendingEventCount, currentPendingItemCount + history.Count - pendingEventCountReported);
                                        pendingEventCountReported = currentPendingItemCount + history.Count;
                                        try
                                        {
                                            await TheCommonUtils.TaskDelayOneEye(30000, 100, cancelToken).ConfigureAwait(false); ;
                                        }
                                        catch (TaskCanceledException) { }
                                    }
                                } while (sendTasks != null && sendTasks.Count > 0 && ++poisonEventRetries < PoisonEventRetryCount);
                            }
                            else
                            {
                                thingToSample.ClearUpdateHistory(historyToken);
                            }
                        } while (history != null && history.Count > 0 && TheBaseAssets.MasterSwitch && !cancelToken.IsCancellationRequested && !senderThing.Disable);
                        // Wait a little bit to avoid tight high CPU in unexpected error cases: we already drained any pending items at this point, so the wait should not impact throughput in high-volume scenarios
                        try
                        {
                            if (!cancelToken.IsCancellationRequested)
                            {
                                await TheCommonUtils.TaskDelayOneEye(1000, 100, cancelToken).ConfigureAwait(false);
                            }
                        }
                        catch { }
                    }
                    catch (Exception e)
                    {
                        TheBaseAssets.MySYSLOG.WriteToLog(95226, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, $"{MyBaseThing.DeviceType} '{MyBaseThing.FriendlyName}' '{thingToSample.FriendlyName}'({thingToSample.cdeMID}): Internal error.", eMsgLevel.l1_Error, e.ToString()));
                        MyBaseThing.SetStatus(3, $"Internal error '{thingToSample.FriendlyName}'({thingToSample.cdeMID}: {e.Message}");
                        senderLoopStatusLevel = 3;
                        try
                        {
                            await TheCommonUtils.TaskDelayOneEye(5000, 100, cancelToken).ConfigureAwait(false); ;
                        }
                        catch { }
                    }
                }
                TheBaseAssets.MySYSLOG.WriteToLog(95009, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, $"{MyBaseThing.DeviceType} '{MyBaseThing.FriendlyName}' '{thingToSample.FriendlyName}'({thingToSample.cdeMID}): Exiting sender loop", eMsgLevel.l3_ImportantMessage));
                return true;
            }
            catch (Exception e)
            {
                TheBaseAssets.MySYSLOG.WriteToLog(95009, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, $"{MyBaseThing.DeviceType} '{MyBaseThing.FriendlyName}'): Exiting sender loop with exception", eMsgLevel.l1_Error, e.ToString()));
                return false;
            }
        }

        protected virtual Task OnStartSenderLoopAsync(TheThing thingToSample, CancellationToken cancelToken)
        {
            return TheCommonUtils.TaskFromResult(true);
        }

        private string GetThingStoreAsString(TheThingStore thingUpdate)
        {
            string result = $"{thingUpdate.cdeCTIM}: ";
            foreach (var prop in thingUpdate.PB)
            {
                var value = TheCommonUtils.CStr(prop.Value);
                if (value?.Length > 20)
                {
                    value = value.Substring(0, 20);
                }
                result += $"{prop.Key}={value},";
            }
            return result;
        }


        private static string GetUniqueExceptions(IEnumerable<ThePendingThingEvent> sendTasks)
        {
            return GetUniqueExceptions(sendTasks.Select(ev => ev.Task));
        }
        public static string GetUniqueExceptions(IEnumerable<Task> sendTasks)
        {
            return sendTasks.Where(t => t?.IsFaulted == true || t?.IsCanceled == true)
                .GroupBy(t => t?.Exception?.Message ?? "Timeout/Canceled")
                .Select(g => g.FirstOrDefault()?.Exception != null ? TheCommonUtils.GetAggregateExceptionMessage(g.FirstOrDefault()?.Exception) : g.Key)
                .Aggregate("", (s, e) => e != null ? $"{s}{e};" : s).TrimEnd(';');
        }

        class SendThingResult
        {
            public bool bSuccess;
            public long totalSizeSent;
            public int totalPropertiesSent;
        }

        private async Task<SendThingResult> SendThingEvents(object myClient, List<TheThingStore> thingUpdatesToSend, TSenderThing senderThing, CancellationToken cancelToken, IEventConverter eventConverter, List<ThePendingThingEvent> sendTasks)
        {
            SendThingResult result;
            List<Task> innerSendTasks;
            result = new SendThingResult();
            try
            {
                cancelToken.ThrowIfCancellationRequested();

                LastSendAttemptTime = DateTimeOffset.Now;

                var sendEventResults = await SendEventsAsync(myClient, senderThing, cancelToken, thingUpdatesToSend, eventConverter).ConfigureAwait(false); ;
                if (sendEventResults.SizeSent < 0)
                {
                    throw new ArgumentOutOfRangeException(String.Format("Unsupported ClientEntity: {0}", myClient.GetType()));
                }
                result.totalSizeSent += sendEventResults.SizeSent;
                result.totalPropertiesSent += thingUpdatesToSend.Aggregate(0, (n, thingUpdate) => n + thingUpdate.PB.Count);
                if (sendEventResults.SendTasks != null)
                {
                    // TODO This can result in the same event/batch getting sent multiple times, if SendEventAsync returns multiple tasks
                    result.bSuccess = true;
                }
                else
                {
                    result.bSuccess = false;
                    Interlocked.Add(ref _pendingKPIs.EventSentErrorCount, 1);
                }
                innerSendTasks = sendEventResults.SendTasks;
#if LOGCONCURRENTSENDS
                                var cs2 = Interlocked.Add(ref _pendingKPIs.ConcurrentSends, -innerSendTasks.Count);
                                _pendingKPIs.ConcurrentSendHistory.Add(Tuple.Create(DateTimeOffset.Now, cs2));
#endif
            }
            catch (Exception e)
            {
                innerSendTasks = null;
                result.bSuccess = false;
#if LOGCONCURRENTSENDS
                                var cs2 = Interlocked.Decrement(ref _pendingKPIs.ConcurrentSends);
                                _pendingKPIs.ConcurrentSendHistory.Add(Tuple.Create(DateTimeOffset.Now, cs2));
#endif
                if (e is OperationCanceledException && cancelToken.IsCancellationRequested)
                {
                    throw;
                }
                Interlocked.Increment(ref _pendingKPIs.EventSentErrorCount);
                TheBaseAssets.MySYSLOG.WriteToLog(95253, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, "SendLoop", eMsgLevel.l2_Warning, String.Format("Error sending: {0}", TheCommonUtils.GetAggregateExceptionMessage(e, !TSM.L(eDEBUG_LEVELS.VERBOSE)))));
            }
            if (innerSendTasks != null)
            {
                sendTasks.AddRange(innerSendTasks.Select((t) => new ThePendingThingEvent { ThingEvents = thingUpdatesToSend, Task = t }));
            }
            return result;
        }


        protected CancellationTokenSource sendCancelationTokenSource = new CancellationTokenSource();


        protected abstract object GetNextConnection();

        protected class SendEventResults
        {
            public long SizeSent;
            public List<Task> SendTasks;
        }

        abstract protected Task<SendEventResults> SendEventsAsync(object client, TSenderThing senderThing, CancellationToken cancelToken, IEnumerable<TheThingStore> thingEventsToSend, IEventConverter eventConverter);

        virtual protected long SendTSMAsync(object client, TSenderTSM senderTSM, CancellationToken cancelToken, TheProcessMessage tsmToSend, out List<Task> sendTasks)
        {
            // TODO generate thing events from TSM for existing senders
            sendTasks = new List<Task>();
            return 0;
        }

        protected TheStorageMirror<TSenderTSM> MySenderTSMs = null;

        public static string CreateStringFromTemplate(string template, TheSenderBase<TSenderThing, TSenderTSM> sender, TheSenderThing senderThing, TheThingStore thingEventToSend)
        {
            TheThing thingBeingSent = senderThing?.GetThing()?.GetBaseThing();

            var parts = patternSplitRegex.Split(template);
            string topic = "";
            foreach (var part in parts)
            {
                if (part.StartsWith("$"))
                {
                    var partName = part.Substring(1);
                    if (partName == "_ISBPath")
                    {
#if MQTT_ISB
                         // TODO Verify if case preservation is guaranteed: need this for base64 encoded ISB strings
                        topic += TheCommonUtils.GetISBPath("", TheBaseAssets.MyServiceHostInfo.MyDeviceInfo.SenderType, cdeSenderType.CDE_SERVICE, 1, Guid.Empty, false);
#endif
                    }
                    else if (partName.StartsWith("sender"))
                    {
                        topic += TheThing.GetSafePropertyString(sender, partName.Substring("sender".Length));
                    }
                    else if (partName.StartsWith("thing"))
                    {
                        topic += TheThing.GetSafePropertyString(thingBeingSent, partName.Substring("thing".Length));
                    }
                    else if (partName.StartsWith("senderThing"))
                    {
                        object value = null;
                        try
                        {
                            value = senderThing?.GetType().GetProperty(partName);
                        }
                        catch { }
                        if (value != null)
                        {
                            topic += TheCommonUtils.CStr(value);
                        }
                        else
                        {
                            topic += part;
                        }
                    }
                    else
                    {
                        object propValue = null;
                        if (thingEventToSend?.PB.TryGetValue(partName, out propValue) == true)
                        {
                            topic += TheCommonUtils.CStr(propValue);
                        }
                        else
                        {
                            topic += part;
                        }
                    }
                }
                else
                {
                    topic += part;
                }
            }
            return topic;
        }

        static Regex _patternSplitRegx;
        static Regex patternSplitRegex
        {
            get
            {
                if (_patternSplitRegx == null)
                {
                    _patternSplitRegx = new Regex(@"(\$[^\$]*)\$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
                }
                return _patternSplitRegx;
            }
        }

        public static string CreateStringFromTemplate(string template, TheSenderBase<TSenderThing, TSenderTSM> sender, TheSenderTSM senderTSM, TSM tsmToSend, Dictionary<string, string> parameters)
        {
            var parts = patternSplitRegex.Split(template);
            string topic = "";
            string[] txtParts = null;
            object plsParsed = null;

            foreach (var part in parts)
            {
                if (part.StartsWith("$"))
                {
                    var partName = part.Substring(1);
                    if (partName == "_ISBPath")
                    {
#if MQTT_ISB
                        // TODO Verify if case preservation is guaranteed: need this for base64 encoded ISB strings
                        topic += TheCommonUtils.GetISBPath("", TheBaseAssets.MyServiceHostInfo.MyDeviceInfo.SenderType, cdeSenderType.CDE_SERVICE, 1, Guid.Empty, false);
#endif
                    }
                    else if (partName.StartsWith("sender"))
                    {
                        topic += TheThing.GetSafePropertyString(sender, partName.Substring("sender".Length));
                    }
                    else if (partName.StartsWith("thing"))
                    {
                        var originatorThing = TheThingRegistry.GetThingByMID(tsmToSend.GetOriginatorThing());
                        if (originatorThing != null)
                        {
                            topic += TheThing.GetSafePropertyString(originatorThing, partName.Substring("thing".Length));
                        }
                    }
                    else if (partName.StartsWith("senderTSM"))
                    {
                        object value = null;
                        try
                        {
                            value = senderTSM.GetType().GetProperty(partName);
                        }
                        catch { }
                        if (value != null)
                        {
                            topic += TheCommonUtils.CStr(value);
                        }
                        else
                        {
                            topic += part;
                        }
                    }
                    else if (partName.StartsWith("TXTPart"))
                    {
                        // Example: for CDE_FILEPUSH:<filename>:<cdeO>:<cookie>:<fileMetaInfo>
                        // $TXTPart1$ = <filename> becomes part of the topic
                        // $TXTPart4$ = <fileMetaInfo> becomes part of the topic
                        // $TXTPart.F.1$ = returns a parameter "F" , <filename>, does not become part of the topic
                        // $TXTPart.R.1$ = returns a parameter "R", <filename>, does not become part of the topic

                        string parameterName = null;
                        var c = partName["TXTPart".Length];
                        if (c == '.')
                        {
                            var lastDot = partName.LastIndexOf(".");
                            if (lastDot > "TXTPart".Length)
                            {
                                parameterName = partName.Substring("TXTPart".Length + 1, lastDot - "TXTPart".Length);
                            }
                        }
                        int txtIndex;
                        if (int.TryParse(partName.Substring("txtPart".Length + (string.IsNullOrEmpty(parameterName) ? 0 : parameterName.Length + 2)), NumberStyles.None, CultureInfo.InvariantCulture, out txtIndex))
                        {
                            if (txtParts == null)
                            {
                                var txt = tsmToSend.TXT;
                                txtParts = TheCommonUtils.cdeSplit(txt, ':', false, false);
                            }
                            if (txtIndex >= 0 && txtIndex < txtParts.Length)
                            {
                                if (!string.IsNullOrEmpty(parameterName))
                                {
                                    // it's a parameter for the caller: pass it back without adding to the output string
                                    parameters?.Add(parameterName, txtParts[txtIndex]);
                                }
                                else
                                {
                                    topic += txtParts[txtIndex];
                                }
                            }
                        }
                    }
                    else if (partName.StartsWith("PLSValue") && !string.IsNullOrEmpty(tsmToSend.PLS))
                    {
                        var valueName = partName.Substring("PLSValue".Length);
                        if (!string.IsNullOrEmpty(valueName))
                        {
                            try
                            {
                                if (plsParsed == null)
                                {
                                    plsParsed = TheCommonUtils.DeserializeJSONStringToObject<object>(tsmToSend.PLS);
                                }
                                if (plsParsed != null)
                                {
                                    var plsValue = TheCommonUtils.GetJSONValueByPath(plsParsed, valueName)?.ToString();
                                    if (!string.IsNullOrEmpty(plsValue))
                                    {
                                        topic += TheCommonUtils.CStr(plsValue);
                                    }
                                }
                            }
                            catch
                            {
                                // Ignored
                            }
                        }
                    }
                    else if (partName == "cdeNewGuid")
                    {
                        topic += TheCommonUtils.CStr(Guid.NewGuid());
                    }
                    else
                    {
                        object value = null;
                        try
                        {
                            value = tsmToSend.GetType().GetField(partName).GetValue(tsmToSend);
                        }
                        catch { }
                        if (value != null)
                        {
                            topic += TheCommonUtils.CStr(value);
                        }
                        else
                        {
                            topic += part;
                        }
                    }
                }
                else
                {
                    topic += part;
                }
            }
            return topic;
        }


        #region KPIs

        protected KPIData _pendingKPIs = new KPIData();
        protected KPIData _sampledKPIs = new KPIData();

        public class KPIData
        {
            public long EventDroppedOutOfOrderCount = 0;
            public long EventScheduledCount = 0;
            public long EventSentCount = 0;
            public long EventTaskCompletedCount = 0;
            public long EventSentSize = 0;
            public long EventSentPropertyCount = 0;
            public long EventSentErrorCount = 0;
            public long EventSentErrorDroppedCount = 0;

            public long PendingEventCount = 0;
            public DateTimeOffset Time;
            public TimeSpan ElapsedTime;

            public long AveragePropertiesPerEventSent
            {
                get { return this.EventSentPropertyCount > 0 && this.EventSentCount > 0 ? this.EventSentPropertyCount / this.EventSentCount : 0; }
            }

            public double RatePropertiesSentPerSecond
            {
                get { return this.EventSentPropertyCount / ElapsedTime.TotalSeconds; }
            }

            public double RateEventsSentPerSecond
            {
                get { return this.EventSentCount / ElapsedTime.TotalSeconds; }
            }
#if LOGCONCURRENTSENDS
            public double RateEventTaskCompletedPerSecond
            {
                get { return this.EventTaskCompletedCount / ElapsedTime.TotalSeconds; }
            }
#endif
            public double RateKBytesSentPerSecond
            {
                get { return this.EventSentSize / ElapsedTime.TotalSeconds / 1024; }
            }

            public KPIData() { }

            public KPIData(KPIData prev)
            {
                EventDroppedOutOfOrderCount = prev.EventDroppedOutOfOrderCount;
                EventScheduledCount = prev.EventScheduledCount;
                EventSentCount = prev.EventSentCount;
                EventSentSize = prev.EventSentSize;
                EventSentPropertyCount = prev.EventSentPropertyCount;
                EventSentErrorCount = prev.EventSentErrorCount;
                EventSentErrorDroppedCount = prev.EventSentErrorDroppedCount;

                PendingEventCount = prev.PendingEventCount;

                Time = prev.Time;
                ElapsedTime = prev.ElapsedTime;
            }
        }

        public KPIData ReadKPIs()
        {
            KPIData readData;
            lock (lastSendTimeLock)
            {
                readData = new KPIData(_sampledKPIs);
            }
            return readData;
        }

        DateTimeOffset lastSendTime = DateTimeOffset.Now;
        object lastSendTimeLock = new object();

        Timer reportTimer;

        void UpdateKPIs(object param)
        {
            try
            {
                lock (lastSendTimeLock)
                {
                    var now = DateTimeOffset.Now;
                    _sampledKPIs.Time = now;
                    _sampledKPIs.ElapsedTime = now - lastSendTime;

                    _sampledKPIs.EventDroppedOutOfOrderCount = Interlocked.Exchange(ref _pendingKPIs.EventDroppedOutOfOrderCount, 0);
                    _sampledKPIs.EventScheduledCount = Interlocked.Exchange(ref _pendingKPIs.EventScheduledCount, 0);
                    _sampledKPIs.EventTaskCompletedCount = Interlocked.Exchange(ref _pendingKPIs.EventTaskCompletedCount, 0);
                    _sampledKPIs.EventSentCount = Interlocked.Exchange(ref _pendingKPIs.EventSentCount, 0);
                    _sampledKPIs.EventSentSize = Interlocked.Exchange(ref _pendingKPIs.EventSentSize, 0);
                    _sampledKPIs.EventSentPropertyCount = Interlocked.Exchange(ref _pendingKPIs.EventSentPropertyCount, 0);
                    _sampledKPIs.EventSentErrorCount = Interlocked.Exchange(ref _pendingKPIs.EventSentErrorCount, 0);
                    _sampledKPIs.EventSentErrorDroppedCount = Interlocked.Exchange(ref _pendingKPIs.EventSentErrorDroppedCount, 0);
                    _sampledKPIs.PendingEventCount = Interlocked.Read(ref _pendingKPIs.PendingEventCount);

                    lastSendTime = now;
                }

                var kpis = ReadKPIs();

                this.PropertiesSent = kpis.EventSentPropertyCount;
                this.PropertiesPerSecond = kpis.RatePropertiesSentPerSecond.cdeTruncate(2);
                this.DataSentKBytesPerSecond = kpis.RateKBytesSentPerSecond.cdeTruncate(3);
                this.EventsPerSecond = kpis.RateEventsSentPerSecond.cdeTruncate(2);
                this.EventsSent = kpis.EventSentCount;
                this.PendingEvents = kpis.PendingEventCount;
                // Cumulative KPIs
                this.EventsSentSinceStart += kpis.EventSentCount;
                this.EventsSentErrorCountSinceStart += kpis.EventSentErrorCount;
                this.DataSentKBytesSinceStart += kpis.EventSentSize;
                this.PropertiesSentSinceStart += kpis.EventSentPropertyCount;

                this.KPITime = kpis.Time;

                if (!this.IsConnected && this.reportTimer != null)
                {
                    try
                    {
                        if (this.reportTimer.Change(Timeout.Infinite, Timeout.Infinite))
                        {
                            this.reportTimer.Dispose();
                        }
                    }
                    finally
                    {
                        reportTimer = null;
                    }
                }
            }
            catch { }
        }

        protected void ResetKPIs()
        {
            this.EventsSentSinceStart = 0;
            this.DataSentKBytesSinceStart = 0;
            this.PropertiesSentSinceStart = 0;
            this.PendingEvents = 0; //CODE-REVIEW: this might not work correctly. But after a KPI Reset, this number was only 0 for a short period of time in the NMI - then went up to a very high number (after starting/stopping the IoT Sender a couple of times).
            this.EventsSentErrorCountSinceStart = 0;
        }

        #endregion


        void LogTaskExceptions(List<Task> tasks)
        {
            var faultedTasks = tasks.FindAll((t) => t.Exception != null);
            Task.Factory.StartNew(() =>
            {
                foreach (var task in faultedTasks)
                {
                    TheBaseAssets.MySYSLOG.WriteToLog(95013, TSM.L(eDEBUG_LEVELS.VERBOSE) ? null : new TSM(MyBaseThing.EngineName, "SendLoop", eMsgLevel.l2_Warning, String.Format("Task {0}: Exception {1}", task.ToString(), TheCommonUtils.GetAggregateExceptionMessage(task.Exception))));
                }
            });
        }

    }
  
}
