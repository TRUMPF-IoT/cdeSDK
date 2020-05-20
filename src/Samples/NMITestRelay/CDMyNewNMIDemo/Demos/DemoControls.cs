// SPDX-FileCopyrightText: Copyright(c) 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
// SPDX-License-Identifier: MPL-2.0

using nsCDEngine.Engines;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.ThingService;
using System;

namespace CDMyNewNMIDemo.Demos
{
    class DemoControls : TheDemoBase
    {
        public DemoControls(TheThing tBaseThing, ICDEPlugin pPluginBase) : base(tBaseThing, pPluginBase)
        {
            MyBaseThing.DeviceType = eTheDemoScreensTypes.DemoControls;
            MyBaseThing.FriendlyName = MyBaseThing.DeviceType;

        }

        public override void DoCreateUX()
        {
            TheFormInfo tMyForm2 = new TheFormInfo(MyBaseThing) { FormTitle = "Welcome the basic NMI-Controls Demo Page", DefaultView = eDefaultView.Form, TileWidth = 12 };
            TheNMIEngine.AddFormToThingUX(MyBaseThing, tMyForm2, "CMyForm", "Control Demo Form", 3, 3, 0, "..Demos", null, new nmiDashboardTile { TileWidth = 4, TileHeight = 2, Thumbnail = "Images/iconToplogo.png;0.1", FriendlyName = "All NMI Controls Demo" });

            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 1, 2, 0, "Single Ended", null, new nmiCtrlTileButton { ParentFld = 200, OnClick = "GRP:Cate:1", TileWidth = 2, TileHeight = 1, TileFactorY = 2, NoTE = true, ClassName = "cdeTransitButton" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 2, 2, 0, "Canvas", null, new nmiCtrlTileButton { ParentFld = 200, OnClick = "GRP:Cate:2", TileWidth = 2, TileHeight = 1, TileFactorY = 2, NoTE = true, ClassName = "cdeTransitButton" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 3, 2, 0, "Base Fields", null, new nmiCtrlTileButton { ParentFld = 200, OnClick = "GRP:Cate:3", TileWidth = 2, TileHeight = 1, TileFactorY = 2, NoTE = true, ClassName = "cdeTransitButton" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 4, 2, 0, "Combo Boxes", null, new nmiCtrlTileButton { ParentFld = 200, OnClick = "GRP:Cate:4", TileWidth = 2, TileHeight = 1, TileFactorY = 2, NoTE = true, ClassName = "cdeTransitButton" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 5, 2, 0, "Compounds", null, new nmiCtrlTileButton { ParentFld = 200, OnClick = "GRP:Cate:5", TileWidth = 2, TileHeight = 1, TileFactorY = 2, NoTE = true, ClassName = "cdeTransitButton" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 6, 2, 0, "Gauges", null, new nmiCtrlTileButton { ParentFld = 200, OnClick = "GRP:Cate:6", TileWidth = 2, TileHeight = 1, TileFactorY = 2, NoTE = true, ClassName = "cdeTransitButton" });


            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.CollapsibleGroup, 10, 2, 0, "Single Ended Based...", null, new nmiCtrlCollapsibleGroup { DoClose = true, TileWidth = 12, IsSmall = true, Group = "Cate:1" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.Currency, 15, 2, 0, "My Currency", "Value1", new TheNMIBaseControl { ParentFld = 10 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.Password, 20, 2, 0, "My Password", "ValuePW", new TheNMIBaseControl { ParentFld = 10 }); // CODE REVIEW: This is not encrypted!
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.SingleEnded, 30, 2, 0, "My Single Ended", "ValueStr", new TheNMIBaseControl { ParentFld = 10 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.Number, 40, 2, 0, "My Number", "Value1", new TheNMIBaseControl { ParentFld = 10 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.URL, 60, 2, 0, "My Url", "ValueStr", new TheNMIBaseControl { ParentFld = 10 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.IPAddress, 70, 2, 0, "My IPAddress", "ValueIP", new TheNMIBaseControl { ParentFld = 10 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TextArea, 80, 2, 0, "My TextArea", "ValueShape", new TheNMIBaseControl { ParentFld = 10 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.eMail, 90, 2, 0, "My eMail", "ValueEM", new TheNMIBaseControl { ParentFld = 10 });

            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.CollapsibleGroup, 100, 2, 0, "Shape/DrawCanvas Based...", null, new nmiCtrlCollapsibleGroup { DoClose = true, TileWidth = 12, IsSmall = true, Group = "Cate:2" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.BarChart, 110, 2, 4, "My Bar", "Value1", new nmiCtrlBarChart { ParentFld = 100, Foreground = "$ValueStr" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.Slider, 130, 2, 0, "My Slide", "Value1", new TheNMIBaseControl { ParentFld = 100 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TouchDraw, 140, 2, 0, "My Touchdraw", "ValueDraw", new TheNMIBaseControl { ParentFld = 100, TileHeight = 2 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.Shape, 150, 2, 0, "My Shape", "ValueShape", new TheNMIBaseControl { ParentFld = 100 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.StatusLight, 160, 2, 0, "My Status Lite", "Value1", new TheNMIBaseControl { ParentFld = 100 });


            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.CollapsibleGroup, 200, 2, 0, "Other Base Fields...", null, new nmiCtrlCollapsibleGroup { DoClose = true, TileWidth = 12, IsSmall = true, Group = "Cate:3" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.CheckField, 210, 2, 0, "My CheckField", "Value1", new nmiCtrlCheckField { ParentFld = 200, FontSize = 24, FldWidth = 1, Bits = 8, TileFactorY = 2, Options = "Service Guest;IT Guest;OT User;Service User;IT User;OT Admin; Service Admin;IT Admin" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.DropUploader, 220, 2, 0, "My DropUploader", "ValueStr", new TheNMIBaseControl { ParentFld = 200 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.FacePlate, 230, 2, 0, "My FacePlate", "ValueStr", new TheNMIBaseControl { ParentFld = 200 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.ProgressBar, 240, 2, 0, "My Progress", "Value1", new TheNMIBaseControl { ParentFld = 200 });

            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.CollapsibleGroup, 245, 2, 0, "Video Viewer...", null, new nmiCtrlTileGroup { ParentFld = 200, TileWidth = 6 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.VideoViewer, 250, 2, 0, "My Video", "VideoUrl", new nmiCtrlVideoViewer { ParentFld = 245, TileHeight = 3, ShowControls = true });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.SingleEnded, 251, 2, 0, "My Video URL", "VideoUrl", new TheNMIBaseControl { ParentFld = 245 });

            //TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.MuTLock, 260, 2, 0, "My Mutlog", "ValueStr", new TheNMIBaseControl { ParentFld = 200 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.Picture, 270, 2, 0, "My Picture", "ValueImg", new nmiCtrlPicture { ParentFld = 200, EnableZoom = true, AutoAdjust = true });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 280, 2, 0, "My Tile Button", "ValueStr", new TheNMIBaseControl { ParentFld = 200 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.SingleCheck, 290, 2, 0, "My Check", true, "ValueBool", null, new TheNMIBaseControl { ParentFld = 200, Green = "pink" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.SmartLabel, 295, 2, 0, "My SmartLabel", true, "ValueStr", null, new nmiCtrlSmartLabel { ParentFld = 200, HorizontalAlignment = "right", VerticalAlignment = "bottom" });


            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.CollapsibleGroup, 300, 2, 0, "Combo Box Based...", null, new nmiCtrlCollapsibleGroup { DoClose = false, TileWidth = 12, IsSmall = true, Group = "Cate:4" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.DateTime, 310, 2, 0, "My DateTime", "ValueDate", new TheNMIBaseControl { NoTE = true, ParentFld = 300 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.DateTime, 320, 2, 0, "My Date", "ValueDate", new nmiCtrlDateTime() { ParentFld = 300, DateOnly = true });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.DateTime, 330, 2, 0, "My Calendar", "ValueDate", new nmiCtrlDateTime() { ParentFld = 300, UseCalendar = true });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.Month, 340, 2, 0, "My Month", "ValueStr", new TheNMIBaseControl { ParentFld = 300 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.Time, 350, 2, 0, "My Time", "ValueDate", new nmiCtrlDateTime { ParentFld = 300 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TimeSpan, 360, 2, 0, "My Time Span", "ValueDate", new TheNMIBaseControl { ParentFld = 300 });

            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.ComboBox, 370, 2, 0, "My ComboBox", "ValueImg", new nmiCtrlComboBox { ParentFld = 300, Options = "Images/iconTopLogo.png;Images/GlasButton.png;Images/UPNPICON.PNG;Images/iconLogout.png;Images/myimg.jpg;Images/c-labs.jpg" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.ComboOption, 380, 2, 0, "My ComboOption", "ValueImg2", new nmiCtrlComboBox { ParentFld = 300, Options = "Images/iconTopLogo.png;Images/GlasButton.png;Images/UPNPICON.PNG;Images/iconLogout.png;Images/myimg.jpg;Images/c-labs.jpg" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.Country, 390, 2, 0, "My Country", "ValueCStr", new TheNMIBaseControl { ParentFld = 300 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.YesNo, 400, 2, 0, "My YesNo", "ValueStr", new TheNMIBaseControl { ParentFld = 300 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TrueFalse, 410, 2, 0, "My TrueFalse", "ValueStr", new TheNMIBaseControl { ParentFld = 300 });

            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.CertPicker, 430, 2, 0, "Certificate Picker", "CertProp", new nmiCtrlCertPicker { ParentFld = 300 });

            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.ComboLookup, 440, 2, 0, "The Lookup Combo", "ThingGuid", new nmiCtrlComboLookup { ParentFld = 300, StorageTarget = "b510837f-3b75-4cf2-a900-d36c19113a13", GroupFld = "MyPropertyBag.DeviceType.Value", NameFld = "MyPropertyBag.FriendlyName.Value", ValueFld = "cdeMID" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.ThingPicker, 450, 2, 0, "New Ping Picker", "ThingGuid", new nmiCtrlThingPicker { ParentFld = 300, IncludeEngines = true, IncludeRemotes = true, Filter = "DeviceType=Ping" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.ThingPicker, 455, 2, 0, "New Thing Picker", "ThingGuid", new nmiCtrlThingPicker { ParentFld = 300 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.PropertyPicker, 480, 2, 0, "Multi Property Picker", "PropProp", new nmiCtrlPropertyPicker { ParentFld = 300, ThingFld = 470, AllowMultiSelect = true, Separator = ";" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.PropertyPicker, 485, 2, 0, "Single Property Picker", "PropProp", new nmiCtrlPropertyPicker { ParentFld = 300, ThingFld = 470, Separator = ";" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.SingleEnded, 470, 0, 0, "Thing ID", "ThingGuid", new TheNMIBaseControl { ParentFld = 300 });
            var tBut = TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 490, 2, 0, "Engage Mapper", null, new TheNMIBaseControl { ParentFld = 300 });
            tBut.RegisterUXEvent(MyBaseThing, eUXEvents.OnClick, "ENGAGE", (sender, para) =>
            {
                this.EngageMapper();
            });

            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.CollapsibleGroup, 500, 2, 0, "Compound Controls...", null, new nmiCtrlCollapsibleGroup { DoClose = true, IsSmall = true, TileWidth = 12, Group = "Cate:5" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.AboutButton, 510, 2, 0, "My About", "Value1", new TheNMIBaseControl { ParentFld = 500 });

            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.CollapsibleGroup, 600, 2, 0, "Gauges...", null, new nmiCtrlCollapsibleGroup { DoClose = true, TileWidth = 12, IsSmall = true, Group = "Cate:6" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.CircularGauge, 610, 2, 4, "My Gauge", "Value1", new nmiCtrlBarChart { ParentFld = 600, Foreground = "$ValueStr", TileHeight = 3 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.SmartGauge, 620, 2, 4, "My Bar", "Value1", new nmiCtrlBarChart { NoTE = true, ParentFld = 600, Foreground = "$ValueStr", TileHeight = 3 });
        }

        Guid mRealThingGuid = Guid.Empty;
        private void EngageMapper()
        {
            TheThingRegistry.UnmapPropertyMapper(mRealThingGuid);
            mRealThingGuid = TheThingRegistry.PropertyMapper(TheThing.GetSafePropertyGuid(MyBaseThing, "ValueGuid"), TheThing.GetSafePropertyString(MyBaseThing, "ValueProp"), MyBaseThing.cdeMID, "ValueStr", false);
            if (mRealThingGuid == Guid.Empty)
            {
                MyBaseThing.StatusLevel = 0;
                MyBaseThing.LastMessage = "Mapper not active";
            }
            else
            {
                MyBaseThing.LastMessage = "Mapper engaged";
                MyBaseThing.StatusLevel = 1;
            }
        }
    }
}
