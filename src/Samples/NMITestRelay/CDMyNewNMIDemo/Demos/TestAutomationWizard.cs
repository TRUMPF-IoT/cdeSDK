// SPDX-FileCopyrightText: Copyright(c) 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
// SPDX-License-Identifier: MPL-2.0

using nsCDEngine.Engines;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.ThingService;
using System;

namespace CDMyNewNMIDemo.Demos
{
    class TestAutoWizard : TheDemoBase
    {
        public TestAutoWizard(TheThing tBaseThing, ICDEPlugin pPluginBase) : base(tBaseThing, pPluginBase)
        {
            MyBaseThing.DeviceType = eTheDemoScreensTypes.TestAutomationWizard;
            MyBaseThing.FriendlyName = MyBaseThing.DeviceType;

        }
        public override void DoCreateUX()
        {
            var flds=TheNMIEngine.AddNewWizard(MyBaseThing, new Guid("6C34871C-D49B-4D7A-84E0-35E25B1F18B8"), "Welcome to the Test Automation Demo", new nmiCtrlWizard { SideBarTitle = "DEMO: Test Automation Wizard", SideBarIconFA = "&#xf545;", Caption = "New Test Automation Wizard" });
            var tMyForm2 = flds["Form"] as TheFormInfo;
            AddWizardHeader(tMyForm2); //Not Required but looks nice. Might become a later addition to the APIs

            var tFlds = TheNMIEngine.AddNewWizardPage(MyBaseThing, tMyForm2, 0, 1, 2, null /*"TEST"*/);
            TheNMIEngine.AddWizardControl(MyBaseThing, tMyForm2, eFieldType.SmartLabel, 1, 0, 0, 0, null, null, new TheNMIBaseControl { Value="Before starting this wizard you need to have a node type for application to test and agents running on your test enviroment", NoTE=true, FontSize=18});
    

            


            tFlds = TheNMIEngine.AddNewWizardPage(MyBaseThing, tMyForm2, 1, 2, 3, null /* "Rule Trigger"*/);
            TheNMIEngine.AddWizardControl(MyBaseThing, tMyForm2, eFieldType.SingleEnded, 2, 1, 2, 0, "TestName", "TestName", new nmiCtrlThingPicker {});
            TheNMIEngine.AddWizardControl(MyBaseThing, tMyForm2, eFieldType.PropertyPicker, 2, 2, 2, 0, "Select Node Type to Test", "NodeType", new nmiCtrlPropertyPicker { ThingFld = 260 });
            TheNMIEngine.AddWizardControl(MyBaseThing, tMyForm2, eFieldType.Number, 2,3,  2, 0, "Instance Counter", "InstanceCounter", new nmiCtrlNumber { Explainer = "Instance Counter: How many instances of this type to be crated" });
            TheNMIEngine.AddWizardControl(MyBaseThing, tMyForm2, eFieldType.Number, 2, 4, 2, 0, "Mesh Counter", "MeshCounter", new nmiCtrlNumber { Explainer = "Mesh Counter: How many customer meshes" });

                                                                              
            tFlds = TheNMIEngine.AddNewWizardPage(MyBaseThing, tMyForm2, 2, 3, 4, null/*"Action Type"*/, "5:'<%MyPropertyBag.ActionObjectType.Value%>'!='CDE_THING' && '<%MyPropertyBag.ActionObjectType.Value%>'!=''");
            TheNMIEngine.AddWizardControl(MyBaseThing, tMyForm2, eFieldType.ComboBox, 3, 1, 2, 0, "Select Controller", "Controller", new nmiCtrlComboBox {  });
            TheNMIEngine.AddWizardControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 3, 2, 2, 0, "Add Controller", "AddController", new nmiCtrlTileButton { Explainer="Click to add to host controller." });
            TheNMIEngine.AddWizardControl(MyBaseThing, tMyForm2, eFieldType.TextArea, 3, 3, 2, 0, "Controllers", "Controllers", new nmiCtrlTileButton { Explainer = "Click to add to host controller." });

            tFlds = TheNMIEngine.AddNewWizardPage(MyBaseThing, tMyForm2, 3, 4, 0, null  /*"Final Settings"*"*/);
            TheNMIEngine.AddWizardControl(MyBaseThing, tMyForm2, eFieldType.SmartLabel, 4, 1, 0, 0, null, null, new nmiCtrlSingleEnded { Value="After finish go to" , NoTE=true, FontSize=18});
            TheNMIEngine.AddWizardControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 4, 2, 2, 0, "My Test Automations", "MyTestAutomations", new nmiCtrlTileButton {TileWidth=3, NoTE=true });
            TheNMIEngine.AddWizardControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 4, 3, 2, 0, "My Test Definitions", "MyTestDefinitionss", new nmiCtrlTileButton { TileWidth = 3, NoTE = true });
            TheNMIEngine.AddWizardControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 4, 4, 2, 0, "My Test Enviroments", "MyTestEnviroments", new nmiCtrlTileButton { TileWidth = 3, NoTE = true });
            TheNMIEngine.AddWizardControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 4, 5, 2, 0, "Go back to test manager", "BackToTestManager", new nmiCtrlTileButton { TileWidth = 3, NoTE = true });



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
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 10, 2, 0, "Welcome", null, new nmiCtrlTileButton { ParentFld = 200, OnClick = "GRP:WizarePage:1", TileWidth = 2, TileHeight = 1, TileFactorY = 2, NoTE = true , FontSize=14});
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 20, 2, 0, "Test Definition", null, new nmiCtrlTileButton { ParentFld = 200, OnClick = "GRP:WizarePage:2", TileWidth = 2, TileHeight = 1, TileFactorY = 2, NoTE = true , FontSize = 14 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 30, 2, 0, "Test Enviroment", null, new nmiCtrlTileButton { ParentFld = 200, OnClick = "GRP:WizarePage:3", TileWidth = 2, TileHeight = 1, TileFactorY = 2, NoTE = true , FontSize = 14 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 40, 2, 0, "Test Automation", null, new nmiCtrlTileButton { ParentFld = 200, OnClick = "GRP:WizarePage:4", TileWidth = 2, TileHeight = 1, TileFactorY = 2, NoTE = true , FontSize = 14 });
            

            //horizontal line
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileGroup, 65, 0, 0, null, null, new nmiCtrlTileGroup { TileWidth = 12, TileHeight = 1, TileFactorY = 12, Background = "white", Opacity = .2 });
        }
    }
}
