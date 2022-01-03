// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System;
using System.Collections.Generic;

using nsCDEngine.Engines;
using nsCDEngine.BaseClasses;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.ThingService;
using nsCDEngine.ViewModels;

namespace $safeprojectname$
{
    [EngineAssetInfo(
        FriendlyName = strFriendlyName,
        Capabilities = new[] { eThingCaps.ConfigManagement, },
        EngineID = "{$guid2$}",
        IsService = true,
        LongDescription = "This service...",
        IconUrl = "toplogo-150.png", // TODO Add your own icon
        Developer = "C-Labs", // TODO Add your own name and URL
        DeveloperUrl = "http://www.c-labs.com",
        ManifestFiles = new string[] { }
    )]
    class cdePluginService1 : ThePluginBase
    {
        // User-interface defintion
        TheDashboardInfo mMyDashboard;

        // TODO: Set plugin friendly name for InitEngineAssets (optional)
        public const String strFriendlyName = "My Hello World Service";              

        public override bool Init()
        {
            if (!mIsInitCalled)
            {
                mIsInitCalled = true;
                MyBaseThing.RegisterEvent(eEngineEvents.IncomingMessage, HandleMessage);
                // Additional initialization processing goes here
                // If additional processing could fail or last longer, set status level to 4 and/or consider finishing Init() asynchronously (return false, fire eThingEvent.Initialized when ready)
                // MyBaseThing.StatusLevel = 4;
                // MyBaseThing.LastMessage="Service is starting";
                MyBaseThing.StatusLevel = 1;
                SetMessage("Hello World service has started.", DateTimeOffset.Now);
                mIsInitialized = true;
                MyBaseEngine.ProcessInitialized();
            }
            return true;
        }

        public override bool CreateUX()
        {
            if (!mIsUXInitCalled)
            {
                mIsUXInitCalled = true;
                mMyDashboard = TheNMIEngine.AddDashboard(MyBaseThing, new TheDashboardInfo(MyBaseEngine, "My Sample Hello World Plugin"));

                var tFlds = TheNMIEngine.AddStandardForm(MyBaseThing, "Welcome to my Hello World Page");
                var tMyForm = tFlds["Form"] as TheFormInfo;

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleEnded, 2, 2, 0, "My Hello World Value", "SampleProperty", new nmiCtrlSingleEnded() { ParentFld = 1 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.BarChart, 3, 2, 0, "My Hello World Bar Chart", "SampleProperty", new nmiCtrlBarChart() { ParentFld = 1, MaxValue = 255, TileHeight = 2, IsVertical = true, Foreground = "blue" });

                TheNMIEngine.AddAboutButton4(MyBaseThing, mMyDashboard, null, true);
                mIsUXInitialized = true;
            }
            return true;
        }

        //TODO: Step 4: Write your Business Logic
    }
}
