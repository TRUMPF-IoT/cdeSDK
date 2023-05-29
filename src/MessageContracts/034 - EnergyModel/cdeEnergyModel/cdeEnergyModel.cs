// SPDX-FileCopyrightText: 2009-2021 TRUMPF Laser GmbH, authors: C-Labs
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
        PowerSensor = 406
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

    [OPCUAType(UATypeNodeId = "nsu=http://c-labs.com/UA/EnergyDevices;i=1001")]
    public class TheEnergyBase : TheThingBase
    {
        public static readonly List<TheEnergyDeviceInfo> EnergyDevices = new List<TheEnergyDeviceInfo>
        {
            new TheEnergyDeviceInfo ( "Energy Storage", eEnergyMessages.EnergyStorageUpdate, "nsu=http://c-labs.com/UA/EnergyDevices;i=1002" ) ,
            new TheEnergyDeviceInfo ( "Energy Producer", eEnergyMessages.EnergyProducerUpdate, "nsu=http://c-labs.com/UA/EnergyDevices;i=1004"),
            new TheEnergyDeviceInfo( "Energy Consumer", eEnergyMessages.EnergyConsumerUpdate , "nsu=http://c-labs.com/UA/EnergyDevices;i=1001"),
            new TheEnergyDeviceInfo( "Energy Transformer", eEnergyMessages.EnergyTransformerUpdate , "nsu=http://c-labs.com/UA/EnergyDevices;i=1021"),
            new TheEnergyDeviceInfo("Energy Sensor", eEnergyMessages.EnergySensorUpdate , "nsu=http://c-labs.com/UA/EnergyDevices;i=1013"),
            new TheEnergyDeviceInfo("Energy Distributor", eEnergyMessages.EnergyDistributionUpdate , "nsu=http://c-labs.com/UA/EnergyDevices;i=1000")
        };

        [ConfigProperty]
        public int PublishInterval
        {
            get { return CU.CInt(TT.MemberGetSafePropertyNumber(MyBaseThing)); }
            set { TT.MemberSetSafePropertyNumber(MyBaseThing, value); }
        }

        [OPCUAProperty(UABrowseName = "CarbonFootprintAtPurchase")]
        public double CFAP
        {
            get { return CU.CDbl(TT.MemberGetSafePropertyNumber(MyBaseThing)); }
            set { TT.MemberSetSafePropertyNumber(MyBaseThing, value); }
        }
        [OPCUAProperty(UABrowseName = "CarbonFootprintAtRuntime")]
        public double CFAR
        {
            get { return CU.CDbl(TT.MemberGetSafePropertyNumber(MyBaseThing)); }
            set { TT.MemberSetSafePropertyNumber(MyBaseThing, value); }
        }

        [OPCUAProperty(UABrowseName = "CurrentPower")]
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
            return base.Init();
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
