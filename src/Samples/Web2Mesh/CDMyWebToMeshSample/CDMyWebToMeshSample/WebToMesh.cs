// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using nsCDEngine.BaseClasses;
using nsCDEngine.Communication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CDMyWebToMeshSample
{
    public class MsgMileRecordHolder
    {
        public int id;
    }

    public class MsgMileRecordHolderResponse
    {
        public int id;
        public TheRecordHolder data;
    }

    public class WebToMesh
    {
        public static TheRecordHolder MeshQueryRecordHolder(int idRecord, Guid node, string strEngineName, Guid cdeMIdThing)
        {
            TheRecordHolder trh = null;

            // Package up request info.
            MsgMileRecordHolder msgRequest = new MsgMileRecordHolder()
            {
                id = idRecord
            };

            // Start asynchronous task to send a message and wait for a reply.
            // Sends a message named nameof(MsgMileRecordHolder)
            // Receives a reply named nameof(MsgMileRecordHolderResponse)
            // See function "HandleMessage" for actual handling.

            Task<MsgMileRecordHolderResponse> t = null;
            try
            {
                TheMessageAddress tma = new TheMessageAddress()
                {
                    Node = Guid.Empty,
                    EngineName = strEngineName,
                    ThingMID = cdeMIdThing,
                    SendToProvisioningService = false,
                };
                t = TheCommRequestResponse.PublishRequestJSonAsync<MsgMileRecordHolder, MsgMileRecordHolderResponse>(tma, msgRequest);
            }
            catch (Exception ex)
            {
                string strMessage = ex.Message;
            }

            // Wait for a bit
            t.Wait(20000);
            bool bTaskCompleted = t.IsCompleted;

            // Check for success.
            if (bTaskCompleted)
            {
                MsgMileRecordHolderResponse msgResponse = t.Result;
                trh = msgResponse.data;
            }

            return trh;
        }
    }
}
