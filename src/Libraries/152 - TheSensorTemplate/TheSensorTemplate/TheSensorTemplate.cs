// SPDX-FileCopyrightText: 2009-2021 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using nsCDEngine.BaseClasses;
using nsCDEngine.Engines;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.StorageService;
using nsCDEngine.Engines.ThingService;
using nsCDEngine.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace TheSensorTemplate
{
    public class TheSensorHistorian : TheSensorHistorian<TheThingStore>
    {
        public TheSensorHistorian(TheThing sender, TheBucketChart pBucket) : base(sender, pBucket) { }
        public TheSensorHistorian(TheThing sender, TheBucketChart pBucket, string propertyName, TimeSpan pSamplePeriod) : base(sender, pBucket, propertyName, pSamplePeriod) { }
    }
    public class TheSensorHistorian<T> : TheMetaDataBase where T : TheDataBase, INotifyPropertyChanged, new()
    {
        #region Historian 
        private readonly TheThing MyBaseThing;
        private TheStorageMirror<T> MySensorHistory { get { return MyBaseThing.GetHistoryStore<T>(SensorHistoryToken); }  }
        protected Guid SensorHistoryToken
        {
            get { return TheThing.GetSafePropertyGuid(MyBaseThing, nameof(SensorHistoryToken)); }
            set { TheThing.SetSafePropertyGuid(MyBaseThing, nameof(SensorHistoryToken), value); }
        }
        public string SensorHistoryStoreID
        {
            get { return MySensorHistory?.MyStoreID; }
        }
        protected string PropertyName { get; set; }
        protected TheChartDefinition MyChart;
        private readonly TheBucketChart<T> mBucket;
        public TheFieldInfo MyChartControl = null;
        int refreshCounter = 0;
        internal TheHistoryParameters historyParams;
        public TheSensorHistorian(TheThing sender, TheBucketChart<T> pBucket) : this(sender, pBucket, "QValue", TimeSpan.Zero, 30,false,true)
        { 
        }
        public TheSensorHistorian(TheThing sender, TheBucketChart<T> pBucket, string propertyName, TimeSpan pSamplePeriod, int KeepForDays=30, bool bComputeMiniStats=false, bool DontUseSafeSafeStore=true, bool ForceUseRAMStore=false)
        { 
            MyBaseThing = sender;
            PropertyName = propertyName;
            var friendlyName = string.IsNullOrEmpty(sender.FriendlyName) ? $"TheSensorHistorian {sender.cdeMID}" : sender.FriendlyName;
            var description = TheThing.GetSafePropertyString(sender, "StoreDescription");
            if (string.IsNullOrEmpty(description))
                description = $"Stores history records for the Sensor Historian of {friendlyName}";
            historyParams = new TheHistoryParameters
            {
                ComputeAvg = bComputeMiniStats,
                ComputeMax = bComputeMiniStats,
                ComputeMin = bComputeMiniStats,
                ///ComputeN = true,
                CooldownPeriod = TimeSpan.Zero,
                SamplingWindow = pSamplePeriod, 
                MaintainHistoryStore = true,
                MaxAge = new TimeSpan(KeepForDays, 0, 0, 0),
                Persistent = !TheCommonUtils.IsNetCore(),
                Properties = TheCommonUtils.CStringToList(propertyName, ';'), 
                OwnerName = friendlyName,
                HistoryStoreParameters = new TheStorageMirrorParameters
                {
                    IsRAMStore = TheCDEngines.MyIStorageService==null || ForceUseRAMStore,
                    IsCached = TheCDEngines.MyIStorageService == null || ForceUseRAMStore,
                    CacheStoreInterval = new TimeSpan(0, 0, 0, 60, 0), // CODE REVIEW: OK to store only every minute instead of 10s?
                    IsCachePersistent = TheCDEngines.MyIStorageService == null || ForceUseRAMStore,
                    UseSafeSave = !DontUseSafeSafeStore,
                    TrackInsertionOrder = true,
                    FriendlyName = friendlyName,
                    Description = description
                }
            };
            if (!sender.RestartUpdateHistory<T>(SensorHistoryToken, historyParams, null))
            {
                SensorHistoryToken = sender.RegisterForUpdateHistory<T>(historyParams);
                TheBaseAssets.MySYSLOG.WriteToLog(8003, TSM.L(eDEBUG_LEVELS.ESSENTIALS) ? null : new TSM(MyBaseThing.EngineName, "New Sensor History token generated", eMsgLevel.l3_ImportantMessage));
            }

            var sensorHistory = MySensorHistory; 
            sensorHistory.RegisterEvent(eStoreEvents.HasUpdates, OnHistoryUpdate);

            mBucket = pBucket;
            MyBaseThing.RegisterOnChange("FriendlyName", sinkUpdateFriendlyName);
            MyBaseThing.RegisterOnChange("StoreDescription", sinkUpdateStoreDescription);
            TheBaseAssets.MySYSLOG.WriteToLog(8002, TSM.L(eDEBUG_LEVELS.ESSENTIALS) ? null : new TSM(MyBaseThing.EngineName, "Sensor History Storage Started", eMsgLevel.l3_ImportantMessage));
            // CODE REVIEW / TODO Do we want to allow externally created storage mirrors to be registered with the Thing Historian? Otherwise we may have to reexpose all the storage mirror options and flavors
            FireEvent("HistorianReady");
            sinkStoreReady(null);
        }

        private void sinkUpdateStoreDescription(cdeP obj)
        {
            MySensorHistory.UpdateStore(new TheStorageMirrorParameters
            {
                Description = TheCommonUtils.CStr(obj.GetValue())
            });
        }

        private void sinkUpdateFriendlyName(cdeP obj)
        {
            MySensorHistory.UpdateStore(new TheStorageMirrorParameters
            {
                FriendlyName = TheCommonUtils.CStr(obj.GetValue())
            });
        }

        public int SetExpiration(int pSeconds)
        {
            MySensorHistory?.SetRecordExpiration(pSeconds, null);
            return pSeconds;
        }

        public void SetCacheStoreInterval(int pSeconds)
        {
            if (MySensorHistory!=null)
                MySensorHistory.CacheStoreInterval = pSeconds;
        }

        public void Reset(bool BackupFirst)
        {
            MySensorHistory?.FlushCache(BackupFirst);
        }

        public void RemoveHistorian(bool BackupFirst)
        {
            MyBaseThing.UnregisterUpdateHistory(SensorHistoryToken);
            MySensorHistory?.RemoveStore(BackupFirst);
        }

        private void sinkStoreReady(StoreEventArgs _)
        {
            if (mBucket != null)
            {
                mBucket.FillBucketFromHistorian(this);
                TheThing.SetSafePropertyString(MyBaseThing, "BucketChart", mBucket.GetBucketArray());
            }
        }

        private void OnHistoryUpdate(StoreEventArgs notUsed)
        {
            refreshCounter++;
            if (refreshCounter > 20 && TheThing.GetSafePropertyString(MyBaseThing, "ScaleFactor") == "LMI")
            {
                refreshCounter = 0;
                MyChartControl?.SetUXProperty(Guid.Empty, "RefreshData=true");
            }
        }

        public Task<List<T>> GetValuesAsync()
        {
            if (MySensorHistory.IsRAMStore)
            {
                var sensorHistory = MySensorHistory;
                if (sensorHistory == null || sensorHistory.Count == 0) return null;
                return TheCommonUtils.TaskFromResult(sensorHistory.TheValues);
            }
            TaskCompletionSource<List<T>> tcs = new TaskCompletionSource<List<T>>();
            void callback(TheStorageMirror<T>.StoreResponse resp)
            {
                tcs.TrySetResult(resp?.MyRecords);
            }
            MySensorHistory.GetRecords(callback,true);
            return tcs.Task;
        }

        internal List<T> GetValues()
        {
            if (MySensorHistory.IsRAMStore)
            {
                var sensorHistory = MySensorHistory;
                if (sensorHistory == null || sensorHistory.Count == 0) return null;
                return sensorHistory.TheValues;
            }
            return GetValuesAsync().Result;
        }
        public cdeConcurrentDictionary<string, TheFieldInfo> CreateHistoryTrendUX(TheFormInfo pForm, int pFldOrder, int pParent, string pGroupTitle, string pChartTitle, string pValName, bool HideScale = false, bool OnlyShowValue = false, ThePropertyBag pChartBag = null)
        {
            var tList = new cdeConcurrentDictionary<string, TheFieldInfo>();

            string tDefSource = "TheSensorHistory";
            var sensorHistory = MySensorHistory;

            var tListCharts = new List<TheChartValueDefinition>();
            var tProps = PropertyName.Split(';');
            if (!OnlyShowValue)
            {
                var tLabels = pValName?.Split(';');
                for (int i = 0; i < tProps.Length; i++)
                {
                    tListCharts.Add(new TheChartValueDefinition(Guid.NewGuid(), tProps[i]) { Label = string.IsNullOrEmpty(pValName) ? tProps[i] : (i < tLabels.Length ? tLabels[i] : tProps[i]) });
                }
            }
            else
            {
                tListCharts.Add(new TheChartValueDefinition(Guid.NewGuid(), tProps[0]) { Label = pValName });
            }

            if (sensorHistory != null)
                tDefSource = $"{sensorHistory.StoreMID};:;0;:;{TheThing.GetSafePropertyString(MyBaseThing, "ScaleFactor")}";
            MyChart = new TheChartDefinition(TheThing.GetSafeThingGuid(MyBaseThing, "SHIS"), pChartTitle, 2000, tDefSource, true, "", "", "", tListCharts) { SubTitleText = "" };
            MyChart.PropertyBag = new ThePropertyBag { "LatestRight=true" };

            if (pChartBag == null)
                pChartBag = new ThePropertyBag { "ParentFld=12000", ".HideSeries=QValue_Min,QValue_Max", "TileWidth=18", ".TileWidth=18" };
            else
            {
                ThePropertyBag.PropBagUpdateValue(pChartBag, "ParentFld", "=", pParent.ToString());
                ThePropertyBag.PropBagUpdateValue(pChartBag, ".HideSeries", "=", "QValue_Min,QValue_Max");
                if (!ThePropertyBag.PropBagHasValue(pChartBag, "TileWidth","="))
                    ThePropertyBag.PropBagUpdateValue(pChartBag, "TileWidth", "=", "18");
                if (!ThePropertyBag.PropBagHasValue(pChartBag, ".TileWidth", "="))
                    ThePropertyBag.PropBagUpdateValue(pChartBag, ".TileWidth", "=", "18");
            }
            if (HideScale)
                ThePropertyBag.PropBagUpdateValue(pChartBag, "HideScale", "=", "true");
            TheNMIEngine.AddChartControl(MyBaseThing, MyChart, pForm, pFldOrder, pGroupTitle, pChartTitle, false, pChartBag);
            return tList;
        }
        
        #endregion
    }

    public class TheBucketChart : TheBucketChart<TheThingStore>
    {
        public TheBucketChart() : base() { }
        public TheBucketChart(int Min, int Max, int Steps, bool AddMore = false) : base(Min, Max, Steps, AddMore) { }
    }

    public class TheBucketChart<T> where T : TheDataBase, INotifyPropertyChanged, new()
    {
        public TheBucketChart()
        {
            mSteps = 10;
        }
        public TheBucketChart(int Min, int Max, int Steps, bool AddMore=false)
        {
            if (Steps == 0)
                Steps = 10;
            if (Max < Min)
            {
                int t = Min;
                Min = Max;
                Max = t;
            }
            if (Max == Min)
                Max = Min + 100;
            mMax = Max;
            mAddMore = AddMore;
            mMin = Min;
            mSteps = Steps;
            Values = new int[(Math.Abs(Min - Max) / Steps)+1];
        }

        public int FeedValue(double pValue)
        {
            if ((!mAddMore && pValue > mMax) || pValue<mMin) return -1; //Outside the rangs
            int bucket = -1;
            try
            {
                if (mAddMore && pValue > mMax)
                    bucket = Values.Length - 1;
                else
                    bucket = (int)Math.Abs(mMin - pValue) / mSteps;
                Values[bucket]++;
            }
            catch { }
            return bucket;
        }

        public string GetBucketArray()
        {
            if (Values == null || Values.Length == 0) return "[]";
            string res = "[";
            for (int i = 0; i < Values.Length; i++)
            {
                res += Values[i];
                if (i < Values.Length - 1) res += ",";
            }
            res += "]";
            return res;
        }

        public bool FillBucketFromHistorian(TheSensorHistorian<T> pHistory)
        {
            if (pHistory == null) return false;

            var t = pHistory.GetValues();
            if (t == null || t.Count == 0) return false;
            var tVal = t.ToArray();

            for (int i=0;i<tVal.Length;i++)
            {
                FeedValue(TheCommonUtils.CDbl(TheCommonUtils.GetPropValue(tVal[i], "Value")));
                //FeedValue(TheCommonUtils.CDbl(tVal[i].Value));
                //FeedValue(TheCommonUtils.CDbl(tVal[i].PB["Value"]));
            }
            return true;
        }

        public string GetBuckets()
        {
            string res = "[";
            for (int i = mMin; i < mMax; i+=mSteps)
            {
                if (res.Length>1) res += ",";
                res += $"\"{i}\"";
            }
            if (mAddMore)
                res += ",\"Higher\"";
            res += "]";
            return res;
        }

        public override string ToString()
        {
            return GetBuckets();
        }

        readonly bool mAddMore;
        readonly int mMin;
        readonly int mMax;
        readonly int mSteps;
        readonly int[] Values;

    }
}
