// SPDX-FileCopyrightText: Copyright(c) 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
// SPDX-License-Identifier: MPL-2.0

#if USE_CODEGEN
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using nsCDEngine.Engines;
using nsCDEngine.BaseClasses;
using nsCDEngine.Communication;
using nsCDEngine.ViewModels;
using nsCDEngine.Engines.StorageService;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.ThingService;

namespace CDMyNewNMIDemo
{
    class TestingPage : ICDEPlugin, ICDEThing
    {
        //TODO: Step 1-2: Fill out "InitEngineAssets" (in here)
#region ICDEPlugin
        private IBaseEngine MyBaseEngine;
        public IBaseEngine GetBaseEngine()
        {
            return MyBaseEngine;
        }
        /// <summary>
        /// This constructor is called by The C-DEngine during initialization in order to register this service
        /// </summary>
        /// <param name="pBase">The C-DEngine is creating a Host for this engine and hands it to the Plugin Service</param>
        public void InitEngineAssets(IBaseEngine pBase)
        {
            MyBaseEngine = pBase;
            MyBaseEngine.SetEngineName(this.GetType().FullName);            //Can be any arbitrary name - recommended is the class name
            MyBaseEngine.SetEngineType(this.GetType());                     //Has to be the type of this class
            MyBaseEngine.SetFriendlyName("My Sample Service");  //TODO: STEP 1: Give your plugin a friendly name
            MyBaseEngine.SetEngineService(true);                            //Keep True if this class is a service

            MyBaseEngine.SetEngineID(new Guid("{BA3FE4B3-EF79-4308-A5EA-B8464A80BF33}")); //TODO: STEP 2: REQUIRED - set Plugin GUID
            MyBaseEngine.SetPluginInfo("This service allows you to ...<describe you service>", 0, null, "toplogo-150.png", "C-Labs", "http://www.c-labs.com", new List<string>() { }); //TODO: Describe your plugin - this will later be used in the Plugin-Store
            MyBaseEngine.SetVersion(3.200);
        }
#endregion
2

#region - Rare to Override
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
            if (MyBaseThing != null)
                return MyBaseThing.GetProperty(pName, DoCreate);
            return null;
        }
        public cdeP SetProperty(string pName, object pValue)
        {
            if (MyBaseThing != null)
                return MyBaseThing.SetProperty(pName, pValue);
            return null;
        }
        public void RegisterEvent(string pName, Action<ICDEThing, object> pCallBack)
        {
            if (MyBaseThing != null)
                MyBaseThing.RegisterEvent(pName, pCallBack);
        }
        public void UnregisterEvent(string pName, Action<ICDEThing, object> pCallBack)
        {
            if (MyBaseThing != null)
                MyBaseThing.UnregisterEvent(pName, pCallBack);
        }
        public void FireEvent(string pEventName, ICDEThing sender, object pPara, bool FireAsync)
        {
            if (MyBaseThing != null)
                MyBaseThing.FireEvent(pEventName, sender, pPara, FireAsync);
        }
        public bool HasRegisteredEvents(string pEventName)
        {
            if (MyBaseThing != null)
                return MyBaseThing.HasRegisteredEvents(pEventName);
            return false;
        }
        protected TheThing MyBaseThing;
        protected bool mIsUXInit;
        protected bool mIsInit;
        public bool IsUXInit()
        { return mIsUXInit; }
        public bool IsInit()
        { return mIsInit; }

        public bool Delete()
        {
            return true;
        }

        /// <summary>
        /// Use this to return to the system if you "Thing" is currently Active
        /// </summary>
        /// <returns>True if the "Thing" associated with this class is Alive/Active</returns>
        public bool IsAlive()
        {
            if (MyBaseEngine == null) return false;
            return MyBaseEngine.GetEngineState().IsEngineReady;
        }
#endregion

        public bool Init()
        {
            if (!mIsInit)
            {
                mIsInit = true;
                MyBaseThing.StatusLevel = 1;
                MyBaseThing.LastMessage = "Plugin has started";
                MyBaseEngine.ProcessInitialized();

            }
            return true;
        }

        public bool CreateUX()
        {
            if (!mIsUXInit)
            {

                mIsUXInit = true;
                //NUI Definition for All clients
                mMyDashboard = TheNMIEngine.AddDashboard(MyBaseThing, new TheDashboardInfo(MyBaseEngine, "Code Gen"));



                TheFormInfo tMyForm = TheNMIEngine.AddForm(new TheFormInfo(MyBaseThing) { FormTitle = "", DefaultView = eDefaultView.Form });
                TheNMIEngine.AddFormToThingUX(MyBaseThing, tMyForm, "CMyForm", "C#", 3, 3, 0, TheNMIEngine.GetNodeForCategory(), null, null);
#region Mark's Goren Stress Test


                //Random rnd = new Random();
                 
               
                //TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 6, Float = "left" });
                //TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Number, 2, 2, 0, "How many control groups would you like", "Count", new nmiCtrlSingleEnded() { ParentFld = 1 });
                //TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileButton, 3, 2, 0, "Start", null, new nmiCtrlTileButton() { Background = "green", ParentFld = 1, OnClick = "GRP:controlGroup:Gr1" });
                //TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileButton, 4, 2, 0, "Stop", null, new nmiCtrlTileButton() { Background = "red", ParentFld = 1 });
                //TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 5, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 19, TileHeight = 20, Background = "white", IsHScrollable = true });


                
                //for (int i = 10; i < 131; i+=4)
                //{
                //    int myNum = rnd.Next(1,100);
                //    this.SetProperty(i.ToString(), myNum);
                //    TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, i, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 5, Background = "white", Float = "left", Group = "controlGroup:Gr1", Visibility = false });
                //    TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, i + 1, 0, 0, null, i.ToString(), new nmiCtrlSmartLabel() { ParentFld = i, TileWidth = 1, TileHeight = 1, FontSize = 12, Background = "white" });
                //    TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleEnded, i + 2, 2, 0, null, i.ToString(), new nmiCtrlSingleEnded() { ParentFld = i, TileWidth = 1, FontSize = 12, TileHeight = 1 });
                //    TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.BarChart, i + 3, 2, 0, null, i.ToString(), new nmiCtrlBarChart() { ParentFld = i, TileWidth = 4, NoTE = true, TileHeight = 1 });
                    
                //}

#endregion



                //for (int fldOrd = 10; fldOrd < 19; fldOrd++)
                //{

                //    for (int ctrlGroup = 1; ctrlGroup < 4; ctrlGroup++)
                //    {
                //        for (int onUpName = 100; onUpName < 104; onUpName++)
                //        {

                //        Random rnd = new Random();
                //        int myNum = rnd.Next(1, 100);

                //            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, ctrlGroup, 0, 0, "ControlGroup", null, new nmiCtrlTileGroup() { TileWidth = 6, TileHeight = 1, Float = "left" });
                //            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleEnded, fldOrd, 2, 0, null, onUpName.ToString(), new nmiCtrlSingleEnded() { ParentFld = ctrlGroup, TileWidth = 1, TileHeight = 1, NoTE = true, Float = "left", Value = (int)myNum });
                //            fldOrd += 1;
                //            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, fldOrd, 2, 0, null, onUpName.ToString(), new nmiCtrlSmartLabel() { ParentFld = ctrlGroup, TileWidth = 1, TileHeight = 1, NoTE = true, Float = "left", Value = (int)myNum });
                //            fldOrd += 1;
                //            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.BarChart, fldOrd, 2, 0, null, onUpName.ToString(), new nmiCtrlBarChart() { ParentFld = ctrlGroup, TileWidth = 4, TileHeight = 1, NoTE = true, Float = "left", Value = (int)myNum });
                //            fldOrd += 1;
                //            ctrlGroup += 1;
                //        }
                //    }
                //}



                TheFieldInfo tToShow = TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TextArea, 20, 2, 0, null, "MyVal", new nmiCtrlTextArea() { TileWidth = 6, TileHeight = 6 });
                SetProperty("MyVal", "new PropertyBag {  };");

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 10, 2, 0, "Color", "ColorVal", new nmiCtrlComboBox() { Options = "not set;Aquamarine;Black;Blue;DarkSkyBlue;Green;LimeGreen;Magenta;MistyRose;Navy;Orchid;PaleGreen;SkyBlue;Teal;White;Yellow" });
                SetProperty("ColorVal", "not set").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    //var t= GetProperty("MyVal", true).ToString();
                    //var t = "new PropertyBag { \"Foreground={FORE}\"  };".Replace("{FORE}", p.ToString());
                    tToShow.SetUXProperty(Guid.Empty, "Value= " + CreatePropertyBagString());

                });


                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 11, 2, 0, "Options", "OptionVal", new nmiCtrlComboBox() { Options = "not set;Option_1;Option_2" });
                SetProperty("OptionVal", "not set").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    tToShow.SetUXProperty(Guid.Empty, "Value= " + CreatePropertyBagString());
                });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 12, 2, 0, "Number", "NumVal", new nmiCtrlComboBox() { Options = "not set;1;2;3;4;5;6;7;8;9;10" });
                SetProperty("NumVal", "not set").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    tToShow.SetUXProperty(Guid.Empty, "Value= " + CreatePropertyBagString());
                });




                TheNMIEngine.AddAboutButton(MyBaseThing, true);
            }
            return true;
        }

        string CreatePropertyBagString()
        {
            var t = "new ThePropertyBag { \"Foreground=<Foreground>\", \"Option=<Option>\", \"Number=<Number>\",  };";

            t = DoABlock(t, "Foreground", "ColorVal");  //t is the incoming string with the propertbag; Fpreground is the property name in the bag; ColorVal is the OnUpdateName
            t = DoABlock(t, "Option", "OptionVal");     //For each property in the bag, you need to create one DoABlock and add the property to the string t
            t = DoABlock(t, "Number", "NumVal");


            return t;
        }

        string DoABlock(string t, string PropertyName, string PropertyVal)
        {
            var currentPropertyContent = GetProperty(PropertyVal, true).ToString();         //Get the content of the OnUpdateName property and put it in a variable "currentPropertyContent"
            if (currentPropertyContent.ToLower() == "not set")                              //check if the content is "not set"
                t = t.Replace(string.Format("\"{0}=<{0}>\", ",PropertyName),"");            //if its not set, remove the property declaration from string t
            else
                t = t.Replace(string.Format("<{0}>",PropertyName), currentPropertyContent); //otherwise replace the macro with the sanme name as "PropertName" with the content
            return t;
        }

        TheDashboardInfo mMyDashboard;

        //TODO: Step 4: Write your Business Logic

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
                default:
                    break;
            }
        }
#endregion
    }
}
#endif