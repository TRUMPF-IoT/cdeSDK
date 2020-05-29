// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System;
using System.Collections.Generic;
using TheCommonMessageContracts;

namespace nsTheThingToPublish
{
    public class TheThingToPublish : TheThingToAddWithHistory
    {
        public string EventFormat { get; set; }
        //public bool ChangeNaNToNull { get; set; }
        //public bool EnableUpdateHistory { get; set; }
        //public string PartitionKey { get; set; }
        public bool PreserveOrder { get; set; }
        //public uint SampleRate { get; set; }
        //public uint ChangeBufferLatency { get; set; }
        //public bool RetryLastValueOnly { get; set; }
        public bool IgnorePartialFailure { get; set; }
        public bool AddThingIdentity { get; set; }
        public string TargetType { get; set; }
        public string TargetName { get; set; }
        public string TargetUnit { get; set; }
        public string PartitionKey { get; set; }
        public bool Disable { get; set; }
        public bool DoNotCreate { get; set; }
    }

    public class MsgAddThingsToPublish : MsgAddThings<TheThingToPublish>
    {
        public MsgAddThingsToPublish() : base() { }
        public MsgAddThingsToPublish(TheThingToPublish thingToAdd) : base(thingToAdd) { }
    }

    public class MsgAddThingsToPublishResponse : MsgAddThingsResponse<TheAddThingStatus>
    {
        public MsgAddThingsToPublishResponse() : base() { }
        public MsgAddThingsToPublishResponse(TheAddThingStatus thingStatus) : base(thingStatus) { }
    }

    public class TheTSMToPublish
    {
        public Guid cdeMID { get; set; }
        public string SourceEngineName { get; set; }
        public string TargetEngineName { get; set; }
        public string TXTPattern { get; set; }
        public string AckTXTTemplate { get; set; }
        public bool AckToAll { get; set; }
        public string AckPLSTemplate { get; set; }
        public bool SerializeTSM { get; set; }
        public bool SendAsFile { get; set; }
        public string MQTTTopicTemplate { get; set; }
    }

    public class MsgAddTSMsToPublish
    {
        public MsgAddTSMsToPublish()
        {
        }

        public MsgAddTSMsToPublish(TheTSMToPublish tsmToAdd)
        {
            TSMs = new List<TheTSMToPublish> { tsmToAdd };
        }

        public List<TheTSMToPublish> TSMs { get; set; }
    }

    public class MsgAddTSMsToPublishResponse : MsgAddThingsResponse<TheAddThingStatus>
    {
        public MsgAddTSMsToPublishResponse() : base() { }
        public MsgAddTSMsToPublishResponse(TheAddThingStatus thingStatus) : base(thingStatus) { }
    }


     
}
