// SPDX-FileCopyrightText: Copyright(c) 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Collections.Generic;
using CDMyNewNMIDemo.Demos;

using nsCDEngine.BaseClasses;
using nsCDEngine.Engines;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.ThingService;
using nsCDEngine.ViewModels;

namespace CDMyNewNMIDemo
{
    public class eTheDemoScreensTypes : TheDeviceTypeEnum
    {
        public const string DemoIFrame = "Demo IFrame Screen";
        public const string DemoStandardForm = "Demo Standard Form";
        public const string DemoControls = "Demo Controls";
        public const string DemoWizard = "Demo Wizard";
        public const string AudioWizard = "Audio Rule Wizard";
        public const string OPCWizard = "OPC Wizard";
        public const string AdvancedControls = "Advanced Controls";
        public const string ControlTestEnvironment = "Control Test Environment";
        public const string MeshPicker ="Mesh Picker";
        public const string TestAutomationWizard = "Test Automation Wizard";
    }

    class TheDemoScreens : ICDEPlugin, ICDEThing
    {
        // Base object references 
        protected TheThing MyBaseThing;      // Base thing
        private IBaseEngine MyBaseEngine;    // Base engine (service)

        // Initialization flags
        protected bool mIsInitStarted = false;
        protected bool mIsInitCompleted = false;
        protected bool mIsUXInitStarted = false;
        protected bool mIsUXInitCompleted = false;

        Guid guidEngineID = new Guid("{4F925FF0-43EF-4563-9518-1391F7D179AB}"); // TODO: Set GUID value for InitEngineAssets (in the next block)
        string strFriendlyName = "My Demo Screen Service";               // TODO: Set plugin friendly name for InitEngineAssets (optional)

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

            MyBaseEngine.SetEngineID(guidEngineID);
            MyBaseEngine.SetFriendlyName(strFriendlyName);

            MyBaseEngine.SetEngineName(GetType().FullName);  // Can be any arbitrary name - recommended is the class name
            MyBaseEngine.SetEngineType(GetType());           // Has to be the type of this class
            MyBaseEngine.SetEngineService(true);             // Keep True if this class is a service

            MyBaseEngine.SetPluginInfo("This service shows some common demo screens",       // Describe plugin for Plugin Store
                                       0,                       // pPrice - retail price (default = 0)
                                       null,                    // Custom home page - default = /ServiceID
                                       "toplogo-150.png",       // pIcon - custom icon.
                                       "C-Labs",                // pDeveloper - name of the plugin developer.
                                       "http://www.c-labs.com", // pDeveloperUrl - URL to developer home page.
                                       new List<string>() { }); // pCategories - Search categories for service.
            //MyBaseEngine.RegisterCSS("PSE/CSS/MyNMIStyle.css", null, sinkResources);
        }

        void sinkResources(TheRequestData pData)
        {
            MyBaseEngine.GetPluginResource(pData);
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

        public void SetBaseThing(TheThing pThing)
        {
            MyBaseThing = pThing;
        }
        public TheThing GetBaseThing()
        {
            return MyBaseThing;
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
                MyBaseThing.StatusLevel = 4;
                MyBaseThing.LastMessage = "Service has started";

                InitServices();
                mIsInitCompleted = true; // For future IsInit() calls
                MyBaseEngine.ProcessInitialized(); //Set the status of the Base Engine according to the status of the Things it manages
            }
            return false;
        }

        public bool CreateUX()
        {
            if (!mIsUXInitStarted)
            {
                mIsUXInitStarted = true;
                mMyDashboard = TheNMIEngine.AddDashboard(MyBaseThing, new TheDashboardInfo(MyBaseEngine, "My NMI Demo Screens") { PropertyBag = new nmiDashboardTile { Caption = "<i class='fa faIcon fa-5x'>&#xf1c5;</i></br>NMI Demo Screens", Category = "NMI Extensions" } });

                var tFlds = TheNMIEngine.CreateEngineForms(MyBaseThing, TheThing.GetSafeThingGuid(MyBaseThing, "MYNAME"), "List of TheDemoScreens", null, 20, 0x0F, 0xF0, TheNMIEngine.GetNodeForCategory(), "REFFRESHME", true, new eTheDemoScreensTypes(), eTheDemoScreensTypes.DemoIFrame);
                TheFormInfo tForm = tFlds["Form"] as TheFormInfo;
                TheNMIEngine.RegisterEngine(MyBaseEngine);
                mIsUXInitCompleted = true;
            }
            return true;
        }
        public bool Delete()
        {
            return true;
        }

        TheDashboardInfo mMyDashboard;
        void InitServices()
        {
            var tThing = TheThingRegistry.GetThingByProperty(MyBaseThing.EngineName, Guid.Empty, "DeviceType", eTheDemoScreensTypes.DemoIFrame);
            var t = new DemoIFrame(tThing, this);

            var tThing2 = TheThingRegistry.GetThingByProperty(MyBaseThing.EngineName, Guid.Empty, "DeviceType", eTheDemoScreensTypes.DemoStandardForm);
            var t2 = new DemoStandardForm(tThing2, this);

            var tThing3 = TheThingRegistry.GetThingByProperty(MyBaseThing.EngineName, Guid.Empty, "DeviceType", eTheDemoScreensTypes.DemoControls);
            var t3 = new DemoControls(tThing3, this);

            var tThing4 = TheThingRegistry.GetThingByProperty(MyBaseThing.EngineName, Guid.Empty, "DeviceType", eTheDemoScreensTypes.DemoWizard);
            var t4 = new DemoWizard(tThing4, this);

            var tThing5 = TheThingRegistry.GetThingByProperty(MyBaseThing.EngineName, Guid.Empty, "DeviceType", eTheDemoScreensTypes.AudioWizard);
            var t5 = new AudioRuleWizard(tThing5, this);

            var tThing9 = TheThingRegistry.GetThingByProperty(MyBaseThing.EngineName, Guid.Empty, "DeviceType", eTheDemoScreensTypes.MeshPicker);
            var t9 = new TheMeshPicker(tThing9, this);

            var tThing10 = TheThingRegistry.GetThingByProperty(MyBaseThing.EngineName, Guid.Empty, "DeviceType", eTheDemoScreensTypes.TestAutomationWizard);
            var t10 = new TestAutoWizard(tThing10, this);

            MyBaseEngine.SetStatusLevel(1);
        }

        #region Message Handling
        public void HandleMessage(ICDEThing sender, object pIncoming)
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
