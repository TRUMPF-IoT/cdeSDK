// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using nsCDEngine.BaseClasses;
using nsCDEngine.Communication;
using nsCDEngine.Engines.ThingService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TheCommonMessageContracts;

namespace CDMyOPCUAClient.Contracts
{
    internal class TheOPCUAClientEngines
    {
        public const string EngineName = "CDMyOPCUAClient.cdeOPCUaClient";
    }
    internal static class eOPCUAClientDeviceTypes
    {
        public const string RemoteServer = "OPC-UA Remote Server";
    }

    /// <summary>
    /// Definition of the events fired by the OPCUA Client
    /// </summary>
    internal static class eOPCUAClientEvents
    {
        /// <summary>
        /// Fired when the connection is established
        /// </summary>
        public const string ConnectComplete = "ConnectComplete";
        /// <summary>
        /// Fired when the connection is established
        /// </summary>
        public const string ConnectFailed = "ConnectFailed";
        /// <summary>
        /// Fired when a connection was closed either due to a request, a timeout/keepalive or by the server
        /// Argument: DisconnectStatus
        /// </summary>
        public const string DisconnectComplete = "DisconnectComplete";
    }



    internal enum ChangeTrigger
    {
        Status = 0,
        Value = 1,
        Timestamp = 2,
    }

    internal class TheEventProperty
    {
        public string Name;
        public string CustomTypeId;
        public string Alias;
        public TheEventProperty Clone()
        {
            return new TheEventProperty
            {
                Name = this.Name,
                CustomTypeId = this.CustomTypeId,
                Alias = this.Alias,
            };
        }
        public override string ToString()
        {
            return $"{Name} {CustomTypeId} {Alias}";
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (!(obj is TheEventProperty other))
            {
                return base.Equals(obj);
            }
            return Name == other.Name
                && CustomTypeId == other.CustomTypeId
                && Alias == other.Alias;
        }
        public override int GetHashCode()
        {
            var hash = 13;
            AddHash(ref hash, Name);
            AddHash(ref hash, CustomTypeId);
            AddHash(ref hash, Alias);
            return hash;
        }
        protected static void AddHash(ref int hash, object obj)
        {
            if (obj != null)
            {
                hash = hash * 7 + obj.GetHashCode();
            }
        }

    }

// Not fully implemented/functional. Syntax is cumbersome. Leaving this in for now in case the feature request becomes high priority
////internal class TheAttributeOperand
////{
////    public string NodeId;
////    public string[] BrowsePath; // TODO RelativePath representation
////    public int AttributeId;
////    public int IndexRangeMix;
////    public int IndexRangeMax;
////}

//internal class TheSimpleAttributeOperand
//{
//    public string TypeId;
//    public string[] BrowsePaths;
//    public uint? AttributeId;
//    public int IndexRangeMix;
//    public int IndexRangeMax;
//}

//internal class TheFilterOperand
//{
//    public string Value;
//    //public TheAttributeOperand Attribute;
//    public TheSimpleAttributeOperand SimpleAttribute;
//    public TheEventWhereClause SubClause;
//}

//internal class TheEventWhereClause
//{
//    public string Operator;
//    public TheFilterOperand[] Operands;

//}

internal class TheEventSubscription
    {
        /// <summary>
        /// Treats the event as a Condition and maintains a list of conditions that are marked with Retain = true
        /// </summary>
        public bool AggregateRetainedConditions { get; set; }
        /// <summary>
        /// If specified, limits the event properties being returned. Otherwise all available properties are returned.
        /// Custom type ids can be specified by appending "::<nodeid>" to the name. 
        /// Names are automatically qualified with the namespace of the type. This can be overridden via a "<namespace>:" prefix, where the namespace can be either a URI or an index.
        /// </summary>
        public List<TheEventProperty> Properties { get; set; }
        //public List<TheEventWhereClause> WhereClauses { get; set; }
        internal TheEventSubscription Clone()
        {
            return new TheEventSubscription { AggregateRetainedConditions = AggregateRetainedConditions, Properties = new List<TheEventProperty>(Properties)
                //, WhereClauses = new List<TheEventWhereClause>(WhereClauses)
            };
        }

        internal List<string> GetSourceNames()
        {
            return Properties.Select(prop => prop.Name).ToList();
        }

        internal List<string> GetPropertyNames()
        {
            return Properties.Select(prop => string.IsNullOrEmpty(prop.Alias) ? prop.Name : prop.Alias).ToList();
        }

        public bool IsEqual(TheEventSubscription other)
        {
            if (other == null)
            {
                return false;
            }
            return AggregateRetainedConditions == other.AggregateRetainedConditions
                && Properties.SequenceEqual(other.Properties);
        }
    }
    
#pragma warning disable CS0649
    internal class MsgOPCUACreateTags
    {
        public class MethodInfo
        {
            public string ObjectId;
            public string NodeId;
            public int CallTimeout;
        }
        public class TagInfo
        {
            public string DisplayName;
            public string NodeId;
            public string BrowsePath;

            /// <summary>
            /// >0 : SamplingInterval ins milli seconds
            /// 0 : Let Server choose fastest practical rate
            /// -1 : Use subscription publishing interval
            /// <= -2 : Do not subscribe (use for polling)
            /// </summary>
            public int SamplingInterval;
            public TheMessageAddress HostThingAddress;
            public TheThingReference HostThing;
            public ChangeTrigger ChangeTrigger;
            public double DeadbandValue;

            public DateTimeOffset? HistoryStartTime { get; set; }

            public override string ToString()
            {
                return $"[{NodeId} {DisplayName} {SamplingInterval} {ChangeTrigger} {DeadbandValue} {HistoryStartTime}]".ToString(CultureInfo.InvariantCulture);
            }
        }
        public class EventInfo
        {
            /// <summary>
            /// Name of the property into which to write the event data as a JSON object (or a JSON array if aggregation is requested)
            /// </summary>
            public string DisplayName;
            /// <summary>
            /// Id of a node with an EventNotifier from which events are to get gathered
            /// </summary>
            public string NodeId;
            /// <summary>
            /// Thing into which the property is to be written
            /// </summary>
            public TheThingReference HostThing;

            public TheEventSubscription Subscription;

            public override string ToString()
            {
                return $"[{NodeId} {DisplayName}]".ToString(CultureInfo.InvariantCulture);
            }
        }

        public List<TagInfo> Tags;
        public List<MethodInfo> Methods;
        public List<EventInfo> Events;
        public bool ReplaceAllTags { get; set; }
        public bool BulkApply { get; set; }
    }


    internal class MsgOPCUACreateTagsResponse
    {
        public class TagResult
        {
            public TheMessageAddress MethodThingAddress;
            public string Error;
        }
        public List<TagResult> Results;
        public string Error;
    }

    internal class MsgOPCUABrowse
    {
    }
    internal class MsgOPCUABrowseResponse
    {
        public List<MsgOPCUACreateTags.TagInfo> Tags;
        public string Error;
    }

    internal class MsgOPCUAReadTags
    {
        public class TagInfo
        {
            public string DisplayName;
            public string NodeId;
        }
        public List<TagInfo> Tags;
    }

    internal class MsgOPCUAReadTagsResponse
    {
        public class TagResult
        {
            public object TagValue;
            public string Error;
            public string StructTypeInfo;
        }
        public List<TagResult> Results;
        public string Error;
    }


    internal class MsgOPCUAConnect
    {
        public bool LogEssentialOnly { get; set; }
        public bool WaitUntilConnected { get; set; }
    }

    internal class MsgOPCUAConnectResponse
    {
        public string Error;
    }

    internal class MsgOPCUADisconnect
    {
    }

    internal class MsgOPCUADisconnectResponse
    {
        public string Error;
    }


    internal class MsgOPCUAMethodCall
    {
        public Dictionary<string, object> Arguments;
        public bool ReturnRawJSON;
    }

    internal class MsgOPCUAMethodCallResponse
    {
        public List<object> OutputArguments;
        public string Error;
    }

    internal class TheMethodInfo
    {
        public string ObjectId;
        public int CallTimeout;
    }

    internal class TheOPCSensorSubscription : TheThing.TheSensorSubscription
    {
        public int? QueueSize { get; set; }
        public int? ChangeTrigger { get; set; }
        public double? DeadbandValue { get; set; }

        public TheEventSubscription EventInfo { get; set; }
        public TheMethodInfo MethodInfo { get; set; }
        public string StructTypeInfo { get; set; }
        public DateTimeOffset? HistoryStartTime { get; set; }

        public TheOPCSensorSubscription() { }
        public TheOPCSensorSubscription(TheThing.TheSensorSubscription baseSubscription) : base(baseSubscription)
        {
            if (baseSubscription.ExtensionData != null)
            {
                if (baseSubscription.ExtensionData.TryGetValue(nameof(QueueSize), out var queueSize))
                {
                    QueueSize = TheCommonUtils.CIntNullable(queueSize);
                    ExtensionData.Remove(nameof(QueueSize));
                }
                if (baseSubscription.ExtensionData.TryGetValue(nameof(ChangeTrigger), out var changeTrigger))
                {
                    ChangeTrigger = TheCommonUtils.CIntNullable(changeTrigger);
                    ExtensionData.Remove(nameof(ChangeTrigger));
                }
                if (baseSubscription.ExtensionData.TryGetValue(nameof(DeadbandValue), out var deadbandValue))
                {
                    DeadbandValue = TheCommonUtils.CIntNullable(deadbandValue);
                    ExtensionData.Remove(nameof(DeadbandValue));
                }
                if (baseSubscription.ExtensionData.TryGetValue(nameof(HistoryStartTime), out var historyStartTime))
                {
                    HistoryStartTime = TheCommonUtils.CDateNullable(historyStartTime);
                    ExtensionData.Remove(nameof(HistoryStartTime));
                }
                if (baseSubscription.ExtensionData.TryGetValue(nameof(EventInfo), out var eventInfo))
                {
                    if (eventInfo is TheEventSubscription)
                    {
                        EventInfo = eventInfo as TheEventSubscription;
                    }
                    else
                    {
                        EventInfo = TheCommonUtils.DeserializeJSONStringToObject<TheEventSubscription>(eventInfo?.ToString());
                    }
                    ExtensionData.Remove(nameof(EventInfo));
                }
            }
        }

        override public TheThing.TheSensorSubscription GetBaseSubscription()
        {
            // Places the additional properties into TheThing.TheSensorSubscription.ExtensionData
            var sensorSubscription = new TheThing.TheSensorSubscription(this, false);
            if (sensorSubscription.ExtensionData == null)
            {
                sensorSubscription.ExtensionData = new Dictionary<string, object>();
            }
            if (QueueSize.HasValue) sensorSubscription.ExtensionData[nameof(QueueSize)] = QueueSize.Value;
            if (ChangeTrigger.HasValue) sensorSubscription.ExtensionData[nameof(ChangeTrigger)] = ChangeTrigger.Value;
            if (DeadbandValue.HasValue) sensorSubscription.ExtensionData[nameof(DeadbandValue)] = DeadbandValue.Value;
            if (HistoryStartTime.HasValue) sensorSubscription.ExtensionData[nameof(HistoryStartTime)] = HistoryStartTime.Value;
            if (EventInfo != null) sensorSubscription.ExtensionData[nameof(EventInfo)] = EventInfo;
            return sensorSubscription;
        }
    }

    //    internal class OPCCustomSubscriptionInfo
    //{
    //    public int? QueueSize { get { return GetInt(nameof(QueueSize)); } set { SetValue(nameof(QueueSize), value); } }
    //    public int? ChangeTrigger { get { return GetInt(nameof(ChangeTrigger)); } set { SetValue(nameof(ChangeTrigger), value); } }
    //    public double? DeadbandValue { get { return GetDouble(nameof(DeadbandValue)); } set { SetValue(nameof(DeadbandValue), value); } }

    //    TheEventSubscription _eventInfo;
    //    public TheEventSubscription EventInfo
    //    {
    //        get
    //        {
    //            if (_eventInfo == null)
    //            {
    //                _eventInfo = new TheEventSubscription();
    //            }
    //            _eventInfo.AggregateRetainedConditions = GetBool(nameof(TheEventSubscription.AggregateRetainedConditions));
    //            _eventInfo.Properties = GetList(nameof(TheEventSubscription.Properties));
    //        }
    //        set
    //        {
    //        }
    //    }

    //    private void SetValue(string propertyName, object value)
    //    {
    //        if (_subInfo.CustomSubscriptionInfo == null)
    //        {
    //            _subInfo.CustomSubscriptionInfo = new Dictionary<string, object>();
    //        }
    //        _subInfo.CustomSubscriptionInfo[propertyName] = value;
    //    }

    //    private bool? GetBool(string propertyName)
    //    {
    //        object valueObj = null; ;
    //        if (_subInfo?.CustomSubscriptionInfo?.TryGetValue(propertyName, out valueObj) == true)
    //        {
    //            return TheCommonUtils.CBool(valueObj);
    //        }
    //        return null;
    //    }
    //    private int? GetInt(string propertyName)
    //    {
    //        object valueObj = null; ;
    //        if (_subInfo?.CustomSubscriptionInfo?.TryGetValue(propertyName, out valueObj) == true)
    //        {
    //            return TheCommonUtils.CInt(valueObj);
    //        }
    //        return null;
    //    }
    //    private double? GetDouble(string propertyName)
    //    {
    //        object valueObj = null; ;
    //        if (_subInfo?.CustomSubscriptionInfo?.TryGetValue(propertyName, out valueObj) == true)
    //        {
    //            return TheCommonUtils.CDbl(valueObj);
    //        }
    //        return null;
    //    }

    //    TheThing.TheSensorSubscription _subInfo;

    //    public OPCCustomSubscriptionInfo()
    //    {
    //        _subInfo = new TheThing.TheSensorSubscription();
    //    }

    //    public OPCCustomSubscriptionInfo(TheThing.TheSensorSubscription subInfo)
    //    {
    //        _subInfo = subInfo;
    //    }

    //    public Dictionary<string, object> GetAsDictionary()
    //    {
    //        return _subInfo?.CustomSubscriptionInfo;
    //    }

    //    public static implicit operator Dictionary<string,object>(OPCCustomSubscriptionInfo si)
    //    {
    //        return si.GetAsDictionary();
    //    }
    //}



#pragma warning restore CS0649
}