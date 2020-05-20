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

namespace CDMyWebToMeshSample
{
    partial class cdePluginService1 : ICDEPlugin, ICDEThing
    {
        // Base object references 
        protected TheThing MyBaseThing;      // Base thing
        private IBaseEngine MyBaseEngine;    // Base engine (service)

        // Operational flags.
        public static bool g_EnableRESTServer = false;       // Listen for REST requests?
        public static bool g_EnableMeshDataQuery = false;    // Send TSM messages out to query for data?
        public static bool g_EnableMeshDataResponse = false; // List for TSM messages from other nodes?

        // Initialization flags
        protected bool mIsInitStarted = false;
        protected bool mIsInitCompleted = false;
        protected bool mIsUXInitStarted = false;
        protected bool mIsUXInitCompleted = false;

        // User-interface defintion
        TheDashboardInfo mMyDashboard;

        // The following GUID was created for CDMyRestApiSampleMileRecordHolders
        // Guid guidEngineID = new Guid("{053D85F4-6C1F-48E7-A43E-D9062DC306B5}");

        // Here is the new GUID for CDMyWebToMeshSample
        Guid guidEngineID = new Guid("{CCFD70B8-5A1F-4F32-BFE7-929D3BBBECB2}");

        String strFriendlyName = "My Web-to-Mesh Service";

        #region ICDEPlugin - interface methods for service (engine)
        public IBaseEngine GetBaseEngine()
        {
            return MyBaseEngine;
        }

        /// <summary>
        /// InitEngineAssets - The C-DEngine calls this initialization
        /// function as part of registering this service (engine)
        /// </summary>
        /// <param name="pBase">The C-DEngine creates a base engine object.
        /// This parameter is a reference to that base engine object.
        /// We keep a copy because it will be very useful to us.
        /// </param>
        public void InitEngineAssets(IBaseEngine pBase)
        {
            MyBaseEngine = pBase;

            MyBaseEngine.SetEngineID(guidEngineID);          // Unique identifier for our service (engine)
            MyBaseEngine.SetFriendlyName(strFriendlyName);

            MyBaseEngine.SetEngineName(GetType().FullName);  // Can be any arbitrary name - recommended is the class name
            MyBaseEngine.SetEngineType(GetType());           // Has to be the type of this class
            MyBaseEngine.SetEngineService(true);             // Keep True if this class is a service

            MyBaseEngine.SetPluginInfo("This service...",       // Describe plugin for Plugin Store
                                       0,                       // pPrice - retail price (default = 0)
                                       null,                    // Custom home page - default = /ServiceID
                                       "toplogo-150.png",       // pIcon - custom icon.
                                       "C-Labs",                // pDeveloper - name of the plugin developer.
                                       "http://www.c-labs.com", // pDeveloperUrl - URL to developer home page.
                                       new List<string>() { }); // pCategories - Search categories for service.
        }
        #endregion

        #region ICDEThing - interface methods (rare to override)
        public bool IsInit()
        {
            return mIsInitCompleted;
        }
        public bool IsUXInit()
        {
            return mIsUXInitCompleted;
        }

        public TheThing GetBaseThing()
        {
            return MyBaseThing;
        }
        public void SetBaseThing(TheThing pThing)
        {
            MyBaseThing = pThing;
        }

        public cdeP GetProperty(string pName, bool DoCreate)
        {
            return MyBaseThing?.GetProperty(pName, DoCreate);
        }
        public cdeP SetProperty(string pName, object pValue)
        {
            return MyBaseThing?.SetProperty(pName, pValue);
        }

        public void RegisterEvent(string pName, Action<ICDEThing, object> pCallBack)
        {
            MyBaseThing?.RegisterEvent(pName, pCallBack);
        }
        public void UnregisterEvent(string pName, Action<ICDEThing, object> pCallBack)
        {
            MyBaseThing?.UnregisterEvent(pName, pCallBack);
        }
        public void FireEvent(string pEventName, ICDEThing sender, object pPara, bool FireAsync)
        {
            MyBaseThing?.FireEvent(pEventName, sender, pPara, FireAsync);
        }
        public bool HasRegisteredEvents(string pEventName)
        {
            if (MyBaseThing != null)
                return MyBaseThing.HasRegisteredEvents(pEventName);
            return false;
        }
        #endregion

        public bool Init()
        {
            if (!mIsInitStarted)
            {
                mIsInitStarted = true;
                MyBaseThing.RegisterEvent(eEngineEvents.IncomingMessage, HandleMessage);

                /// Note: Where this sample is interesting is when one (or both) 
                /// of the following flags are defined on the command line of
                /// the application host:
                /// -- CDMyWebToMeshSample_EnableREST=true -- Enable REST Server Support.
                /// -- CDMyWebToMeshSample_EnableMeshDataQuery -- Whether to send TSM messages out for data query.

                g_EnableRESTServer = utils.CheckCommandLineFlag("CDMyWebToMeshSample_EnableREST");
                g_EnableMeshDataQuery = utils.CheckCommandLineFlag("CDMyWebToMeshSample_EnableMeshDataQuery");
                g_EnableMeshDataResponse = utils.CheckCommandLineFlag("CDMyWebToMeshSample_EnableMeshDataResponse");

                // Initialize http handler for REST handling.
                if (g_EnableRESTServer)
                {
                    RegisterHttpInterceptor();

                }

                MyBaseThing.StatusLevel = 1;
                MyBaseThing.LastMessage = "Mile Record Holder service has started.";
                mIsInitCompleted = true;
                MyBaseEngine.ProcessInitialized();
            }
            return true;
        }

        public bool CreateUX()
        {
            if (!mIsUXInitStarted)
            {
                mIsUXInitStarted = true;
                mMyDashboard = TheNMIEngine.AddDashboard(MyBaseThing, new TheDashboardInfo(MyBaseEngine, "My Sample Hello World Plugin"));

                var tFlds = TheNMIEngine.AddStandardForm(MyBaseThing, "Welcome to my Hello World Page");
                var tMyForm = tFlds["Form"] as TheFormInfo;

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleEnded, 2, 2, 0, "My Hello World Value", "SampleProperty", new nmiCtrlSingleEnded() { ParentFld = 1 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.BarChart, 3, 2, 0, "My Hello World Bar Chart", "SampleProperty", new nmiCtrlBarChart() { ParentFld = 1, MaxValue = 255, TileHeight = 2, IsVertical = true, Foreground = "blue" });

                TheNMIEngine.AddAboutButton4(MyBaseThing, mMyDashboard, null, true);
                mIsUXInitCompleted = true;
            }
            return true;
        }

        public bool Delete()
        {
            return true;
        }


        #region Message Handling
        /// <summary>
        /// Handles Messages sent from a host sub-engine to its clients
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pIncoming"></param>
        public void HandleMessage(ICDEThing sender, object pIncoming)
        {
            TheProcessMessage pMsg = pIncoming as TheProcessMessage;
            if (pMsg == null) return;

            string[] cmd = pMsg.Message.TXT.Split(':');
            switch (cmd[0])
            {
                case "CDE_INITIALIZED":
                    MyBaseEngine.SetInitialized(pMsg.Message);
                    break;
                case nameof(MsgMileRecordHolder):
                    if (g_EnableMeshDataResponse)
                    {
                        // Request from another node for mile record holder information.
                        var request = TheCommRequestResponse.ParseRequestMessageJSON<MsgMileRecordHolder>(pMsg.Message);
                        var MsgResponse = new MsgMileRecordHolderResponse();
                        if (request != null)
                        {
                            MsgResponse.data = TheRecordHolder.QueryRecordHolder(request.id);
                        }
                        TheCommRequestResponse.PublishResponseMessageJson(pMsg.Message, MsgResponse);

                        MsgResponse = null;   // Prevent legacy response handler for being sent.
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}
