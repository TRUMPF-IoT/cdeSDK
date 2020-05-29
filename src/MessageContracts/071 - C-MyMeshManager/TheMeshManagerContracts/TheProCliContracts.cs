// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System;
using System.Collections.Generic;

namespace CDMyMeshManager.Contracts
{

    public class TheTestRunTargetInfo
    {
        public string TestAutomationName { get; set; }
        public string CustomerMeshPrefix { get; set; }
        public List<TheTestTargetInfo> Tests { get; set; }
        public Guid TestRunId { get; set; }
    }

    public class TheTestTargetInfo
    {
        public string TestDefinitionName { get; set; }
        public TheNodeTypeTargetInfo NodeType { get; set; }
        public int InstanceCount { get; set; }
        public int MeshCount { get; set; }
        //public string AppConfigTransform { get; set; }
    }

    public class TheNodeTypeTargetInfo
    {
        public string Name { get; set; }
        public string TypeID { get; set; }
        public int Platform { get; set; }
    }


    public class MsgGetTestAgentInfo
    {
        public string NodeId { get; set; }
    }
    public class MsgGetTestAgentInfoResponse
    {
        public string NodeId { get; set; }
        public string TestAgentName { get; set; }
        public List<TheTestRunTargetInfo> TestRuns { get; set; }
        public string Error { get; set; }
    }
    public class MsgRefreshTestAgentInfo
    {
        public string NodeId { get; set; }
    }

    public enum eTestStatus
    {
        Success = 1,
        PartialSuccess = 2,
        Failure = 3,
        Running = 4,
        Scheduled = 5,
    }
    public class MsgReportTestStatus
    {
        public Guid NodeId { get; set; }
        public Guid TestRunId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public eTestStatus Status { get; set; }
        public double? SuccessRate { get; set; }
        public double? PercentCompleted { get; set; }
        public Dictionary<string, object> ResultDetails { get; set; }

        public System.Threading.Tasks.Task<MsgReportTestStatusResponse> Publish()
        {
            return nsCDEngine.Communication.TheCommRequestResponse.PublishRequestJSonAsync<MsgReportTestStatus, MsgReportTestStatusResponse>(new nsCDEngine.Communication.TheMessageAddress { SendToProvisioningService = true }, this);
        }

        public bool HasFinished()
        {
            return HasFinished(Status);
        }
        public static bool HasFinished(eTestStatus status)
        {
            return status != eTestStatus.Running && status != eTestStatus.Scheduled;
        }
    }

    public class MsgReportTestStatusResponse
    {
        public string Error { get; set; }
    }
}
