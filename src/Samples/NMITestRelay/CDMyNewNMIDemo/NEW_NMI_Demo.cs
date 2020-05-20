// SPDX-FileCopyrightText: Copyright(c) 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
// SPDX-License-Identifier: MPL-2.0

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
    class NEW_NMI_Demo : ICDEPlugin, ICDEThing
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

            MyBaseEngine.SetEngineID(new Guid("{23617789-6736-48EE-9716-C6FCBF75E339}")); //TODO: STEP 2: REQUIRED - set Plugin GUID
            MyBaseEngine.SetPluginInfo("This service allows you to ...<describe you service>", 0, null, "toplogo-150.png", "C-Labs", "http://www.c-labs.com", new List<string>() { }); //TODO: Describe your plugin - this will later be used in the Plugin-Store
            MyBaseEngine.SetVersion(3.204);
            MyBaseEngine.RegisterCSS("PSE/CSS/MyNMIStyle.css", null, sinkResources);
        }

        #endregion


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
        protected TheThing MyBaseThing = null;
        protected bool mIsUXInitCalled = false;
        protected bool mIsUXInitialized = false;
        protected bool mIsInitCalled = false;
        protected bool mIsInitialized = false;
        public bool IsUXInit()
        { return mIsUXInitialized; }
        public bool IsInit()
        { return mIsInitialized; }


        #endregion

        public bool Init()
        {
            if (mIsInitCalled) return false;
            mIsInitCalled = true;
                MyBaseThing.StatusLevel = 4;
                SetProperty("LastMessage", "Plugin has started");
                SetProperty("BarVal", 20);
                SetProperty("ProgressBarVal", 50);
                SetProperty("SingleEndedVal", "Sample");
                SetProperty("EndlessSlider1", 50);


                MyBaseThing.RegisterEvent(eEngineEvents.IncomingMessage, HandleMessage);
                TheThing.SetSafePropertyString(MyBaseThing, "Sample", null); //Declare the Property and optionally Initalize (change null with initialization)
            mIsInitialized = true;
            MyBaseEngine.ProcessInitialized();
            return true;
        }
        public bool Delete()
        {
            mIsInitialized = false;
            // TODO Properly implement delete
            return true;
        }

        void sinkResources(TheRequestData pData)
        {
            MyBaseEngine.GetPluginResource(pData);
        }

        TheFieldInfo HeaderName = null;

        public bool CreateUX()
        {
            if (mIsUXInitCalled) return false;
            mIsUXInitCalled = true;

                //NUI Definition for All clients
                mMyDashboard = TheNMIEngine.AddDashboard(MyBaseThing, new TheDashboardInfo(MyBaseEngine, "My New NMI Demo"));
                
                TheFormInfo tMyForm = TheNMIEngine.AddForm(new TheFormInfo(MyBaseThing) { FormTitle = "Welcome Sanja NMI Demo Page V2", DefaultView = eDefaultView.Form});
                TheNMIEngine.AddFormToThingUX(MyBaseThing, tMyForm, "CMyForm", "My Demo Form", 3, 3, 0, "Samples", null, null);
                //CONTAINER
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 4, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth=18, TileHeight= 18, Background="white"}); 

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 6, 0, 0, null, null, new nmiCtrlSmartLabel() {TileWidth=3, TileHeight=1, ParentFld=4, Float = "left", NoTE= true, FontSize=28,Foreground = "#051bc8",Background="transparent", Value="Control Name"});
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 8, 0, 0, null, null, new nmiCtrlSmartLabel() { TileWidth=7, TileHeight=1, ParentFld=4, Float="left", NoTE=true, Value="Name of the control", FontSize=15, Foreground="black", Background="transparent"});
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 9, 0, 0, null, null, new nmiCtrlSmartLabel() { TileWidth = 8, TileHeight = 1, ParentFld = 4, Float = "left", NoTE = true,FontSize=28, Foreground = "#051bc8", Background = "transparent", HorizontalAlignment ="left", Value= "Description" });
             

                //left side
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 50, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 4, TileWidth = 3, TileHeight = 18,  Float = "left" }); //Background = "white",
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileButton, 51, 2, 0, "ctrlBarChart", null, new nmiCtrlTileButton() { NoTE = true, TileWidth = 3, TileHeight = 1, Foreground = "#051bc8", ParentFld = 50, OnClick = "GRP:controlGroup:Bar" });//.RegisterUXEvent(MyBaseThing, eUXEvents.OnClick, "Bar Chart", sinkChange);
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileButton, 52, 2, 0, "ctrlCheckField", null, new nmiCtrlTileButton() { NoTE = true, TileWidth = 3, TileHeight = 1, Foreground = "#051bc8", ParentFld = 50, OnClick = "GRP:controlGroup:CheckField" });//.RegisterUXEvent(MyBaseThing, eUXEvents.OnClick, "Check Field", sinkChange);
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileButton, 53, 2, 0, "ctrlComboBox", null, new nmiCtrlTileButton() { NoTE = true, TileWidth = 3, TileHeight = 1, Foreground = "#051bc8", ParentFld = 50 ,OnClick = "GRP:controlGroup:ComboBox" });//.RegisterUXEvent(MyBaseThing, eUXEvents.OnClick, "Combo Box", sinkChange);
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileButton, 54, 2, 0, "ctrlDropUploader", null, new nmiCtrlTileButton() { NoTE = true, TileWidth = 3, TileHeight = 1, Foreground = "#051bc8", ParentFld = 50,
                    OnClick = "GRP:controlGroup:DropUploader"});//.RegisterUXEvent(MyBaseThing, eUXEvents.OnClick, "Drop Uploader", sinkChange);
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileButton, 55, 2, 0, "ctrlProgressBar", null, new nmiCtrlTileButton() { NoTE = true, TileWidth = 3, TileHeight = 1, Foreground = "#051bc8", ParentFld = 50,
                    OnClick = "GRP:controlGroup:ProgressBar" });//.RegisterUXEvent(MyBaseThing, eUXEvents.OnClick, "Progress Bar", sinkChange);
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileButton, 56, 2, 0, "ctrlSingleCheck", null, new nmiCtrlTileButton() { NoTE = true, TileWidth = 3, TileHeight = 1, Foreground = "#051bc8", ParentFld = 50,
                    OnClick = "GRP:controlGroup:SingleCheck" });//.RegisterUXEvent(MyBaseThing, eUXEvents.OnClick, "Single Check", sinkChange);
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileButton, 57, 2, 0, "ctrlSingleEnded", null, new nmiCtrlTileButton() { NoTE = true, TileWidth = 3, TileHeight = 1, Foreground = "#051bc8", ParentFld = 50,
                    OnClick = "GRP:controlGroup:SingleEnded" });//.RegisterUXEvent(MyBaseThing, eUXEvents.OnClick, "Single Ended", sinkChange);
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileButton, 58, 2, 0, "ctrlSmartLabel", null, new nmiCtrlTileButton() { NoTE = true, TileWidth = 3, TileHeight = 1, Foreground = "#051bc8", ParentFld = 50,
                    OnClick = "GRP:controlGroup:SmartLabel" });//.RegisterUXEvent(MyBaseThing, eUXEvents.OnClick, "Smart Label", sinkChange);
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileButton, 59, 2, 0, "ctrlTileButton", null, new nmiCtrlTileButton() { NoTE = true, TileWidth = 3, TileHeight = 1, Foreground = "#051bc8", ParentFld = 50,
                    OnClick = "GRP:controlGroup:TileButton"});//.RegisterUXEvent(MyBaseThing, eUXEvents.OnClick, "Tile Button", sinkChange);
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileButton, 60, 2, 0, "ctrlTileGroup", null, new nmiCtrlTileButton() { NoTE = true, TileWidth = 3, TileHeight = 1, Foreground = "#051bc8", ParentFld = 50,
                 OnClick = "GRP:controlGroup:TileGroup"}); //.RegisterUXEvent(MyBaseThing, eUXEvents.OnClick, "Tile Group", sinkChange);
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileButton, 61, 2, 0, "ctrlTouchDraw", null, new nmiCtrlTileButton() { NoTE = true, TileWidth = 3, TileHeight = 1, Foreground = "#051bc8", ParentFld = 50, OnClick = "GRP:controlGroup:TouchDraw" });//.RegisterUXEvent(MyBaseThing, eUXEvents.OnClick, "Touch Draw", sinkChange);
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileButton, 62, 2, 0, "ctrlEndlessSlider", null, new nmiCtrlTileButton() { NoTE = true, TileWidth = 3, TileHeight = 1, Foreground = "#051bc8", ParentFld = 50,
                    OnClick = "GRP:controlGroup:EndlessSlider" });//.RegisterUXEvent(MyBaseThing, eUXEvents.OnClick, "Endless Slider", sinkChange);
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileButton, 63, 2, 0, "ctrlVideoViewer", null, new nmiCtrlTileButton() { NoTE = true, TileWidth = 3, TileHeight = 1, Foreground = "#051bc8", ParentFld = 50,
                    OnClick = "GRP:controlGroup:VideoViewer" });//.RegisterUXEvent(MyBaseThing, eUXEvents.OnClick, "Video Viewer", sinkChange);
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileButton, 64, 2, 0, "ctrlPicture", null, new nmiCtrlTileButton() { NoTE = true, TileWidth = 3, TileHeight = 1, Foreground = "#051bc8", ParentFld = 50,
                    OnClick = "GRP:controlGroup:Picture" });//.RegisterUXEvent(MyBaseThing, eUXEvents.OnClick, "Zoom Image", sinkChange);


                //barChart
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 100, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld=4, TileWidth = 15, TileHeight = 16, Group="controlGroup:Bar",Float = "left" , Visibility=true});//, Background="white" 
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 101, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 100, TileWidth = 8, TileHeight = 2, Float = "left" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 102, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 100, TileWidth = 7, TileHeight = 2, Float = "left" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 103, 0, 0, null, null, new nmiCtrlSmartLabel() { ParentFld = 102,TileWidth=4, HorizontalAlignment = "left", NoTE = true, FontSize=20, Foreground="#000000", Background="transparent", Value = "This control displays the “Value” property as a vertical or horizontal bar." });
                TheFieldInfo Bar1= TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.BarChart, 106, 2, 0, "BarChart", "BarVal", new nmiCtrlBarChart() { ParentFld = 101 });
                //mdl right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 110, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 100, TileWidth = 8, TileHeight = 6,Float="left" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 111, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 100, TileWidth = 7, TileHeight = 6,Float="left" });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 112, 2, 0, "Background", "BarBackground", new nmiCtrlComboBox() { ParentFld = 110, Options = "transparent;yellow;green;blue;red;purple;black" });
                SetProperty("BarBackground", "blue").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    Bar1.SetUXProperty(Guid.Empty, "Background=" + p.ToString());

                });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 113, 2, 0, "Main Background", "BarMainBackground", new nmiCtrlComboBox() {ParentFld = 111, Options = "transparent;white;gold;black" });
                SetProperty("BarMainBackground", "white").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    Bar1.SetUXProperty(Guid.Empty, "MainBackground=" + p.ToString());

                });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 114, 2, 0, "Foreground", "BarForeground", new nmiCtrlComboBox() { ParentFld = 110, Options = "transparent;yellow;green;blue;red;purple;black" });
                SetProperty("BarForeground", "green").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    Bar1.SetUXProperty(Guid.Empty, "Foreground=" + p.ToString());

                });
              
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 115, 2, 0, "Foreground Opacity", "BarForegroundOpacity", new nmiCtrlComboBox() { ParentFld = 111, Options = "0.1;0.2;0.3;0.4;0.5;0.6;0.7;0.8;0.9;1.0" });
                SetProperty("BarForegroundOpacity", "1.0").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    Bar1.SetUXProperty(Guid.Empty, "ForegroundOpacity=" + p.ToString());

                });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 116, 2, 0, "Max Value", "BarMaxValue", new nmiCtrlComboBox() { ParentFld = 110, Options = "100;200;300" });
                SetProperty("BarMaxValue", 100).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    Bar1.SetUXProperty(Guid.Empty, "MaxValue=" + p.ToString());

                });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 117, 2, 0, "Min Value", "BarMinValue", new nmiCtrlComboBox() { ParentFld = 111, Options = "0;1;10" });
                SetProperty("BarMinValue", 0).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    Bar1.SetUXProperty(Guid.Empty, "MinValue=" + p.ToString());

                });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 118, 2, 0, "Is Vertical", "BarIsVertical", new nmiCtrlSingleCheck() { ParentFld = 110 });
                SetProperty("BarIsVertical", false).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    Bar1.SetUXProperty(Guid.Empty, "IsVertical=" + p.ToString());

                });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 119, 2, 0, "Is Inverted", "BarIsInverted", new nmiCtrlSingleCheck() { ParentFld = 110 });
                SetProperty("BarIsInverted", false).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    Bar1.SetUXProperty(Guid.Empty, "IsInverted=" + p.ToString());

                });

                //TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 120, 2, 0, "Auto Adjust", "BarAutoAdjust", new nmiCtrlSingleCheck() {  ParentFld = 111 });
                //SetProperty("BarAutoAdjust", false).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                //{
                //    Bar1.SetUXProperty(Guid.Empty, "AutoAdjust=" + p.ToString());

                //}); //CM: Does not exists on barchart (anymore)

                //bottom right (yes I know I am using extra tileGroups = it is to be able to set the blue background on alternate rows and make it look like a table
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 150, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 15, TileHeight = 6, ParentFld = 100 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 151, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 150, Background = "rgba(5,27,200, .05)" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 152, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 150, Background = "white"});
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 153, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 150, Background = "rgba(5,27,200, .05)"});


                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 154, 0, 0, "Usage scenario:", null, new nmiCtrlSmartLabel() {Foreground="black", Background="transparent",FontSize=28, ParentFld = 151, TileWidth = 14, TileHeight = 1, HorizontalAlignment="left", Value = "If the bar is set to user-interactive, the “Value” can be modified by moving the bar. The new value is sent as soon as finger is taken off the bar." });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 155, 0, 0, "The meaning of the Value:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize=28, ParentFld = 152, TileWidth = 14, TileHeight = 1, HorizontalAlignment="left", Value = "The Value property is the length of the bar in the chart." });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 156, 0, 0, "Note:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize=28, ParentFld = 153, TileWidth = 14, TileHeight = 1, HorizontalAlignment="left", Value = "If no properties are set, bar chart will appear as a 1x1 tile square in gray color. This is because the “Value” property needs to be set." });






                //CHECK FIELD
                //
                //

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 200, 0, 0, null, null, new nmiCtrlTileGroup() {ParentFld=4 , TileWidth = 15, TileHeight = 16,Group="controlGroup:CheckField",  Float = "left", Visibility = false });//Background = "white",
                //top right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 201, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld =200, TileWidth=8, TileHeight=2, Float="left" });
                //sample container
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 202, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 200, TileWidth=4, TileHeight=3, Float="left"});
                TheFieldInfo CheckField1 = TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.CheckField, 204, 2, 0, "Check Field Title", "CheckFieldVal", new nmiCtrlCheckField() { TileWidth = 6, TileHeight = 3, ParentFld = 201, Bits=8 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 205, 0, 0, null, null, new nmiCtrlSmartLabel() { TileWidth = 4, TileHeight = 2, ParentFld = 202,  HorizontalAlignment = "left", NoTE = true,FontSize = 20, Foreground = "#000000", Background = "transparent", Value = "Creates a field of check boxes that is a bitwise representation of a numeric value (byte, int, word, short, long)." });
                //mdl right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 210, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 200, TileWidth = 8, TileHeight = 6, Float = "left" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 211, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 200, TileWidth = 7, TileHeight = 6, Float = "left" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 212, 2, 0, "Image List", "CheckFieldImageList", new nmiCtrlComboBox() { ParentFld = 210, Foreground = "#000000", Options = "none;ACLs" });
                SetProperty("CheckFieldImageList", "none").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    string tCheckList = "";
                    if (p.ToString() == "ACLs")
                        tCheckList = "<span class='fa-stack fa-lg'><i class='fa fa-stack-1x'>&#xf21b;</i><i class='fa fa-stack-2x'>&#x2003;</i></span>," +
                                "<span class='fa-stack fa-lg'><i class='fa fa-stack-1x'>&#xf044;</i><i class='fa fa-stack-2x'>&#x2003;</i></span>," +
                                "<span class='fa-stack fa-lg'><i class='fa fa-stack-1x'>&#xf10b;</i><i class='fa fa-stack-2x text-danger' style='opacity:0.5'>&#xf05e;</i></span>," +
                                "<span class='fa-stack fa-lg'><i class='fa fa-stack-1x'>&#xf0ce;</i><i class='fa fa-stack-2x text-danger' style='opacity:0.5'>&#xf05e;</i></span>," +
                                "<span class='fa-stack fa-lg'><i class='fa fa-stack-1x'>&#xf0f6;</i><i class='fa fa-stack-2x text-danger' style='opacity:0.5'>&#xf05e;</i></span>," +
                                "<span class='fa-stack fa-lg'><i class='fa fa-stack-1x'>&#xf15c;</i><i class='fa fa-stack-2x text-danger' style='opacity:0.5'>&#xf05e;</i></span>";
                    CheckField1.SetUXProperty(Guid.Empty, "ImageList=" + tCheckList);
                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 213, 2, 0, "Options", "CheckFieldOptions", new nmiCtrlComboBox() { ParentFld = 211, Foreground = "#000000", Options = "none;Names" });
                SetProperty("CheckFieldOptions", "none").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    string tCheckList = "";
                    if (p.ToString() == "Names")
                        tCheckList = "One;Two;Three;Kit;Dog";
                        CheckField1.SetUXProperty(Guid.Empty, "Options=" + tCheckList);
                });

                //TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 214, 2, 0, "IsOverlay", "CheckFieldIsOverlay", new nmiCtrlSingleCheck() { ParentFld = 210, Foreground = "#000000" });
                //SetProperty("CheckFieldIsOverlay", "orange").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                //{
                //    CheckField1.SetUXProperty(Guid.Empty, "IsOverlay=" + p.ToString());
                //}); //CM: Cannot be set by SmartControl
                //bottom right 
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 250, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 15, TileHeight = 6, ParentFld = 200 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 251, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 250, Background = "rgba(5,27,200, .05)" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 252, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 250, Background = "white" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 253, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 250, Background = "rgba(5,27,200, .05)" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 254, 0, 0, "Usage scenario:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 251, TileWidth = 14, TileHeight = 1, FldWidth = 12, HorizontalAlignment = "left", Value = "Every checkbox in the field represents one bit of the numeric value (i.e: 5 = 0101; Bitfield would show 4 singlechecks; False true false true)."});
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 255, 0, 0, "The meaning of the Value:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 252, TileWidth = 14, TileHeight = 1, HorizontalAlignment = "left", Value = "Numeric value calculated from the binary value of the checkField." });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 256, 0, 0, "Note:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 253, TileWidth = 14, TileHeight = 1, HorizontalAlignment = "left", Value = "Property FldWidth is used to set the amount of checks in the field." });


                
               
                
                
                //COMBO BOX
                //
                //
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 300, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld=4, TileWidth = 15, TileHeight = 16,Group = "controlGroup:ComboBox", Background="white", Float="left" ,Visibility = false });
                //top right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 301, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 300,TileWidth = 8, TileHeight = 2, Float="left"  });
                //sample container
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 302, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 300, TileWidth = 4, TileHeight = 3, Float = "left" });
              
                TheFieldInfo ComboBox1 = TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 304, 2, 0, "Combo Box", null, new nmiCtrlComboBox() { TileWidth = 6, TileHeight = 3, ParentFld = 301, Options="Option_1;Option_2" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 305, 0, 0, null, null, new nmiCtrlSmartLabel() { TileWidth = 4, TileHeight = 2, ParentFld = 302, HorizontalAlignment = "left", NoTE = true, FontSize = 20, Foreground = "#000000", Background = "transparent", Value = "This control allows user to select between pre - set options.Label on left, options exposed when onClick event is detected." });

                //mdl right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 310, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 300, TileWidth = 8, TileHeight = 6, Float = "left" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 311, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 300, TileWidth = 7, TileHeight = 6, Float = "left" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 312, 2, 0, "Background", "ComboBoxBackground", new nmiCtrlComboBox() {ParentFld = 310, Foreground = "#000000", Options = "transparent;yellow;green;blue;red;purple;black" });
                SetProperty("ComboBoxBackground", "").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    ComboBox1.SetUXProperty(Guid.Empty, "Background=" + p.ToString());

                });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 313, 2, 0, "Foreground", "ComboBoxForeground", new nmiCtrlComboBox() { ParentFld = 311, Foreground = "#000000", Options = "transparent;yellow;green;blue;red;purple;black" });
                SetProperty("ComboBoxForeground", "").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    ComboBox1.SetUXProperty(Guid.Empty, "Foreground=" + p.ToString());

                });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 314, 2, 0, "Options", "ComboBoxOptions", new nmiCtrlComboBox() { ParentFld = 310, Foreground = "#000000" , Options="One;Two;Three"});
                SetProperty("ComboBoxOptions", "One").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    string tOpt = "Opt1;Opt2;Opt3";
                    switch (p.ToString())
                    {
                        case "One":
                            tOpt = "Dog;Cat;House";
                            break;
                        case "Two":
                            tOpt = "Group A;A;B;C;:;Group B;U:4;V:5;B:6";
                            break;
                        case "Three":
                            tOpt = "Nothing:2;something:1";
                            break;
                    }
                    ComboBox1.SetUXProperty(Guid.Empty, "Options=" + tOpt);

                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 315, 2, 0, "Inner Class Name", "ComboBoxInnerClassName", new nmiCtrlComboBox() { ParentFld = 311, Foreground = "#000000", Options = "BackGreen;BackWhite" });
                SetProperty("ComboBoxInnerClassName", "BackWhite").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    ComboBox1.SetUXProperty(Guid.Empty, "InnerClassName=" + p.ToString());

                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 316, 2, 0, "Theme", "ComboBoxTheme", new nmiCtrlComboBox() { ParentFld = 310, Foreground = "#000000", Options = "android-ics" });
                SetProperty("ComboBoxTheme", "android-ics").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    ComboBox1.SetUXProperty(Guid.Empty, "Theme=" + p.ToString());

                });


                //bottom right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup,  350, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 15, TileHeight = 6, ParentFld = 300 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup,  351, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 350, Background = "rgba(5,27,200, .05)" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup,  352, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 350, Background = "white" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup,  353, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 350, Background = "rgba(5,27,200, .05)" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 354, 0, 0, "Usage scenario:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 351, TileWidth = 14, TileHeight = 1, FldWidth = 12, HorizontalAlignment = "left", Value = "Use as a standard combo box." });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 355, 0, 0, "The meaning of the Value:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 352, TileWidth = 14, TileHeight = 1, HorizontalAlignment = "left", Value = "Value property is the selected option in the combo box." });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 356, 0, 0, "Note:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 353, TileWidth = 14, TileHeight = 1, HorizontalAlignment = "left", Value = " Options of the Combobox separated by ;. If value and option need to be different use Option:Value. If grouping is desired separate the grooups with ;:;. For the ThingPicker use LOOKUP:THINGPICKER. For the PropertyPicker use LOOKUP:PROPERTYPICKER:ThingGUID"});

                //DROP UPLOADER
                //
                //
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 400, 0, 0, null, null, new nmiCtrlTileGroup() {ParentFld=4, TileWidth = 15, TileHeight = 16, Group = "controlGroup:DropUploader", Background="white" ,Float = "left", Visibility = false ,});
                //top right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 401, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 400, TileWidth = 8, TileHeight = 2, Float = "left" }); 
                //sample container
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 402, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 400, TileWidth = 4, TileHeight = 3, Float = "left" });
                TheFieldInfo DropUploader1 = TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.DropUploader, 404, 2, 0, "Drop", "DropUploaderVal", new nmiCtrlDropUploader() { ParentFld = 401});
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 405, 0, 0, null, null, new nmiCtrlSmartLabel() { TileWidth = 4, TileHeight = 2, ParentFld = 402, HorizontalAlignment = "left", NoTE = true, FontSize = 20, Foreground = "#000000", Background = "transparent", Value = "Uploads files that are dropped into it to the Relay." });
                //mdl right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 410, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 8, TileHeight = 6, ParentFld = 400, Float="left" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 411, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 7, TileHeight = 6, ParentFld = 400, Float = "left" });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 412, 2, 0, "Title", "DropUploaderTitle", new nmiCtrlComboBox() { TileWidth = 4, TileHeight = 1, ParentFld = 410, Foreground = "#000000", Options = "Title_One;Title_Two" });
                SetProperty("DropUploaderTitle", "Title_One").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    DropUploader1.SetUXProperty(Guid.Empty, "Title=" + p.ToString());

                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 413, 2, 0, "Max File Size", "MaxFileSize", new nmiCtrlComboBox() { TileWidth = 4, TileHeight = 1, ParentFld = 411, Foreground = "#000000", Options = "100;200" });
                SetProperty("DropUploaderMaxFileSize", 100).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    DropUploader1.SetUXProperty(Guid.Empty, "MaxFileSize=" + p.ToString());

                });

                //bottom right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup,  450, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 15, TileHeight = 6, ParentFld = 400 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup,  451, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 450, Background = "rgba(5,27,200, .05)" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup,  452, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 450, Background = "white" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup,  453, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 450, Background = "rgba(5,27,200, .05)" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 454, 0, 0, "Usage scenario:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 451, TileWidth = 14, TileHeight = 1, FldWidth = 12, HorizontalAlignment = "left", Value =" Use to upload files.Files dropped in the Uploader area are sent to the plugin set by the “EngineName” property using the TXT =”CDE_FILEPUSH” command." });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 455, 0, 0, "The meaning of the Value:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 452, TileWidth = 14, TileHeight = 1, HorizontalAlignment = "left", Value = "The DropUploader does not use the Value property." });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 456, 0, 0, "Note:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 453, TileWidth = 14, TileHeight = 1, HorizontalAlignment = "left", Value = " Options of the Combobox separated by ;. If value and option need to be different use Option:Value. If grouping is desired separate the grooups with ;:;. For the ThingPicker use LOOKUP:THINGPICKER. For the PropertyPicker use LOOKUP:PROPERTYPICKER:ThingGUID" });




                //PROGRESS BAR
                //
                //
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 500, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 4, TileWidth = 15, TileHeight = 16, Group = "controlGroup:ProgressBar", Background = "white", Float = "left", Visibility = false });
                //top right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 501, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 500, TileWidth=8, TileHeight=2, Float="left" });
                //sample container
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 502, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 4, TileHeight = 3,Float="left" , ParentFld = 500 });

                TheFieldInfo ProgressBar1= TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ProgressBar, 504, 2, 0, "Progress Bar", "ProgressBarVal", new nmiCtrlProgressBar() { ParentFld = 501});
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 505, 0, 0, null, null, new nmiCtrlSmartLabel() { TileWidth = 4, TileHeight = 2, ParentFld = 502, HorizontalAlignment = "left", NoTE = true, FontSize = 20, Foreground = "#000000", Background = "transparent", Value = "A progress bar is used to represent a current position in a process.." });
                //mdl right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 510, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 8, TileHeight = 6, ParentFld = 500, Float = "left" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 511, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 7, TileHeight = 6, ParentFld = 500, Float = "left" });


                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 512, 2, 0, "Max Value", "ProgressBarMaxValue", new nmiCtrlComboBox() { ParentFld = 510, Foreground = "#000000", Options = "100;1000;10000" });
                SetProperty("ProgressBarMaxValue", 100).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    ProgressBar1.SetUXProperty(Guid.Empty, "MaxValue=" + p.ToString());

                });     
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 513, 2, 0, "Background", "ProgressBarBackground", new nmiCtrlComboBox() {  ParentFld = 511, Foreground = "#000000", Options = "transparent;yellow;orange;green;blue;red;brown;black" });
                SetProperty("ProgressBarBackground", "red").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    ProgressBar1.SetUXProperty(Guid.Empty, "Background=" + p.ToString());

                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 514, 2, 0, "Foreground", "ProgressBarForeground", new nmiCtrlComboBox() { ParentFld = 510, Foreground = "#000000", Options = "transparent;yellow;orange;green;blue;red;brown;black" });
                SetProperty("ProgressBarForeground", "").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    ProgressBar1.SetUXProperty(Guid.Empty, "Foreground=" + p.ToString());

                });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 515, 2, 0, "Bar Class Name", "Bar Class Name", new nmiCtrlComboBox() {ParentFld = 511, Foreground = "#000000", Options = "Class_Name_One;Class_Name_Two" });
                SetProperty("ProgressBarBarClassName", "").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    ProgressBar1.SetUXProperty(Guid.Empty, "BarClassName=" + p.ToString());

                });
                //bottom right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 550, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 15, TileHeight = 6, ParentFld = 500 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 551, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 550, Background = "rgba(5,27,200, .05)" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 552, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 550, Background = "white" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 553, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 550, Background = "rgba(5,27,200, .05)" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 554, 0, 0, "Usage scenario:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 551, TileWidth = 14, TileHeight = 1, FldWidth = 12, HorizontalAlignment = "left", Value = "Use to represent a current position in a process." });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 555, 0, 0, "The meaning of the Value:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 552, TileWidth = 14, TileHeight = 1, HorizontalAlignment = "left", Value = "Value of the current position of the progress bar. Must be between 0 and MaxValue." });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 556, 0, 0, "Note:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 553, TileWidth = 14, TileHeight = 1, HorizontalAlignment = "left", Value = "TBD" });

                
                //SINGLE CHECK
                //
                //
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 600, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld=4 ,TileWidth = 15, TileHeight = 16, Float = "left", Visibility = false, Group = "controlGroup:SingleCheck", Background="white" });
                //top right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 601, 0, 0, null, null, new nmiCtrlTileGroup() {ParentFld = 600, TileWidth=8, TileHeight=2,Float="left" });
                //sample container
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 602, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 4, TileHeight = 3, ParentFld = 600, Float="left" });

                TheFieldInfo SingleCheck1 = TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 604, 2, 0, "Single Check", "SingleCheckVal", new nmiCtrlSingleCheck() {ParentFld = 601 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 605, 0, 0, null, null, new nmiCtrlSmartLabel() { TileWidth = 4, TileHeight = 2, ParentFld = 602, HorizontalAlignment = "left", NoTE = true, FontSize = 20, Foreground = "#000000", Background = "transparent", Value = "This control displays a check box with or without checkmark and can be customized by simply setting properties." });
                //mdl right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 610, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 8, TileHeight = 6, ParentFld = 600, Float = "left" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 611, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 7, TileHeight = 6, ParentFld = 600, Float = "left" });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 612, 2, 0, "Check Image", "SingleCheckCheckImage", new nmiCtrlComboBox() { ParentFld = 610, Options = "images/iconTopLogo.png;none" });
                SetProperty("SingleCheckCheckImage", "none").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    SingleCheck1.SetUXProperty(Guid.Empty, "CheckImage=" + p.ToString());

                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 613, 2, 0, "Background", "CheckBarBackground", new nmiCtrlComboBox() {  ParentFld = 611,  Options = "transparent;yellow;orange;green;blue;red;brown;black" });
                SetProperty("CheckBarBackground", "red").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    SingleCheck1.SetUXProperty(Guid.Empty, "Background=" + p.ToString());

                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 614, 2, 0, "Foreground", "SingleCheckForegroud", new nmiCtrlComboBox() {  ParentFld = 610, Options = "transparent;yellow;orange;green;blue;red;brown;black" });
                SetProperty("SingleCheckForegroud", "red").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    SingleCheck1.SetUXProperty(Guid.Empty, "Foreground=" + p.ToString());

                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 615, 2, 0, "Title", "SingleCheckTitle", new nmiCtrlComboBox() {  ParentFld = 611,  Options = "Title_1;Title_2" });
                SetProperty("SingleCheckTitle", "Title_1").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    SingleCheck1.SetUXProperty(Guid.Empty, "Title=" + p.ToString());

                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 616, 2, 0, "Are you sure", "SingleCheckAreYouSure", new nmiCtrlComboBox() { ParentFld = 610, Options="Yes;No" });
                SetProperty("SingleCheckAreYouSure", "no").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    SingleCheck1.SetUXProperty(Guid.Empty, "AreYouSure=" + p.ToString());

                });

                //TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 617, 2, 0, "Is Overlay", "SingleCheckIsOverlay", new nmiCtrlSingleCheck() { ParentFld = 611 });
                //SetProperty("SingleCheckIsOverlay", false).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                //{
                //    SingleCheck1.SetUXProperty(Guid.Empty, "IsOverlay=" + p.ToString());

                //});


                //bottom right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 650, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 15, TileHeight = 6, ParentFld = 600 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 651, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 650, Background = "rgba(5,27,200, .05)" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 652, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 650, Background = "white" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 653, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 650, Background = "rgba(5,27,200, .05)" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 654, 0, 0, "Usage scenario:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 651, TileWidth = 14, TileHeight = 1, FldWidth = 12, HorizontalAlignment = "left", Value = "Show an on/off true/false 0/1 Boolean state." });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 655, 0, 0, "The meaning of the Value:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 652, TileWidth = 14, TileHeight = 1, HorizontalAlignment = "left", Value = "The Value property is interpreted as a Boolean variable(true, false, “true”, “false”, 0, 1).If the Boolean is true, a checkbox image will appear.If the Value is false, the checkbox image will be only visible as a ghosted image." });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 656, 0, 0, "Note:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 653, TileWidth = 14, TileHeight = 1, HorizontalAlignment = "left", Value = "TBD" });









                //SINGLE ENDED
                //
                //
                
               

                
                
                //mdl right


                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 700, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 4, TileWidth = 15, TileHeight = 16, Float = "left", Visibility = false, Group = "controlGroup:SingleEnded", Background = "white" });
                //top right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 701, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 700, TileWidth = 8, TileHeight = 2, Float = "left" });
                //sample container
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 702, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 700, TileWidth = 4, TileHeight = 3,  Float = "left" });

                TheFieldInfo SingleEnded1 = TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleEnded, 704, 2, 0, "Single Ended", "SingleEndedVal", new nmiCtrlSingleEnded() { ParentFld = 701,  });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 705, 0, 0, null, null, new nmiCtrlSmartLabel() { TileWidth = 4, TileHeight = 2, ParentFld = 702, HorizontalAlignment = "left", NoTE = true, FontSize = 20, Foreground = "#000000", Background = "transparent", Value = "Input control with label on the left." });
                //mdl right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 710, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 8, TileHeight = 6, ParentFld = 700, Background = "white" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 711, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 7, TileHeight = 6, ParentFld = 700, Background = "white" });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 712, 2, 0, "Background", "SingleEndedBackground", new nmiCtrlComboBox() {  ParentFld = 710,  Options = "transparent;yellow;orange;green;blue;red;brown;black" });
                SetProperty("SingleEndedBackground", "red").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    SingleEnded1.SetUXProperty(Guid.Empty, "Background=" + p.ToString());

                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 713, 2, 0, "Foreground", "SingleEndedForegroud", new nmiCtrlComboBox() { ParentFld = 711, Options = "transparent;yellow;orange;green;blue;red;brown;black" });
                SetProperty("SingleEndedForegroud", "red").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    SingleEnded1.SetUXProperty(Guid.Empty, "Foreground=" + p.ToString());

                });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 714, 2, 0, "Inner Class Name", "SingleEndedInnerClassName", new nmiCtrlComboBox() {ParentFld = 710, Options = "BackGreen;BackWhite;SELabel" });
                SetProperty("SingleEndedInnerClassName", "SELabel").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    SingleEnded1.SetUXProperty(Guid.Empty, "InnerClassName=" + p.ToString());

                });
      
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 715, 2, 0, "Format", "SingleEndedFormat", new nmiCtrlComboBox() { ParentFld = 711, Options = "{0} Dollar;Hello {0} World" });
                SetProperty("SingleEndedFormat", "{0} Dollar").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    SingleEnded1.SetUXProperty(Guid.Empty, "Format=" + p.ToString());

                });

                
                //TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 716, 2, 0, "Is Overlay", "SingleEndedIsOverlay", new nmiCtrlSingleCheck() {  ParentFld = 710 });
                //SetProperty("SingleEndedIsOverlay", false).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                //{
                //    SingleEnded1.SetUXProperty(Guid.Empty, "IsOverlay=" + p.ToString());

                //});

                //bottom right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 750, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 15, TileHeight = 6, ParentFld = 700 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 751, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 750, Background = "rgba(5,27,200, .05)" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 752, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 750, Background = "white" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 753, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 750, Background = "rgba(5,27,200, .05)" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 754, 0, 0, "Usage scenario:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 751, TileWidth = 14, TileHeight = 1, FldWidth = 12, HorizontalAlignment = "left", Value = "Use as an input field." });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 755, 0, 0, "The meaning of the Value:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 752, TileWidth = 14, TileHeight = 1, HorizontalAlignment = "left", Value = "Input value." });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 756, 0, 0, "Note:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 753, TileWidth = 14, TileHeight = 1, HorizontalAlignment = "left", Value = "TBD" });









                //SMART LABEL  Customizable output field
                //
                //
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 800, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 4, TileWidth = 15, TileHeight = 16, Float = "left", Visibility = false, Group = "controlGroup:SmartLabel", Background = "white" });
                //top right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 801, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 800, TileWidth = 8, TileHeight = 2, Float = "left" });
                //sample container
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 802, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 800, TileWidth = 4, TileHeight = 3, Float = "left" });

                TheFieldInfo SmartLabel1 = TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 804, 2, 0, "Smart Label", "SmartLabelVal", new nmiCtrlSmartLabel() {ParentFld = 801 , Value="sample"});
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 805, 0, 0, null, null, new nmiCtrlSmartLabel() { TileWidth = 4, TileHeight = 2, ParentFld = 802, HorizontalAlignment = "left", NoTE = true, FontSize = 20, Foreground = "#000000", Background = "transparent", Value = "Customizable output field." });

                //mdl right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 810, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 8, TileHeight = 6, ParentFld = 800, Background = "white" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 811, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 7, TileHeight = 6, ParentFld = 800, Background = "white" });


                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleEnded, 812, 2, 0, "Enter a text", "SmartLabelVal", new nmiCtrlSingleEnded() { ParentFld = 810 });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 813, 2, 0, "Background", "SmartLabelBackground", new nmiCtrlComboBox() { ParentFld = 810, Options = "transparent;yellow;orange;green;blue;red;brown;black" });
                SetProperty("SmartLabelBackground", "red").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    SmartLabel1.SetUXProperty(Guid.Empty, "Background=" + p.ToString());

                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 814, 2, 0, "Foreground", "SmartLabelForegroud", new nmiCtrlComboBox() { ParentFld = 811, Options = "transparent;yellow;orange;green;blue;red;brown;black" });
                SetProperty("SmartLabelForegroud", "white").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    SmartLabel1.SetUXProperty(Guid.Empty, "Foreground=" + p.ToString());

                });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 815, 2, 0, "Format", "SmartLabelFormat", new nmiCtrlComboBox() { ParentFld = 810, Options = "{0} $;I love {0};This {0} is bad" });
                SetProperty("SmartLabelFormat", "{0} $").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    SmartLabel1.SetUXProperty(Guid.Empty, "Format=" + p.ToString());

                });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 816, 2, 0, "Element", "SmartLabelElement", new nmiCtrlComboBox() { ParentFld = 811, Options = "h1;p" });
                SetProperty("SmartLabelElement", "p").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    SmartLabel1.SetUXProperty(Guid.Empty, "Element=" + p.ToString());

                });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 817, 2, 0, "Is Read Only", "SmartLabelIsReadOnly", new nmiCtrlSingleCheck() { ParentFld = 810});
                SetProperty("SmartLabelIsReadOnly", false).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    SmartLabel1.SetUXProperty(Guid.Empty, "IsReadOnly=" + p.ToString());

                });

                //bottom right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 850, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 15, TileHeight = 6, ParentFld = 800 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 851, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 850, Background = "rgba(5,27,200, .05)" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 852, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 850, Background = "white" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 853, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 850, Background = "rgba(5,27,200, .05)" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 854, 0, 0, "Usage scenario:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 851, TileWidth = 14, TileHeight = 1, FldWidth = 12, HorizontalAlignment = "left", Value = "Use to show any read-only value in Forms." });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 855, 0, 0, "The meaning of the Value:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 852, TileWidth = 14, TileHeight = 1, HorizontalAlignment = "left", Value = "Value to be Output." });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 856, 0, 0, "Note:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 853, TileWidth = 14, TileHeight = 1, HorizontalAlignment = "left", Value = "In Tables the SmartLabel is used to show any field as a standard output value until its touched/clicked – then the smartlabel “morphs” into the field it was defined in the Table.In Forms the read - only Output of value is used." });

                





                //TILE BUTTON
                //
                //
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 900, 0, 0, null, null, new nmiCtrlTileGroup() {ParentFld=4, TileWidth = 15, TileHeight = 16, Float = "left", Visibility = false, Group="controlGroup:TileButton" });
                //top right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 901, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 8, TileHeight = 2, ParentFld = 900 , Background="pink"}); // bug - button does not show if background here is not set
                //sample container
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 902, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 4, TileHeight = 3,  ParentFld = 900 });

                int clickCnt = 0;
                TheFieldInfo TileButton1 = TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileButton,  904, 2, 0, "Tile Button", "TileButtonVal", new nmiCtrlTileButton() { ParentFld = 901 , Title="Click me"});
                TileButton1.RegisterUXEvent(MyBaseThing, eUXEvents.OnClick,"clicked", (sender, para) => 
                {
                    TileButton1.SetUXProperty(Guid.Empty, string.Format("Title={0} Clicked", clickCnt++));
                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 905, 0, 0, null, null, new nmiCtrlSmartLabel() { TileWidth = 4, TileHeight = 2, ParentFld = 902, HorizontalAlignment = "left", NoTE = true, FontSize = 20, Foreground = "#000000", Background = "transparent", Value = "Customizable button." });

                //mdl right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 910, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 8, TileHeight = 7, ParentFld = 900, Background = "white" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 911, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 7, TileHeight = 7, ParentFld = 900, Background = "white" });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 912, 2, 0, "Background", "TileButtonBackground", new nmiCtrlComboBox() { ParentFld = 910, Options = "transparent;yellow;orange;green;blue;red;brown;black" });
                SetProperty("TileButtonBackground", "red").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    TileButton1.SetUXProperty(Guid.Empty, "Background=" + p.ToString());

                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 913, 2, 0, "Foreground", "TileButtonForegroud", new nmiCtrlComboBox() { ParentFld = 911,  Options = "transparent;yellow;orange;green;blue;red;brown;black" });
                SetProperty("TileButtonForegroud", "yellow").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    TileButton1.SetUXProperty(Guid.Empty, "Foreground=" + p.ToString());

                });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 914, 2, 0, "Thumbnail", "TileButtonThumbnail", new nmiCtrlComboBox() { ParentFld = 910, Options = "images/pin.png;images/iconTopLogo.png;images/upnpicon.png" });
                SetProperty("TileButtonThumbnail", "").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    TileButton1.SetUXProperty(Guid.Empty, "Thumbnail=" + p.ToString());

                });

                //TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 915, 2, 0, "Format", "TileButtonFormat", new nmiCtrlComboBox() {  ParentFld = 911, Options = "Format_1;Format_2" });
                //SetProperty("TileButtonFormat", "").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                //{
                //    TileButton1.SetUXProperty(Guid.Empty, "Format=" + p.ToString());

                //});
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleEnded, 916, 2, 0, "Title", "TileButtonTitle", new nmiCtrlSingleEnded() { ParentFld = 910 });
                SetProperty("TileButtonTitle", "").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    TileButton1.SetUXProperty(Guid.Empty, "Title=" + p.ToString());

                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 917, 2, 0, "On Click", "TileButtonOnClick", new nmiCtrlComboBox() { ParentFld = 911 });
                SetProperty("TileButtonOnClick", "").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    TileButton1.SetUXProperty(Guid.Empty, "OnClick=" + p.ToString());

                });

                //TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 918, 2, 0, "On Tile Down", "TileButtonOnTileDown", new nmiCtrlComboBox() { ParentFld = 910 });
                //SetProperty("TileButtonOnTileDown", "").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                //{
                //    TileButton1.SetUXProperty(Guid.Empty, "OnTileDown=" + p.ToString());

                //});
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleEnded, 919, 2, 0, "HTML Url", "TileButtonHTMLUrl", new nmiCtrlComboBox() { ParentFld = 911 });
                SetProperty("TileButtonHTMLUrl", "").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    TileButton1.SetUXProperty(Guid.Empty, "HTMLUrl=" + p.ToString());

                });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleEnded, 920, 2, 0, "HTML", "TileButtonHTML", new nmiCtrlComboBox() { ParentFld = 910 });
                SetProperty("TileButtonHTML", "").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    TileButton1.SetUXProperty(Guid.Empty, "HTML=" + p.ToString());

                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 921, 2, 0, "Are You Sure", "TileButtonAreYouSure", new nmiCtrlComboBox() { ParentFld = 911, Options="Are you sure;No:CDE_NOP" });
                SetProperty("TileButtonAreYouSure", "").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    TileButton1.SetUXProperty(Guid.Empty, "AreYouSure=" + p.ToString());

                });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 922, 2, 0, "Group ID", "TileButtonGroupID", new nmiCtrlComboBox() { ParentFld = 910, Options = "group_AL:910;group_B:911" });
                SetProperty("TileButtonGroupID", "").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    TileButton1.SetUXProperty(Guid.Empty, "GroupID=" + p.ToString());

                });


                //TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 923, 2, 0, "TabIndex", "TileButtonTabIndex", new nmiCtrlComboBox() { ParentFld = 911, Options = "1;2;3" });
                //SetProperty("TileButtonTabIndex", 1).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                //{
                //    TileButton1.SetUXProperty(Guid.Empty, "TabIndex=" + p.ToString());

                //});

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 924, 2, 0, "Enable Tap", "TileButtonEnableTap", new nmiCtrlSingleCheck() { ParentFld = 910 });
                SetProperty("TileButtonEnableTap", false).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    TileButton1.SetUXProperty(Guid.Empty, "EnableTap=" + p.ToString());
                });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 925, 2, 0, "Format", "TileButtonFormat", new nmiCtrlComboBox() { ParentFld = 910, Options = "{0} $;I love {0};This {0} is bad" });
                SetProperty("TileButtonFormat", "").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    TileButton1.SetUXProperty(Guid.Empty, "Format=" + p.ToString());

                });


                //bottom right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 950, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 15, TileHeight = 6, ParentFld = 900 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 951, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 950, Background = "rgba(5,27,200, .05)" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 952, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 950, Background = "white" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 953, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 950, Background = "rgba(5,27,200, .05)" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 954, 0, 0, "Usage scenario:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 951, TileWidth = 14, TileHeight = 1, FldWidth = 12, HorizontalAlignment = "left", Value = "Used as a push button in Forms and Tables.." });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 955, 0, 0, "The meaning of the Value:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 952, TileWidth = 14, TileHeight = 1, HorizontalAlignment = "left", Value = " Can be used as a dynamic Label of the Button – if the property Title AND Value exists, the Value will be used." });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 956, 0, 0, "Note:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 953, TileWidth = 14, TileHeight = 1, HorizontalAlignment = "left", Value = "TBD" });






                ////TILE GROUP
                ////
                ////
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1000, 0, 0, null, null, new nmiCtrlTileGroup() {ParentFld = 4, TileWidth = 15, TileHeight = 16, Float = "left", Visibility = false, Group = "controlGroup:TileGroup" });
                //top right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1001, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 8, TileHeight = 2, ParentFld = 1000});
                //sample container
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1002, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 4, TileHeight = 3, ParentFld = 1000 });

                TheFieldInfo TileGroup1 = TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1004, 2, 0, "Tile Group", "TileGroupVal", new nmiCtrlTileGroup() { TileWidth = 4, TileHeight = 2, ParentFld = 1001 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 1005, 0, 0, null, null, new nmiCtrlSmartLabel() { TileWidth = 4, TileHeight = 2, ParentFld = 1002, HorizontalAlignment = "left", NoTE = true, FontSize = 20, Foreground = "#000000", Background = "transparent", Value = "Allows to group controls together in a Group of elements by setting the ParentFld to the FldOrder of the TileGroup." });



                    //mdl right
                    TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1010, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 8, TileHeight = 7, ParentFld = 1000, Background = "white" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1011, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 7, TileHeight = 7, ParentFld = 1000, Background = "white" });


                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 1012, 2, 0, "Background", "TileGroupBackground", new nmiCtrlComboBox() { ParentFld = 1010, Options = "transparent;yellow;orange;green;blue;red;brown;black" });
                SetProperty("TileGroupBackground", "red").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    TileGroup1.SetUXProperty(Guid.Empty, "Background=" + p.ToString());

                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox,1013, 2, 0, "Caption", "TileGroupCaption", new nmiCtrlComboBox() { ParentFld = 1011, Options = "Title_One;Title_Two" });
                SetProperty("TileGroupCaption", "Caption").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    TileGroup1.SetUXProperty(Guid.Empty, "Caption=" + p.ToString());

                });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 1014, 2, 0, "Label ClassName", "TileGroupLabelClassName", new nmiCtrlComboBox() { ParentFld = 1010, Options = "Caption_Class_Name_One;Caption_Class_Name_Two" });
                SetProperty("TileGroupLabelClassName", "Caption_Class_Name").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    TileGroup1.SetUXProperty(Guid.Empty, "LabelClassName=" + p.ToString());

                });


                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 1015, 2, 0, "Label Foreground", "TileGroupLabelForeground", new nmiCtrlComboBox() { ParentFld = 1011, Options = "transparent;yellow;orange;green;blue;red;brown;black" });
                SetProperty("TileGroupLabelForeground", "blue").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    TileGroup1.SetUXProperty(Guid.Empty, "LabelForeground=" + p.ToString());

                });



                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 1016, 2, 0, "Overflow", "TileGroupOverflow", new nmiCtrlComboBox() { ParentFld = 1010, Options = "One;Two" });
                SetProperty("TileGroupOverflow", "").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    TileGroup1.SetUXProperty(Guid.Empty, "Overflow=" + p.ToString());

                });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 1017, 2, 0, "Format", "TileGroupFormat", new nmiCtrlComboBox() { ParentFld = 1011, Options = "One;Two" });
                SetProperty("TileGroupFormat", "One").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    TileGroup1.SetUXProperty(Guid.Empty, "Format=" + p.ToString());

                });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 1018, 2, 0, "IsHScrollable", "TileGroupIsHScrollable", new nmiCtrlSingleCheck() { ParentFld = 1010 });
                SetProperty("TileGroupIsHScrollable", false).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    TileGroup1.SetUXProperty(Guid.Empty, "IsHScrollable=" + p.ToString());

                });


                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 1019, 2, 0, "IsVScrollable", "TileGroupIsVScrollable", new nmiCtrlSingleCheck() { ParentFld = 1011});
                SetProperty("TileGroupIsVScrollable", false).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    TileGroup1.SetUXProperty(Guid.Empty, "IsVScrollable=" + p.ToString());

                });


                //bottom right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1050, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 15, TileHeight = 6, ParentFld = 1000 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1051, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 1050, Background = "rgba(5,27,200, .05)" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1052, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 1050, Background = "white" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1053, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 1050, Background = "rgba(5,27,200, .05)" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 1054, 0, 0, "Usage scenario:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 1051, TileWidth = 14, TileHeight = 1, FldWidth = 12, HorizontalAlignment = "left", Value = "Used to Group other controls together." });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 1055, 0, 0, "The meaning of the Value:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 1052, TileWidth = 14, TileHeight = 1, HorizontalAlignment = "left", Value = " Can be used as a dynamic Title of the Group – if the property Title AND Value exists, the Value will be used." });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 1056, 0, 0, "Note:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 1053, TileWidth = 14, TileHeight = 1, HorizontalAlignment = "left", Value = "If TW or TH the hight or width is meassured by the controls inside. Its recommended to set the TileWidth and TileHeight of the Button to set the size. " });






                ////TOUCH DRAW  
                ////
                ////
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1100, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 4, TileWidth = 15, TileHeight = 16, Float = "left", Visibility = false, Group = "controlGroup:TouchDraw" });
                //top right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1101, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 8, TileHeight = 2, ParentFld = 1100 });
                //sample container
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1102, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 4, TileHeight = 3, MaxTileWidth = 6, ParentFld = 1100 });
                TheFieldInfo TouchDraw1 = TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TouchDraw, 1104, 2, 0, "Touch Draw", "TouchDrawVal", new nmiCtrlTouchDraw() { ParentFld = 1101, TileHeight=2 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 1105, 0, 0, null, null, new nmiCtrlSmartLabel() { TileWidth = 4, TileHeight = 2, ParentFld = 1102, HorizontalAlignment = "left", NoTE = true, FontSize = 20, Foreground = "#000000", Background = "transparent", Value = "An area the user can draw into" });
                //mdl right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1110, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 8, TileHeight = 7, ParentFld = 1100, Background = "white" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1111, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 7, TileHeight = 7, ParentFld = 1100, Background = "white" });


                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 1112, 2, 0, "Is Synced", "TouchDrawIsSynced", new nmiCtrlSingleCheck() {ParentFld = 1110});
                SetProperty("TouchDrawIsSynced", false).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    TouchDraw1.SetUXProperty(Guid.Empty, "IsSynced=" + p.ToString());

                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 1113, 2, 0, "Show Save", "TouchDrawShowSave", new nmiCtrlSingleCheck() { ParentFld = 1111 });
                SetProperty("TouchDrawShowSave", false).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    TouchDraw1.SetUXProperty(Guid.Empty, "ShowSave=" + p.ToString());

                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 1114, 2, 0, "Show Play", "TouchDrawShowPlay", new nmiCtrlSingleCheck() { ParentFld = 1110 });
                SetProperty("TouchDrawShowPlay", false).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    TouchDraw1.SetUXProperty(Guid.Empty, "ShowPlay=" + p.ToString());

                });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 1115, 2, 0, "Background", "TDBackground", new nmiCtrlComboBox() { ParentFld = 1110, Options = "transparent;yellow;orange;green;blue;red;brown;black" });
                SetProperty("TDBackground", "").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    TouchDraw1.SetUXProperty(Guid.Empty, "Background=" + p.ToString());
                });

                //TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 1115, 2, 0, "Is Overlay", "TouchDrawIsOverlay", new nmiCtrlSingleCheck() { ParentFld = 1111 });
                //SetProperty("TouchDrawIsOverlay", false).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                //{
                //    TouchDraw1.SetUXProperty(Guid.Empty, "IsOverlay=" + p.ToString());

                //});


                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 1116, 2, 0, "Show Colors", "TouchDrawShowColors", new nmiCtrlSingleCheck() { ParentFld = 1110 });
                SetProperty("TouchDrawShowColors", false).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    TouchDraw1.SetUXProperty(Guid.Empty, "ShowColors=" + p.ToString());

                });

                //bottom right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1150, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 15, TileHeight = 6, ParentFld = 1100 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1151, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 1150, Background = "rgba(5,27,200, .05)" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1152, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 1150, Background = "white" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1153, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 1150, Background = "rgba(5,27,200, .05)" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 1154, 0, 0, "Usage scenario:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 1151, TileWidth = 14, TileHeight = 1, FldWidth = 12, HorizontalAlignment = "left", Value = "User to create signature fields, annotation fields, or other pen/touch recording purposes."});
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 1155, 0, 0, "The meaning of the Value:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 1152, TileWidth = 14, TileHeight = 1, HorizontalAlignment = "left", Value = " The strokes the user drew to the control." });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 1156, 0, 0, "Note:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 1153, TileWidth = 14, TileHeight = 1, HorizontalAlignment = "left", Value = "TBD" });








                //SLIDER 
                //
                //
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1200, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 4, TileWidth = 15, TileHeight = 16, Float = "left", Visibility = false,Group= "controlGroup:EndlessSlider" });
                //top right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1201, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 8, TileHeight = 2, ParentFld = 1200 , Background="pink"});
                //sample container
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1202, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 4, TileHeight = 3, ParentFld = 1200 });
                TheFieldInfo EndlessSlider1 = TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Slider, 1204, 2, 0, "Endless Slider", "EndlessSliderVal", new nmiCtrlEndlessSlider() {ParentFld = 1201 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 1205, 0, 0, null, null, new nmiCtrlSmartLabel() { TileWidth = 4, TileHeight = 2, ParentFld = 1202, HorizontalAlignment = "left", NoTE = true, FontSize = 20, Foreground = "#000000", Background = "transparent", Value = "A set of lines aligned to make an endless slider." });
                //mdl right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1210, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 8, TileHeight = 7, ParentFld = 1200, Background = "white" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1211, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 7, TileHeight = 7, ParentFld = 1200, Background = "white" });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 1212, 2, 0, "Background", "EndlessSliderBackground", new nmiCtrlComboBox() { ParentFld = 1210, Options = "transparent;yellow;orange;green;blue;red;brown;black" });
                SetProperty("EndlessSliderBackground", "").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    EndlessSlider1.SetUXProperty(Guid.Empty, "Background=" + p.ToString());
                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 1213, 2, 0, "Foreground", "EndlessSliderForegroud", new nmiCtrlComboBox() {ParentFld =1211, Options = "transparent;yellow;orange;green;blue;red;brown;black" });
                SetProperty("EndlessSliderForegroud", "blue").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    EndlessSlider1.SetUXProperty(Guid.Empty, "Foreground=" + p.ToString());
                });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 1214, 2, 0, "Max Value", "EndlessSliderMaxValue", new nmiCtrlComboBox() { ParentFld = 1210, Options = "100;200;300" });
                SetProperty("EndlessSliderMaxValue", 1000).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    EndlessSlider1.SetUXProperty(Guid.Empty, "MaxValue=" + p.ToString());
                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 1215, 2, 0, "Min Value", "EndlessSliderMinValue", new nmiCtrlComboBox() { ParentFld = 1211, Options = "0;1" });
                SetProperty("EndlessSliderMinValue", 0).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    EndlessSlider1.SetUXProperty(Guid.Empty, "MinValue=" + p.ToString());
                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 1216, 2, 0, "Main Background", "EndlessSliderMainBackground", new nmiCtrlComboBox() {  ParentFld = 1210, Options = "transparent;yellow;orange;green;blue;red;brown;black" });
                SetProperty("EndlessSliderMainBackground", "").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    EndlessSlider1.SetUXProperty(Guid.Empty, "MainBackground=" + p.ToString());
                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 1217, 2, 0, "Step Factor", "EndlessSliderStepFactor", new nmiCtrlComboBox() { ParentFld = 1211, Options = "1.0;2.0;3.0;4.0" });
                SetProperty("EndlessSliderStepFactor",4.0).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    EndlessSlider1.SetUXProperty(Guid.Empty, "StepFactor=" + p.ToString());
                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 1218, 2, 0, "Line Width", "EndlessSliderLineWidth", new nmiCtrlComboBox() { ParentFld = 1210, Options = "1;3;5;10" });
                SetProperty("EndlessSliderLineWidth",10).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    EndlessSlider1.SetUXProperty(Guid.Empty, "LineWidth=" + p.ToString());
                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 1219, 2, 0, "Line Gap", "EndlessSliderLineGap", new nmiCtrlComboBox() { ParentFld = 1211, Options = "15;20;30;50;80" });
                SetProperty("EndlessSliderLineGap", 20).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    EndlessSlider1.SetUXProperty(Guid.Empty, "LineGap=" + p.ToString());
                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 1220, 2, 0, "Allow Rollover", "EndlessSliderAllowRollover", new nmiCtrlSingleCheck() {ParentFld =1210 });
                SetProperty("EndlessSliderAllowRollover", false).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    EndlessSlider1.SetUXProperty(Guid.Empty, "AllowRollover=" + p.ToString());
                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 1221, 2, 0, "Is Vertical", "EndlessSliderIsVertical", new nmiCtrlSingleCheck() { ParentFld = 1211 });
                SetProperty("EndlessSliderIsVertical", false).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    EndlessSlider1.SetUXProperty(Guid.Empty, "IsVertical=" + p.ToString());
                });

                //bottom right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1250, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 15, TileHeight = 6, ParentFld = 1200 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1251, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 1250, Background = "rgba(5,27,200, .05)" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1252, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 1250, Background = "white" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1253, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 1250, Background = "rgba(5,27,200, .05)" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 1254, 0, 0, "Usage scenario:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 1251, TileWidth = 14, TileHeight = 1, FldWidth = 12, HorizontalAlignment = "left", Value = "An endless slider is a way of changing a value up or down by moving the slider up or down." });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 1255, 0, 0, "The meaning of the Value:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 1252, TileWidth = 14, TileHeight = 1, HorizontalAlignment = "left", Value = " Numeric Value of the slider." });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 1256, 0, 0, "Note:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 1253, TileWidth = 14, TileHeight = 1, HorizontalAlignment = "left", Value = "By setting MaxValue and MinValue the sliders end points can be defined. If “AllowRollover” is true, the slider will go back to MinValue once MaxValue is reached and vs." });





                //VIDEO VIEWER 
                //
                //

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1300, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 4, TileWidth = 15, TileHeight = 16, Float = "left", Visibility = false, Group = "controlGroup:VideoViewer" });
                //top right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1301, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 8, TileHeight = 2, ParentFld = 1300 });
                //sample container
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1302, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 4, TileHeight = 3, ParentFld = 1300 });

                TheFieldInfo VideoViewer1 = TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.VideoViewer, 1304, 2, 0, "Video Viewer", "VideoViewerVal", new nmiCtrlVideoViewer() { ParentFld = 1301 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 1305, 0, 0, null, null, new nmiCtrlSmartLabel() { TileWidth = 4, TileHeight = 2, ParentFld = 1302, HorizontalAlignment = "left", NoTE = true, FontSize = 20, Foreground = "#000000", Background = "transparent", Value = "This control allows to show videos in a form." });
                //mdl right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1310, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 8, TileHeight = 7, ParentFld = 1300, Background = "white" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1311, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 7, TileHeight = 7, ParentFld = 1300, Background = "white" });

             
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 1312, 2, 0, "Background", "VideoViewerBackground", new nmiCtrlComboBox() { ParentFld = 1310, Options = "transparent;yellow;orange;green;blue;red;brown;black" });
                SetProperty("VideoViewerBackground", "").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    VideoViewer1.SetUXProperty(Guid.Empty, "Background=" + p.ToString());

                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 1313, 2, 0, "Main Background", "VidoViewerMainBackgroud", new nmiCtrlComboBox() {ParentFld = 1311, Options = "transparent;yellow;orange;green;blue;red;brown;black" });
                SetProperty("VidoViewerMainBackgroud", "").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    VideoViewer1.SetUXProperty(Guid.Empty, "MainBackground=" + p.ToString());

                });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 1314, 2, 0, "Source", "VdeoViewerSource", new nmiCtrlComboBox() { ParentFld = 1310, Options = "[{ \"N\":\"Ted 1\", \"V\":\"http://download.ted.com/talks/TristanHarris_2014X-480p.mp4?apikey=489b859150fc58263f17110eeb44ed5fba4a3b22\"}, { \"N\":\"Ted 2\",\"V\":\"http://download.ted.com/talks/StephenWilkes_2016-480p.mp4?apikey=489b859150fc58263f17110eeb44ed5fba4a3b22\"} ]" });
                SetProperty("VdeoViewerSource", "").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    VideoViewer1.SetUXProperty(Guid.Empty, "Source=" + p.ToString());

                });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 1315, 2, 0, "Show Cam", "VideoViewerShowCam", new nmiCtrlSingleCheck() { ParentFld = 1311});
                SetProperty("VideoViewerShowCam", false).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    VideoViewer1.SetUXProperty(Guid.Empty, "ShowCam=" + p.ToString());

                });


                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 1316, 2, 0, "Show Controls", "VideoViewerShowControls", new nmiCtrlSingleCheck() {ParentFld = 1310 });
                SetProperty("VideoViewerShowControls", false).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    VideoViewer1.SetUXProperty(Guid.Empty, "ShowControls=" + p.ToString());

                });


                //bottom right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1350, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 15, TileHeight = 6, ParentFld = 1300 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1351, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 1350, Background = "rgba(5,27,200, .05)" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1352, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 1350, Background = "white" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1353, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 1350, Background = "rgba(5,27,200, .05)" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 1354, 0, 0, "Usage scenario:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 1351, TileWidth = 14, TileHeight = 1, FldWidth = 12, HorizontalAlignment = "left", Value = "Show videos in forms." });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 1355, 0, 0, "The meaning of the Value:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 1352, TileWidth = 14, TileHeight = 1, HorizontalAlignment = "left", Value = "The Value property is the link to the Video." });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 1356, 0, 0, "Note:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 1353, TileWidth = 14, TileHeight = 1, HorizontalAlignment = "left", Value = "TBD" });







           
                //PICTURE
                //
                //

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1400, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 4, TileWidth = 15, TileHeight = 16, Float = "left", Visibility = false , Group = "controlGroup:Picture"});
                //top right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1401, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 8, TileHeight = 2, ParentFld = 1400 });
                //sample container
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1402, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 4, TileHeight = 3, ParentFld = 1400 });

                TheFieldInfo ZoomImage1 = TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Picture, 1404, 2, 0, "Picture", "PictureVal", new nmiCtrlPicture() { ParentFld = 1401 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 1405, 0, 0, null, null, new nmiCtrlSmartLabel() { TileWidth = 4, TileHeight = 2, ParentFld = 1402, HorizontalAlignment = "left", NoTE = true, FontSize = 20, Foreground = "#000000", Background = "transparent", Value = "This control allows to display an image, and provides zoom in and out functionality. " });

                //mdl right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1410, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 8, TileHeight = 7, ParentFld = 1400, Background = "white" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1411, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 7, TileHeight = 7, ParentFld = 1400, Background = "white" });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 1412, 2, 0, "Background", "PictureBackground", new nmiCtrlComboBox() { ParentFld = 1410, Options = "transparent;yellow;orange;green;blue;red;brown;black" });
                SetProperty("PictureBackground", "").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    ZoomImage1.SetUXProperty(Guid.Empty, "Background=" + p.ToString());
                });
               TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 1413, 2, 0, "Source", "PictureSource", new nmiCtrlComboBox() { ParentFld = 1411, Options = "images/pin.png;images/iconTopLogo.png;images/upnpicon.png;images/toplogo-150.png" });
                SetProperty("PictureSource", "").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    ZoomImage1.SetUXProperty(Guid.Empty, "Source=" + p.ToString());
                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 1414, 2, 0, "Name", "PictureName", new nmiCtrlComboBox() { ParentFld = 1410, Options = "My Image;Another name" });
                SetProperty("PictureName", "NameIt").RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    ZoomImage1.SetUXProperty(Guid.Empty, "Name=" + p.ToString());
                });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 1415, 2, 0, "Zoom Level", "PictureZoomLevel", new nmiCtrlComboBox() { ParentFld = 1411, Options = "1;2;3;4" });
                SetProperty("PictureZoomLevel", 2).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    ZoomImage1.SetUXProperty(Guid.Empty, "ZoomLevel=" + p.ToString());
                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 1416, 2, 0, "Width", "PictureWidth", new nmiCtrlComboBox() { ParentFld = 1410, Options = "10;20;30;50;80;100" });
                SetProperty("PictureWidth", 110).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    ZoomImage1.SetUXProperty(Guid.Empty, "Width=" + p.ToString());
                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 1417, 2, 0, "Image Opacity", "PictureOpacity", new nmiCtrlComboBox() { ParentFld = 1411, Options = "0.10;0.20;0.30;0.50;0.80;1.0" });
                SetProperty("PictureOpacity", 110).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    ZoomImage1.SetUXProperty(Guid.Empty, "ImageOpacity=" + p.ToString());
                });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 1418, 2, 0, "Height", "PictureHeight", new nmiCtrlComboBox() { ParentFld = 1410, Options = "10;100;150;200" });
                SetProperty("PictureHeight", 10).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    ZoomImage1.SetUXProperty(Guid.Empty, "Height=" + p.ToString());
                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 1419, 2, 0, "Last Seq No", "PictureLastSeqNo", new nmiCtrlComboBox() { ParentFld = 1411, Options = "10;100;150;200" });
                SetProperty("PictureLastSeqNo", 10).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    ZoomImage1.SetUXProperty(Guid.Empty, "LastSeqNo=" + p.ToString());
                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 1420, 2, 0, "Full Width", "PictureFullWidth", new nmiCtrlComboBox() { ParentFld = 1410, Options = "10;100;150;200" });
                SetProperty("PictureFullWidth", 10).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    ZoomImage1.SetUXProperty(Guid.Empty, "FullWidth=" + p.ToString());
                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 1421, 2, 0, "Full Height", "PictureFullHeight", new nmiCtrlComboBox() { ParentFld = 1411, Options = "10;100;150;200" });
                SetProperty("PictureFullHeight", 10).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    ZoomImage1.SetUXProperty(Guid.Empty, "FullHeight=" + p.ToString());
                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 1422, 2, 0, "Enable Zoom", "PictureEnableZoom", new nmiCtrlSingleCheck() { ParentFld = 1410 });
                SetProperty("PictureEnableZoom", false).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    ZoomImage1.SetUXProperty(Guid.Empty, "EnableZoom=" + p.ToString());
                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 1423, 2, 0, "Start Sequence", "PictureStartSequence", new nmiCtrlSingleCheck() { ParentFld = 1411});
                SetProperty("PictureStartSequence", false).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    ZoomImage1.SetUXProperty(Guid.Empty, "StartSequence=" + p.ToString());
                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 1424, 2, 0, "Do Loop", "PictureDoLoop", new nmiCtrlSingleCheck() { ParentFld = 1410 });
                SetProperty("PictureDoLoop", false).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    ZoomImage1.SetUXProperty(Guid.Empty, "DoLoop=" + p.ToString());
                });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 1425, 2, 0, "Auto Adjust", "PictureAutoAdjust", new nmiCtrlSingleCheck() { ParentFld = 1411 });
                SetProperty("PictureAutoAdjust", false).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
                {
                    ZoomImage1.SetUXProperty(Guid.Empty, "AutoAdjust=" + p.ToString());
                });

                //bottom right
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1450, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 15, TileHeight = 10, ParentFld = 1400 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1451, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 1450, Background = "rgba(5,27,200, .05)" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1452, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 1450, Background = "white" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 1453, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 14, TileHeight = 2, ParentFld = 1450, Background = "rgba(5,27,200, .05)" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 1454, 0, 0, "Usage scenario:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 1451, TileWidth = 14, TileHeight = 1, FldWidth = 12, HorizontalAlignment = "left", Value = "TBD" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 1455, 0, 0, "The meaning of the Value:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 1452, TileWidth = 14, TileHeight = 1, HorizontalAlignment = "left", Value = "TBD" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 1456, 0, 0, "Note:", null, new nmiCtrlSmartLabel() { Foreground = "black", Background = "transparent", FontSize = 28, ParentFld = 1453, TileWidth = 14, TileHeight = 1, HorizontalAlignment = "left", Value = "TBD" });



                TheNMIEngine.AddAboutButton(MyBaseThing, true);
                TheNMIEngine.AddSmartPage(MyBaseThing, MyBaseEngine, "/SAMPLE", "SamplePage", true, false, true);       //Allows quick access to this form http://<MyStationUrl>:<MyStationPort>/SAMPLE
                TheNMIEngine.RegisterEngine(MyBaseEngine);      //Registers this engine and its "SmartPage" with the System

            mIsUXInitialized = true;
            return true;
        }





        void sinkChange(ICDEThing sender, object para)
        {
            TheProcessMessage pmg = para as TheProcessMessage;

            HeaderName.SetUXProperty(Guid.Empty, "Value="+ pmg.Message.TXT.Split(':')[2]);
        }

        TheDashboardInfo mMyDashboard;

        //TODO: Step 4: Write your Business Logic

        #region Message Handling
        /// <summary>
        /// Handles Messages sent from a host sub-engine to its clients
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="pMessage"></param>
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
