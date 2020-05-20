// SPDX-FileCopyrightText: Copyright(c) 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
// SPDX-License-Identifier: MPL-2.0

using nsCDEngine.Engines;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.ThingService;
using System;

namespace CDMyNewNMIDemo.Demos
{
    class DemoWizard : TheDemoBase
    {
        public DemoWizard(TheThing tBaseThing, ICDEPlugin pPluginBase) : base(tBaseThing, pPluginBase)
        {
            MyBaseThing.DeviceType = eTheDemoScreensTypes.DemoWizard;
            MyBaseThing.FriendlyName = MyBaseThing.DeviceType;

        }
        public override void DoCreateUX()
        {
            var flds=TheNMIEngine.AddNewWizard(MyBaseThing, new Guid("6C34871C-D49B-4D7A-84E0-35E25B1F18B0"), "Welcome to the Rule Wizard Demo", new nmiCtrlWizard { SideBarTitle = "DEMO: Rules Wizard", SideBarIconFA = "&#xf545;", Caption = "Demo Wizard" });
            var tMyForm2 = flds["Form"] as TheFormInfo;
            AddWizardHeader(tMyForm2); //Not Required but looks nice. Might become a later addition to the APIs

            var tFlds = TheNMIEngine.AddNewWizardPage(MyBaseThing, tMyForm2, 0, 1, 2, null /*"TEST"*/);
            TheNMIEngine.AddWizardControl(MyBaseThing, tMyForm2, eFieldType.SingleEnded, 1, 1, 2, 0, "Rule Name", "FriendlyName", new TheNMIBaseControl { Explainer = "1. Enter name for the new rule." });

            tFlds = TheNMIEngine.AddNewWizardPage(MyBaseThing, tMyForm2, 1, 2, 3, null /* "Rule Trigger"*/);
            TheNMIEngine.AddWizardControl(MyBaseThing, tMyForm2, eFieldType.ThingPicker, 2, 1, 2, 0, "Trigger Thing", "TriggerObject", new nmiCtrlThingPicker { IncludeEngines = true, Explainer = "1. Click to see all available trigger objects" });
            TheNMIEngine.AddWizardControl(MyBaseThing, tMyForm2, eFieldType.PropertyPicker, 2, 2, 2, 0, "Trigger Property", "TriggerProperty", new nmiCtrlPropertyPicker { ThingFld = 260, Explainer = "2. Select Trigger property" });
            TheNMIEngine.AddWizardControl(MyBaseThing, tMyForm2, eFieldType.ComboBox, 2, 3, 2, 0, "Trigger Condition", "TriggerCondition", new nmiCtrlComboBox { DefaultValue = "2", Options = "Fire:0;State:1;Equals:2;Larger:3;Smaller:4;Not:5;Contains:6;Set:7;StartsWith:8;EndsWith:9;Flank:10", Explainer = "3. Select Trigger condition" });
            TheNMIEngine.AddWizardControl(MyBaseThing, tMyForm2, eFieldType.SingleEnded, 2, 4, 2, 0, "Trigger Value", "TriggerValue", new nmiCtrlSingleEnded { Explainer = "4. Enter trigger value" });

            tFlds = TheNMIEngine.AddNewWizardPage(MyBaseThing, tMyForm2, 2, 3, 4, null/*"Action Type"*/, "5:'<%MyPropertyBag.ActionObjectType.Value%>'!='CDE_THING' && '<%MyPropertyBag.ActionObjectType.Value%>'!=''");
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.ComboBox, 360, 2, 0, "What Action to do", "ActionObjectType", new nmiCtrlComboBox { ParentFld = tFlds["RightSide"].FldOrder, DefaultValue = "CDE_THING", Options = "Set Property on a Thing:CDE_THING;Publish Central:CDE_PUBLISHCENTRAL;Publish to Service:CDE_PUBLISH2SERVICE" });
            //HELP SECTION action type help section
            TheNMIEngine.AddWizardExplainer(MyBaseThing, tMyForm2, 3, 1, 0, "Action Object Type: Defines how the action should be executed:", new nmiCtrlSmartLabel { });
            TheNMIEngine.AddWizardExplainer(MyBaseThing, tMyForm2, 3, 2, 0, "DEFAULT:Set Property on a Thing: sets a property of a Thing to the Action Value", new nmiCtrlSmartLabel { });
            TheNMIEngine.AddWizardExplainer(MyBaseThing, tMyForm2, 3, 3, 0, "Publish Central: sends a message to all nodes in the mesh with the given parameters", new nmiCtrlSmartLabel { });
            TheNMIEngine.AddWizardExplainer(MyBaseThing, tMyForm2, 3, 4, 0, "Publish to Service: sends a message to a specific service in the mesh", new nmiCtrlSmartLabel { });


            tFlds = TheNMIEngine.AddNewWizardPage(MyBaseThing, tMyForm2, 3, 4, 6, null  /*"Action Object"*/);
            TheNMIEngine.AddWizardControl(MyBaseThing, tMyForm2, eFieldType.ThingPicker, 4, 1, 2, 0, "Action Thing", "ActionObject", new nmiCtrlThingPicker { IncludeEngines = true, Explainer = "1. Select the “Thing” (object) that contains the property to be changed with the action" });
            TheNMIEngine.AddWizardControl(MyBaseThing, tMyForm2, eFieldType.PropertyPicker, 4, 2, 2, 0, "Action Property", "ActionProperty", new nmiCtrlPropertyPicker { ThingFld = 460, Explainer = "2. Select action property" });
            TheNMIEngine.AddWizardControl(MyBaseThing, tMyForm2, eFieldType.SingleEnded, 4, 3, 2, 0, "Action Value", "ActionValue", new nmiCtrlSingleEnded { Explainer = "3. Select Action Value" });

            tFlds = TheNMIEngine.AddNewWizardPage(MyBaseThing, tMyForm2, 3, 5, 6, null /* "Action TSM"*/ );
            TheNMIEngine.AddWizardControl(MyBaseThing, tMyForm2, eFieldType.ThingPicker, 5, 1, 2, 0, "TSM Engine", "TSMEngine", new nmiCtrlThingPicker { IncludeEngines = true, Filter = "DeviceType=IBaseEngine", Explainer = "1. Specify the Target Engine the message will be send to." });
            TheNMIEngine.AddWizardControl(MyBaseThing, tMyForm2, eFieldType.SingleEnded, 5, 2, 2, 0, "TSM Text", "TSMText", new nmiCtrlSingleEnded { Explainer = "2. Specify the command/text of the message.Target plugins are using this to parse the content of the payload" });
            TheNMIEngine.AddWizardControl(MyBaseThing, tMyForm2, eFieldType.TextArea, 5, 3, 2, 0, "TSM Payload", "TSMPayload", new nmiCtrlSingleEnded { TileHeight = 2, Explainer = "3. specify the payload message (TSM.PLS)" });

            tFlds = TheNMIEngine.AddNewWizardPage(MyBaseThing, tMyForm2, 3, 6, 0, null  /*"Final Settings"*/);
            TheNMIEngine.AddWizardControl(MyBaseThing, tMyForm2, eFieldType.SingleCheck, 6, 1, 2, 0, "Activate Rule Now", "IsRuleActive", new nmiCtrlSingleCheck { Explainer = "1. Click to make rule active." });
            TheNMIEngine.AddWizardControl(MyBaseThing, tMyForm2, eFieldType.SingleCheck, 6, 2, 2, 0, "Log Rule", "IsRuleLogged", new nmiCtrlSingleCheck { Explainer = "2. Click to log rule." });
        }

        private void AddWizardHeader(TheFormInfo tMyForm2)
        {
            //Sanja header
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileGroup, 2, 0, 0, null, null, new nmiCtrlTileGroup { TileWidth = 12, TileHeight = 1 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileGroup, 3, 0, 0, null, null, new nmiCtrlTileGroup { ParentFld = 2, TileWidth = 4, TileHeight = 1 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileGroup, 4, 0, 0, null, null, new nmiCtrlTileGroup { ParentFld = 2, TileWidth = 8, TileHeight = 1, });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.Picture, 5, 0, 0, null, null, new nmiCtrlPicture { ParentFld = 3, TileWidth = 4, TileHeight = 1, Source = "Images/logowz.png", NoTE = true, });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.SmartLabel, 6, 0, 0, null, null, new nmiCtrlSmartLabel { ParentFld = 4, TileWidth = 8, TileHeight = 1, FontSize = 30, VerticalAlignment = "center", NoTE = true, Text = "Welcome to the rule creating wizard." });

            //Remove this section if we decide no direct navivation to a page
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 10, 2, 0, "Welcome", null, new nmiCtrlTileButton { ParentFld = 200, OnClick = "GRP:WizarePage:1", TileWidth = 2, TileHeight = 1, TileFactorY = 2, NoTE = true });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 20, 2, 0, "Rule Trigger", null, new nmiCtrlTileButton { ParentFld = 200, OnClick = "GRP:WizarePage:2", TileWidth = 2, TileHeight = 1, TileFactorY = 2, NoTE = true });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 30, 2, 0, "Action Type", null, new nmiCtrlTileButton { ParentFld = 200, OnClick = "GRP:WizarePage:3", TileWidth = 2, TileHeight = 1, TileFactorY = 2, NoTE = true });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 40, 2, 0, "Action Object", null, new nmiCtrlTileButton { ParentFld = 200, OnClick = "GRP:WizarePage:4", TileWidth = 2, TileHeight = 1, TileFactorY = 2, NoTE = true });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 50, 2, 0, "Action TSM", null, new nmiCtrlTileButton { ParentFld = 200, OnClick = "GRP:WizarePage:5", TileWidth = 2, TileHeight = 1, TileFactorY = 2, NoTE = true });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 60, 2, 0, "Finish", null, new nmiCtrlTileButton { ParentFld = 200, OnClick = "GRP:WizarePage:6", TileWidth = 2, TileHeight = 1, TileFactorY = 2, NoTE = true });

            //horizontal line
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileGroup, 65, 0, 0, null, null, new nmiCtrlTileGroup { TileWidth = 12, TileHeight = 1, TileFactorY = 12, Background = "white", Opacity = .2 });
        }
    }
}
