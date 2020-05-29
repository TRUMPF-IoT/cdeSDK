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

using System.Runtime.Serialization;
using nsCDEngine.ViewModels;

//using nsTheSenderBase;

//#if !NET35
//using Microsoft.ServiceBus.Messaging;
//#else

////// Dummy class just to make this file compile for Net35 (to avoid lots of #if's)
////public class EventData
////{
////    public EventData() { }
////    public EventData(byte[] x) { }
////    public byte[] GetBytes() { return null; }
////    public Dictionary<string, object> Properties;
////    public DateTimeOffset EnqueuedTimeUtc;
////}
//#endif

namespace nsTheEventConverters
{

    /// <summary>
    /// </summary>
    public class AxoomIotManagerEventConverter : IEventConverter
    {
        #region REST Classes from Axoom
        public class ObservMessage
        {
            public object value { get; set; }
            public DateTime timestamp { get; set; }
        }

        public class MessageEnvelope
        {
            public object message { get; set; }
            public DateTime timestamp { get; set; }
        }

        public class TheAxoomGateObservation
        {
            public ObservMessage message { get; set; }
            public DateTime timestamp { get; set; }
            public string signature { get; set; }
        }
        public class TheAxoomGateDataStreamObservation
        {
            public string datastream_id { get; set; }
            public List<ObservMessage> observations { get; set; }
        }
        public class TheAxoomGateDataStreamObservations
        {
            public List<TheAxoomGateDataStreamObservation> datastreams { get; set; }
        }

        #endregion

        //public class DeviceInfo
        //{
        //    public string identifier;
        //    public string manufacturer;
        //    public string model;
        //}

        //public class SensorData
        //{
        //    public string identifier;
        //    public object value;
        //    public string unit;
        //}

        //public class JSonSensorEvent
        //{
        //    public DateTime serverTimestamp;
        //    public DeviceInfo device;
        //    public SensorData sensor;
        //}

        IEnumerable<TheAxoomGateDataStreamObservation> GetObservations(IEnumerable<TheThingStore> thingUpdates, Dictionary<string, List<SensorInfo>> sensorIds)
        {
            var observationsByDataStreamId = new Dictionary<string, List<ObservMessage>>();
            foreach (var thingUpdate in thingUpdates)
            {
                GetObservations(thingUpdate, observationsByDataStreamId, sensorIds);
            }

            var observations = observationsByDataStreamId.Select(kv =>
               new TheAxoomGateDataStreamObservation
               {
                   datastream_id = kv.Key,
                   observations = kv.Value,
               });
            return observations;
        }

        public class SensorInfo
        {
            public string DataStreamId { get; set; }
            public ePropertyTypes cdeType { get; set; }
            public string AxoomType { get; set; }
        }

        void GetObservations(TheThingStore thingUpdate, Dictionary<string, List<ObservMessage>> observationsByDataStreamId, Dictionary<string, List<SensorInfo>> sensorIds)
        {
            //// TODO Replace with linkid some identifier that represents the activated instance/customer, and make this more efficient (avoid or cache thing registry lookup)
            //string thingIdentifier = null;
            //string manufacturer = null;
            //string model = null;

            //if (thingUpdate.PB.ContainsKey("SerialNumber"))
            //{
            //    thingIdentifier = TheCommonUtils.CStr(thingUpdate.PB["SerialNumber"]);
            //}

            //if (thingUpdate.PB.ContainsKey("VendorName"))
            //{
            //    manufacturer = TheCommonUtils.CStr(thingUpdate.PB["VendorName"]);
            //}
            //else if (thingUpdate.PB.ContainsKey("Manufacturer"))
            //{
            //    manufacturer = TheCommonUtils.CStr(thingUpdate.PB["Manufacturer"]);
            //}

            //if (thingUpdate.PB.ContainsKey("Model"))
            //{
            //    model = TheCommonUtils.CStr(thingUpdate.PB["Model"]);
            //}

            //var tThing = TheThingRegistry.GetThingByMID("*", TheCommonUtils.CGuid(thingUpdate.cdeMID), false);
            //if (tThing != null)
            //{
            //    if (String.IsNullOrEmpty(thingIdentifier))
            //    {
            //        thingIdentifier = TheThing.GetSafePropertyString(tThing, "SerialNumber");
            //    }
            //    if (String.IsNullOrEmpty(thingIdentifier))
            //    {
            //        thingIdentifier = tThing.FriendlyName;
            //    }
            //    if (String.IsNullOrEmpty(manufacturer))
            //    {
            //        manufacturer = TheThing.GetSafePropertyString(tThing, "VendorName");
            //    }
            //    if (String.IsNullOrEmpty(manufacturer))
            //    {
            //        manufacturer = TheThing.GetSafePropertyString(tThing, "Manufacturer");
            //    }
            //    if (String.IsNullOrEmpty(model))
            //    {
            //        model = tThing.DeviceType;
            //    }
            //}
            //if (String.IsNullOrEmpty(thingIdentifier))
            //{
            //    if (thingUpdate.PB.ContainsKey("Address"))
            //    {
            //        thingIdentifier = TheCommonUtils.CStr(thingUpdate.PB["Address"]);
            //    }
            //}
            //if (manufacturer == null)
            //{
            //    manufacturer = thingIdentifier; // "Baumer Electric AG";
            //}
            //if (model == null)
            //{
            //    model = thingIdentifier; // "O500.GP-11125079";
            //}
            foreach(var propKV in thingUpdate.PB.ToList())
            {
                if (!reservedProps.Contains(propKV.Key))
                {
                    List<SensorInfo> sensorInfoList = null;
                    if (sensorIds?.TryGetValue(propKV.Key, out sensorInfoList) != true || sensorInfoList == null)
                    {
                        throw new Exception($"No sensor type info available yet for {propKV.Key}");
                        //TheBaseAssets.MySYSLOG.WriteToLog(95249, new TSM(nameof(AxoomIotManagerEventConverter), "IoT Sender: ignoring sensors that haven't been cached yet", eMsgLevel.l1_Error, $"{propKV.Key}"));
                    }
                    foreach (var sensorInfo in sensorInfoList)
                    {
                        var dataStreamId = sensorInfo.DataStreamId;
                        var propertyType = sensorInfo.cdeType;
                        var AxoomType = sensorInfo.AxoomType;
                        if (!string.IsNullOrEmpty(dataStreamId))
                        {
                            var propValue = propKV.Value;
                            if (propertyType == ePropertyTypes.TNumber
                                &&
                                (
                                    propValue is string
                                    ||
                                        (
                                            !(propValue is double)
                                        && !(propValue is float)
                                        && !(propValue is decimal)
                                        && !(propValue is Int64) && !(propValue is UInt64)
                                        && !(propValue is Byte) && !(propValue is SByte)
                                        && !(propValue is Int32) && !(propValue is UInt32)
                                        && !(propValue is Int16) && !(propValue is UInt16)
                                        )
                                ))
                            {
                                propValue = TheCommonUtils.CDbl(propValue);
                            }
                            else if (propertyType == ePropertyTypes.TBoolean && !(propKV.Value is bool))
                            {
                                propValue = TheCommonUtils.CBool(propValue);
                            }
                            switch (AxoomType)
                            {
                                case "Double":
                                    {
                                        var dblValue = TheCommonUtils.CDbl(propValue);
                                        if (double.IsPositiveInfinity(dblValue) || double.IsNegativeInfinity(dblValue) || double.IsNaN(dblValue))
                                        {
                                            propValue = null;
                                        }
                                        else
                                        {
                                            propValue = dblValue;
                                        }
                                    }
                                    break;
                                case "Float":
                                    {
                                        var dblValue = TheCommonUtils.CFloat(propValue);
                                        if (float.IsPositiveInfinity(dblValue) || float.IsNegativeInfinity(dblValue) || float.IsNaN(dblValue))
                                        {
                                            propValue = null;
                                        }
                                        else
                                        {
                                            propValue = dblValue;
                                        }
                                    }
                                    break;
                                case "Integer":
                                    propValue = TheCommonUtils.CInt(propValue);
                                    break;
                                case "Byte":
                                    propValue = TheCommonUtils.CByte(propValue);
                                    break;
                                case "Long":
                                    propValue = TheCommonUtils.CLng(propValue);
                                    break;
                                case "UInteger":
                                    propValue = TheCommonUtils.CUInt(propValue);
                                    break;
                                case "SByte":
                                    propValue = TheCommonUtils.CSByte(propValue);
                                    break;
                                case "ULong":
                                    propValue = TheCommonUtils.CULng(propValue);
                                    break;
                                case "DateTime":
                                    propValue = TheCommonUtils.CDate(propValue);
                                    break;
                                case "String":
                                    if (propValue is DateTimeOffset || propValue is DateTime)
                                    {
                                        propValue = "_" + TheCommonUtils.CStr(propValue); // Workaround for Axoom IoTHub bug that rejects strings with valid date strings
                                    }
                                    else
                                    {
                                        propValue = TheCommonUtils.CStr(propValue);
                                    }
                                    break;
                                case "Boolean":
                                    propValue = TheCommonUtils.CBool(propValue);
                                    break;

                            }
                            if (propValue != null)
                            {
                                List<ObservMessage> observations;
                                if (!observationsByDataStreamId.TryGetValue(dataStreamId, out observations))
                                {
                                    observations = new List<ObservMessage>();
                                    observationsByDataStreamId.Add(dataStreamId, observations);
                                }

                                var observation = new ObservMessage
                                {
                                    timestamp = thingUpdate.cdeCTIM.UtcDateTime,
                                    value = propValue,
                                };
                                observations.Add(observation);
                            }
                            else
                            {
                                TheBaseAssets.MySYSLOG.WriteToLog(95290, TSM.L(eDEBUG_LEVELS.VERBOSE) ? null : new TSM(nameof(AxoomIotManagerEventConverter), "IoT Sender: dropping property because it is null", eMsgLevel.l1_Error, $"{propKV.Key}"));
                            }
                        }
                    }
                }
            }
        }
        HashSet<string> reservedProps = new HashSet<string> { "Manufacturer", "SerialNumber", "VendorName", "Model", "__AxoomSensorIds" };

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


        string SerializeJSonArray(IEnumerable<TheAxoomGateDataStreamObservation> jsonArray)
        {
            if (!(jsonArray is List<TheAxoomGateDataStreamObservation>))
            {
                jsonArray = new List<TheAxoomGateDataStreamObservation>(jsonArray);
            }

            var AxoomMessage = new MessageEnvelope
            {
                timestamp = DateTime.UtcNow,
                message = new TheAxoomGateDataStreamObservations { datastreams = jsonArray as List<TheAxoomGateDataStreamObservation> },
            };

            string eventPayloadJson = TheCommonUtils.SerializeObjectToJSONString(AxoomMessage);
            return eventPayloadJson;
        }

        public override IEnumerable<object> GetEventData(IEnumerable<TheThingStore> thingUpdates, IThingToConvert senderThing, int maxEventDataSize, bool doNotBatchEvents)
        {
            Dictionary<string, List<SensorInfo>> sensorIds = null;
            if (thingUpdates.First().PB.TryGetValue("__AxoomSensorIds", out var sensorIdsObj))
            {
                sensorIds = sensorIdsObj as Dictionary<string, List<SensorInfo>>;
            }

            var jsonArray = GetObservations(thingUpdates, sensorIds);
            if (!jsonArray.Any())
            {
                // Don't send empty observation lists (can happen if not all properties are in the sensorIds dictionary)
                return new List<object>();
            }
            var events = GetEvents(jsonArray, maxEventDataSize);
            //if (events.Count() > 1)
            //{
            //    Console.WriteLine("Returning {0} events for {1} properties", events.Count(), jsonArray.Count);
            //}
            return events;
        }

        IEnumerable<object> GetEvents(IEnumerable<TheAxoomGateDataStreamObservation> jsonArray, int maxEventDataSize)
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
            var json = DecodeEventAsString(evnt);
            List<TheAxoomGateObservation> payload = null;
            string error = null;
            try
            {
                payload = TheCommonUtils.DeserializeJSONStringToObject<List<TheAxoomGateObservation>>(json);
                //payload = TheCommonUtils.DeserializeJSONStringToObject<List<JSonArrayElement>>(json);
                if (payload == null)
                {
                    payload = new List<TheAxoomGateObservation> { TheCommonUtils.DeserializeJSONStringToObject<TheAxoomGateObservation>(json) };
                }
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
                            //if (prop..sensor.identifier != null)
                            //{
                            //    var tProp = thing.SetProperty(prop.sensor.identifier, prop.sensor.value, new DateTimeOffset(prop.serverTimestamp, DateTimeOffset.Now.Offset));
                            //}
                            //else
                            //{ }
                        }
                        else
                        {
                            // TODO Implement deserialization (no customer requirement at this point)
                        }
                    }
                }
            }
            catch (Exception e)
            {
                error = e.ToString();
            }
            return error;
        }
    }
}
