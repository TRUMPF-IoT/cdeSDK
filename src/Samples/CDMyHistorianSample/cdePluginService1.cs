// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System;
using System.Collections.Generic;

using nsCDEngine.BaseClasses;
using nsCDEngine.Communication;
using nsCDEngine.Engines;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.StorageService;
using nsCDEngine.Engines.ThingService;
using nsCDEngine.ViewModels;

// TODO: Add reference for C-DEngine.dll
// TODO: Make sure plugin DLL file name starts with either CDMy or C-DMy
// TODO: Set new GUID engine id value and engine friendly name (see below).

namespace CDMyHistorianSample
{
    class cdePluginService1 : ThePluginBase
    {
        // User-interface defintion
        TheDashboardInfo mMyDashboard;

        // TODO: Set GUID value for InitEngineAssets (in the next block)
        Guid guidEngineID = new Guid("{D5F337C1-FF09-4EC3-905F-FEBAA5C0952F}");

        // TODO: Set plugin friendly name for InitEngineAssets (optional)
        String strFriendlyName = "My Historian Sample";

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
                MyBaseThing.LastMessage = "Historian Sample service has started.";
                mIsInitialized = true;
                MyBaseEngine.ProcessInitialized();

                StartPropertyUpdateLoop();
                ConsumeHistory();

            }
            return true;
        }

        public override bool CreateUX()
        {
            if (!mIsUXInitCalled)
            {
                mIsUXInitCalled = true;
                mMyDashboard = TheNMIEngine.AddDashboard(MyBaseThing, new TheDashboardInfo(MyBaseEngine, "Historian Sample"));

                TheNMIEngine.AddAboutButton4(MyBaseThing, mMyDashboard, null, true);
                mIsUXInitialized = true;
            }
            return true;
        }

        //TODO: Step 4: Write your Business Logic

        TheThing testThing; // Thing on which we will change properties and consume history
        Guid historyToken; // Used for retrieving the history
        readonly int propertyCount = 3;
        readonly List<string> properties = new List<string> // Names of properties we will set and log
        {
            "TestValue1", "TestValue2", "TestValue3",
            "[TestValue1].[Avg]", "[TestValue1].[Min]", "[TestValue1].[Max]",
            "[TestValue2].[Avg]", "[TestValue2].[Min]", "[TestValue2].[Max]",
            "[TestValue3].[Avg]", "[TestValue3].[Min]", "[TestValue3].[Max]"
        };

        public void StartPropertyUpdateLoop()
        {
            testThing = new TheThing() { FriendlyName = "MyTestSensor" };
            TheThingRegistry.RegisterThing(testThing);
            Random rand = new Random(); // Used for generating "fake" sensor values
            cdeP[] props = new cdeP[propertyCount];

            // Run the loop on another thread
            TheCommonUtils.cdeRunAsync("PropertyUpdateLoop", true, (o) =>
            {
                while(TheBaseAssets.MasterSwitch)
                {
                    // Set the properties and log
                    props[0] = testThing.SetProperty(properties[0], rand.Next(25, 51));
                    props[1] = testThing.SetProperty(properties[1], rand.NextDouble());
                    props[2] = testThing.SetProperty(properties[2], rand.Next(10000, 50000));
                    LogChanges(false, props);

                    // Wait timeout of 1 second between property updates
                    TheCommonUtils.SleepOneEye(1000, 500);
                }
            });
        }

        // Register for history on our Thing
        public void ConsumeHistory()
        {
            if(testThing != null)
            {
                TheHistoryParameters historyParameters = new TheHistoryParameters
                {
                    Properties = properties,
                    SamplingWindow = TimeSpan.FromMilliseconds(5000), // Every 5 seconds
                    ReportUnchangedProperties = true,
                    ReportInitialValues = true,
                    Persistent = true,
                    MaintainHistoryStore = false,
                    ComputeAvg = true,
                    ComputeMax = true,
                    ComputeMin = true
                };
                historyToken = testThing.RegisterForUpdateHistory(historyParameters);
                TheQueuedSenderRegistry.RegisterHealthTimer((l) =>
                {
                    // Every 6 seconds, log the aggregated values. Sampling window for historian is 5 seconds.
                    if (l % 6 == 0)
                    {
                        LogChanges(true);
                    }
                });
            }
        }

        public void LogChanges(bool IsConsumer, cdeP[] changedProperties = null)
        {
            string logMessage = "";
            if(IsConsumer)
            {
                List<TheThingStore> history = testThing.GetThingHistory(historyToken, 1, false);
                logMessage = "Aggregated - ";
                foreach (TheThingStore snapShot in history)
                {
                    foreach (string property in properties)
                    {
                        if (snapShot.PB.TryGetValue(property, out var propValue))
                            logMessage += $"{property}: {propValue} | ";
                    }
                }
            }
            else
            {
                logMessage = $"{testThing.FriendlyName} - ";
                if(changedProperties != null)
                {
                    foreach(cdeP property in changedProperties)
                    {
                        logMessage += $"{property.Name}: {property.Value} | ";
                    }
                }
            }
            // Log the message to SYSLOG
            TheBaseAssets.MySYSLOG.WriteToLog(
                2020,
                TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseEngine.GetEngineName(), logMessage, IsConsumer ? eMsgLevel.l3_ImportantMessage : eMsgLevel.l4_Message));
        }
    }
}
