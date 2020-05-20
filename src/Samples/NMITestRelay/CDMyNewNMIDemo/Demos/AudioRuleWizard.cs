// SPDX-FileCopyrightText: Copyright(c) 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
// SPDX-License-Identifier: MPL-2.0

using nsCDEngine.Engines;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.ThingService;
using System;

namespace CDMyNewNMIDemo.Demos
{
    class AudioRuleWizard : TheDemoBase
    {
        public AudioRuleWizard(TheThing tBaseThing, ICDEPlugin pPluginBase) : base(tBaseThing, pPluginBase)
        {
            MyBaseThing.DeviceType = eTheDemoScreensTypes.AudioWizard;
            MyBaseThing.FriendlyName = MyBaseThing.DeviceType;

        }
        public override void DoCreateUX()
        {
            //TheFormInfo tMyForm2 = new TheFormInfo(new Guid("{55555555-6AD1-45AE-BE61-96AF02329613}"), eEngineName.NMIService, "Welcome to the Audio Rule Demo Wizard", "TheThing;:;")
            //{ DefaultView = eDefaultView.Form, IsPostingOnSubmit = true, IsAlwaysEmpty = true, TileWidth = 12, TableReference = "{786F6598-8213-4770-96DA-C5D2830A0D63}", PropertyBag = new nmiCtrlWizard { SideBarTitle ="DEMO: Audio Wizard", SideBarIconFA = "&#xf545;" } };
            //TheNMIEngine.AddFormToThingUX(MyBaseThing, tMyForm2, "CMyForm", "<span class='far fa-file-audio fa-3x'>&#xf1c7;</span></br>Audio Wizard", 3, 3, 0, "..Screens", null, new nmiDashboardTile { TileWidth = 2, TileHeight = 2, FriendlyName = "Demo Wizard" });

            var Fls = TheNMIEngine.AddNewWizard(MyBaseThing, new Guid("{786F6598-8213-4770-96DA-C5D2830A0D63}"), "Audio Rule Wizard", new nmiCtrlWizard { SideBarTitle = "DEMO: Audio Wizard", SideBarIconFA = "&#xf545;", Category = "..Demos", Caption = "Demo Audio Wizard", PanelTitle = "<span class='far fa-file-audio fa-3x'>&#xf1c7;</span></br>Audio Wizard" });
            var tMyForm2 = Fls["Form"] as TheFormInfo;
            AddCustomHeader(tMyForm2);

            var tFlds = TheNMIEngine.AddNewWizardPage(MyBaseThing, tMyForm2, 0, 1, 2, "Audio Rule");
            //Controls for the wizard on the right side using parent Fld "RightSide" and Fld ORder 151 - 189 (190-199 is reserved for navigation buttons)
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.SingleEnded, 160, 2, 0, "Rule Name", "FriendlyName", new TheNMIBaseControl { ParentFld = tFlds["RightSide"].FldOrder });
            //HELP SECTION  Help to the left.must use parent fld "LeftSide" and Fld Order between 111 (page# *100)+11 - 149
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.SmartLabel, 120, 0, 0, null, null, new nmiCtrlSmartLabel { ParentFld = tFlds["LeftSide"].FldOrder, ClassName = "helpTextStyle", Text = "1. Enter name for the new rule.", TileWidth = 3, NoTE = true });


            tFlds = TheNMIEngine.AddNewWizardPage(MyBaseThing, tMyForm2, 1, 2, 3, "Rule Trigger");
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.ThingPicker, 260, 2, 0, "Thing", "Thing", new nmiCtrlThingPicker { ParentFld = tFlds["RightSide"].FldOrder, IncludeEngines = true });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.PropertyPicker, 265, 2, 0, "Property", "Property", new nmiCtrlPropertyPicker { ParentFld = tFlds["RightSide"].FldOrder, ThingFld = 260 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.ComboBox, 270, 2, 0, "Condition", "Condition", new nmiCtrlComboBox { ParentFld = tFlds["RightSide"].FldOrder, DefaultValue = "2", Options = "Fire:0;State:1;Equals:2;Larger:3;Smaller:4;Not:5;Contains:6;Set:7;StartsWith:8;EndsWith:9;Flank:10" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.SingleEnded, 275, 2, 0, "Value", "TestValue", new nmiCtrlSingleEnded { ParentFld = tFlds["RightSide"].FldOrder });
            //HELP SECTION trigger object help section
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.SmartLabel, 213, 0, 0, null, null, new nmiCtrlSmartLabel { ParentFld = tFlds["LeftSide"].FldOrder, ClassName = "helpTextStyle", Text = "1. Click to see all available trigger objects (objects to monitor).", TileWidth = 3, NoTE = true });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.SmartLabel, 214, 0, 0, null, null, new nmiCtrlSmartLabel { ParentFld = tFlds["LeftSide"].FldOrder, ClassName = "helpTextStyle", Text = "2. Select Trigger property", TileWidth = 3, NoTE = true });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.SmartLabel, 215, 0, 0, null, null, new nmiCtrlSmartLabel { ParentFld = tFlds["LeftSide"].FldOrder, ClassName = "helpTextStyle", Text = "3. Select Trigger condition", TileWidth = 3, NoTE = true });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.SmartLabel, 216, 0, 0, null, null, new nmiCtrlSmartLabel { ParentFld = tFlds["LeftSide"].FldOrder, ClassName = "helpTextStyle", Text = "4. Enter trigger value", TileWidth = 3, NoTE = true });


            tFlds = TheNMIEngine.AddNewWizardPage(MyBaseThing, tMyForm2, 2, 3, 4, "Playback");
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.ComboBox, 353, 2, 0, "Playback", "Playback", new nmiCtrlComboBox { ParentFld = tFlds["RightSide"].FldOrder, DefaultValue = "Audio Files", Options = "Audio Files:0;MIDI Events:1" });

            //HELP SECTION action object help section
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.SmartLabel, 313, 0, 0, null, null, new nmiCtrlSmartLabel { ParentFld = tFlds["LeftSide"].FldOrder, ClassName = "helpTextStyle", Text = "1. Select the method of audio playback. (Default:Audio Files)", TileWidth = 3, NoTE = true });


            tFlds = TheNMIEngine.AddNewWizardPage(MyBaseThing, tMyForm2, 3, 4, 5, "Sound");
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.ThingPicker, 460, 2, 0, "Sound", "Sound", new nmiCtrlThingPicker { ParentFld = tFlds["RightSide"].FldOrder, Filter = "DeviceType=Sound File" });

            //HELP SECTION Sound help section
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.SmartLabel, 462, 0, 0, null, null, new nmiCtrlSmartLabel { ParentFld = tFlds["LeftSide"].FldOrder, ClassName = "helpTextStyle", Text = "1. Select sound", TileWidth = 3, NoTE = true });

            tFlds = TheNMIEngine.AddNewWizardPage(MyBaseThing, tMyForm2, 4, 5, 0, "Final Settings");
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.SingleCheck, 560, 2, 0, "Activate Rule Now", "IsRuleActive", new nmiCtrlSingleCheck { ParentFld = tFlds["RightSide"].FldOrder });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.SingleCheck, 570, 2, 0, "Log Rule", "IsRuleLogged", new nmiCtrlSingleCheck { ParentFld = tFlds["RightSide"].FldOrder, TileWidth = 6 });
            //HELP SECTION final step help section
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.SmartLabel, 522, 0, 0, null, null, new nmiCtrlSmartLabel { ParentFld = tFlds["LeftSide"].FldOrder, ClassName = "helpTextStyle", Text = "1. Click to make rule active.", TileWidth = 3, NoTE = true });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.SmartLabel, 523, 0, 0, null, null, new nmiCtrlSmartLabel { ParentFld = tFlds["LeftSide"].FldOrder, ClassName = "helpTextStyle", Text = "2. Click to log rule", TileWidth = 3, NoTE = true, });



        }

        private void AddCustomHeader(TheFormInfo tMyForm2)
        {

            //Sanja header
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileGroup, 2, 0, 0, null, null, new nmiCtrlTileGroup { TileWidth = 12, TileHeight = 1 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileGroup, 3, 0, 0, null, null, new nmiCtrlTileGroup { ParentFld = 2, TileWidth = 4, TileHeight = 1 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileGroup, 4, 0, 0, null, null, new nmiCtrlTileGroup { ParentFld = 2, TileWidth = 8, TileHeight = 1, });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.Picture, 5, 0, 0, null, null, new nmiCtrlPicture { ParentFld = 3, TileWidth = 4, TileHeight = 1, Source = "Images/logowz.png", NoTE = true, });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.SmartLabel, 6, 0, 0, null, null, new nmiCtrlSmartLabel { ParentFld = 4, TileWidth = 8, TileHeight = 1, FontSize = 30, VerticalAlignment = "center", NoTE = true, Text = "Welcome to the audio output rule creating wizard." });

            //Remove this section if we decide no direct navivation to a page
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 10, 2, 0, "Welcome", null, new nmiCtrlTileButton { ParentFld = 200, OnClick = "GRP:WizarePage:1", TileWidth = 2, TileHeight = 1, TileFactorY = 2, NoTE = true });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 20, 2, 0, "Rule Trigger", null, new nmiCtrlTileButton { ParentFld = 200, OnClick = "GRP:WizarePage:2", TileWidth = 2, TileHeight = 1, TileFactorY = 2, NoTE = true });
            //  TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 30, 2, 0, "Action Type", null, new nmiCtrlTileButton { ParentFld = 200, OnClick = "GRP:WizarePage:3", TileWidth = 2, TileHeight = 1, TileFactorY = 2, NoTE = true });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 40, 2, 0, "Playback", null, new nmiCtrlTileButton { ParentFld = 200, OnClick = "GRP:WizarePage:4", TileWidth = 2, TileHeight = 1, TileFactorY = 2, NoTE = true });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 50, 2, 0, "Sound", null, new nmiCtrlTileButton { ParentFld = 200, OnClick = "GRP:WizarePage:5", TileWidth = 2, TileHeight = 1, TileFactorY = 2, NoTE = true });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 60, 2, 0, "Finish", null, new nmiCtrlTileButton { ParentFld = 200, OnClick = "GRP:WizarePage:6", TileWidth = 2, TileHeight = 1, TileFactorY = 2, NoTE = true });


            //horizontal line
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileGroup, 65, 0, 0, null, null, new nmiCtrlTileGroup { TileWidth = 12, TileHeight = 1, TileFactorY = 12, Background = "white", Opacity = .2 });
        }
    }
}
