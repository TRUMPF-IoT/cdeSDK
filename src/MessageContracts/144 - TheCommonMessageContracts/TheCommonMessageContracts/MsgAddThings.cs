// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System;
using System.Collections.Generic;
using nsCDEngine.Communication;
using nsCDEngine.BaseClasses;
using nsCDEngine.Engines.ThingService;

namespace TheCommonMessageContracts
{
#pragma warning disable CS0649
    public class TheThingToAdd
    {
        /// <summary>
        /// The cdeMID of the entry in the list of things maintained by the plug-in receiving the message
        /// In some case a server can expose the same TheThing multiple times, with different configurations, which is why this cdeMID is usually different from the ThingMID.
        /// </summary>
        public Guid cdeMID { get; set; }
        string _thingMID;
        /// <summary>
        /// cdeMID of the TheThing that is to be added to the server. Use ThingAddress for more complex thing references.
        /// </summary>
        public string ThingMID
        {
            get
            {
                if (_thingMID == null)
                {
                    return ThingAddress != null ? TheCommonUtils.cdeGuidToString(ThingAddress.ThingMID) : "";
                }
                return _thingMID;
            }
            set
            {
                _thingMID = value;
            }
        }
        /// <summary>
        /// The address of the thing to be added to the server. Use ThingMID for simple, local thing references.
        /// </summary>
        public TheMessageAddress ThingAddress { get; set; }
        public string EngineName { get; set; }
        public string FriendlyName { get; set; }
        public string DeviceType { get; set; }
        public Dictionary<string, object> PropertiesToMatch { get; set; }
        /// <summary>
        /// If no ThingMID is specified, all things that match on ThingAddress.EngineName and the Properties in PropertiesToMatch will be added.
        /// If ContinueMatching = false: the match is performed only once and any new things that get created later will not be included
        /// If ContinueMatching = true, any new things will get added as they appear.
        /// </summary>
        public bool ContinueMatching { get; set; }

        /// <summary>
        /// Replace any existing entries of ThingMID in the server's list of things. Used for simple cases where exactly one thing is to be exposed in the server (avoids creating/maintaining a cdeMID for the entry).
        /// </summary>
        public bool ReplaceExistingThing { get; set; }
    }

    public class TheThingToAddWithHistory : TheThingToAdd
    {
        public uint? SamplingWindow { get; set; }
        [Obsolete("Use SamplingWindow unless targetting older plug-ins")]
        public uint ChangeBufferTimeBucketSize { get; set; }
        public uint? CooldownPeriod { get; set; }
        public bool SendUnchangedValue { get; set; }
        public bool? SendInitialValues { get; set; }
        public bool? IgnoreExistingHistory { get; set; }
        public uint? TokenExpirationInHours { get; set; }
        public List<string> PropertiesIncluded { get; set; }
        /// <summary>
        /// Considers all properties, not just sensor properties, even if the thing is a sensor container
        /// </summary>
        public bool? ForceAllProperties { get; set; }
        public bool? ForceConfigProperties { get; set; }
        public List<string> PropertiesExcluded { get; set; }
        public Dictionary<string, object> StaticProperties { get; set; }
        public bool KeepDurableHistory { get; set; }
        public uint MaxHistoryTime { get; set; }
        public int MaxHistoryCount { get; set; }
    }


    public class MsgAddThings<T> where T : TheThingToAdd
    {
        public MsgAddThings()
        {
            Things = new List<T>();
        }

        public MsgAddThings(T thingToAdd)
        {
            Things = new List<T> { thingToAdd };
        }

        public List<T> Things { get; set; }
    }

    public class TheAddThingStatus
    {
        public Guid cdeMid;
        public string Error;
    }

    public class MsgAddThingsResponse<T> where T : TheAddThingStatus
    {
        public MsgAddThingsResponse()
        {
            ThingStatus = new List<T>();
        }
        public MsgAddThingsResponse(T thingStatus)
        {
            ThingStatus = new List<T> { thingStatus };
        }
        public string Error;
        public List<T> ThingStatus;

        public const string strErrorMoreThanOneMatchingThingFound = "More than one matching Thing found";
    }
    //public class TheThingReference
    //{
    //    public Guid? ThingMID { get; set; }
    //public string EngineName { get; set; }
    //public string DeviceType { get; set; }
    //public string FriendlyName { get; set; }
    //public string ID { get; set; }
    //public string Address { get; set; }
    //public Dictionary<string, object> PropertiesToMatch { get; set; }

    //public TheThingReference() { }
    //public TheThingReference(TheThing tThing)
    //{
    //    ThingMID = tThing.cdeMID;
    //}
    //public static implicit operator TheThingReference(TheThing targetThing)
    //{
    //    return new TheThingReference(targetThing);
    //}
}
