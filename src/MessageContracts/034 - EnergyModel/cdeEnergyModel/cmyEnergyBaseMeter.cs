// SPDX-FileCopyrightText: 2009-2023 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0
using nsCDEngine.BaseClasses;
using nsCDEngine.Engines;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.ThingService;
using nsCDEngine.ViewModels;
using System;
using System.Collections.Generic;
using CU = nsCDEngine.BaseClasses.TheCommonUtils;
using NMI = nsCDEngine.Engines.NMIService.TheNMIEngine;
using TT = nsCDEngine.Engines.ThingService.TheThing;

namespace cdeEnergyBase
{
    [OPCUAType(UATypeNodeId = "nsu=http://c-labs.com/UA/Energy;i=1015")]
    public class TheBaseEnergyMeter : TheEnergyBase
    {
        [OPCUAVariable(UABrowseName = "Is Online", UAMandatory = true, UADescription = "Is true if the device is online", UATypeNodeId = "nsu=http://c-labs.com/UA/Energy;i=6028", UASourceType = "bool")]
        public bool IsOnline
        {
            get { return TT.MemberGetSafePropertyBool(MyBaseThing); }
            set { TT.MemberSetSafePropertyBool(MyBaseThing, value); }
        }

        public override bool Init()
        {
            base.Init();
            MyBaseThing.DeclareSecureProperty(nameof(PWD), ePropertyTypes.TString);
            MyBaseThing.StatusLevel = 0;
            return true;
        }

        protected TheFormInfo MyStatusForm;
        protected TheDashPanelInfo mDashboardButton;
        protected TheFieldInfo mChart;
        protected TheFieldInfo mGauge;
        protected TheFieldInfo mTileGauge;
        public override bool CreateUX()
        {
            if (mIsUXInitCalled) return false;
            mIsUXInitCalled = true;

            var t = NMI.AddStandardForm(MyBaseThing, "FACEPLATE:/Pages/EnergyFace.html", 6, null, "Value", 0, ".Energy Consumer");
            TheDashPanelInfo tDas = t["DashIcon"] as TheDashPanelInfo;
            MyStatusForm = t["Form"] as TheFormInfo;

            NMI.AddSmartControl(MyBaseThing, MyStatusForm, eFieldType.CollapsibleGroup, 550, 2, 0, "Power Data", null, ThePropertyBag.Create(new nmiCtrlCollapsibleGroup() { TileWidth = 6, DoClose = false, IsSmall = true, ParentFld = 1 }));

            //TheNMIEngine.AddSmartControl(MyBaseThing, MyStatusForm, eFieldType.BarChart, 73, 0, 0x0, "Amps", "LastAmps", new nmiCtrlBarChart() { MaxValue = 2000, TileWidth = 6, ParentFld = 50 });
            //TheNMIEngine.AddSmartControl(MyBaseThing, MyStatusForm, eFieldType.Number, 72, 0, 0x0, "Amps", "LastAmps", new nmiCtrlBarChart() { TileWidth = 6, ParentFld = 50, UseTE = true });

            //mChart = TheNMIEngine.AddSmartControl(MyBaseThing, MyStatusForm, eFieldType.UserControl, 18, 0, 0, "Current Watts", "Value", new ThePropertyBag() { "ControlType=Speed Chart",
            //        "TileWidth=6","TileHeight=6", "MaxValue=2000", "SubTitle=Watts", "NoTE=true", "ParentFld=1",
            //        "PlotBand=[{ \"from\": 0, \"to\": 500, \"color\": \"#55BF3B\" }, { \"from\": 500, \"to\": 1000, \"color\": \"#DDDF0D\" }, { \"from\": 1000, \"to\": 2000, \"color\": \"#DF5353\" }]",
            //        "SetSeries={ \"name\": \"Watts\",\"data\": [80],\"tooltip\": { \"valueSuffix\": \" watt\"}}",
            //         });

            mGauge = NMI.AddSmartControl(MyBaseThing, MyStatusForm, eFieldType.SmartGauge, 551, 0, 0, null, "Value",
                new nmiCtrlHalfGauge { ParentFld = 550, NoTE = true, LabelForeground = "#888", TileHeight = 3, SubTitle = "Watts", MinValue = 0, MaxValue = CU.CInt(1000), TileWidth = 6, LowerLimit = 0, UpperLimit = CU.CInt(900) });

            mChart = NMI.AddSmartControl(MyBaseThing, MyStatusForm, eFieldType.UserControl, 555, 0, 0, null,"ChartValue", new ThePropertyBag() { "ControlType=Live Chart", "EngineName=CDMyC3.TheC3Service",
                    "Style='float:left;'","Title=Sensor Data", "SubTitle=Meter","TileWidth=6", "TileHeight=2", "Speed=1000","MaxValue=200", "Delay=0", "Background=rgba(0,0,0,0.01)", "ParentFld=550","NoTE=true",
                    "SetSeries={ \"name\": \"Watts\",\"data\": [80],\"tooltip\": { \"valueSuffix\": \" watt\"}}",
                    "SeriesNames=[{ \"name\":\"Watts\", \"lineColor\":\"rgba(0,255,0,0.39)\"}, { \"name\":\"Solar\", \"lineColor\":\"rgba(0,0,255,0.64)\"}, { \"name\":\"Amps\", \"lineColor\":\"rgba(0,255,255,0.64)\"}, { \"name\":\"Volts\", \"lineColor\":\"rgba(255,0,255,0.39)\"}]"
                    });
            //TheNMIEngine.AddSmartControl(MyBaseThing, MyStatusForm, eFieldType.BarChart, 56, 0, 0x0, "Watts", "Value", new nmiCtrlBarChart() { MaxValue = 2000, TileWidth = 6, ParentFld = 50 });
            //TheNMIEngine.AddSmartControl(MyBaseThing, MyStatusForm, eFieldType.Number, 99991, 0, 0x0, "Current Power", "QValue", new nmiCtrlBarChart() { TileWidth = 6, UseTE = true });

            NMI.AddSmartControl(MyBaseThing, MyStatusForm, eFieldType.CollapsibleGroup, 64, 0x2, 0x0, "Settings...", null, new nmiCtrlCollapsibleGroup() { ParentFld = 1, MaxTileWidth = 12, DoClose = true, IsSmall = true });
            NMI.AddSmartControl(MyBaseThing, MyStatusForm, eFieldType.SingleCheck, 70, 2, 0x0, "Is Site Consumer", nameof(IsSiteConsumer), new nmiCtrlSingleCheck() { TileWidth = 3, ParentFld = 64, UseTE = true });

            var ts = NMI.AddStatusBlock(MyBaseThing, MyStatusForm, 10);
            ts["Group"].SetParent(1);
            ts["Group"].PropertyBag = new nmiCtrlCollapsibleGroup { DoClose = true };
            ts["Value"].Header = "Current Power";

            mTileGauge=NMI.AddSmartControl(MyBaseThing, MyStatusForm, eFieldType.SmartGauge, 99990, 0, 0, null, "Value", new nmiCtrlHalfGauge { NoTE = true, LabelForeground = "#00aa00", Foreground = "green", TileHeight = 2, SubTitle = "Watts", MinValue = 0, MaxValue = 8000, TileWidth = 4, LowerLimit = 0, UpperLimit = 0, RenderTarget = "METER%cdeMID%" });


            DoCreateUX(MyStatusForm);
            mIsUXInitialized = true;
            return true;
        }

        public virtual void DoCreateUX(TheFormInfo pForm)
        {

        }

        public double Value
        {
            get { return CU.CDbl(TheThing.GetSafePropertyString(MyBaseThing, "Value")); }
            set
            {
                TheThing.SetSafePropertyNumber(MyBaseThing, "Value", value);
                TheThing.SetSafePropertyNumber(MyBaseThing, "QValue", value);
            }
        }
        public double LastAmps
        {
            get { return CU.CDbl(TheThing.MemberGetSafePropertyNumber(MyBaseThing)); }
            set
            {
                TheThing.MemberSetSafePropertyNumber(MyBaseThing, value);
            }
        }
        public double LastSolar
        {
            get { return CU.CDbl(TheThing.MemberGetSafePropertyNumber(MyBaseThing)); }
            set
            {
                TheThing.MemberSetSafePropertyNumber(MyBaseThing, value);
            }
        }
        public bool IsSiteConsumer
        {
            get { return TheThing.MemberGetSafePropertyBool(MyBaseThing); }
            set { TheThing.MemberSetSafePropertyBool(MyBaseThing, value); }
        }
        public bool IsActive
        {
            get { return TheThing.MemberGetSafePropertyBool(MyBaseThing); }
            set { TheThing.MemberSetSafePropertyBool(MyBaseThing, value); }
        }
        public int Interval
        {
            get { return CU.CInt(TheThing.MemberGetSafePropertyNumber(MyBaseThing)); }
            set { TheThing.MemberSetSafePropertyNumber(MyBaseThing, value); }
        }
        public DateTimeOffset LastUpdate
        {
            get { return TheThing.MemberGetSafePropertyDate(MyBaseThing); }
            set { TheThing.MemberSetSafePropertyDate(MyBaseThing, value); }
        }

        public string ModelName
        {
            get { return TheThing.MemberGetSafePropertyString(MyBaseThing); }
            set { TheThing.MemberSetSafePropertyString(MyBaseThing, value); }
        }
        public string UID
        {
            get { return TheThing.GetSafePropertyString(MyBaseThing, "UserName"); }
            set { TheThing.SetSafePropertyString(MyBaseThing, "UserName", value); }
        }
        public string PWD
        {
            get { return TheThing.GetSafePropertyString(MyBaseThing, "Password"); }
            set { TheThing.SetSafePropertyString(MyBaseThing, "Password", value); }
        }

        public int PublishDelay
        {
            get { return (int)TheThing.MemberGetSafePropertyNumber(MyBaseThing); }
            set
            {
                if (value < 60) value = 60;
                TheThing.MemberSetSafePropertyNumber(MyBaseThing, value);
            }
        }
        protected TheEnergyData LastEnergyReading = null;
        public virtual void NewMeterData(TheEnergyData data)
        {

            if (data != null)
            {
                LastEnergyReading = data;
                Watts=data.Watts;
                Ampere = data.Amps;
                Volts=data.Volts;
                LastSolar = data.SolarPower;
                //if (data.Amps > 0 && LastAmps != data.Amps) LastAmps = data.Amps; else LastEnergyReading.Amps = LastAmps;
                //if (data.Watts > 0 && Value != data.Watts) Value = data.Watts; else LastEnergyReading.Watts = Value;
                //if (data.SolarEnergy > 0 && LastSolar != data.SolarEnergy) LastSolar = data.SolarEnergy; else LastEnergyReading.SolarEnergy = LastSolar;
            }
            else
                return;
            if (IsSending) return;
            IsSending = true;

            List<TheChartPoint> tPoint = new List<TheChartPoint>();
            tPoint.Add(new TheChartPoint() { name = "Volts", value = data.Volts });
            tPoint.Add(new TheChartPoint() { name = "Amps", value = data.Amps });
            tPoint.Add(new TheChartPoint() { name = "Watts", value = data.Watts });
            tPoint.Add(new TheChartPoint() { name = "Solar", value = data.SolarPower });
            TheThing.SetSafePropertyString(MyBaseThing, "ChartValue", TheCommonUtils.SerializeObjectToJSONString(tPoint));

            if (DateTimeOffset.Now.Subtract(LastUpdate).TotalSeconds < PublishDelay)
            {
                IsSending = false;
                return;
            }
            LastUpdate = DateTime.Now;
            data.StationName = FriendlyName;
            LastEnergyReading = data;
            IsSending = false;
        }

        private bool IsSending = false;
        protected IBaseEngine MyBaseEngine;

        public virtual bool Stop()
        {
            return true;
        }
        public virtual bool Start()
        {
            return true;
        }

        public TheBaseEnergyMeter(TheThing pThing, IBaseEngine pEngine, string id)
        {
            if (pThing != null)
                MyBaseThing = pThing;
            else
                MyBaseThing = new TheThing();
            MyBaseEngine = pEngine;
            MyBaseThing.EngineName = pEngine.GetEngineName();

            if (PublishDelay < 10)
                PublishDelay = 10;
            if (!string.IsNullOrEmpty(id))
                MyBaseThing.ID = id;
            MyBaseThing.AddCapability(eThingCaps.EnergyMeter);
            MyBaseThing.SetIThingObject(this);
        }
    }
}
