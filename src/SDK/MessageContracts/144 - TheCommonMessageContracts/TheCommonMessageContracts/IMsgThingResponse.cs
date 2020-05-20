// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TheCommonMessageContracts
{
    interface IMsgThingResponse
    {
        string Error { get; set; }
    }
}
