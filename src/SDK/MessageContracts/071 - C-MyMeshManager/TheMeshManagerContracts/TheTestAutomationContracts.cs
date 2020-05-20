// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System;
using System.Collections.Generic;

using nsCDEngine.Engines.ThingService;

namespace CDMyMeshManager.Contracts
{
    public class MsgStartNewTestRun
    {
        public string TestRunName { get; set; }
        public string TestName { get; set; }
        public string Environment { get; set; }
    }
    public class MsgStartNewTestRunResponse : TheThing.IMsgResponse
    {
        public string Error { get; set; }
        public Guid? TestRunId { get; set; }
    }

    public class MsgGetTestRunStatus
    {
        public string TestRunName { get; set; }
        public string TestName { get; set; }
        public string Environment { get; set; }
    }

    public class TheTestRunStatus
    {
        public Guid? Id { get; set; }
        public String FriendlyName { get; set; }
        public String Environment { get; set; }
        public DateTimeOffset? CreationTime { get; set; } // Time the test was created/activated
        public DateTimeOffset? StartTime { get; set; } // Time the test started (first test progress result reported)
        public DateTimeOffset? EndTime { get; set; } // Time the test results reached 100% progress rate
        public DateTimeOffset? LastUpdate { get; set; } // Time the last test result was reported
        public bool IsActive { get; set; }
        public string Status { get; set; }
        // For now: use CDMyMeshManager.Contracts.TestStatus
        // TODO define outcomes, i.e. Scheduled, Running, Success, Failure
        //   => Reuse some of the Performance Tuner code/concepts?
        //   => How can we integrate with test systems like DevOps?
        public double? SuccessRate { get; set; }
        public double? PercentComplete { get; set; }
        public Dictionary<string, object> ResultDetails { get; set; }

        public TheTestRunStatus() { }
    }

    public class MsgGetTestRunStatusResponse : TheThing.IMsgResponse
    {
        public string Error { get; set; }
        public List<TheTestRunStatus> TestStatus { get; set; }
    }

}