// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using nsCDEngine.BaseClasses;
using System;

namespace CDMyOPCUAClient.Contracts
{
    internal class TheOPCUAValue
    {
        public object value { get; set; }
        public uint? statusCode { get; set; }
        public DateTime? serverTimestamp { get; set; }
        public ushort? sourcePicoseconds { get; set; }
        public ushort? serverPicoseconds { get; set; }
        public override string ToString()
        {
            return TheCommonUtils.SerializeObjectToJSONString(this);
        }
    }
}
