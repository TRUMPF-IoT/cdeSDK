// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System;
using System.Collections.Generic;
using nsCDEngine.Communication;
using nsCDEngine.BaseClasses;

namespace TheCommonMessageContracts
{
#pragma warning disable CS0649

    public class MsgConnectDisconnect
    {
        public bool? Reconnect { get; set; }
        public bool? Connect { get; set; }
        public bool? AutoConnect { get; set; }
        public int? WaitTimeBeforeReconnect { get; set; }
    }
    public class MsgConnectDisconnectResponse
    {
        public bool Connected { get; set; }
        public string Error { get; set; }
    }

}
