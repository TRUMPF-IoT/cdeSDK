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

namespace cdeEnergyBase
{
    public enum eEnergyThingCaps : int
    {
        EnergyProducer = 400,
        EnergyConsumer = 401,
        EnergyStorage = 402,
        EnergyBreaker = 403,
        EnergyPanel = 404,
        EnergyTank = 405
    }

    public class eEnergyMessages
    {
        public const string EnergyStorageUpdate = "EnergyStorageUpdate";
        public const string EnergyProducerUpdate = "EnergyStoragProducerUpdate";
        public const string EnergyConsumerUpdate = "EnergyConsumerUpdate";
        public const string EnergyBreakerUpdate = "EnergyBreakerUpdate";
        public const string EnergyPanelUpdate = "EnergyPanelUpdate";
        public const string EnergyTankUpdate = "EnergyTankUpdate";
    }

    [OPCUAType(UATypeNodeId = "nsu=http://c-labs.com/UA/EnergyDevices;i=1001")]
    public class TheEnergyBase : TheThingBase
    {
        [ConfigProperty]
        public int PublishInterval
        {
            get { return CU.CInt(TT.MemberGetSafePropertyNumber(MyBaseThing)); }
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
            TT.SetSafePropertyNumber(MyBaseThing, "ManufacturingCarbonFootprint", 0.001); //1KG if not specified
            TheCDEngines.RegisterNewMiniRelay("EnergyMessages");
            return base.Init();
        }

        private DateTimeOffset LastPublish = DateTimeOffset.MinValue;

        public virtual void SendEnergyData(eEnergyThingCaps pSenderType, TheEnergyData LastEnergyData, bool Force = false)
        {
            if (LastEnergyData == null || (Force == false && (PublishInterval == 0 || DateTimeOffset.Now.Subtract(LastPublish).TotalSeconds < PublishInterval)))
                return;
            LastPublish = DateTimeOffset.Now;
            LastEnergyData.Time = DateTime.Now;
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
            LastEnergyData.StationName = $"{tMessageTxt}: {MyBaseThing.FriendlyName}"; ;
            TSM msgEnergy2 = new TSM("EnergyMessages", $"{tMessageTxt}:{MyBaseThing.cdeMID}", CU.SerializeObjectToJSONString(LastEnergyData));
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
