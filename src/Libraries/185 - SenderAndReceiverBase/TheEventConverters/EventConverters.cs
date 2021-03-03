// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using Newtonsoft.Json;

using nsCDEngine.BaseClasses;
using nsCDEngine.Engines.ThingService;

using CDMyOPCUAClient.Contracts;
using nsCDEngine.ViewModels;
using System.Runtime.Serialization;

//using nsTheSenderBase;

//#if !NET35
//using Microsoft.ServiceBus.Messaging;
//#else
//#endif

namespace nsTheEventConverters
{
    public interface IThingToConvert
    {
        bool AddThingIdentity { get; }
        TheThing GetThing();
        HashSet<string> GetPropertiesToSend(bool useCached);
    }

    public abstract class IEventConverter
    {
        /// <summary>
        /// Turns the thingEvent into a transport representation, typically a serialized string, but it can also be a specialized representation for a given transport (i.e. an EventData message for EventHub where thing properties are encoded as AMQP Properties).
        /// If the caller does not understand the type of the returned transport representation, it should treat it as an error ("Transport specific encoding can not used with XYZ").
        /// </summary>
        /// <param name="thingEvent"></param>
        /// <param name="maxEventDataSize">Maximum payload size for each returned object.</param>
        /// <param name="doNotBatchEvents">Do not return multiple events/properties in a single object (i.e. as a JSON array), but use one object per event/property.</param>
        /// <returns></returns>
        public abstract IEnumerable<object> GetEventData(IEnumerable<TheThingStore> thingUpdates, IThingToConvert senderThing, int maxEventDataSize, bool doNotBatchEvents);

        //IEnumerable<TSM> GetMessages(TheSenderBase.ThingEvent thingEvent);
        public Func<TheThing, Task<bool>> NewThingCallback { get; set; }
        public abstract string ProcessEventData(TheThing thing, object evnt, DateTimeOffset defaultTime);
        //string ProcessJsonData(TheThing theThing, string payloadJson);
        public Func<object, string> eventDecoder { get; set; }
        public string DecodeEventAsString(object evnt)
        {
            return evnt is string ? evnt as string : eventDecoder?.Invoke(evnt);
        }

        public Dictionary<string, object> StaticPropsToAdd;
        public virtual bool UpdatePropertyValuesToSend(TheThingStore thingEvent, bool changeDateTimeOffset, IThingToConvert senderThing, bool skipBaseProps)
        {
            if (changeDateTimeOffset)
            {
                foreach (var kv in thingEvent.PB.ToList())
                {
                    if (kv.Value is DateTimeOffset)
                    {
                        thingEvent.PB[kv.Key] = ((DateTimeOffset)kv.Value).UtcDateTime;
                    }
                }
            }
            //if (!thingEvent.PB.ContainsKey(strTimeProp)) thingEvent.PB.Add(strTimeProp, changeDateTimeOffset ? thingEvent.cdeCTIM.UtcDateTime : thingEvent.cdeCTIM);

            if (senderThing.AddThingIdentity)
            {
                if (!skipBaseProps)
                {
                    if (!thingEvent.PB.ContainsKey(strThingIdProp)) thingEvent.PB.Add(strThingIdProp, thingEvent.cdeMID);
                    if (!thingEvent.PB.ContainsKey(strThingNodeProp)) thingEvent.PB.Add(strThingNodeProp, thingEvent.cdeN);
                }
                var sampledThing = senderThing.GetThing();
                if (sampledThing != null)
                {
                    // TODO Speed up schema updates via change notifications?
                    //sampledThing.RegisterEvent(eThingEvents.PropertyAdded, OnPropertyAdded);
                    //sampledThing.RegisterEvent(eThingEvents.PropertyDeleted, OnPropertyDeleted);

                    bool doUpdateNow = false;
                    if (!lastThingSchemaUpdateSent.TryGetValue(sampledThing.cdeMID, out var lastSchemaUpdate))
                    {
                        lastSchemaUpdate = new SchemaUpdateStatus { LastUpdate = DateTimeOffset.MinValue, DataNotReady = TheThing.GetSafePropertyBool(sampledThing, "cdeDataNotReady") } ;
                        if (lastSchemaUpdate.DataNotReady)
                        {

                        }
                        doUpdateNow = true;
                    }
                    else
                    {
                        if (lastSchemaUpdate.DataNotReady && !TheThing.GetSafePropertyBool(sampledThing, "cdeDataNotReady"))
                        {
                            lastSchemaUpdate.DataNotReady = false;
                            doUpdateNow = true;
                        }
                    }

                    var now = DateTimeOffset.Now;
                    if (doUpdateNow || lastSchemaUpdate.LastUpdate + new TimeSpan(0, 5, 0) < now)
                    {
                        // Include schema update
                        var schemaThing = new TheThing();
                        sampledThing.CloneThingAndPropertyMetaData(schemaThing, true);
                        sampledThing.ClonePropertyValues(schemaThing);

                        // Only include the properties that are being sent
                        var propertyList = senderThing.GetPropertiesToSend(true);
                        if (propertyList != null)
                        {
                            foreach (var prop in schemaThing.GetAllProperties(10))
                            {
                                if (!propertyList.Contains(prop.Name) && !identityProperties.Contains(prop.Name))
                                {
                                    schemaThing.RemoveProperty(prop.Name);
                                }
                            }
                        }

                        thingEvent.ThingMetaData = schemaThing;
                        lastThingSchemaUpdateSent.AddOrUpdate(sampledThing.cdeMID, lastSchemaUpdate, (g, status) => { if (status.LastUpdate < now) { status.LastUpdate = now; } return status; } );
                    }

                    foreach(var identityProp in identityProperties)
                    {
                        if (!thingEvent.PB.ContainsKey(identityProp)) thingEvent.PB.Add(identityProp, TheThing.GetSafePropertyString(sampledThing, identityProp));
                    }
                    //if (!thingEvent.PB.ContainsKey(strEngineNameProp)) thingEvent.PB.Add(strEngineNameProp, sampledThing.EngineName);
                    //if (!thingEvent.PB.ContainsKey(strDeviceTypeProp)) thingEvent.PB.Add(strDeviceTypeProp, sampledThing.DeviceType);
                    //if (!thingEvent.PB.ContainsKey(nameof(TheThing.FriendlyName))) thingEvent.PB.Add(nameof(TheThing.FriendlyName), sampledThing.FriendlyName);

                    //if (!valueDict.ContainsKey(strAddressProp)) valueDict.Add(strAddressProp, sampledThing.Address);
                    //if (!valueDict.ContainsKey(strTimeMaxProp)) valueDict.Add(strTimeMaxProp, changeDateTimeOffset ? this.TimeMax.UtcDateTime : this.TimeMax);
                }
            }
            return true;
        }

        HashSet<string> identityProperties = new HashSet<string> { nameof(TheThing.EngineName), nameof(TheThing.DeviceType), nameof(TheThing.ID), nameof(TheThing.FriendlyName) };
        class SchemaUpdateStatus
        {
            public DateTimeOffset LastUpdate;
            public bool DataNotReady;
        }
        cdeConcurrentDictionary<Guid, SchemaUpdateStatus> lastThingSchemaUpdateSent = new cdeConcurrentDictionary<Guid, SchemaUpdateStatus>();

        public const string strThingIdProp = "cdeMID";
        public const string strThingNodeProp = "cdeN";
        public const string strEngineNameProp = "EngineName";
        public const string strDeviceTypeProp = "DeviceType";
        //public const string strAddressProp = "Address";
        //public const string strTimeProp = "cdeTime";
        //public const string strTimeMaxProp = "cdeTimeMax";
    }
    public class InvalidTransportEncodingException : Exception
    {
    }

    public static class TheEventConverters
    {
        // Future: Discover and load all types that implement IEventConverter
        private static Dictionary<string, IEventConverter> _eventConverters;
        private static object _eventConverterLock = new object();
        
        static Dictionary<string, IEventConverter> EventConverters
        {
            get
            {
                if (_eventConverters == null)
                {
                    lock (_eventConverterLock)
                    {
                        if (_eventConverters == null)
                        {
                            // TODO Load via reflection or implement as things?
                            _eventConverters = new Dictionary<string, IEventConverter>
                            {
                                { "JSON Things", new JSonThingEventConverter() },
                                { "JSON Objects", new JSonObjectEventConverter() },
                                { "JSON Objects Rooted", new JSonObjectEventConverterWithRoot() },
                                { "JSON Properties", new JSonPropertyEventConverter() },
                                { "CSV", new CSVEventConverter() },
#if INCLUDE_PPMP_EVENTCONVERTER
                                { "Bosch PPMP Measurement", new PPMPMeasurementEventConverter() },
#endif
#if INCLUDE_AMQPEVENTCONVERTER // No Azure in Standard yet (missing std library from MSFT)
                                { "AppProperties", new AMQPPropertyEventConverter() },
#endif
                                //{ "JSON Full Things", new JSonFullThingEventConverter() },
                            };
                        }
                    }
                }
                return _eventConverters;
            }
        }

        public static IEventConverter GetEventConverter(string eventFormat, bool newInstance = false)
        {
            if (String.IsNullOrEmpty(eventFormat))
            {
                eventFormat = TheEventConverters.GetDisplayName(typeof(JSonThingEventConverter));
                if (eventFormat == null)
                {
                    eventFormat = TheEventConverters.GetDisplaynames().First();
                }
            }

            IEventConverter converter;
            if (EventConverters.TryGetValue(eventFormat, out converter))
            {
                if (!newInstance)
                {
                    return converter;
                }
                try
                {
                    var newConverter = Activator.CreateInstance(converter.GetType());
                    return newConverter as IEventConverter;
                }
                catch { };
            }
            return null;
        }

        public static void AddEventConverter(string eventFormat, IEventConverter converter)
        {
            lock (_eventConverterLock)
            {
                EventConverters.Add(eventFormat, converter);
            }
        }

        public static IEnumerable<string> GetDisplaynames()
        {
            return EventConverters.Select(kv => kv.Key);
        }
        public static string GetDisplayName(IEventConverter converter)
        {
            return EventConverters.FirstOrDefault(kv => kv.Value == converter).Key;
        }
        public static string GetDisplayName(Type T)
        {
            return EventConverters.FirstOrDefault(kv => kv.Value.GetType() == T).Key;
        }
        public static string GetDisplayNamesAsSemicolonSeperatedList()
        {
            return EventConverters.Aggregate("", (agg, kv) => agg + kv.Key + ";").TrimEnd(';');
        }
    }

    /// <summary>
    /// Represents a Thing as a serialized CDEngine TheThing object
    /// Example for a Thing with two integer properties Prop1 = 1 and Prop2 = 2
    /// TODO Update... { "Prop1" : 1, " Prop2" : 2, "cdeThingId" : myThing.cdeMid, "cdeAddress" : myThing.Address, "cdeDeviceType" : myThing.DeviceType, "cdeTime" : myThing.cdeCTIM, "cdeTimeMax" : myThing.cdeCTIM }
    /// </summary>
    public class JSonThingEventConverter : IEventConverter
    {
        public string DisplayName { get; set; }
        public Func<TheThing, TheThingStore, bool> ApplyUpdateCallback { get; internal set; }

        public override IEnumerable<object> GetEventData(IEnumerable<TheThingStore> thingUpdates, IThingToConvert senderThing, int maxEventDataSize, bool doNotBatchEvents)
        {
            var thingList = new List<object>();
            foreach (var thingUpdate in thingUpdates)
            {
                UpdatePropertyValuesToSend(thingUpdate, false, senderThing, true);
                if (doNotBatchEvents)
                {
                    string eventPayloadJson = TheCommonUtils.SerializeObjectToJSONString(thingUpdate);
                    thingList.Add(eventPayloadJson);
                }
            }
            if (!doNotBatchEvents)
            {
                return GetEvents(thingUpdates, maxEventDataSize);
            }
            return thingList;
        }

        IEnumerable<object> GetEvents(IEnumerable<TheThingStore> updates, int maxEventDataSize)
        {
            if (!(updates is List<TheThingStore>))
            {
                updates = updates.ToList();
            }
            string eventPayloadJson = TheCommonUtils.SerializeObjectToJSONString(updates as List<TheThingStore>);
            if (eventPayloadJson.Length > maxEventDataSize)
            {
                var updateCount = updates.Count();
                if (updateCount > 1)
                {
                    //Console.WriteLine("Splitting array");
                    var firstHalf = updates.Count() / 2;
                    return GetEvents(updates.Take(firstHalf), maxEventDataSize)
                        .Concat(GetEvents(updates.Skip(firstHalf), maxEventDataSize));
                }
            }
            return new List<object> { eventPayloadJson };
        }


        object MyMetaDataLock = new object();
        public override string ProcessEventData(TheThing thing, object evnt, DateTimeOffset defaultTime)
        {
            string error = null;
            string json = evnt is string ? evnt as string : eventDecoder?.Invoke(evnt);
            if (json == null)
            {
                error = "Invalid payload";
            }
            else
            {
                List<TheThingStore> thingUpdates;
                try
                {
                    if (json.StartsWith("["))
                    {
                        thingUpdates = TheCommonUtils.DeserializeJSONStringToObject<List<TheThingStore>>(json);
                    }
                    else
                    {
                        thingUpdates = new List<TheThingStore> { TheCommonUtils.DeserializeJSONStringToObject<TheThingStore>(json) };
                    }

                    if (thingUpdates == null)
                    {
                        // TODO track invalid events
                        error = "No data in payload";
                    }
                    else
                    {
                        foreach (var tThingUpdateReceived in thingUpdates)
                        {
                            bool registerThing = false;
                            TheThing thingToUpdate = null;
                            if (thing != null)
                            {
                                thingToUpdate = thing;
                            }
                            else
                            {
                                if (thingToUpdate == null && tThingUpdateReceived.PB.TryGetValue("cdeMID", out var thingMid))
                                {
                                    thingToUpdate = TheThingRegistry.GetThingByMID(TheCommonUtils.CGuid(thingMid), true);
                                }
                                if (thingToUpdate == null && tThingUpdateReceived.PB.TryGetValue("ID", out object idReceived))
                                {
                                    var engineName = "*";
                                    if (tThingUpdateReceived.PB.TryGetValue(nameof(TheThing.EngineName), out var engineNameObj))
                                    {
                                        engineName = TheCommonUtils.CStr(engineNameObj);
                                    }
                                    thingToUpdate = TheThingRegistry.GetThingByID(engineName, TheCommonUtils.CStr(idReceived), true);
                                }
                                if (thingToUpdate == null)
                                {
                                    if (tThingUpdateReceived.ThingMetaData != null)
                                    {
                                        TheBaseAssets.MySYSLOG.WriteToLog(95250, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM("Receiver", $"Thing Metadata Received: Registering new thing. {tThingUpdateReceived.ThingMetaData.cdeMID} - {tThingUpdateReceived.ThingMetaData.FriendlyName}", eMsgLevel.l3_ImportantMessage, TSM.L(eDEBUG_LEVELS.OFF) ? "" : TheCommonUtils.SerializeObjectToJSONString(tThingUpdateReceived.ThingMetaData)));
                                        thingToUpdate = tThingUpdateReceived.ThingMetaData;
                                        registerThing = true;
                                    }
                                    // Do not register just based on a thingUpdate: require a full TheThing schema
                                    //else
                                    //{
                                    //    thingToUpdate = new TheThing();
                                    //    tThingReceived.CloneTo(thingToUpdate);
                                    //    registerThing = true;
                                    //}
                                }
                                else
                                {
                                    if (tThingUpdateReceived.ThingMetaData != null)
                                    {
                                        lock (MyMetaDataLock)
                                        {
                                            // Apply any changes in property schema to existing thing, but don't apply any property values
                                            TheBaseAssets.MySYSLOG.WriteToLog(95251, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM("Receiver", $"Thing Metadata Received: updating existing thing. {thingToUpdate.cdeMID} {thingToUpdate.FriendlyName} with {tThingUpdateReceived.cdeMID} {tThingUpdateReceived.ThingMetaData.cdeMID} - {tThingUpdateReceived.ThingMetaData.FriendlyName}", eMsgLevel.l3_ImportantMessage, TSM.L(eDEBUG_LEVELS.VERBOSE) ? "" : TheCommonUtils.SerializeObjectToJSONString(tThingUpdateReceived.ThingMetaData)));
                                            tThingUpdateReceived.ThingMetaData.CloneThingAndPropertyMetaData(thingToUpdate, false);
                                        }
                                    }
                                }
                            }

                            if (thingToUpdate != null)
                            {
                                thingToUpdate.cdeSEQ = tThingUpdateReceived.cdeSEQ;
                                if (StaticPropsToAdd != null)
                                {
                                    foreach (var staticProp in StaticPropsToAdd)
                                    {
                                        tThingUpdateReceived.PB.Add(staticProp.Key, staticProp.Value);
                                    }
                                }
                                if (ApplyUpdateCallback?.Invoke(thingToUpdate, tThingUpdateReceived) != false)
                                {
                                    thingToUpdate.SetProperties(tThingUpdateReceived.PB, tThingUpdateReceived.cdeCTIM);
                                }
                                if (registerThing && !String.IsNullOrEmpty(thingToUpdate.EngineName))
                                {
                                    if (NewThingCallback == null || NewThingCallback(thingToUpdate).Result == true)
                                    {
                                        TheBaseAssets.MySYSLOG.WriteToLog(95252, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM("Receiver", $"Thing Metadata Received: new Thing registered. {thingToUpdate.cdeMID} - {thingToUpdate.FriendlyName}", eMsgLevel.l6_Debug, TheCommonUtils.SerializeObjectToJSONString(thingToUpdate)));
                                        TheThingRegistry.RegisterThing(thingToUpdate);
                                    }
                                    else
                                    {
                                        TheBaseAssets.MySYSLOG.WriteToLog(95252, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM("Receiver", $"Thing Metadata Received: new Thing not registered because of caller decision. {thingToUpdate.cdeMID} - {thingToUpdate.FriendlyName}", eMsgLevel.l6_Debug, TheCommonUtils.SerializeObjectToJSONString(thingToUpdate)));
                                    }
                                }
                                else
                                {
                                    //TheThingRegistry.UpdateThing(thingToUpdate, true);
                                }

                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    error = e.ToString();
                }
            }
            return error;
        }
    }

    /// <summary>
    /// Represents a Thing as a serialized CDEngine TheThing object
    /// Example for a Thing with two integer properties Prop1 = 1 and Prop2 = 2
    /// TODO Update... { "Prop1" : 1, " Prop2" : 2, "cdeThingId" : myThing.cdeMid, "cdeAddress" : myThing.Address, "cdeDeviceType" : myThing.DeviceType, "cdeTime" : myThing.cdeCTIM, "cdeTimeMax" : myThing.cdeCTIM }
    /// </summary>
    public class JSonFullThingEventConverter : IEventConverter
    {
        public override IEnumerable<object> GetEventData(IEnumerable<TheThingStore> thingUpdates, IThingToConvert senderThing, int maxEventDataSize, bool doNotBatchEvents)
        {
            var thingList = new List<object>();
            foreach (var thingUpdate in thingUpdates)
            {
                UpdatePropertyValuesToSend(thingUpdate, false, senderThing, false);

                var props = thingUpdate.PB;

                var originalThing = TheThingRegistry.GetThingByMID(thingUpdate.cdeMID, true);

                var tThing = new TheThing();
                originalThing.CloneTo(tThing);
                tThing.Capabilities = originalThing.Capabilities;
                foreach (var propKV in props)
                {
                    tThing.SetProperty(propKV.Key, propKV.Value, originalThing.GetPropertyType(propKV.Key), thingUpdate.cdeCTIM, -1, null);
                }

                string eventPayloadJson = TheCommonUtils.SerializeObjectToJSONString(tThing);
                //string eventPayloadJson = TheCommonUtils.SerializeObjectToJSONString(eventPayload);

                //using (var x = System.IO.File.AppendText("testoutput.json")) { x.WriteLine(eventPayloadJson); x.Flush(); x.Close(); }
                //EventData eventToSend = new EventData(Encoding.UTF8.GetBytes(eventPayloadJson));

                thingList.Add(eventPayloadJson);
            }
            return thingList;
        }

        public override string ProcessEventData(TheThing thing, object evnt, DateTimeOffset defaultTime)
        {
            string error = null;
            string json = evnt is string ? evnt as string : eventDecoder?.Invoke(evnt);
            if (json == null)
            {
                error = "Invalid payload";
            }
            else
            {
                List<TheThing> payload;
                try
                {
                    if (json.StartsWith("["))
                    {
                        payload = TheCommonUtils.DeserializeJSONStringToObject<List<TheThing>>(json);
                    }
                    else
                    {
                        payload = new List<TheThing> { TheCommonUtils.DeserializeJSONStringToObject<TheThing>(json) };
                    }

                    if (payload == null)
                    {
                        // TODO track invalid events
                        error = "No data in payload";
                    }
                    else
                    {
                        foreach (var tThingReceived in payload)
                        {
                            TheThing thingToUpdate;
                            if (thing != null)
                            {
                                if (thing.cdeMID != tThingReceived.cdeMID)
                                {
                                    thingToUpdate = null;
                                }
                                else
                                {
                                    thingToUpdate = thing;
                                }
                            }
                            else
                            {
                                thingToUpdate = TheThingRegistry.GetThingByMID(tThingReceived.cdeMID, true);
                                if (thingToUpdate == null)
                                {
                                    TheThingRegistry.RegisterThing(tThingReceived);
                                }
                            }

                            if (thingToUpdate != null)
                            {
                                tThingReceived.ClonePropertyValues(thingToUpdate); // This
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    error = e.ToString();
                }
            }
            return error;
        }
    }


    /// <summary>
    /// Represents a Thing as a JSON object, where each Thing property maps to a JSON name/value pair.
    /// Example for a Thing with two integer properties Prop1 = 1 and Prop2 = 2
    /// { "Prop1" : 1, " Prop2" : 2, "cdeMID" : myThing.cdeMid, "cdeAddress" : myThing.Address, "cdeDeviceType" : myThing.DeviceType, "cdeTime" : myThing.cdeCTIM, "cdeTimeMax" : myThing.cdeCTIM }
    /// </summary>
    public class JSonObjectEventConverter : IEventConverter
    {
        public override IEnumerable<object> GetEventData(IEnumerable<TheThingStore> thingUpdates, IThingToConvert senderThing, int maxEventDataSize, bool doNotBatchEvents)
        {
            var objectList = new List<object>();
            foreach (var thingUpdate in thingUpdates)
            {
                UpdatePropertyValuesToSend(thingUpdate, false, senderThing, false);
                if (!thingUpdate.PB.ContainsKey("time") && thingUpdate.cdeCTIM != DateTimeOffset.MinValue)
                {
                    thingUpdate.PB.Add("time", thingUpdate.cdeCTIM);
                }
                var eventPayload = thingUpdate.PB;

                string eventPayloadJson = TheCommonUtils.SerializeObjectToJSONString(eventPayload);
                //string eventPayloadJson = TheCommonUtils.SerializeObjectToJSONString(eventPayload);

                //using (var x = System.IO.File.AppendText("testoutput.json")) { x.WriteLine(eventPayloadJson); x.Flush(); x.Close(); }
                //EventData eventToSend = new EventData(Encoding.UTF8.GetBytes(eventPayloadJson));
                objectList.Add(eventPayloadJson);
            }
            return objectList;
        }

        public override string ProcessEventData(TheThing thing, object evnt, DateTimeOffset defaultTime)
        {
            string error = null;
            string json = evnt is string ? evnt as string : eventDecoder?.Invoke(evnt);
            if (json == null)
            {
                error = "Invalid payload";
            }
            else
            {
                List<Dictionary<string, object>> payload;
                try
                {
                    if (json.StartsWith("["))
                    {
                        payload = TheCommonUtils.DeserializeJSONStringToObject<List<Dictionary<string, object>>>(json);
                    }
                    else
                    {
                        payload = new List<Dictionary<string, object>> { TheCommonUtils.DeserializeJSONStringToObject<Dictionary<string, object>>(json) };
                    }

                    if (payload == null)
                    {
                        // TODO track invalid events
                        error = "No data in payload";
                    }
                    else
                    {
                        foreach (var item in payload)
                        {
                            TheThing tThing = thing;
                            bool bRegisterThing = false;
                            if (thing == null)
                            {
                                Guid thingID = Guid.Empty;
                                if (item.ContainsKey(IEventConverter.strThingIdProp))
                                {
                                    thingID = TheCommonUtils.CGuid(item[IEventConverter.strThingIdProp]);
                                }
                                // For compatibility with V3.2 JSON Objects serializer
                                else if (item.ContainsKey("cdeThingId"))
                                {
                                    thingID = TheCommonUtils.CGuid(item["cdeThingId"]);
                                }
                                tThing = TheThingRegistry.GetThingByMID(thingID, true);
                                if (tThing == null)
                                {
                                    tThing = new TheThing();
                                    bRegisterThing = true;
                                }
                            }


                            DateTimeOffset time;
                            //if (item.ContainsKey(TheSenderBase.ThingEvent.strTimeMaxProp))
                            //{
                            //    time = TheCommonUtils.CDate(item[TheSenderBase.ThingEvent.strTimeMaxProp]);
                            //}
                            //else
                            //if (item.ContainsKey(IEventConverter.strTimeProp))
                            //{
                            //    time = TheCommonUtils.CDate(item[IEventConverter.strTimeProp]);
                            //}
                            //else
                            if (item.ContainsKey("time"))
                            {
                                time = TheCommonUtils.CDate(item["time"]);
                            }
                            else if (defaultTime != DateTimeOffset.MinValue)
                            {
                                time = defaultTime;
                            }
                            else
                            {
                                time = DateTimeOffset.Now;
                            }

                            if (tThing != null)
                            {
                                foreach (var prop in item)
                                {
                                    switch (prop.Key)
                                    {
                                        case IEventConverter.strThingIdProp:
                                            tThing.cdeMID = TheCommonUtils.CGuid(prop.Value);
                                            break;
                                        case IEventConverter.strThingNodeProp:
                                            tThing.cdeN = TheCommonUtils.CGuid(prop.Value);
                                            break;
                                        case IEventConverter.strEngineNameProp:
                                            tThing.EngineName = TheCommonUtils.CStr(prop.Value);
                                            break;
                                        case IEventConverter.strDeviceTypeProp:
                                            tThing.DeviceType = TheCommonUtils.CStr(prop.Value);
                                            break;
                                        case "cdeDeviceType": // for compatilibty with V3.2 Device Gate
                                            if (String.IsNullOrEmpty(tThing.DeviceType))
                                            {
                                                tThing.DeviceType = TheCommonUtils.CStr(prop.Value);
                                            }
                                            break;
                                        //case IEventConverter.strTimeMaxProp:
                                        //    tThing.cdeCTIM = TheCommonUtils.CDate(prop.Value);
                                        //    break;
                                        //case IEventConverter.strTimeProp:
                                        //    tThing.cdeCTIM = TheCommonUtils.CDate(prop.Value);
                                        //    break;

                                        default:
                                            var tProp = tThing.SetProperty(prop.Key, prop.Value, time);
                                            //var tProp = thing.GetProperty(prop.Key, true);
                                            //tProp.cdeCTIM = time;
                                            //tProp.Value = prop.Value;
                                            //tProp.cdeE = 0x40; // NMI visible
                                            break;
                                    }
                                }
                                if (bRegisterThing)
                                {
                                    if (String.IsNullOrEmpty(tThing.EngineName))
                                    {
                                        // TODO Workaround for limitation in V3.2 Device Gate (no engine name is sent, only device type)
                                        if (tThing.DeviceType == "MyProduct Machine Connector")
                                        {
                                            tThing.EngineName = "CDMyMyProductRCI.MyProductRCIService";
                                        }
                                    }
                                    if (!String.IsNullOrEmpty(tThing.EngineName))
                                    {
                                        TheThingRegistry.RegisterThing(tThing);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    error = e.ToString();
                }
            }
            return error;
        }
    }

    /// <summary>
    /// Represents a Thing as a JSON object, where each Thing property maps to a JSON name/value pair, and that is contained in a single root value.
    /// Example for a Thing with two integer properties Prop1 = 1 and Prop2 = 2
    /// { "Entries"":{ "Prop1" : 1, " Prop2" : 2, "cdeThingId" : myThing.cdeMid, "cdeAddress" : myThing.Address, "cdeDeviceType" : myThing.DeviceType, "cdeTime" : myThing.cdeCTIM, "cdeTimeMax" : myThing.cdeCTIM } }
    /// </summary>
    public class JSonObjectEventConverterWithRoot : IEventConverter
    {
        public override IEnumerable<object> GetEventData(IEnumerable<TheThingStore> thingUpdates, IThingToConvert senderThing, int maxEventDataSize, bool doNotBatchEvents)
        {
            var objectList = new List<object>();
            foreach (var thingUpdate in thingUpdates)
            {
                this.UpdatePropertyValuesToSend(thingUpdate, true, senderThing, false);
                var props = thingUpdate.PB;
                string rootElementName;
                object rootElementNameObj;
                if (props.TryGetValue("cdeRootName", out rootElementNameObj))
                {
                    rootElementName = TheCommonUtils.CStr(rootElementNameObj);
                    props.Remove("cdeRootName");
                }
                else
                {
                    rootElementName = DefaultRootElementName;
                }
                var eventPayload = new Dictionary<string, Dictionary<string, object>>
                {
                    { rootElementName, props },
                };

                string eventPayloadJson = TheCommonUtils.SerializeObjectToJSONString(eventPayload);

                //using (var x = System.IO.File.AppendText("testoutput.json")) { x.WriteLine(eventPayloadJson); x.Flush(); x.Close(); }
                objectList.Add(eventPayloadJson);
            }
            return objectList;
        }

        public string DefaultRootElementName = "Entries";
        //class RootWithProperties
        //{
        //    public Dictionary<string, object> Entries;
        //}

        public override string ProcessEventData(TheThing theThing, object evnt, DateTimeOffset defaultTimestamp)
        {
            string payloadJson = DecodeEventAsString(evnt);
            Dictionary<string, Dictionary<string,object>> payload = null;
            string error = "";
            if (payloadJson == null)
            {
                error = "Invalid payload";
            }
            else
            {
                try
                {
                    payload = TheCommonUtils.DeserializeJSONStringToObject<Dictionary<string, Dictionary<string, object>>>(payloadJson);

                    if (payload == null || payload.Count == 0)
                    {
                        // TODO: track invalid events
                        error = "No data in payload";
                    }
                    else if (payload.Count > 1)
                    {
                        error = $"Multiple root elements in payload";
                    }
                    else
                    {
                        Dictionary<string, object> innerPayload;
                        innerPayload = payload.Values.First();
                        if (innerPayload != null)
                        {
                            DateTimeOffset timeValue = DateTimeOffset.Now;
                            object timeObject;
                            if (innerPayload.TryGetValue("TimeStamp", out timeObject))
                            {
                                var unixTime = TheCommonUtils.CLng(timeObject);
                                timeValue = new DateTime(unixTime * TimeSpan.TicksPerMillisecond, DateTimeKind.Local);
                            }
                            foreach (var prop in innerPayload)
                            {
                                if (prop.Key != null)
                                {
                                    var tProp = theThing.SetProperty(prop.Key, prop.Value, timeValue);
                                }
                                else
                                { }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    error = e.ToString();
                }
            }
            return error;
        }
    }

    /// <summary>
    /// Represents a Thing as a JSON array of objects, where each Thing property maps to a JSON object in that array.
    /// Example for a Thing with two integer properties Prop1 = 1 and Prop2 = 2
    /// [
    ///   { "name" : "Prop1", "value" : 1, "time": cdeP.cdeCTIM},
    ///   { "name" : "Prop2", "value" : 2, "time": cdeP.cdeCTIM}
    /// ]
    /// </summary>
    public class JSonPropertyEventConverter : IEventConverter
    {
        public class JSonArrayElement: Dictionary<string,object>
        {
            [IgnoreDataMember()]
            public DateTimeOffset time
            {
                get
                {
                    if (this.TryGetValue(nameof(time), out var retval))
                        return TheCommonUtils.CDate(retval);
                    return default(DateTimeOffset);
                }
                set { this[nameof(time)] = value; }
            }
            //public string address;
            [IgnoreDataMember()]
            public string name
            {
                get
                {
                    if (this.TryGetValue(nameof(name), out var retval))
                        return TheCommonUtils.CStr(retval);
                    return null;
                }

                set { this[nameof(name)] = value; }
            }
            // value: from base class
            [IgnoreDataMember()]
            public object value
            {
                get
                {
                    if (this.TryGetValue(nameof(this.value), out var retval))
                        return retval;
                    return null;
                }
                set { this[nameof(this.value)] = value; }
            }

            static readonly char[] brackets = { '[', ']' };

            public JSonArrayElement(string propName, DateTimeOffset propTime, object propValue, TheThingStore parentThing)
            {
                time = propTime;
                //address = thingEvent.Address,
                name = propName;

                TheOPCUAValue propOPCValue = null;
                if (!(propValue is TheOPCUAValue))
                {
                    if (parentThing != null)
                    {
                        var propOfPropPrefix = $"[{propName}].";
                        var propsOfProp = parentThing.PB.Where(p => p.Key.StartsWith(propOfPropPrefix));
                        foreach (var prop in propsOfProp)
                        {
                            // [n1].[n2].[n3]
                            var subPropName = prop.Key.Substring(propOfPropPrefix.Length);
                            if (!subPropName.Contains("].["))
                            {
                                // Remove brackets on first level sub-properties
                                subPropName = subPropName.Trim(brackets);
                            }
                            this[subPropName] = prop.Value;
                        }
                    }
                    // Check if the value looks like a JSON object, and if so attempt to deserialize it as an TheOPCUAValue (can happen after restart, when thing registry was loaded)
                    if (propValue is String && ((String)propValue).StartsWith("{") && ((String)propValue).EndsWith("}"))
                    {
                        try
                        {
                            propOPCValue = TheCommonUtils.DeserializeJSONStringToObject<TheOPCUAValue>((String)propValue);
                            if (propOPCValue.statusCode == null && propOPCValue.value == null && propOPCValue.serverTimestamp == null && propOPCValue.sourcePicoseconds == null && propOPCValue.serverPicoseconds == null)
                            {
                                propOPCValue = null;
                            }
                        }
                        catch { }
                    }
                }
                else
                {
                    propOPCValue = propValue as TheOPCUAValue;
                }

                if (propOPCValue == null)
                {
                    value = propValue;
                }
                else
                {
                    // Inline the additional properties, so we don't end up with unnecessary nesting
                    // i.e. resulting JSON is
                    // { "name": "whatever", "value": 1234, "statusCode" : 12345678 }
                    // instead of
                    // { "name": "whatever", "value": { "value" : 1234, "statusCode" : 12345678 } }

                    value = propOPCValue.value;
                    this["statusCode"] = propOPCValue.statusCode;
                    this["serverTimestamp"] = propOPCValue.serverTimestamp;
                    this["sourcePicoseconds"] = propOPCValue.sourcePicoseconds;
                    this["serverPicoseconds"] = propOPCValue.serverPicoseconds;
                }
                //if (parentThing.cdeSEQ != null)
                //{
                //    this["sequenceNumber"] = parentThing.cdeSEQ.Value;
                //}
            }

        }

        IEnumerable<JSonArrayElement> GetJSONArray(TheThingStore thingEvent)
        {
            var jsonArray = new List<JSonArrayElement>();

            jsonArray.AddRange(thingEvent.PB.Where(propKV => !propKV.Key.StartsWith("[")).Select(propKV =>
            {
                return new JSonArrayElement(propKV.Key, thingEvent.cdeCTIM, propKV.Value, thingEvent);
            }));
            return jsonArray;
        }

        string SerializeJSonArray(IEnumerable<JSonArrayElement> jsonArray)
        {
            if (!(jsonArray is List<JSonArrayElement>))
            {
                jsonArray = new List<JSonArrayElement>(jsonArray);
            }
            string eventPayloadJson = TheCommonUtils.SerializeObjectToJSONString(jsonArray as List<JSonArrayElement>);
            return eventPayloadJson;
        }

        public override IEnumerable<object> GetEventData(IEnumerable<TheThingStore> thingUpdates, IThingToConvert senderThing, int maxEventDataSize, bool doNotBatchEvents)
        {
            var events = new List<object>();
            foreach (var thingUpdate in thingUpdates)
            {
                var jsonArray = GetJSONArray(thingUpdate);
                if (doNotBatchEvents)
                {
                    events.AddRange(jsonArray.Select(elem => (object)TheCommonUtils.SerializeObjectToJSONString(elem)));
                }

                events.AddRange(GetEvents(jsonArray, maxEventDataSize));
                //if (events.Count() > 1)
                //{
                //    Console.WriteLine("Returning {0} events for {1} properties", events.Count(), jsonArray.Count);
                //}
            }
            return events;
        }

        IEnumerable<object> GetEvents(IEnumerable<JSonArrayElement> jsonArray, int maxEventDataSize)
        {
            string eventPayloadJson = SerializeJSonArray(jsonArray);
            //string eventPayloadJson = TheCommonUtils.SerializeObjectToJSONString<List<JSonArrayElement>>(jsonArray as List<JSonArrayElement>);
            if (eventPayloadJson.Length > maxEventDataSize)
            {
                //Console.WriteLine("Splitting array");
                var firstHalf = jsonArray.Count() / 2;
                return GetEvents(jsonArray.Take(firstHalf), maxEventDataSize)
                    .Concat(GetEvents(jsonArray.Skip(firstHalf), maxEventDataSize));
            }
            return new List<object> { eventPayloadJson };
        }

        public override string ProcessEventData(TheThing thing, object evnt, DateTimeOffset defaultTimestamp)
        {
            if (thing == null)
            {
                return "P08 Event Converter does not support thing mapping";
            }
            List<JSonArrayElement> payload = null;
            string error = "";
            var json = DecodeEventAsString(evnt);
            if (json == null)
            {
                error = "Invalid payload";
            }
            else
            {
                try
                {
                    payload = TheCommonUtils.DeserializeJSONStringToObject<List<JSonArrayElement>>(json);
                    //payload = TheCommonUtils.DeserializeJSONStringToObject<List<JSonArrayElement>>(json);
                    if (payload == null || payload.Count == 0)
                    {
                        error = "No data in payload";
                    }
                    else
                    {

                        foreach (var prop in payload)
                        {
                            if (prop != null)
                            {
                                if (prop.name != null)
                                {
                                    thing.SetProperty(prop.name, prop.value, prop.time);
                                    foreach(var propOfProp in prop)
                                    {
                                        switch (propOfProp.Key)
                                        {
                                            case "name":
                                            case "value":
                                            case "time":
                                                break;
                                            default:
                                                thing.SetProperty($"[{prop.name}].{propOfProp.Key}", propOfProp.Value, prop.time);
                                                break;
                                        }
                                    }
                                }
                                else
                                { }
                            }
                            else { }
                        }
                    }
                }
                catch (Exception e)
                {
                    error = e.ToString();
                }
            }
            return error;
        }

    }

    public class CSVEventConverter : IEventConverter
    {
        public override IEnumerable<object> GetEventData(IEnumerable<TheThingStore> thingUpdates, IThingToConvert senderThing, int maxEventDataSized, bool doNotBatchEvents)
        {
            var eventList = new List<object>();
            foreach (var thingUpdate in thingUpdates)
            {
                var eventPayloadCSV = GetThingEventAsCSV(thingUpdate, senderThing);
                eventList.Add(eventPayloadCSV);
            }
            return eventList;
        }

        class CSV
        {
            public StringBuilder header;
            public StringBuilder row;
        };
        string GetThingEventAsCSV(TheThingStore thingEvent, IThingToConvert senderThing)
        {
            this.UpdatePropertyValuesToSend(thingEvent, false, senderThing, false);
            var eventPayload = thingEvent.PB;
            var eventHeaderAndRow = (from e in eventPayload orderby e.Key select e).Aggregate(new CSV { header = new StringBuilder(), row = new StringBuilder() },
                (headerAndRow, kv) =>
                {
                    headerAndRow.header.Append(kv.Key).Append(",");
                    headerAndRow.row.Append(TheCommonUtils.CStr(kv.Value)).Append(",");
                    return headerAndRow;
                });

            string eventPayloadCSV = eventHeaderAndRow.header.ToString().TrimEnd(',') + "\r\n" + eventHeaderAndRow.row.ToString().TrimEnd(',') + "\r\n";
            return eventPayloadCSV;
        }

        public override string ProcessEventData(TheThing thing, object evnt, DateTimeOffset defaultTimestamp)
        {
            throw new NotImplementedException();
        }

    }
}
