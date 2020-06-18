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

namespace $safeprojectname$
{
    public class e$safeprojectname$DeviceTypes : TheDeviceTypeEnum
    {
        public const string cdeThingDeviceTypeA = "My Device Type A";
    }

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
        // TODO: Set plugin friendly name for InitEngineAssets (optional)
        public const String strFriendlyName = "My Hello World Service";              

        public override bool Init()
        {
            if (!mIsInitCalled)
            {
                mIsInitCalled = true;
                MyBaseThing.StatusLevel = 4;
                MyBaseThing.LastMessage = "Service has started";

                MyBaseThing.RegisterEvent(eEngineEvents.IncomingMessage, HandleMessage);
                MyBaseEngine.RegisterEvent(eEngineEvents.ThingDeleted, OnThingDeleted);

                // If not lengthy initialized you can remove cdeRunasync and call this synchronously
                TheCommonUtils.cdeRunAsync(MyBaseEngine.GetEngineName() + " Init Services", true, (o) =>
                {
                    // Perform any long-running initialization (i.e. network access, file access) here that must finish before other plug-ins or the C-DEngine can use the plug-in
                    InitServices();

                    // Declare the thing initialized 
                    mIsInitialized = true; // For future IsInit() calls
                    FireEvent(eThingEvents.Initialized, this, true, true); // Notify the C-DEngine and other plug-ins that the thing is initialized
                    MyBaseEngine.ProcessInitialized(); //Set the status of the Base Engine according to the status of the Things it manages
                });
            }
            return false;
        }

        // User-interface defintion
        TheDashboardInfo mMyDashboard;

        public override bool CreateUX()
        {
            if (!mIsUXInitCalled)
            {
                mIsUXInitCalled = true;

                mMyDashboard = TheNMIEngine.AddDashboard(MyBaseThing, new TheDashboardInfo(MyBaseEngine, "My Demo Plugin Screen with Things"));

                var tFlds=TheNMIEngine.CreateEngineForms(MyBaseThing, TheThing.GetSafeThingGuid(MyBaseThing, "MYNAME"), "List of $safeitemrootname$", null, 20, 0x0F, 0xF0, TheNMIEngine.GetNodeForCategory(), "REFFRESHME", true, new e$safeprojectname$DeviceTypes(), e$safeprojectname$DeviceTypes.cdeThingDeviceTypeA);
                TheFormInfo tForm = tFlds["Form"] as TheFormInfo;
                tForm.AddButtonText = "Add new $safeitemrootname$";

                TheNMIEngine.RegisterEngine(MyBaseEngine); //Registers this engine and its resources with the C-DEngine
                mIsUXInitialized = true;
            }
            return true;
        }

        void InitServices()
        {
            List<TheThing> tDevList = TheThingRegistry.GetThingsOfEngine(MyBaseThing.EngineName); 
            foreach (TheThing tDev in tDevList)
            {
			    if (!tDev.HasLiveObject)
				{
                    switch (tDev.DeviceType)
                    {
                        case e$safeprojectname$DeviceTypes.cdeThingDeviceTypeA:
                            TheThingRegistry.RegisterThing(new cdeThingDeviceTypeA(tDev, this));
                            break;
                    }
                }
            }
            MyBaseEngine.SetStatusLevel(-1); //Calculates the current statuslevel of the service/engine
        }

        void OnThingDeleted(ICDEThing pEngine, object pDeletedThing) // CODE REVIEW: Is this really still needed?
        {
            if (pDeletedThing is ICDEThing)
            {
                //TODO: Stop Resources, Thread etc associated with this Thing
                ((ICDEThing)pDeletedThing).FireEvent(eEngineEvents.ShutdownEvent, pEngine, null, false);
            }
        }

//TODO: Step 4: Write your Business Logic

#region Message Handling
public override void HandleMessage(ICDEThing sender, object pIncoming)
        {
            TheProcessMessage pMsg = pIncoming as TheProcessMessage;
            if (pMsg == null) return;

            string[] cmd = pMsg.Message.TXT.Split(':');
            switch (cmd[0])
            {
                case "CDE_INITIALIZED":
                    MyBaseEngine.SetInitialized(pMsg.Message);      //Sets the Service to "Ready". ProcessInitialized() internally contains a call to this Handler and allows for checks right before SetInitialized() is called. 
            break;
                case "REFFRESHME":
                    InitServices();
                    mMyDashboard.Reload(pMsg, false);
                    break;
            }
        }
#endregion
	}
}
