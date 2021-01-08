// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using nsCDEngine.BaseClasses;
using nsCDEngine.Engines;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.ThingService;
using nsCDEngine.ViewModels;
using System;

namespace TheSensorTemplate
{
    public partial class TheDefaultSensor<T> : ICDEThing where T : TheMetaDataBase, System.ComponentModel.INotifyPropertyChanged, new()
    {
        protected bool IsUXReady = false;
        internal TheFormInfo SensorForm = null;
        internal TheFieldInfo ValueField = null;
        internal TheFieldInfo LiveChartFld = null;
        internal TheFieldInfo BucketChartFld = null;
        internal TheFieldInfo GaugeFld = null;
        // protected TheDashPanelInfo SummaryForm;

        [ConfigProperty(DefaultValue = false)]
        public bool IsDisabled
        {
            get { return TheThing.MemberGetSafePropertyBool(MyBaseThing); }
            set { TheThing.MemberSetSafePropertyBool(MyBaseThing, value); }
        }

        public override bool CreateUX()
        {
            if (!mIsInitialized)
            {
                IsUXReady = true;
                TheBaseAssets.MySYSLOG.WriteToLog(500, new TSM(MyBaseThing.EngineName, $"Create UX delays because Init not called for Sensor {MyBaseThing.FriendlyName}", eMsgLevel.l1_Error));
                return false;
            }
            if (!mIsUXInitCalled)
            {
                mIsUXInitCalled = true;
                var pReportName = TheThing.GetSafePropertyString(MyBaseThing, "ReportName");
                if (string.IsNullOrEmpty(pReportName))
                    pReportName = "Sensor Report";
                var pReportFace = TheThing.GetSafePropertyString(MyBaseThing, "ReportFace");
                if (string.IsNullOrEmpty(pReportFace))
                    pReportFace = "/pages/ThingFace.html";
                var pReportCate = TheThing.GetSafePropertyString(MyBaseThing, "ReportCategory");
                TheFormInfo tMyForm = TheSensorNMI.CreateSensorForm(MyBaseThing, pReportFace, pReportName, pReportCate);
                tMyForm.ModelID = "DefaultSensorTemplateForm";
                //tMyForm.RegisterEvent(eUXEvents.OnShow, (sender, para) =>
                //{
                //    var pMsg = para as TheProcessMessage;
                //    if (pMsg?.Message == null) return;
                //        ValueField?.SetUXProperty(pMsg.Message.GetOriginator(), $"Title={TheThing.GetSafePropertyString(MyBaseThing, "StateSensorValueName")}");
                //});
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.CollapsibleGroup, 210, 6, 0, "Sensor Info...", null, new nmiCtrlCollapsibleGroup { IsSmall = true, DoClose = true, ParentFld = 1 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleEnded, 230, 2, 0, "Vendor name", "VendorName", new nmiCtrlTextArea { ParentFld = 210 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Number, 240, 2, 0, "Vendor ID", "VendorID", new nmiCtrlNumber { ParentFld = 210 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleEnded, 241, 2, 0, "Vendor Text", "VendorText", new nmiCtrlTextArea { ParentFld = 210 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleEnded, 245, 2, 0, "Product Name", "ProductName", new nmiCtrlTextArea { ParentFld = 210 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleEnded, 250, 2, 0, "Product ID", "ProductID", new nmiCtrlTextArea { ParentFld = 210 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TextArea, 255, 2, 0, "Product Text", "ProductText", new nmiCtrlTextArea { ParentFld = 210 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleEnded, 260, 2, 0, "Serial number", "SerialNumber", new nmiCtrlTextArea { ParentFld = 210 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleEnded, 261, 2, 0, "Sensor Units", "StateSensorUnit", new nmiCtrlNumber { TileWidth = 6, TileHeight = 1, ParentFld = 210 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleEnded, 262, 2, 0, "Sensor Value Name", "StateSensorValueName", new nmiCtrlNumber { TileWidth = 6, TileHeight = 1, ParentFld = 210 });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.CollapsibleGroup, 400, 6, 0, "Sensor Settings...", null, new nmiCtrlCollapsibleGroup { ParentFld = 210, IsSmall = true, DoClose = true });
                
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 401, 6, 0, null, null, new nmiCtrlTileGroup { ParentFld = 400, TileWidth=6, TileHeight=3 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 402, 2, 0, "Publish Sensor Value", "PublishValue", new nmiCtrlSingleCheck { ParentFld = 401, TileWidth = 3 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Number, 403, 2, 0, "Publish Every (sec)", "PublishEvery", new nmiCtrlNumber { TileWidth = 3, TileHeight = 1, MinValue = 0, ParentFld = 401 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 404, 2, 0, "Sensor Category", "SensorCategory", new nmiCtrlComboBox { Options = ";Energy-Sensor;Energy-Power;Energy-Current;Energy-Voltage;Temperature;Humidity", ParentFld = 401, TileWidth = 6 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.ComboBox, 405, 2, 0, "Target Chart", "TargetChart", new nmiCtrlComboBox { ParentFld = 401, TileWidth = 6, Options = "none;Sensor Chart" });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 410, 6, 0, null, null, new nmiCtrlTileGroup { ParentFld = 400, TileWidth = 6, TileHeight = 3 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Number, 411, 2, 0, "Min Value", "StateSensorMinValue", new nmiCtrlNumber { TileWidth = 3, TileHeight = 1, MinValue = int.MinValue, ParentFld = 410 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Number, 412, 2, 0, "Max Value", "StateSensorMaxValue", new nmiCtrlNumber { TileWidth = 3, TileHeight = 1, ParentFld = 410, MinValue = int.MinValue });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Number, 414, 2, 0, "Threshold", "StateSensorAverage", new nmiCtrlNumber { TileWidth = 3, TileHeight = 1, ParentFld = 410, MinValue = int.MinValue });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 415, 2, 0, "Alarm is Low", "IsLowAlarm", new nmiCtrlSingleCheck { ParentFld = 410, TileWidth = 3 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Number, 416, 2, 0, "Digits", "Digits", new nmiCtrlNumber { TileWidth = 3, TileHeight = 1, ParentFld = 410 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Number, 417, 2, 0, "Value Divider", "ValScaleFactor", new nmiCtrlNumber { DefaultValue = "0", TileWidth = 3, TileHeight = 1, ParentFld = 410 });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 420, 6, 0, null, null, new nmiCtrlTileGroup { ParentFld = 400, TileWidth = 4, TileHeight = 3 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Number, 426, 2, 0, "Bucket Steps", "StateSensorSteps", new nmiCtrlNumber { TileWidth = 3, TileHeight = 1, ParentFld = 420 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Number, 427, 2, 0, "Update Throttle (ms)", "UpdateThrottle", new nmiCtrlNumber { TileWidth = 3, TileHeight = 1, ParentFld = 420 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Number, 428, 2, 0, "Time Until Absent (sec)", "TimeToAbsent", new nmiCtrlNumber { TileWidth = 3, TileHeight = 1, ParentFld = 420 });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.CollapsibleGroup, 430, 6, 0, "Advanced Settings...", null, new nmiCtrlCollapsibleGroup { ParentFld = 400, IsSmall = true, DoClose = true });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 431, 2, 0, "Disable Sensor", nameof(IsDisabled), new nmiCtrlSingleCheck { ParentFld = 430, TileWidth = 3 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleCheck, 432, 2, 0xC0, "Replicate", "IsGlobal", new nmiCtrlNumber { ParentFld = 430, TileWidth = 3 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Number, 433, 2, 0, "Calc Aggregation (sec)", "CalcAggregation", new nmiCtrlNumber { TileWidth = 6, TileHeight = 1, LabelFontSize=16, ParentFld = 430, DefaultValue="30" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Number, 434, 2, 0, "Bucket Calculation Interval (ms)", "BucketCalcInterval", new nmiCtrlNumber { TileWidth = 6, TileHeight = 1, LabelFontSize = 16, ParentFld = 430, DefaultValue = "3000" });

                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Number, 435, 2, 0, "History Retain (days)", "HistoryRetain", new nmiCtrlNumber { TileWidth = 6, TileHeight = 1, LabelFontSize = 16, ParentFld = 430, DefaultValue = "7" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.Number, 436, 2, 0, "History Sample Period (sec)", "HistorySamplePeriod", new nmiCtrlNumber { TileWidth = 6, TileHeight = 1, LabelFontSize = 16, ParentFld = 430, DefaultValue = "1" });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleEnded, 437, 2, 0, "History Fields", "HistoryFields", new nmiCtrlNumber { TileWidth = 6, TileHeight = 1, ParentFld = 430, DefaultValue = "QValue;QValue_Ave;QValue_Min;QValue_Max" });


                DoCreateUX(tMyForm);
                SensorForm = tMyForm;
                mIsUXInitialized = true;
            }
            return true;
        }


        public virtual void DoCreateUX(TheFormInfo tMyForm, ThePropertyBag pChartControlBag=null)
        {
            var tQVN = TheThing.GetSafePropertyString(MyBaseThing, "StateSensorValueName");
            //var tQV = TheThing.GetSafePropertyString(MyBaseThing, "StateSensorValue");
            var tQU = TheThing.GetSafePropertyString(MyBaseThing, "StateSensorUnit");

            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 12460, 0, 0, null, null, new nmiCtrlTileGroup { ParentFld = 12011, TileWidth = 6, TileHeight = 3 });
            ValueField = TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 12461, 0, 0, $"{tQVN}", "QValue", new nmiCtrlSmartLabel { ParentFld = 12460, TileWidth = 6, TileFactorY = 2, TileHeight = 3, FontSize = 96 });
            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SmartLabel, 12462, 0, 0, $"{tQU}", "StateSensorUnit", new nmiCtrlSmartLabel { ParentFld = 12460, NoTE = true, TileWidth = 6, TileFactorY = 2, TileHeight = 1, FontSize = 18, HorizontalAlignment = "right", VerticalAlignment = "top" });

            var tG = TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileGroup, 12000, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 18 });
            tG.AddOrUpdatePlatformBag(eWebPlatform.Mobile, new nmiPlatBag { MaxTileWidth = 6 });
            tG.AddOrUpdatePlatformBag(eWebPlatform.HoloLens, new nmiPlatBag { MaxTileWidth = 12 });
            tG.AddOrUpdatePlatformBag(eWebPlatform.TeslaXS, new nmiPlatBag { MaxTileWidth = 12 });

            string pSensorPicSource = TheThing.GetSafePropertyString(MyBaseThing, "StateSensorLogo");
            if (string.IsNullOrEmpty(pSensorPicSource))
                pSensorPicSource = "SENSORS/Images/SensorLogo_156x78.png";
            TheSensorNMI.CreatePerformanceHeader(MyBaseThing, tMyForm, 12010, 12000, pSensorPicSource);

            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.CollapsibleGroup, 12030, 2, 0, $"Live Chart", null, new nmiCtrlCollapsibleGroup() { ParentFld = 12000, TileWidth = 6, NoTE = true, Background = "transparent", Foreground = "black", FontSize = 10, IsSmall = true, HorizontalAlignment = "left" });//LabelClassName = "cdeTileGroupHeaderSmall SensorGroupLabel", LabelForeground = "white",
            LiveChartFld = TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.UserControl, 12031, 2, 0, $"{tQVN} Chart", "QValue", new ThePropertyBag()
                {
                    "ControlType=Live Chart",
                    "ParentFld=12030", "NoTE=true", $"Title={tQVN}",
                    "SeriesNames=[{ \"name\":\"Current Temp\", \"lineColor\":\"rgba(0,255,0,0.39)\"}, { \"name\":\"Max Temp\", \"lineColor\":\"rgba(0,0,255,0.64)\"}]", "TileWidth=6", "TileHeight=4", "Speed=500", $"MaxValue={TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorMaxValue")}", "Delay=0", "Background=rgba(0,0,0,0.01)"
                });


            TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.CollapsibleGroup, 12040, 2, 0, "Distribution Curve", null, new nmiCtrlCollapsibleGroup() { ParentFld = 12000, TileWidth = 6, NoTE = true, Background = "transparent", Foreground = "black", FontSize = 10, IsSmall = true, HorizontalAlignment = "left" });//LabelClassName = "cdeTileGroupHeaderSmall SensorGroupLabel", LabelForeground = "white",
            BucketChartFld = TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.UserControl, 12041, 0, 0, $"{tQVN} Chart", "BucketChart", new ThePropertyBag()
                {
                    "NoTE=true", "ParentFld=12040", $"SubTitle={tQVN}", $"SetSeries={{\"name\": \"{tQVN}\"}}",
                    "TileWidth=6", "TileHeight=4", "ControlType=Stack Chart",
                    $"XAxis={{ \"categories\": {mBucket.GetBuckets()} }}", $"iValue={mBucket.GetBucketArray()}"
                });

            TheSensorNMI.CreateDeviceDetails(MyBaseThing, tMyForm, 12060, 12000, null); // new List<string> { $"{tQVN},{tQV}" });

            if (pChartControlBag == null)
                pChartControlBag = new ThePropertyBag { "DoClose=true" };
            else
                ThePropertyBag.PropBagUpdateValue(pChartControlBag, "DoClose", "=", "true");
            var tt = MyHistorian?.CreateHistoryTrendUX(tMyForm, 12300, 12000, $"{tQVN} Trend", $"{tQVN} Trend", TheThing.GetSafePropertyString(MyBaseThing, "HistoryFields"), false,false,pChartControlBag);
        }

        public static bool CopyStateSensorInfo(TheThing pBaseThing)
        {
            if (pBaseThing == null) return false;
            var t = TheThingRegistry.GetThingByMID("*", TheThing.GetSafePropertyGuid(pBaseThing, "RealSensorThing"));
            if (t != null && TheThing.GetSafePropertyBool(t, "IsStateSensor"))
            {
                if (string.IsNullOrEmpty(TheThing.GetSafePropertyString(pBaseThing, "StateSensorType")))
                    TheThing.SetSafePropertyString(pBaseThing, "StateSensorType", TheThing.GetSafePropertyString(t, "StateSensorType"));
                if (string.IsNullOrEmpty(TheThing.GetSafePropertyString(pBaseThing, "StateSensorUnit")))
                    TheThing.SetSafePropertyString(pBaseThing, "StateSensorUnit", TheThing.GetSafePropertyString(t, "StateSensorUnit"));
                if (string.IsNullOrEmpty(TheThing.GetSafePropertyString(pBaseThing, "StateSensorValueName")))
                    TheThing.SetSafePropertyString(pBaseThing, "StateSensorValueName", TheThing.GetSafePropertyString(t, "StateSensorValueName"));
                if (pBaseThing.GetProperty("StateSensorAverage") == null)
                    TheThing.SetSafePropertyNumber(pBaseThing, "StateSensorAverage", TheThing.GetSafePropertyNumber(t, "StateSensorAverage"));
                if (pBaseThing.GetProperty("StateSensorMinValue") == null)
                    TheThing.SetSafePropertyNumber(pBaseThing, "StateSensorMinValue", TheThing.GetSafePropertyNumber(t, "StateSensorMinValue"));
                if (pBaseThing.GetProperty("StateSensorMaxValue") == null)
                    TheThing.SetSafePropertyNumber(pBaseThing, "StateSensorMaxValue", TheThing.GetSafePropertyNumber(t, "StateSensorMaxValue"));
                if (string.IsNullOrEmpty(TheThing.GetSafePropertyString(pBaseThing, "StateSensorIcon")))
                    TheThing.SetSafePropertyString(pBaseThing, "StateSensorIcon", TheThing.GetSafePropertyString(t, "StateSensorIcon"));
                return true;
            }
            return false;
        }

        public void AddSpeedGauge(TheFormInfo tMyForm, int pFldNumber = 25000, int pParentFld = TheDefaultSensor.SensorGaugeGroup, bool IsHighGood = false)
        {
            string Plotband = "";
            if (IsHighGood)
                Plotband = $"PlotBand=[{{ \"from\": {TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorMinValue")}, \"to\": {TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorAverage")}, \"color\": \"#FF000088\" }}, {{ \"from\": {TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorAverage")}, \"to\": {TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorMaxValue")}, \"color\": \"#00FF0044\" }}]";
            else
                Plotband = $"PlotBand=[{{ \"from\": {TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorMinValue")}, \"to\": {TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorAverage")}, \"color\": \"#00FF0088\" }}, {{ \"from\": {TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorAverage")}, \"to\": {TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorMaxValue")}, \"color\": \"#FF000044\" }}]";

            GaugeFld = TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.UserControl, pFldNumber, 0, 0, null, "QValue", new ThePropertyBag() { "ControlType=Speed Gauge", "NoTE=true",$"ParentFld={pParentFld}",
                    "TileWidth=6","TileHeight=3",
                    $"MinValue={TheThing.GetSafePropertyNumber(MyBaseThing,"StateSensorMinValue")}",
                    $"MaxValue={TheThing.GetSafePropertyNumber(MyBaseThing,"StateSensorMaxValue")}",
                    $"SubTitle={TheThing.GetSafePropertyString(MyBaseThing, "StateSensorUnit")}",
                    Plotband,
                    $"SetSeries={{ \"name\": \"{TheThing.GetSafePropertyString(MyBaseThing, "StateSensorValueName")}\",\"data\": [{TheThing.GetSafePropertyNumber(MyBaseThing,"QValue")}],\"tooltip\": {{ \"valueSuffix\": \" {TheThing.GetSafePropertyString(MyBaseThing, "StateSensorUnit")}\"}}}}",
                $"Value={MyBaseThing.Value}"
                     });
        }

        void sinkUpdateUX2(cdeP prop)
        {
            string Plotband;
            if (TheThing.GetSafePropertyBool(MyBaseThing, "IsLowAlarm"))
                Plotband = $"SubTitle={TheThing.GetSafePropertyString(MyBaseThing, "StateSensorUnit")}:;:PlotBand=[{{ \"from\": {TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorMinValue")}, \"to\": {TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorAverage")}, \"color\": \"#FF000088\" }}, {{ \"from\": {TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorAverage")}, \"to\": {TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorMaxValue")}, \"color\": \"#00FF0044\" }}]";
            else
                Plotband = $"SubTitle={TheThing.GetSafePropertyString(MyBaseThing, "StateSensorUnit")}:;:PlotBand=[{{ \"from\": {TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorMinValue")}, \"to\": {TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorAverage")}, \"color\": \"#00FF0088\" }}, {{ \"from\": {TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorAverage")}, \"to\": {TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorMaxValue")}, \"color\": \"#FF000044\" }}]";
            GaugeFld?.SetUXProperty(Guid.Empty, Plotband);
        }

        void sinkUpdateUX(cdeP prop)
        {
            if (prop.Name == "StateSensorValueName")
                ValueField?.SetUXProperty(Guid.Empty, $"Title={TheThing.GetSafePropertyString(MyBaseThing, "StateSensorValueName")}");
            if (prop.Name == "StateSensorMaxValue" || prop.Name == "StateSensorMinValue" || prop.Name == "StateSensorSteps")
            {
                LiveChartFld?.SetUXProperty(Guid.Empty, $"MaxValue={TheThing.GetSafePropertyString(MyBaseThing, "StateSensorMaxValue")}:;:MinValue={TheThing.GetSafePropertyString(MyBaseThing, "StateSensorMinValue")}");
                GaugeFld?.SetUXProperty(Guid.Empty, $"MaxValue={TheThing.GetSafePropertyString(MyBaseThing, "StateSensorMaxValue")}:;:MinValue={TheThing.GetSafePropertyString(MyBaseThing, "StateSensorMinValue")}");

                mBucket = new TheBucketChart<T>((int)TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorMinValue"), (int)TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorMaxValue"), (int)TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorSteps"), true);
                BucketChartFld?.SetUXProperty(Guid.Empty, $"XAxis={{ \"categories\": {mBucket.GetBuckets()} }}");
                TheThing.SetSafePropertyString(MyBaseThing, "BucketChart", mBucket.GetBucketArray());
            }
        }
    }
}
