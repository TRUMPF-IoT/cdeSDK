// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using nsCDEngine.BaseClasses;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.ThingService;
using nsCDEngine.ViewModels;
using System;
using System.Collections.Generic;

namespace TheSensorTemplate
{
    static class TheSensorNMI
    {
        static public TheFormInfo CreateSensorForm(TheThing MyBaseThing, string pSensorFace = "/pages/ThingFace.html", string pReportName = "Sensor Report", string pCategory = null)
        {
            if (!string.IsNullOrEmpty(pSensorFace))
                TheNMIEngine.ParseFacePlateUrl(MyBaseThing, pSensorFace,false, Guid.Empty);
            TheFormInfo tMyForm = new TheFormInfo(MyBaseThing) { FormTitle = null, DefaultView = eDefaultView.Form, PropertyBag = new ThePropertyBag { "MaxTileWidth=18", "Background=rgba(255, 255, 255, 0.04)", "HideCaption=true" } };
            tMyForm.AddOrUpdatePlatformBag(eWebPlatform.Mobile, new nmiPlatBag { MaxTileWidth = 6 });
            tMyForm.AddOrUpdatePlatformBag(eWebPlatform.HoloLens, new nmiPlatBag { MaxTileWidth = 12 });
            tMyForm.AddOrUpdatePlatformBag(eWebPlatform.TeslaXS, new nmiPlatBag { MaxTileWidth = 12 });
            TheNMIEngine.AddFormToThingUX(MyBaseThing, tMyForm, "CMyForm", "Loading... <i class='fa fa-spinner fa-pulse'></i>", 3, 3, 0, pCategory, null, new nmiDashboardTile { TileWidth = 4, TileHeight = 3, HTMLUrl = pSensorFace, RenderTarget="HomeCenterStage" });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.StatusLight, 1100, 0, 0, null, "StatusLevel", new TheNMIBaseControl { NoTE = true, TileWidth = 1, TileHeight = 1, TileFactorX = 2, TileFactorY = 2, RenderTarget = "VSSTATLGHT%cdeMID%" });

            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.CollapsibleGroup, 1, 2, 0, pReportName, null, new nmiCtrlCollapsibleGroup { IsSmall = true, DoClose = true, LabelClassName = "cdeTileGroupHeaderSmall SensorGroupLabel", LabelForeground = "white" });

            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 38, 0, 0, null, null, new nmiCtrlTileGroup { TileWidth = 6, ParentFld = 1 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleEnded, 39, 2, 0, "Friendly Name", "FriendlyName", new nmiCtrlSingleEnded() { TileWidth = 6, TileHeight = 1, ParentFld = 38 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.StatusLight, 40, 0, 0, null, "StatusLevel", new TheNMIBaseControl { NoTE = true, TileHeight = 2, TileWidth = 2, ParentFld = 38 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TextArea, 41, 0, 0, null, "LastMessage", new nmiCtrlTextArea { TileHeight = 2, ParentFld = 38, TileWidth = 4, NoTE = true });
            if (string.IsNullOrEmpty(TheThing.GetSafePropertyString(MyBaseThing, "StateSensorIcon")))
                TheThing.SetSafePropertyString(MyBaseThing, "StateSensorIcon", "/Images/iconToplogo.png");

            var tFld2 = TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, TheDefaultSensor.SensorActionArea, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 1, TileWidth = 6, TileHeight = 3, Background = "transparent" });
            tFld2.AddOrUpdatePlatformBag(eWebPlatform.Any, new nmiPlatBag { Hide = true });
            tFld2.AddOrUpdatePlatformBag(eWebPlatform.Desktop, new nmiPlatBag { Show = true });
            var tFld3 = TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, TheDefaultSensor.SensorActionArea2, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = 1, TileWidth = 6, TileHeight = 3, Background = "transparent" });
            tFld3.AddOrUpdatePlatformBag(eWebPlatform.Any, new nmiPlatBag { Hide = true });
            tFld3.AddOrUpdatePlatformBag(eWebPlatform.Desktop, new nmiPlatBag { Show = true });
            return tMyForm;
        }

        static public void TheSensorLabel(TheThing pBaseThing, TheFormInfo pFormInfo, int pFldOrder, int pParent, string LabelTitle, string pValue, string pFormat = "")
        {
            TheNMIEngine.AddSmartControl(pBaseThing, pFormInfo, eFieldType.SmartLabel, pFldOrder, 0, 0, LabelTitle, pValue, new nmiCtrlSmartLabel()
            {
                ParentFld = pParent,
                TileFactorY = 2,
                TileWidth = 6,
                LabelClassName = "cdeSensorLabel",
                Background = "transparent",
                FontSize = 16,
                HorizontalAlignment = "left",
                VerticalAlignment = "center",
                Format = pFormat
                // Value = pValue
            });
        }

        static public bool CreatePerformanceHeader(TheThing pBaseThing, TheFormInfo pFormInfo, int pFldOrder, int pParent, string pSensorPicSource = "SENSORS/Images/SensorLogo_156x78.png", string pLogo = null)
        {
            var tG = TheNMIEngine.AddSmartControl(pBaseThing, pFormInfo, eFieldType.TileGroup, pFldOrder, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = pParent, /*MaxTileWidth = 18,*/ Background = "transparent" });
            tG.AddOrUpdatePlatformBag(eWebPlatform.Mobile, new nmiPlatBag { MaxTileWidth = 6 });
            tG.AddOrUpdatePlatformBag(eWebPlatform.HoloLens, new nmiPlatBag { MaxTileWidth = 12 });

            TheNMIEngine.AddSmartControl(pBaseThing, pFormInfo, eFieldType.TileGroup, pFldOrder + 1, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = pFldOrder, TileWidth = 6 });
            //TheNMIEngine.AddSmartControl(pBaseThing, pFormInfo, eFieldType.StatusLight, pFldOrder + 2, 0, 0, "Current status", "StatusLevel", new TheNMIBaseControl { ParentFld = pFldOrder + 1, TileHeight = 1, TileWidth = 4 });
            //TheNMIEngine.AddSmartControl(pBaseThing, pFormInfo, eFieldType.SmartLabel, pFldOrder + 3, 0, 0, null, "LastMessage", new nmiCtrlTextArea() { ParentFld = pFldOrder + 1, TileWidth = 6, TileHeight = 1, Background = "transparent",NoTE=true, FontSize = 20, HorizontalAlignment = "left", VerticalAlignment = "center"/*, Value = pCurrentStatus*/ });

            var tMiniStatus = TheNMIEngine.AddSmartControl(pBaseThing, pFormInfo, eFieldType.TileGroup, pFldOrder + 5, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = pFldOrder, TileWidth = 6, Background = "transparent" });
            if (!string.IsNullOrEmpty(pLogo))
            {
                var tBut2 = TheNMIEngine.AddSmartControl(pBaseThing, pFormInfo, eFieldType.Picture, pFldOrder + 7, 2, 0, null, null, new nmiCtrlPicture() { ParentFld = pFldOrder + 5, TileWidth = 2, TileHeight = 1, NoTE = true, Background = "transparent", Source = pLogo });
                tBut2.RegisterUXEvent(pBaseThing, eUXEvents.OnClick, "ImageClick2", (sender, para) =>
                {
                    uint min = TheCommonUtils.CUInt(TheThing.GetSafePropertyNumber(pBaseThing, "StateSensorMinValue"));
                    uint max = TheCommonUtils.CUInt(TheThing.GetSafePropertyNumber(pBaseThing, "StateSensorMaxValue"));
                    TheThing.SetSafePropertyNumber(pBaseThing, "QValue", TheCommonUtils.GetRandomUInt(min, max));
                });
            }

            var tFld = TheNMIEngine.AddSmartControl(pBaseThing, pFormInfo, eFieldType.TileGroup, pFldOrder + 8, 0, 0, null, null, new nmiCtrlTileGroup() { ParentFld = pFldOrder, TileWidth = 6, TileHeight = 2, Background = "transparent" });
            tFld.AddOrUpdatePlatformBag(eWebPlatform.Any, new nmiPlatBag { Hide = true });
            tFld.AddOrUpdatePlatformBag(eWebPlatform.Desktop, new nmiPlatBag { Show = true });
            tFld.AddOrUpdatePlatformBag(eWebPlatform.XBox, new nmiPlatBag { Show = true });
            TheNMIEngine.AddSmartControl(pBaseThing, pFormInfo, eFieldType.SmartLabel, pFldOrder + 9, 0, 0, null, "FriendlyName", new nmiCtrlSmartLabel() { ParentFld = pFldOrder + 8, NoTE = true, TileWidth = 3, TileHeight = 1, LabelClassName = "cdeSelDevice", FontSize = 20, HorizontalAlignment = "left", VerticalAlignment = "center"/*, Value = pSelectedDeviceName*/ });
            if (!string.IsNullOrEmpty(pSensorPicSource))
            {
                var tBut3 = TheNMIEngine.AddSmartControl(pBaseThing, pFormInfo, eFieldType.Picture, pFldOrder + 10, 0, 0, null, null, new nmiCtrlPicture() { NoTE = true, ParentFld = pFldOrder + 8, TileWidth = 3, TileHeight = 1, Source = pSensorPicSource }); ;
                tBut3.RegisterUXEvent(pBaseThing, eUXEvents.OnClick, "LogoClick3", (sender, para) =>
                {
                    uint min = TheCommonUtils.CUInt(TheThing.GetSafePropertyNumber(pBaseThing, "StateSensorMinValue"));
                    uint max = TheCommonUtils.CUInt(TheThing.GetSafePropertyNumber(pBaseThing, "StateSensorMaxValue"));
                    TheThing.SetSafePropertyNumber(pBaseThing, "QValue", TheCommonUtils.GetRandomUInt(min, max));
                });
            }
            TheNMIEngine.AddSmartControl(pBaseThing, pFormInfo, eFieldType.StatusLight, pFldOrder + 12, 0, 0, null, "StatusLevel", new TheNMIBaseControl { NoTE = true, TileHeight = 1, TileWidth = 1, ParentFld = pFldOrder + 8 });
            TheNMIEngine.AddSmartControl(pBaseThing, pFormInfo, eFieldType.TextArea, pFldOrder + 11, 0, 0, null, "LastMessage", new nmiCtrlTextArea { TileHeight = 1, ParentFld = pFldOrder + 8, TileWidth = 5, FontSize = 12, NoTE = true, Foreground = "gray", Background = "Transparent" });

            return true;
        }



        /// <summary>
        ///  Similar to the MidTypeC, but with more items and is scrollable 
        /// creates a tile group (8x8) with Title(8x1) and 11 label/value smart labels (8x1)  TEMPERATURE
        /// </summary>
        /// <param name="pBaseThing"></param>
        /// <param name="pFormInfo"></param>
        /// <param name="pFldOrder"></param>
        /// <param name="pParent"></param>
        /// <param name="pTtile"></param>
        /// <param name="pManufacturer"></param>
        /// <param name="pDeviceType"></param>
        /// <param name="pDeviceID"></param>
        /// <param name="pOrderID"></param>
        /// <param name="pSerialNum"></param>
        /// <param name="pHardwareVersion"></param>
        /// <param name="pSoftwareVersion"></param>
        /// <param name="pInstallDate"></param>
        /// <param name="pFunction"></param>
        /// <param name="pLocation"></param>
        /// <returns></returns>
        static public bool CreateDeviceDetails(TheThing pBaseThing, TheFormInfo pFormInfo, int pFldOrder, int pParent, List<string> tCustom = null)
        {
            var tD = TheNMIEngine.AddSmartControl(pBaseThing, pFormInfo, eFieldType.CollapsibleGroup, pFldOrder, 2, 0, "Device Description", null, new nmiCtrlCollapsibleGroup() { ParentFld = pParent, TileWidth = 6, Background = "transparent", Foreground = "black", IsSmall = true, HorizontalAlignment = "left" }); //LabelClassName = "cdeTileGroupHeaderSmall SensorGroupLabel", LabelForeground = "white",
            tD.AddOrUpdatePlatformBag(eWebPlatform.HoloLens, new nmiPlatBag { Hide = true });
            tD.AddOrUpdatePlatformBag(eWebPlatform.TeslaXS, new nmiPlatBag { Hide = true });
            tD.AddOrUpdatePlatformBag(eWebPlatform.Mobile, new nmiCtrlCollapsibleGroup { DoClose = true });
            TheNMIEngine.AddSmartControl(pBaseThing, pFormInfo, eFieldType.TileGroup, pFldOrder + 1, 0, 0, null, null, new nmiCtrlCollapsibleGroup() { ParentFld = pFldOrder, TileWidth = 6, TileHeight = 4, IsVScrollable = true });
            TheSensorLabel(pBaseThing, pFormInfo, pFldOrder + 2, pFldOrder + 1, "Manufacturer", "VendorName");
            TheSensorLabel(pBaseThing, pFormInfo, pFldOrder + 3, pFldOrder + 1, "Product Name", "ProductName");
            TheSensorLabel(pBaseThing, pFormInfo, pFldOrder + 4, pFldOrder + 1, "Product ID", "ProductID");
            TheSensorLabel(pBaseThing, pFormInfo, pFldOrder + 5, pFldOrder + 1, "Product Text", "ProductText");
            TheSensorLabel(pBaseThing, pFormInfo, pFldOrder + 6, pFldOrder + 1, "Serial Number", "SerialNumber");
            TheSensorLabel(pBaseThing, pFormInfo, pFldOrder + 7, pFldOrder + 1, "Application Tag", "AppTag");
            if (tCustom != null && tCustom.Count > 0)
            {
                var i = 0;
                foreach (string t in tCustom)
                {
                    var ta = t.Split(',');
                    if (ta.Length > 1)
                    {
                        TheSensorLabel(pBaseThing, pFormInfo, (pFldOrder + i) + 8, pFldOrder + 1, ta[0], ta[1], ta.Length > 2 ? ta[2] : "");
                        i++;
                    }
                }
            }
            return true;
        }
    }

}
