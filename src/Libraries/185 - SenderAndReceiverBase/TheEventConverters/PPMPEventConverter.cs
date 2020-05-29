// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿#if true //INCLUDE_PPMP_EVENTCONVERTER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public class PPMPMeasurementEventConverter : IEventConverter
    {
        #region REST Classes per Bosch spec
        public class MeasurementPayload
        {
            public MeasurementPayload()
            {
                contentspec = "urn:spec://bosch.com/cindy/measurement-message#v2"; //"urn:spec://eclipse.org/unide/measurement-message#v2";
                device = new Device();
                measurements = new List<Measurement>();
            }
//#if !CDE_STANDARD // no Newtonsoft for NetStd yet
//            [Newtonsoft.Json.JsonProperty(PropertyName = "content-spec")]
//#endif
            [DataMember(Name ="content-spec")]
            public string contentspec { get; set; }
            public Device device { get; set; }
            public List<Measurement> measurements { get; set; }
        }

        public class Device
        {
            public string deviceID { get; set; }
            public string operationalStatus { get; set; }
            public Dictionary<string,object> metaData { get; set; }

        }

        public class Measurement
        {
            public Measurement()
            {
                series = new Dictionary<string, List<double>>();
            }
            public DateTimeOffset ts { get; set; }
            public Dictionary<string, List<double>> series { get; set; }
        }

        #endregion

        IEnumerable<Measurement> GetMeasurements(TheThingStore thingEvent)
        {
            var jsonArray = new List<Measurement>();
            jsonArray.AddRange(thingEvent.PB
                .Where(prop => !reservedProps.Contains(prop.Key))
                .Select(propKV => GetMeasurement(propKV.Key, thingEvent.cdeCTIM, propKV.Value)));

            return jsonArray.OrderBy(ev => ev.ts);
        }
        HashSet<string> reservedProps = new HashSet<string> { "deviceID" };

        Measurement GetMeasurement(string propertyName, DateTimeOffset propTime, object propValue)
        {
            var m = new Measurement
            {
                ts = propTime,
            };
            m.series.Add(propertyName, new List<double> { TheCommonUtils.CDbl(propValue) });
            m.series.Add("$_time", new List<double> { 0 });
            return m;
        }

        string SerializeJSonArray(TheThingStore thingEvent, IEnumerable<Measurement> measurements)
        {
            if (!(measurements is List<Measurement>))
            {
                measurements = new List<Measurement>(measurements);
            }

            var payload = new MeasurementPayload();

            object deviceId;
            if (thingEvent.PB.TryGetValue("deviceID", out deviceId))
            {
                payload.device.deviceID = TheCommonUtils.CStr(deviceId);
            }
            else
            {
                payload.device.deviceID = TheCommonUtils.cdeGuidToString(thingEvent.cdeMID);
            }
            payload.measurements.AddRange(measurements);

            string eventPayloadJson = TheCommonUtils.SerializeObjectToJSONString(payload);
            //eventPayloadJson = eventPayloadJson.Replace("\"contentspec\"", "\"content-spec\""); // Hack to avoid newtonsoft dependency for now: attribute ignored by cdeEngine internal serializer
            return eventPayloadJson;
        }

        public override IEnumerable<object> GetEventData(IEnumerable<TheThingStore> thingUpdates, IThingToConvert senderThing, int maxEventDataSize, bool doNotBatchEvents)
        {
            // TODO Optimization: Create time series inside each measurement rather thant a new measurement for each data point
            var eventList = new List<object>();
            foreach (var thingUpdate in thingUpdates)
            {
                var measurements = GetMeasurements(thingUpdate);

                eventList.AddRange(GetEvents(thingUpdate, measurements, maxEventDataSize));
                //if (events.Count() > 1)
                //{
                //    Console.WriteLine("Returning {0} events for {1} properties", events.Count(), jsonArray.Count);
                //}
            }
            return eventList;
        }

        IEnumerable<object> GetEvents(TheThingStore thingEvent, IEnumerable<Measurement> measurements, int maxEventDataSize)
        {
            string eventPayloadJson = SerializeJSonArray(thingEvent, measurements);
            //string eventPayloadJson = TheCommonUtils.SerializeObjectToJSONString<List<JSonArrayElement>>(jsonArray as List<JSonArrayElement>);
            if (eventPayloadJson.Length > maxEventDataSize)
            {
                //Console.WriteLine("Splitting array");
                var firstHalf = measurements.Count() / 2;
                return GetEvents(thingEvent, measurements.Take(firstHalf), maxEventDataSize)
                    .Concat(GetEvents(thingEvent, measurements.Skip(firstHalf), maxEventDataSize));
            }

            return new List<object> { eventPayloadJson };
        }

        public override string ProcessEventData(TheThing thing, object evnt, DateTimeOffset defaultTimestamp)
        {
            string error = null;
            string json = DecodeEventAsString(evnt);
            if (json == null)
            {
                error = "Invalid payload";
            }
            else
            {
                MeasurementPayload payload = null;
                try
                {
                    payload = TheCommonUtils.DeserializeJSONStringToObject<MeasurementPayload>(json);
                    if (payload == null)
                    {
                        error = "No data in payload";
                    }
                    else
                    {
                        // TODO Implement deserializer (not customer requirement at this point)
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
#endif
