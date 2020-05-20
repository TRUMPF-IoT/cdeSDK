// SPDX-FileCopyrightText: Copyright(c) 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
// SPDX-License-Identifier: MPL-2.0

using nsCDEngine.Engines;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.ThingService;
using System;
using System.Collections.Generic;
using System.Text;

namespace CDMyNewNMIDemo.Demos
{
    class TheMeshPicker : TheDemoBase
    {
        public TheMeshPicker(TheThing tBaseThing, ICDEPlugin pPluginBase) : base(tBaseThing, pPluginBase)
        {
            MyBaseThing.DeviceType = eTheDemoScreensTypes.MeshPicker;
            MyBaseThing.FriendlyName = MyBaseThing.DeviceType;

        }

        public override void DoCreateUX()
        {
            TheFormInfo tMyForm2 = new TheFormInfo(MyBaseThing) { FormTitle = "Mesh Picker", DefaultView = eDefaultView.Form, TileWidth = 12 };
            TheNMIEngine.AddFormToThingUX(MyBaseThing, tMyForm2, "CMyForm", "Mesh Picker", 3, 3, 0, "..Demos", null, new nmiDashboardTile { TileWidth = 4, TileHeight = 2, Thumbnail = "Images/iconToplogo.png;0.1", FriendlyName = "All NMI Controls Demo" });

            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileGroup, 200, 0, 0, null, null, new nmiCtrlTileGroup { ParentFld = 200, TileWidth = 12, TileHeight = 11, TileFactorY=2, NoTE = true, });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.SmartLabel, 210, 0, 0, null, null, new nmiCtrlTileGroup { ParentFld = 200, TileWidth = 12, TileHeight = 1,  NoTE = true, Value = "Welcome User Name" }); ;
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.SmartLabel, 211, 0, 0, null, null, new nmiCtrlTileGroup { ParentFld = 200, TileWidth = 12, TileHeight = 1,TileFactorY=3, FontSize=18, NoTE = true, Value = "Please Select Mesh" }); ;
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileGroup, 212, 0, 0, null, null, new nmiCtrlTileGroup { ParentFld = 200, TileWidth = 6, TileHeight = 11, TileFactorY = 2, NoTE = true, Background = "lightgray" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileGroup, 214, 0, 0, null, null, new nmiCtrlTileGroup { ParentFld = 200, TileWidth = 6, TileHeight = 11, TileFactorY = 2, NoTE = true, Background = "gray" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.ComboBox, 220, 2, 0, "Search Mesh", null, new nmiCtrlComboBox { ParentFld = 212, LabelForeground = "black", Options = "XBBY...;_XWR;HJDA" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileGroup, 230, 0, 0, null, null, new nmiCtrlTileGroup { ParentFld = 212, TileWidth = 6, TileHeight = 4, NoTE = true, IsVScrollable = true }); ;

            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileGroup, 235, 0, 0, null, null, new nmiCtrlTileGroup { ParentFld = 230, TileWidth = 1, TileHeight = 66, NoTE = true}); ;
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 250, 2, 0, "XXAB- MyBrand1 ... +13 1623552f - ec21 - 59a4 - 8d73 - 6350adbdc185", null, new nmiCtrlTileButton { ParentFld = 230, TileWidth = 4, TileHeight = 1, NoTE = true, ClassName = "cdeTransitButton" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 300, 2, 0, "XXWR- MyBrand2 ... +12 1623552f - ec21 - 59a4 - 8d73 - 6350adbdc185", null, new nmiCtrlTileButton { ParentFld = 230, TileWidth = 4, TileHeight = 1, NoTE = true, ClassName = "cdeTransitButton" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 350, 2, 0, "XXDA- MyBrand3 ... +12 1623552f - ec21 - 59a4 - 8d73 - 6350adbdc185", null, new nmiCtrlTileButton { ParentFld = 230, TileWidth = 4, TileHeight = 1, NoTE = true, ClassName = "cdeTransitButton" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileButton, 400, 2, 0, "XXDJ- MyBrand4 ... +13 1623552f - ec21 - 59a4 - 8d73 - 6350adbdc185", null, new nmiCtrlTileButton { ParentFld = 230, TileWidth = 4, TileHeight = 1, NoTE = true, ClassName = "cdeTransitButton" });


            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.SmartLabel, 500, 0, 0, null, null, new nmiCtrlSmartLabel { ParentFld = 214, TileWidth = 6, TileHeight = 1, NoTE = true, Value="Mesh Info" });

            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TileGroup, 505, 0, 0, null, null, new nmiCtrlTileGroup { ParentFld = 214, TileWidth = 1, TileHeight = 5, NoTE = true });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm2, eFieldType.TextArea, 510, 0, 0, null, null, new nmiCtrlTextArea { ParentFld = 214, TileWidth = 4, TileHeight = 5, NoTE=true,  Value = "Lorem ipsum dolor sit amet consectetur adipiscing elit, scelerisque quis sociosqu vehicula dapibus commodo aptent pharetra, eu mauris nulla mi ridiculus conubia. Quisque laoreet malesuada eu montes lectus cubilia consequat, tellus vivamus fermentum sollicitudin feugiat rhoncus, hendrerit facilisi bibendum augue ut tempor. Egestas turpis curabitur in scelerisque vestibulum nunc, commodo cras proin condimentum phasellus hac orci, porttitor mi praesent nisl fusce." }); ;
        }
    }
}
