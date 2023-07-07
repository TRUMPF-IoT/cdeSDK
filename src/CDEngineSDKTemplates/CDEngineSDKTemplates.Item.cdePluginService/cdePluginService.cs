// SPDX-FileCopyrightText: 2009-2023 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0
using nsCDEngine.BaseClasses;
using nsCDEngine.Engines;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.ThingService;
using nsCDEngine.ViewModels;
using System;

using NMI = nsCDEngine.Engines.NMIService.TheNMIEngine;
using CU = nsCDEngine.BaseClasses.TheCommonUtils;
using TCC = nsCDEngine.Communication.TheCommCore;
using TT = nsCDEngine.Engines.ThingService.TheThing;

namespace $rootnamespace$
{
    [EngineAssetInfo(
        FriendlyName = "My Sample Service", //TODO give friendly name
        Capabilities = new[] { eThingCaps.ConfigManagement, },
        EngineID = "{$guid2$}",
        IsService = true,
        LongDescription = "This service...", //TODO describe your service
        IconUrl = "toplogo-150.png", // TODO Add your own icon
        Developer = "C-Labs", // TODO Add your own name and URL
        DeveloperUrl = "http://www.c-labs.com",
        ManifestFiles = new string[] { }
    )]
class $safeitemrootname$: ThePluginBase
    {
        TheDashboardInfo mMyDashboard;

public override bool Init()
{
    if (!mIsInitCalled)
    {
        mIsInitCalled = true;
        MyBaseThing.RegisterEvent(eEngineEvents.IncomingMessage, HandleMessage);
        SetMessage("Service has Started", 1, DateTimeOffset.Now);
        base.Init();
        mIsInitialized = true;
    }
    return true;
}

public override bool CreateUX()
{
    if (!mIsUXInitCalled)
    {
        mIsUXInitCalled = true;
        mMyDashboard = NMI.AddDashboard(MyBaseThing, new TheDashboardInfo(MyBaseEngine, "My Sample Plugin Screens"));

        var tFlds = TheNMIEngine.AddStandardForm(MyBaseThing, "Welcome to my Sample Page");
        var tMyForm = tFlds["Form"] as TheFormInfo;

        NMI.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleEnded, 2, 2, 0, "My Sample Value Is", "SampleProperty", new nmiCtrlSingleEnded() { ParentFld = 1 });
        NMI.AddSmartControl(MyBaseThing, tMyForm, eFieldType.BarChart, 3, 2, 0, "My Sample Value Bar", "SampleProperty", new nmiCtrlBarChart() { ParentFld = 1, MaxValue = 255, TileHeight = 2, IsVertical = true, Foreground = "blue" });

        NMI.AddAboutButton4(MyBaseThing, mMyDashboard, null, true);
        mIsUXInitialized = true;
    }
    return true;
}
}
}
