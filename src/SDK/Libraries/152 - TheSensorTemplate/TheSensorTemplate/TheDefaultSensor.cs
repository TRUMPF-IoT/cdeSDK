// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System;
using nsCDEngine.Engines;
using nsCDEngine.BaseClasses;
using nsCDEngine.Communication;
using nsCDEngine.ViewModels;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.ThingService;
using System.Collections.Generic;
using TheSensorTemplate;
using System.Linq;

namespace TheSensorTemplate
{
    public class TheSensorValue : TheMetaDataBase
    {
        public string FriendlyName { get; set; }
        public double Value { get; set; }
        public string StateSensorValueName { get; set; }
        public string StateSensorType { get; set; }
        public string StateSensorUnit { get; set; }
        public double StateSensorMaxValue { get; set; }
        public double StateSensorMinValue { get; set; }
        public double StateSensorSteps { get; set; }
        public double StateSensorAverage { get; set; }

        public string TargetToken { get; set; }
        public string StationName { get; set; }
    }

    public class TheDefaultSensor : TheDefaultSensor<TheThingStore>
    {
        public TheDefaultSensor() : base() { }
        public TheDefaultSensor(TheThing tBaseThing, ICDEPlugin pPluginBase) : base(tBaseThing, pPluginBase) { }

        public const int SensorValueGroup = 12460;
        public const int SensorReportGroup1 = 12001;
        public const int SensorGaugeGroup = 12015;
        public const int SensorInfoSettingsGroup = 210;
        public const int SensorSettingsGroup = 400;
        public const int SensorActionArea = 45;
        public const int SensorActionArea2 = 46;
        public const int SensorLiveChartGroup = 12030;
        public const int SensorBucketGroup = 12040;
        public const int SensorInfoGroup = 12060;
        public const int SensorTrendGroup = 12300;

        private int PingFreq=30;
        private bool IsInCyclic = false;
        protected override void SensorCyclicCalc(long timer)
        {
            base.SensorCyclicCalc(timer);
            if ((timer % PingFreq) != 0) return;
            if (IsInCyclic) return;
            IsInCyclic = true;
            PingFreq = (int)TheThing.GetSafePropertyNumber(MyBaseEngine.GetBaseThing(), "CalcAggregation");
            if (PingFreq == 0)
                PingFreq = 30;

            var tVals = MyHistorian?.GetValues();   //TODO: This returns null if the SensorHistorian is in the SQL Server. We need to think about what we do here!
            if (tVals?.Any()==true)
            {
                string tValues = TheThing.GetSafePropertyString(MyBaseThing, "HistoryFields");
                if (tValues.Contains("QValue_Ave"))
                    TheThing.SetSafePropertyNumber(MyBaseThing, "QValue_Ave", tVals.Average(w => w.PB.ContainsKey("QValue") ? TheCommonUtils.CDbl(w.PB["QValue"]) : 0));
                if (tValues.Contains("QValue_Min"))
                    TheThing.SetSafePropertyNumber(MyBaseThing, "QValue_Min", tVals.Min(w => w.PB.ContainsKey("QValue") ? TheCommonUtils.CDbl(w.PB["QValue"]) : 0));
                if (tValues.Contains("QValue_Max"))
                    TheThing.SetSafePropertyNumber(MyBaseThing, "QValue_Max", tVals.Max(w => w.PB.ContainsKey("QValue") ? TheCommonUtils.CDbl(w.PB["QValue"]) : 0));
            }
            IsInCyclic = false;
        }
    }
    public partial class TheDefaultSensor<T> : ICDEThing where T : TheMetaDataBase, System.ComponentModel.INotifyPropertyChanged, new()
    {
        internal TheSensorHistorian<T> MyHistorian;
        protected TheBucketChart<T> mBucket = null;
        protected IBaseEngine MyBaseEngine;
        public bool IsConnected
        {
            get { return TheThing.GetSafePropertyBool(MyBaseThing, "IsConnected"); }
            set { TheThing.SetSafePropertyBool(MyBaseThing, "IsConnected", value); }
        }
        public bool AutoConnect
        {
            get { return TheThing.GetSafePropertyBool(MyBaseThing, "AutoConnect"); }
            set { TheThing.SetSafePropertyBool(MyBaseThing, "AutoConnect", value); }
        }

        public TheDefaultSensor()
        {
        }

        public static void InitAssets(IBaseEngine pBaseEngine)
        {
            pBaseEngine.RegisterCSS("/SENSORS/CSS/SensorsDark.min.css", "/SENSORS/CSS/SensorsLite.min.css", null);
        }

        public TheDefaultSensor(TheThing tBaseThing, ICDEPlugin pPluginBase)
        {
            MyBaseThing = tBaseThing ?? new TheThing();
            MyBaseThing.EngineName = pPluginBase?.GetBaseEngine().GetEngineName();
            MyBaseThing.SetIThingObject(this);
            MyBaseEngine = pPluginBase?.GetBaseEngine();
            TheThing.SetSafePropertyBool(MyBaseThing, "HistorianNoSafeSave", true);
            InitAssets(MyBaseEngine);
        }

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
            return MyBaseThing?.GetProperty(pName, DoCreate);
        }
        public cdeP SetProperty(string pName, object pValue)
        {
            return MyBaseThing?.SetProperty(pName, pValue);
        }
        public void RegisterEvent(string pName, Action<ICDEThing, object> pCallBack)
        {
            MyBaseThing?.RegisterEvent(pName, pCallBack);
        }
        public void UnregisterEvent(string pName, Action<ICDEThing, object> pCallBack)
        {
            MyBaseThing?.UnregisterEvent(pName, pCallBack);
        }
        public void FireEvent(string pEventName, ICDEThing sender, object pPara, bool FireAsync)
        {
            MyBaseThing?.FireEvent(pEventName, sender, pPara, FireAsync);
        }
        public bool HasRegisteredEvents(string pEventName)
        {
            if (MyBaseThing != null)
                return MyBaseThing.HasRegisteredEvents(pEventName);
            return false;
        }
        protected TheThing MyBaseThing;
        protected bool mIsUXInitCalled;
        protected bool mIsInit;
        protected bool mIsUXInit;
        protected bool mIsInitCalled;
        public bool IsUXInit()
        { return mIsUXInit; }
        public bool IsInit()
        { return mIsInit; }

        /// <summary>
        /// Use this to return to the system if you "Thing" is currently Active
        /// </summary>
        /// <returns>True if the "Thing" associated with this class is Alive/Active</returns>
        public bool IsAlive()
        {
            return MyBaseThing != null;
        }
        #endregion

        public virtual bool Init()
        {
            if (!mIsInitCalled)
            {
                mIsInitCalled = true;
                IsConnected = false;
                MyBaseThing.StatusLevel = 0;
                MyBaseThing.LastMessage = "Offline";
                DoInit();
                cdeP p = GetProperty(TheThing.GetSafePropertyString(MyBaseThing, "StateSensorValue"), true);
                p.RegisterEvent(eThingEvents.PropertyChanged, sinkQValChanged);
                p.cdeE |= 8;
                string tValues = TheThing.GetSafePropertyString(MyBaseThing, "HistoryFields");
                if (string.IsNullOrEmpty(tValues))
                {
                    tValues = "QValue;QValue_Ave;QValue_Min;QValue_Max";
                    TheThing.SetSafePropertyString(MyBaseThing, "HistoryFields", tValues);
                }
                int tDays = TheCommonUtils.CInt(TheThing.GetSafePropertyNumber(MyBaseThing, "HistoryRetain"));
                if (tDays==0)
                {
                    tDays = 7;
                    TheThing.SetSafePropertyNumber(MyBaseThing, "HistoryRetain", tDays);
                }
                int tSaWi = TheCommonUtils.CInt(TheThing.GetSafePropertyNumber(MyBaseThing, "HistorySamplePeriod"));
                if (tSaWi == 0)
                {
                    tSaWi = 1;
                    TheThing.SetSafePropertyNumber(MyBaseThing, "HistorySamplePeriod", tSaWi);
                }
                bool ForceUseRAMStore = TheThing.GetSafePropertyBool(MyBaseThing, "ForceUseRAMStore");
                MyHistorian = new TheSensorHistorian<T>(MyBaseThing, mBucket, tValues, new TimeSpan(0, 0, tSaWi), tDays, false, true, ForceUseRAMStore);
                MyHistorian.RegisterEvent2("HistorianReady", sinkHistReady);
                //MyHistorian.Init();
                mIsInit = true;
                if (IsUXReady)
                    CreateUX();

                var tTime = TheCommonUtils.CInt(TheThing.GetSafePropertyNumber(MyBaseThing, "UpdateThrottle"));
                if (tTime > 0)
                    MyBaseThing.SetPublishThrottle(tTime);
                Start();
            }
            return true;
        }

        void sinkHistReady(TheProcessMessage para, object sender)
        {
            FireEvent("HistorianReady", this, para, true);
        }

        /// <summary>
        /// Update cyclically the IO-Link values in the Plugin database 
        /// </summary>

        public virtual void DoInit()
        {
            TheThing.SetSafePropertyBool(MyBaseThing, "IsStateSensor", true);
            if (string.IsNullOrEmpty(TheThing.GetSafePropertyString(MyBaseThing, "StateSensorValueName")))
                TheThing.SetSafePropertyString(MyBaseThing, "StateSensorValueName", "Value");
            if (string.IsNullOrEmpty(TheThing.GetSafePropertyString(MyBaseThing, "StateSensorValue")))
                TheThing.SetSafePropertyString(MyBaseThing, "StateSensorValue", "Value");
            if (string.IsNullOrEmpty(TheThing.GetSafePropertyString(MyBaseThing, "StateSensorType")))
                TheThing.SetSafePropertyString(MyBaseThing, "StateSensorType", "analog");
            if (string.IsNullOrEmpty(MyBaseThing.ID))
            {
                MyBaseThing.ID = Guid.NewGuid().ToString();
            }
            //TheThing.SetSafePropertyNumber(MyBaseThing, "StateSensorMinValue", 0);
            if (TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorAverage") == 0)
                TheThing.SetSafePropertyNumber(MyBaseThing, "StateSensorAverage", 50);
            if (TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorMaxValue") == 0)
                TheThing.SetSafePropertyNumber(MyBaseThing, "StateSensorMaxValue", 100);
            if (TheThing.GetSafePropertyNumber(MyBaseThing, "CalcAggregation") == 0)
                TheThing.SetSafePropertyNumber(MyBaseThing, "CalcAggregation", 30);
            if (TheThing.GetSafePropertyNumber(MyBaseThing, "BucketCalcInterval") == 0)
                TheThing.SetSafePropertyNumber(MyBaseThing, "BucketCalcInterval", 3000);

            if (TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorSteps") == 0)
                TheThing.SetSafePropertyNumber(MyBaseThing, "StateSensorSteps", 30);
            mBucket = new TheBucketChart<T>((int)TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorMinValue"), (int)TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorMaxValue"), (int)TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorSteps"), true);
            GetProperty("IsGlobal", true).RegisterEvent(eThingEvents.PropertyChanged, (p) =>
            {
                if (TheCommonUtils.CBool(p.ToString()))
                    TheThingRegistry.RegisterThingGlobally(MyBaseThing);
                else
                    TheThingRegistry.UnregisterThingGlobally(MyBaseThing);
            });

            GetProperty("StateSensorValueName", true).RegisterEvent(eThingEvents.PropertyChanged, sinkUpdateUX);
            GetProperty("StateSensorMaxValue", true).RegisterEvent(eThingEvents.PropertyChanged, sinkUpdateUX);
            GetProperty("StateSensorMinValue", true).RegisterEvent(eThingEvents.PropertyChanged, sinkUpdateUX);
            GetProperty("StateSensorSteps", true).RegisterEvent(eThingEvents.PropertyChanged, sinkUpdateUX);
            GetProperty("StateSensorAverage", true).RegisterEvent(eThingEvents.PropertyChanged, sinkUpdateUX2);
            GetProperty("IsLowAlarm", true).RegisterEvent(eThingEvents.PropertyChanged, sinkUpdateUX2);
            GetProperty("StateSensorUnit", true).RegisterEvent(eThingEvents.PropertyChanged, sinkUpdateUX2);
        }


        public void Start()
        {
            if (!IsConnected)
            {
                if (StartSensor())
                {
                    MyBaseThing.StatusLevel = 4;
                    //MyBaseThing.LastMessage = "Sensor connected";
                    IsConnected = true;
                }
            }
        }
        public virtual void DoEndMe(ICDEThing pEngine, object notused)
        {
            if (IsConnected)
            {
                if (StopSensor())
                {
                    MyBaseThing.StatusLevel = 0;
                    //MyBaseThing.LastMessage = "Sensor disconnected";
                    IsConnected = false;
                }
            }
        }


        void PublishValue(double pValue)
        {
            TheSensorValue newValue = new TheSensorValue()
            {
                FriendlyName = MyBaseThing.FriendlyName,
                StationName = $"Sensor: {MyBaseThing.FriendlyName}",
                StateSensorAverage = TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorAverage"),
                StateSensorMaxValue = TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorMaxValue"),
                StateSensorMinValue = TheThing.GetSafePropertyNumber(MyBaseThing, "StateSensorMinValue"),
                StateSensorType = TheThing.GetSafePropertyString(MyBaseThing, "StateSensorType"),
                StateSensorUnit = TheThing.GetSafePropertyString(MyBaseThing, "StateSensorUnit"),
                StateSensorValueName = TheThing.GetSafePropertyString(MyBaseThing, "StateSensorValueName"),
                Value = pValue
            };
            TSM msgEnergy2 = new TSM(eEngineName.ContentService, "NEW_SENSOR_VALUE", TheCommonUtils.SerializeObjectToJSONString(newValue));
            switch (TheThing.GetSafePropertyString(MyBaseThing,"TargetChart"))
            {
                default:
                case "Sensor Chart":
                    msgEnergy2.ENG = "CDMyMSSQLStorage.TheStorageService";
                    msgEnergy2.TXT += $":{TheThing.GetSafePropertyString(MyBaseThing, "TargetChart")}";
                    break;
            }
            msgEnergy2.SetNoDuplicates(true);
            msgEnergy2.SetToServiceOnly(true);
            TheCommCore.PublishCentral(msgEnergy2, true);
        }


        public void SendEnergyData(double pWatts, double pVolts, double pAmps)
        {
            CDMyEnergy.ViewModels.TheEnergyData LastEnergyData = new CDMyEnergy.ViewModels.TheEnergyData()
            {
                Watts = pWatts,
                Volts = pVolts,
                Amps = pAmps,
                StationName = $"Sensor: {MyBaseThing.FriendlyName}",
                StationID = MyBaseThing.cdeMID,
                Time = DateTime.Now
            };
            TSM msgEnergy2 = new TSM("CDMyEnergy.TheCDMyEnergyEngine", "NEWENERGYREADING", TheCommonUtils.SerializeObjectToJSONString<CDMyEnergy.ViewModels.TheEnergyData>(LastEnergyData));
            msgEnergy2.SetNoDuplicates(true);
            TheCommCore.PublishCentral(msgEnergy2, true);
        }

        #region virtual functions

        protected virtual void SensorCyclicCalc(long timer)
        {
            //MyBaseThing.LastUpdate = DateTimeOffset.Now;
            var tta = TheThing.GetSafePropertyNumber(MyBaseThing, "TimeToAbsent");
            if (tta>0 && DateTimeOffset.Now.Subtract(LastSet).TotalSeconds > tta && MyBaseThing.StatusLevel!=7)
                MyBaseThing.StatusLevel = 7;
        }

        protected DateTimeOffset LastSet = DateTimeOffset.MinValue;
        protected DateTimeOffset LastBucketSet = DateTimeOffset.MinValue;
        protected virtual void sinkQValChanged(cdeP P)
        {
            if (P.HasChanged)
                MyBaseThing.StatusLevel = 1;
            var tTime = TheThing.GetSafePropertyNumber(MyBaseThing, "UpdateThrottle");
            if (tTime>0 && DateTimeOffset.Now.Subtract(LastSet).TotalMilliseconds < tTime)
                return;
            MyBaseThing.LastUpdate = LastSet = DateTimeOffset.Now;
            this.SetProperty("QValue", P.Value);
            if (TheThing.GetSafePropertyBool(MyBaseThing, "IsEnergySensor"))
                SendEnergyData(TheCommonUtils.CDbl(P.Value),0,0);
            if (TheThing.GetSafePropertyBool(MyBaseThing, "PublishValue"))
                PublishValue(TheCommonUtils.CDbl(P.Value));
            //MyHistorian?.AddHistorianValue(TheCommonUtils.CDbl(P.ToString()));
            if (mBucket != null)
            {
                tTime = TheThing.GetSafePropertyNumber(MyBaseThing, "BucketCalcInterval");
                if (tTime > 0 && DateTimeOffset.Now.Subtract(LastBucketSet).TotalMilliseconds < tTime)
                    return;
                LastBucketSet = DateTimeOffset.Now;
                int tVal = TheCommonUtils.CInt(P);
                mBucket.FeedValue(tVal);
                TheThing.SetSafePropertyString(MyBaseThing, "BucketChart", mBucket.GetBucketArray());
            }
        }

        public virtual bool StopSensor()
        {
            TheQueuedSenderRegistry.UnregisterHealthTimer(SensorCyclicCalc);
            return true;
        }

        public virtual bool Delete()
        {
            DoEndMe(this, null);
            MyHistorian.RemoveHistorian(false);
            return true;
        }

        public virtual void HandleMessage(ICDEThing sender, object pIncoming)
        {
        }

        public virtual bool NewStatusReceived(string pStatus1, string pStatus2)
        {
            MyBaseThing.LastMessage = $"{pStatus1} : {pStatus2}";
            return true;
        }

        public virtual bool StartSensor()
        {
            TheQueuedSenderRegistry.UnregisterHealthTimer(SensorCyclicCalc);
            TheQueuedSenderRegistry.RegisterHealthTimer(SensorCyclicCalc);
            return true;
        }

        #endregion



    }
}
