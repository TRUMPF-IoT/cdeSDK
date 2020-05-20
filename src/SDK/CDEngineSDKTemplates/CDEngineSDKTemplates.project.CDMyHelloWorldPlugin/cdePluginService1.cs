// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System;
using System.Collections.Generic;

using nsCDEngine.BaseClasses;
using nsCDEngine.Engines;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.ThingService;
using nsCDEngine.ViewModels;

// TODO: Add reference for C-DEngine.dll
// TODO: Make sure plugin DLL file name starts with either CDMy or C-DMy
// TODO: Set new GUID engine id value and engine friendly name (see below).

namespace $safeprojectname$
{
    class cdePluginService1 : ThePluginBase
    {
        // User-interface defintion
        TheDashboardInfo mMyDashboard;

        // TODO: Set GUID value for InitEngineAssets (in the next block)
        Guid guidEngineID = new Guid("{<<Use Create GUID tool>>}"); 
    
        // TODO: Set plugin friendly name for InitEngineAssets (optional)
        String strFriendlyName = "My Hello World Service";              

#region ICDEPlugin - interface methods for service (engine)
        /// <summary>
        /// InitEngineAssets - The C-DEngine calls this initialization
        /// function as part of registering this service (engine)
        /// </summary>
        /// <param name="pBase">The C-DEngine creates a base engine object.
        /// This parameter is a reference to that base engine object.
        /// We keep a copy because it will be very useful to us.
        /// </param>
        public override void InitEngineAssets(IBaseEngine pBase)
        {
            base.InitEngineAssets(pBase);
            MyBaseEngine.SetEngineID(guidEngineID);          // Unique identifier for our service (engine)
            MyBaseEngine.SetFriendlyName(strFriendlyName);

            MyBaseEngine.SetPluginInfo("This service...",       // Describe plugin for Plugin Store
                                       0,                       // pPrice - retail price (default = 0)
                                       null,                    // Custom home page - default = /ServiceID
                                       "toplogo-150.png",       // pIcon - custom icon.
                                       "C-Labs",                // pDeveloper - name of the plugin developer.
                                       "http://www.c-labs.com", // pDeveloperUrl - URL to developer home page.
                                       new List<string>() { }); // pCategories - Search categories for service.
        }
#endregion

        public override bool Init()
        {
            if (!mIsInitCalled)
            {
                mIsInitCalled = true;
                MyBaseThing.RegisterEvent(eEngineEvents.IncomingMessage, HandleMessage);
                // Additional initialization processing goes here

                MyBaseThing.StatusLevel = 1;
                MyBaseThing.LastMessage = "Hello World service has started.";
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
