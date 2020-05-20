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

using nsTheEventConverters;
using nsTheSenderBase;
using System.Runtime.Serialization;
using nsCDEngine.ViewModels;

//#if !NET35
//using Microsoft.ServiceBus.Messaging;
//#else

//// Dummy class just to make this file compile for Net35 (to avoid lots of #if's)
//public class EventData
//{
//    public EventData() { }
//    public EventData(byte[] x) { }
//    public byte[] GetBytes() { return null; }
//    public Dictionary<string, object> Properties;
//    public DateTimeOffset EnqueuedTimeUtc;
//}
//#endif

namespace nsTheEventConverters
{

    /// <summary>
    /// Represents a Thing as a JSON array of objects, where each Thing property maps to a JSON object in that array.
    /// Example for a Thing with two integer properties Prop1 = 1 and Prop2 = 2
    /// [
    ///   { "machineid" : NodeId, "linkid" : myThing.FriendlyName, "namespace" : "http://TRUMPF.com/UAInterfaces/MDA" , "identifier" : "Prop1", "servertimestamp": cdeP.cdeCTIM, "sourcetimestamp": cdeP.cdeCTIM,  "value" : 1},
    ///   { "machineid" : NodeId, "linkid" : myThing.FriendlyName, "namespace" : "http://TRUMPF.com/UAInterfaces/MDA" , "identifier" : "Prop2", "servertimestamp": cdeP.cdeCTIM, "sourcetimestamp": cdeP.cdeCTIM,  "value" : 2},
    /// ]
    /// </summary>
    class JsonOpcUaEventConverter : IEventConverter
    {
        class JSonOpcArrayElement
        {
            public string machineid;    
            public string linkid;

            public string @namespace;
            public string identifier;
            public long servertimestamp;
            public long sourcetimestamp;
            public object value;
        }

        IEnumerable<JSonOpcArrayElement> GetJSONArray(TheThingStore thingEvent)
        {
            // TODO Replace with linkid some identifier that represents the activated instance/customer, and make this more efficient (avoid or cache thing registry lookup)
            string linkid;
            var tThing = TheThingRegistry.GetThingByMID("*", TheCommonUtils.CGuid(thingEvent.cdeMID), true);
            if (tThing != null && !String.IsNullOrEmpty(tThing.FriendlyName))
            {
                linkid = tThing.FriendlyName;
            }
            else
            {
                if (thingEvent.PB.ContainsKey("Address"))
                {
                    linkid = TheCommonUtils.CStr(thingEvent.PB["Address"]);
                }
                else
                {
                    if (tThing != null)
                    {
                        linkid = tThing.Address;
                    }
                    else
                    {
                        linkid = TheCommonUtils.cdeGuidToString(thingEvent.cdeMID);
                    }
                }
            }

            var jsonArray = new List<JSonOpcArrayElement>();
            jsonArray.AddRange(thingEvent.PB.Select(propKV => GetJSONArrayElement(linkid, propKV.Key, thingEvent.cdeCTIM, propKV.Value)));
            return jsonArray.OrderBy(ev => ev.sourcetimestamp);
        }

        JSonOpcArrayElement GetJSONArrayElement(string linkid, string propertyName, DateTimeOffset propTime, object propValue)
        {
            //object valueToSend = null;
            //if (prop.Value is string)
            //{
            //    string value = prop.Value as string;
            //    try
            //    {
            //        if (value.StartsWith("{")) // TODO check cdeM as well!
            //        {
            //            // Assume OPC struct: 
            //            // Generate desired format as { "<prop1Name>" : value1, "<prop2Name>" : value2, ... }
            //            var opcStruct = TheCommonUtils.DeserializeJSONStringToObject<Dictionary<string, object>>(value);
            //            //var outputStruct = new Dictionary<string, object>();
            //            //foreach (var p in opcStruct)
            //            //{
            //            //    outputStruct.Add(p.Name, GetTypedValue(p));
            //            //}
            //            valueToSend = opcStruct;
            //        }
            //        else if (value.StartsWith("["))
            //        {
            //            // Assume OPC array of structs
            //            // Generate desired format as [ { "<element1prop1Name> : value1, ... }, { ...} ]
            //            var opcArrayOfStructs = TheCommonUtils.DeserializeJSONStringToObject<List<Dictionary<string, object>>>(value);
            //            //var outputArray = new List<Dictionary<string, object>>();
            //            //foreach (var opcStruct in opcArrayOfStructs)
            //            //{
            //            //    var outputStruct = new Dictionary<string, object>();
            //            //    foreach (var p in opcStruct)
            //            //    {
            //            //        outputStruct.Add(p.Name, GetTypedValue(p));
            //            //    }
            //            //    outputArray.Add(outputStruct);
            //            //}
            //            valueToSend = opcArrayOfStructs;
            //        }
            //    }
            //    catch
            //    {
            //        valueToSend = null;
            //    }
            //    if (valueToSend == null)
            //    {
            //        valueToSend = value;
            //    }
            //}
            //else
            //{
            //    valueToSend = prop.Value;
            //}

            return new JSonOpcArrayElement
            {
                machineid = TheCommonUtils.cdeGuidToString(TheBaseAssets.MyServiceHostInfo.MyDeviceInfo.DeviceID),
                linkid = linkid,
                @namespace = "http://TRUMPF.com/UAInterfaces/MDA",
                servertimestamp = propTime.UtcTicks,  // TODO Replace with OPC server timestamp
                sourcetimestamp = propTime.UtcTicks,
                identifier = propertyName,
                value = propValue,
            };

        }

        //private static object GetTypedValue(StructMemberValue member)
        //{
        //    // TODO Use OPC and other type hints from cdeM to improve the type fidelity?
        //    // TODO Move this into TheCommonUtils or cdeP itself
        //    object retVal = member.Value;
        //    switch (member.cdeT)
        //    {
        //        case (int)ePropertyTypes.TNumber:
        //            retVal = TheCommonUtils.CDbl(member.Value);
        //            break;
        //        case (int)ePropertyTypes.TBoolean:
        //            retVal = TheCommonUtils.CBool(member.Value);
        //            break;
        //        case (int)ePropertyTypes.TDate:
        //            retVal = TheCommonUtils.CDate(member.Value);
        //            break;
        //        case (int)ePropertyTypes.TBinary:
        //            retVal = TheCommonUtils.CByte(member.Value);
        //            break;
        //        case (int)ePropertyTypes.TGuid:
        //            retVal = TheCommonUtils.CGuid(member.Value);
        //            break;
        //        case (int)ePropertyTypes.TString:
        //        default:
        //            retVal = member.Value;
        //            break;
        //    }
        //    return retVal;
        //}
    

        string SerializeJSonArray(IEnumerable<JSonOpcArrayElement> jsonArray)
        {
            if (!(jsonArray is List<JSonOpcArrayElement>))
            {
                jsonArray = new List<JSonOpcArrayElement>(jsonArray);
            }
            string eventPayloadJson = TheCommonUtils.SerializeObjectToJSONString(jsonArray as List<JSonOpcArrayElement>);
            return eventPayloadJson;
        }

        public override IEnumerable<object> GetEventData(IEnumerable<TheThingStore> thingUpdates, TheSenderThing senderThing, int maxEventDataSize, bool doNotBatchEvents)
        {
            var eventList = new List<object>();
            foreach (var thingUpdate in thingUpdates)
            {
                var jsonArray = GetJSONArray(thingUpdate);
                if (doNotBatchEvents)
                {
                    eventList.AddRange(jsonArray.Select(elem => (object)TheCommonUtils.SerializeObjectToJSONString(elem)));
                }

                eventList.AddRange(GetEvents(jsonArray, maxEventDataSize));
                //if (events.Count() > 1)
                //{
                //    Console.WriteLine("Returning {0} events for {1} properties", events.Count(), jsonArray.Count);
                //}
            }
            return eventList;
        }

        IEnumerable<object> GetEvents(IEnumerable<JSonOpcArrayElement> jsonArray, int maxEventDataSize)
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
            List<JSonOpcArrayElement> payload = null;
            string error = null;
            string json = DecodeEventAsString(evnt);
            if (json == null)
            {
                error = "Invalid payload";
            }
            else
            {
                try
                {
                    payload = TheCommonUtils.DeserializeJSONStringToObject<List<JSonOpcArrayElement>>(json);
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
                                if (prop.identifier != null)
                                {
                                    var tProp = thing.SetProperty(prop.identifier, prop.value, new DateTimeOffset(prop.sourcetimestamp, DateTimeOffset.Now.Offset));
                                    // TODO Capture servertimestamp
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
}
