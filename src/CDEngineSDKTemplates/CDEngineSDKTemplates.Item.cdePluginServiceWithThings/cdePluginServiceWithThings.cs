// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System;
using System.Collections.Generic;

// TODO: Add reference for C-DEngine.dll
// TODO: Make sure plugin file name starts with either CDMy or C-DMy
using nsCDEngine.BaseClasses;
using nsCDEngine.Engines;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.ThingService;
using nsCDEngine.ViewModels;

namespace $rootnamespace$
{
    public class e$safeitemrootname$Types : TheDeviceTypeEnum
    {
        public const string ThingTypeA = "My Thing Type A";
    }

    class $safeitemrootname$: ThePluginBase 
	{
        Guid guidEngineID = new Guid("{<<Use Create GUID tool>>}"); // TODO: Set GUID value for InitEngineAssets (in the next block)
        String strFriendlyName = "My Sample Service";               // TODO: Set plugin friendly name for InitEngineAssets (optional)

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

            MyBaseEngine.SetEngineID(guidEngineID);
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

        public override bool CreateUX()
        {
            if (!mIsUXInitCalled)
            {
                mIsUXInitCalled = true;

                mMyDashboard = TheNMIEngine.AddDashboard(MyBaseThing, new TheDashboardInfo(MyBaseEngine, "My Demo Plugin Screen with Things"));

                var tFlds=TheNMIEngine.CreateEngineForms(MyBaseThing, TheThing.GetSafeThingGuid(MyBaseThing, "MYNAME"), "List of $safeitemrootname$", null, 20, 0x0F, 0xF0, TheNMIEngine.GetNodeForCategory(), "REFFRESHME", true, new e$safeitemrootname$Types(), e$safeitemrootname$Types.ThingTypeA);
                TheFormInfo tForm = tFlds["Form"] as TheFormInfo;
                tForm.AddButtonText = "Add new $safeitemrootname$";

                TheNMIEngine.RegisterEngine(MyBaseEngine);      //Registers this engine and its resources with the C-DEngine
                mIsUXInitialized = true;
            }
            return true;
        }

        TheDashboardInfo mMyDashboard;
        void InitServices()
        {
            List<TheThing> tDevList = TheThingRegistry.GetThingsOfEngine(MyBaseThing.EngineName); 
            if (tDevList.Count > 0)
            {
                foreach (TheThing tDev in tDevList)
                {
                    switch (tDev.DeviceType)
                    {
                        case e$safeitemrootname$Types.ThingTypeA:
                            //                    CreateOrUpdateService<TheThingClass>(tDev, true);
                    break;
                    }
                }
            }
            MyBaseEngine.SetStatusLevel(-1); //Calculates the current statuslevel of the service/engine
        }

//        T CreateOrUpdateService<T>(TheThing tDevice, bool bRegisterThing) where T : TheThingClass
//{
//            T tServer;
//            if (tDevice == null || !tDevice.HasLiveObject)
//            {
//                tServer = (T)Activator.CreateInstance(typeof(T), tDevice, this);
//                if (bRegisterThing)
//                    TheThingRegistry.RegisterThing((ICDEThing)tServer);
//            }
//            else
//            {
//                tServer = tDevice.GetObject() as T;
//                if (tServer != null)
//                    tServer.Connect(null);    //Expose a Connect(TheProcessMessage) method on TheThing
//                else
//                    tServer = (T)Activator.CreateInstance(typeof(T), tDevice, this);
//            }
//            return tServer;
//        }

void OnThingDeleted(ICDEThing pEngine, object pDeletedThing)
        {
            if (pDeletedThing != null && pDeletedThing is ICDEThing)
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
