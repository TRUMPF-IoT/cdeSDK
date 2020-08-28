// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿#if INCLUDE_AMQPEVENTCONVERTER
using System;
using System.Collections.Generic;

using nsCDEngine.BaseClasses;
using nsCDEngine.Engines.ThingService;

using nsCDEngine.ViewModels;

using nsTheSenderBase;

using Microsoft.Azure.EventHubs;

namespace nsTheEventConverters
{
    class AMQPPropertyEventConverter : IEventConverter
    {

        public override IEnumerable<object> GetEventData(IEnumerable<TheThingStore> thingUpdates, IThingToConvert senderThing, int maxEventDataSize, bool doNotBatchEvents)
        {
            var eventList = new List<object>();
            foreach (var thingUpdate in thingUpdates)
            {
                EventData eventToSend = new EventData(new byte[0]);

                this.UpdatePropertyValuesToSend(thingUpdate, false, senderThing, false);
                var props = thingUpdate.PB;

                foreach (var p in props)
                {
                    eventToSend.Properties.Add(p.Key, p.Value);
                }
                eventList.Add(eventToSend);
            }
            return eventList;
        }

        public override string ProcessEventData(TheThing thing, object evnt, DateTimeOffset defaultTimestamp)
        {
            if (!(evnt is EventData))
            {
                return "Invalid Payload";
            }
            if (thing == null)
            {
                return "AMQP Converter does not support thing mapping";
            }
            var eventData = evnt as EventData;
            DateTimeOffset time;
            if (eventData.Properties.ContainsKey("time"))
            {
                time = TheCommonUtils.CDate(eventData.Properties["time"]);
            }
            else if (defaultTimestamp == DateTimeOffset.MinValue)
            {
                time = eventData.SystemProperties.EnqueuedTimeUtc;
            }
            else
            {
                time = defaultTimestamp;
            }

            foreach (var prop in eventData.Properties)
            {
                var tProp = thing.SetProperty(prop.Key, prop.Value, time);
                //var tProp = thing.GetProperty(prop.Key, true);
                //tProp.cdeCTIM = time;
                //tProp.Value = prop.Value;
                //tProp.cdeE = 0x40; // NMI visible
            }
            return null;
        }

    }
}

#endif
