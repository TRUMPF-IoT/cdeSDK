// SPDX-FileCopyrightText: 2009-2023 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0
using CDMyEnergy.ViewModels;
using nsCDEngine.BaseClasses;
using nsCDEngine.Engines;
using nsCDEngine.Engines.ThingService;
using System;
using CU = nsCDEngine.BaseClasses.TheCommonUtils;
using TCC = nsCDEngine.Communication.TheCommCore;
using TT = nsCDEngine.Engines.ThingService.TheThing;
using NMI = nsCDEngine.Engines.NMIService.TheNMIEngine;
using System.Collections.Generic;
using nsCDEngine.ViewModels;
using cdeEnergyBase;

namespace cdeEnergyBase
{
    public enum eEnergyThingCaps 
    {
        EnergyProducer = 400,
        EnergyConsumer = 401,
        EnergyStorage = 402,
        EnergyBreaker = 403,
        EnergyPanel = 404,
        EnergyTank = 405,
        EnergyTransformer = 406,
        EnergyDistributor = 407,
        EnergySensor = 408
    }

#pragma warning disable S101 // Types should be named in PascalCase
    public static class eEnergyMessages //NOSONAR - C-Labs Convention
#pragma warning restore S101 // Types should be named in PascalCase
    {
        public const string EnergyStorageUpdate = "EnergyStorageUpdate";
        public const string EnergyProducerUpdate = "EnergyStoragProducerUpdate";
        public const string EnergyConsumerUpdate = "EnergyConsumerUpdate";
        public const string EnergySensorUpdate = "EnergySensorUpdate";
        public const string EnergyTransformerUpdate = "EnergyTransformerUpdate";
        public const string EnergyDistributionUpdate = "EnergyDistributionUpdate";
        public const string EnergyBreakerUpdate = "EnergyBreakerUpdate";
        public const string EnergyPanelUpdate = "EnergyPanelUpdate";
        public const string EnergyTankUpdate = "EnergyTankUpdate";
        public const string EnergySiteConsumptionUpdate = "EnergySiteConsumptionUpdate";
        public const string EnergySitePowerOutageDetected = "EnergySitePowerOutageDetected";
        public const string EnergySitePowerOutageResolved = "EnergySitePowerOutageResolved";
        public const string EnergySiteOnBattery = "EnergySiteOnBattery";
        public const string EnergySiteOnGrid = "EnergySiteOnGrid";
    }

    public class TheEnergyDeviceInfo : TheDataBase
    {
        public string DeviceType { get; set; }
        public string MessageContract { get; set; }
        public string OPCUADefinitionTypeId { get; set; }
        public TheEnergyDeviceInfo()
        {

        }
        public TheEnergyDeviceInfo(string pType, string pContract, string pUATypeId)
        {
            DeviceType = pType;
            MessageContract = pContract;
            OPCUADefinitionTypeId = pUATypeId;
        }
    }

    public abstract class TheEnergyPlugin: TheEnergyBase, ICDEPlugin
    {
        /// <summary>
        /// Pointer to the IBaseEngine of this Plugin
        /// </summary>
        protected IBaseEngine MyBaseEngine;
        /// <summary>
        /// Returns the BaseEngine of this plugin (called by the C-DEngine during startup)
        /// </summary>
        /// <returns></returns>
        public virtual IBaseEngine GetBaseEngine()
        {
            return MyBaseEngine;
        }
        /// <summary>
        /// This method is called by The C-DEngine during initialization in order to register this service
        /// You must add several calls to this method for the plugin to work correctly:
        /// MyBaseEngine.SetFriendlyName("Friendly name of your plugin");
        /// MyBaseEngine.SetEngineID(new Guid("{a fixed guid you create with the GUID Tool}"));
        /// MyBaseEngine.SetPluginInfo(...All Information of the plugin for the Store...);
        /// </summary>
        /// <param name="pBase">The C-DEngine is creating a Host for this engine and hands it to the Plugin Service</param>
        public virtual void InitEngineAssets(IBaseEngine pBase)
        {
            MyBaseEngine = pBase;
            MyBaseEngine.SetEngineName(GetType().FullName);
            MyBaseEngine.SetEngineType(GetType());
            MyBaseEngine.SetEngineService(true);
            MyBaseEngine.RegisterEvent(eEngineEvents.ThingDeleted, OnThingDeleted);
        }

        public virtual void OnThingDeleted(ICDEThing pEngine, object pDeletedThing)
        {
            if (pDeletedThing is ICDEThing _)
            {
                TCC.PublishCentral(new TSM(eEngineName.ContentService, "ENERGY_DEVICE_UPDATED"), true);
            }
        }
    }

    [OPCUAVariable(cdePName = "IsOnline", UATypeNodeId = "nsu=http://c-labs.com/UA/Energy;i=6028", UAMandatory = true)]
    [OPCUAVariable(cdePName = "StatusLevel", UATypeNodeId = "nsu=http://c-labs.com/UA/Energy;i=6056", UAMandatory = true)]
    public class TheEnergyBase : TheThingBase
    {
        public static readonly List<TheEnergyDeviceInfo> Energy = new List<TheEnergyDeviceInfo>
        {
            new TheEnergyDeviceInfo ( "Energy Storage", eEnergyMessages.EnergyStorageUpdate, "nsu=http://c-labs.com/UA/Energy;i=1002" ) ,
            new TheEnergyDeviceInfo ( "Energy Producer", eEnergyMessages.EnergyProducerUpdate, "nsu=http://c-labs.com/UA/Energy;i=1004"),
            new TheEnergyDeviceInfo( "Energy Consumer", eEnergyMessages.EnergyConsumerUpdate , "nsu=http://c-labs.com/UA/Energy;i=1001"),
            new TheEnergyDeviceInfo( "Energy Transformer", eEnergyMessages.EnergyTransformerUpdate , "nsu=http://c-labs.com/UA/Energy;i=1021"),
            new TheEnergyDeviceInfo("Energy Sensor", eEnergyMessages.EnergySensorUpdate , "nsu=http://c-labs.com/UA/Energy;i=1013"),
            new TheEnergyDeviceInfo("Energy Distributor", eEnergyMessages.EnergyDistributionUpdate , "nsu=http://c-labs.com/UA/Energy;i=1000")
        };

        [OPCUAProperty(UATypeNodeId = "nsu=http://c-labs.com/UA/Energy;i=6056", UAMandatory = true)]
        public virtual string Status
        {
            get 
            {
                string t = "";
                switch (MyBaseThing.StatusLevel)
                {
                    case 0: t = "Not Running"; break;
                    case 1: t = "Running"; break;
                    case 2: t = "Warning"; break;
                    case 3: t = "Error"; break;
                    case 4: t = "Ramp Up"; break;
                    case 5: t = "Maintenance"; break;
                    case 6: t = "Shutdown"; break;
                    case 7: t = "Unknown"; break;
                }
                return t;
            }
        }

        [ConfigProperty]
        [OPCUAProperty(UATypeNodeId = "nsu=http://c-labs.com/UA/Energy;i=6014", UAMandatory = true)]
        public string Address
        {
            get { return MyBaseThing.Address; }
            set { MyBaseThing.Address=value; }
        }
        [ConfigProperty]
        [OPCUAProperty(UATypeNodeId = "nsu=http://c-labs.com/UA/Energy;i=6037", UAMandatory = true)]
        public string FriendlyName
        {
            get { return MyBaseThing.FriendlyName; }
            set { MyBaseThing.FriendlyName = value; }
        }

        [ConfigProperty]
        [OPCUAProperty(UATypeNodeId = "nsu=http://c-labs.com/UA/Energy;i=6041", UAMandatory = true)]
        public string Description
        {
            get { return TT.MemberGetSafePropertyString(MyBaseThing); }
            set { TT.MemberSetSafePropertyString(MyBaseThing, value); }
        }

        [ConfigProperty]
        public int PublishInterval
        {
            get { return CU.CInt(TT.MemberGetSafePropertyNumber(MyBaseThing)); }
            set { TT.MemberSetSafePropertyNumber(MyBaseThing, value); }
        }

        [OPCUAProperty(UATypeNodeId = "nsu=http://c-labs.com/UA/Energy;i=6023", UAMandatory = true)]
        public double CFAP
        {
            get { return CU.CDbl(TT.MemberGetSafePropertyNumber(MyBaseThing)); }
            set { TT.MemberSetSafePropertyNumber(MyBaseThing, value); }
        }
        [OPCUAProperty(UATypeNodeId = "nsu=http://c-labs.com/UA/Energy;i=6012", UAMandatory = true)]
        public double CFAR
        {
            get { return CU.CDbl(TT.MemberGetSafePropertyNumber(MyBaseThing)); }
            set { TT.MemberSetSafePropertyNumber(MyBaseThing, value); }
        }

        [OPCUAProperty(UABrowseName = "CurrentWatts")]
        public double Watts
        {
            get { return CU.CDbl(TT.MemberGetSafePropertyNumber(MyBaseThing)); }
            set { TT.MemberSetSafePropertyNumber(MyBaseThing, value); }
        }
        [OPCUAProperty(UABrowseName = "CurrentVolt")]
        public double Volts
        {
            get { return CU.CDbl(TT.MemberGetSafePropertyNumber(MyBaseThing)); }
            set { TT.MemberSetSafePropertyNumber(MyBaseThing, value); }
        }
        [OPCUAProperty(UABrowseName = "CurrentAmps")]
        public double Ampere
        {
            get { return CU.CDbl(TT.MemberGetSafePropertyNumber(MyBaseThing)); }
            set { TT.MemberSetSafePropertyNumber(MyBaseThing, value); }
        }

        public override bool Init()
        {
            CFAP = 1; //1KG if not specified
            TheCDEngines.RegisterNewMiniRelay("EnergyMessages");
            MyBaseThing.RegisterProperty(nameof(Watts));
            MyBaseThing.RegisterProperty(nameof(Volts));
            MyBaseThing.RegisterProperty(nameof(Ampere));
            var tS = GetProperty("StatusLevel", true);
            mIsInitialized = true;
            tS.RegisterEvent(eThingEvents.PropertySet, sinkStatusChanged);
            sinkStatusChanged(tS);
            return true;
        }

        public virtual void sinkStatusChanged(cdeP prop)
        {
            SetProperty("IsOnline", (IsInit() && CU.CInt(prop.GetValue()) < 3));
        }

        private DateTimeOffset LastPublish = DateTimeOffset.MinValue;

        public virtual void SendEnergyData(eEnergyThingCaps pSenderType, TheEnergyData LastEnergyData, bool Force = false)
        {
            SendEnergyData(pSenderType, LastEnergyData, MyBaseThing.cdeMID.ToString(), Force);
        }
        public virtual void SendEnergyData(eEnergyThingCaps pSenderType, TheEnergyData LastEnergyData, string pID, bool Force = false)
        {
            if (LastEnergyData == null || (!Force && (PublishInterval == 0 || DateTimeOffset.Now.Subtract(LastPublish).TotalSeconds < PublishInterval)))
                return;
            LastPublish = DateTimeOffset.Now;
            LastEnergyData.Time = DateTime.Now;
            if (LastEnergyData.StationID==Guid.Empty) 
                LastEnergyData.StationID = MyBaseThing.cdeMID;
            string tMessageTxt = eEnergyMessages.EnergyConsumerUpdate;
            switch (pSenderType)
            {
                case eEnergyThingCaps.EnergyBreaker:
                    tMessageTxt = eEnergyMessages.EnergyBreakerUpdate;
                    break;
                case eEnergyThingCaps.EnergyStorage:
                    tMessageTxt = eEnergyMessages.EnergyStorageUpdate;
                    break;
                case eEnergyThingCaps.EnergyPanel:
                    tMessageTxt = eEnergyMessages.EnergyPanelUpdate;
                    break;
                case eEnergyThingCaps.EnergyProducer:
                    tMessageTxt = eEnergyMessages.EnergyProducerUpdate;
                    break;
                case eEnergyThingCaps.EnergyTank:
                    tMessageTxt = eEnergyMessages.EnergyTankUpdate;
                    break;
            }
            if (string.IsNullOrEmpty(LastEnergyData.StationName))
                LastEnergyData.StationName = $"{tMessageTxt}: {MyBaseThing.FriendlyName}"; 
            TSM msgEnergy2 = new TSM("EnergyMessages", $"{tMessageTxt}:{pID}", CU.SerializeObjectToJSONString(LastEnergyData));
            msgEnergy2.SetNoDuplicates(true);
            TCC.PublishCentral(msgEnergy2, true);
        }
        public virtual void SendEnergyData(eEnergyThingCaps pSenderType, double pWatts, double pVolts, double pAmps)
        {
            TheEnergyData LastEnergyData = new TheEnergyData()
            {
                Watts = pWatts,
                Volts = pVolts,
                Amps = pAmps,
            };
            SendEnergyData(pSenderType, LastEnergyData);
        }
    }
}
