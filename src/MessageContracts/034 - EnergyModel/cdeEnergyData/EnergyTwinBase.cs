using nsCDEngine.Engines.ThingService;
using TT = nsCDEngine.Engines.ThingService.TheThing;

namespace cdeEnergyBase
{
    [OPCUAType(UATypeNodeId = "nsu=http://c-labs.com/UA/Energy;i=1001")]
    [OPCUAProperty(cdePName = "EnergyPerYear", UATypeNodeId = "nsu=http://c-labs.com/UA/Energy;i=6036", UAMandatory = true)]
    [OPCUAProperty(cdePName = "EnergyPerMonth", UATypeNodeId = "nsu=http://c-labs.com/UA/Energy;i=6035", UAMandatory = true)]
    [OPCUAProperty(cdePName = "EnergyPerHour", UATypeNodeId = "nsu=http://c-labs.com/UA/Energy;i=6034", UAMandatory = true)]
    [OPCUAProperty(cdePName = "CarbonFootprintAtRuntime", UATypeNodeId = "nsu=http://c-labs.com/UA/Energy;i=6012", UAMandatory = true)]
    [OPCUAProperty(cdePName = "CarbonFootprintAtPurchase", UATypeNodeId = "nsu=http://c-labs.com/UA/Energy;i=6023", UAMandatory = true)]
    [OPCUAVariable(cdePName = "StatusLevel", UATypeNodeId = "nsu=http://c-labs.com/UA/Energy;i=6008", UAMandatory = true)]
    public class TheEnergyTwinBase : TheUATwinBase
    {
        public TheEnergyTwinBase(TT tBaseThing, string pID) : base(tBaseThing, pID)
        {
        }

        bool _nmiUpdates;
        public bool HasNMIRelevantUpdates
        {
            private set { _nmiUpdates = value; }
            get 
            {
                var t = _nmiUpdates;
                _nmiUpdates = false;
                return t; 
            }
        }

        [OPCUAVariable(UABrowseName = "IsOnline", UAMandatory = true, UADescription = "Is true if the device is online", UATypeNodeId = "nsu=http://c-labs.com/UA/Energy;i=6028", UASourceType = "bool")]
        public bool IsOnline
        {
            get { return TT.MemberGetSafePropertyBool(MyBaseThing); }
            set
            {
                if (value != IsOnline)
                {
                    if (MyBaseThing.IsInit())
                        HasNMIRelevantUpdates = true;
                    TT.MemberSetSafePropertyBool(MyBaseThing, value);
                }
            }
        }

        [OPCUAProperty(UATypeNodeId = "nsu=http://c-labs.com/UA/Energy;i=6056", UAMandatory = true)]
        public virtual string StatusText
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
    }
}
