using nsCDEngine.Engines.ThingService;
using System.Collections.Generic;

namespace cdeEnergyBase
{
    public class eEmsPinTypeName : ePinTypeName
    {
        public const string GenericEMS = "Generic EMS Pin";
        public const string GenericPower = "nsu=http://c-labs.com/UA/Energy;i=2003"; // "Generic EMS Power";
        public const string GenericFlow = "nsu=http://c-labs.com/UA/Energy;i=2000"; // "Generic EMS Flow";
        public const string GenericStorage = "Generic EMS Storage";
        public const string ACPowerFlow = "nsu=http://c-labs.com/UA/Energy;i=2004"; // "AC Power Flow";
        public const string DCPowerFlow = "nsu=http://c-labs.com/UA/Energy;i=2006"; //"DC Power Flow";
        public const string H2Flow = "nsu=http://c-labs.com/UA/Energy;i=2002"; //"H2 Flow";
        public const string WaterFlow = "nsu=http://c-labs.com/UA/Energy;i=2005"; //"Water Flow";
        public const string WaterStorage = "Water Storage";
        public const string H2Storage = "H2 Storage";
        public const string ACPowerStorage = "AC Battery";
        public const string DCPowerStorage = "DC Battery";
        public const string O2Flow = "Oxigen Flow";
        public const string Pressure = "Pressure Pin";
    }

    public class TheEMSPin : ThePin
    {
        public TheEMSPin()
        {
            PinType = eEmsPinTypeName.Generic;
        }
        public override string NMIGetPinLineFace()
        {
            if (NMIPinTopPosition < 0)
                return "";
            string dire = "left";
            string fdire = "right";
            if (NMIIsPinRight)
            {
                dire = "right";
                if (IsInbound)
                    fdire = "left";
            }
            else
            {
                if (!IsInbound)
                    fdire = "left";
            }
            ThePin tP2 = null;
            if (MyPins?.Count > 0)
                tP2 = MyPins.Find(s => s.NMIPinTopPosition >= 0);
            return $"""
                 <div class="emsPinDiv">
                    {(PinProperty == null ? "" : $"""<div class="emsPinTopLabel_{dire}"><%C12:1:{PinProperty}%> {Units}</div>""")}
                    <div cdeTAG="<%C:{PinProperty}_css%>">
                        <div class="{NMIClass}_{fdire}" style="animation-delay: 0s;"></div>
                        <div class="{NMIClass}_{fdire}" style="animation-delay: 2s;"></div>
                        <div class="{NMIClass}_{fdire}" style="animation-delay: 4s;"></div>
                    </div>
                    {(tP2?.PinProperty == null ? "" : $"""<div class="emsPinBottomLabel_{dire}"><%C12:1:{tP2.PinProperty}%> {tP2.Units}</div>""")}
                </div>
                """;
        }
    }

    public class emsPowerPin : TheEMSPin
    {
        public emsPowerPin()
        {
            PinType = eEmsPinTypeName.GenericPower;
            MyPins = new List<ThePin>
            {
                new ThePin { PinName = "Volts", Units = "v" },
                new ThePin { PinName = "Ampere", Units = "a" },
            };
        }
    }

    public class emsACPowerPin : emsPowerPin
    {
        public emsACPowerPin() : base()
        {
            PinType = eEmsPinTypeName.ACPowerFlow;
            PinName = "AC Power In";
            CanConnectToPinType = new List<string> {
                eEmsPinTypeName.ACPowerFlow
            };
            Units = "w";

            NMIPinWidth = 64;
            NMIClass = "emsflowPower";
            MyPins = new List<ThePin>
            {
                new ThePin { PinName = "VA", Units = "VA", PinProperty="emsVA", NMIPinTopPosition=-1 },
                new ThePin { PinName = "Volts", Units = "v", PinProperty="emsVolts", NMIPinTopPosition=-1, EURange = new UARange { High = 240, Low = 110 } },
                new ThePin { PinName = "Ampere", Units = "a", PinProperty="emsAmps", NMIPinTopPosition=-1 },
            };
        }
    }

    public class emsDCPowerPin : emsPowerPin
    {
        public emsDCPowerPin()
        {
            PinType = eEmsPinTypeName.DCPowerFlow;
            PinName = "DC Power In";
            CanConnectToPinType = new List<string> {
                eEmsPinTypeName.DCPowerFlow
            };
            Units = "w";

            NMIIsPinRight = true;
            NMIPinTopPosition = 5;
            NMIPinWidth = 64;
            NMIClass = "emsflowPowerDC";
            MyPins = new List<ThePin>
            {
                new ThePin { PinName = "Volts", Units = "v", PinProperty="emsVolts", NMIPinTopPosition=-1, EURange = new UARange { High = 48, Low = 24 } },
                new ThePin { PinName = "Ampere", Units = "a", PinProperty="emsAmps", NMIPinTopPosition=-1  },
            };
        }
    }

    public class emsFlowPin : TheEMSPin
    {
        public emsFlowPin()
        {
            PinType = eEmsPinTypeName.GenericFlow;
            MyPins = new List<ThePin>
            {
                new ThePin { PinName = "Pressure", Units = "bar" }
            };
        }
    }

    public class emsWaterFlowPin : emsFlowPin
    {
        public emsWaterFlowPin() : base()
        {
            PinType = eEmsPinTypeName.WaterFlow;
            PinName = "Water In Flow";
            CanConnectToPinType = new List<string>
            {
                eEmsPinTypeName.WaterFlow
            };
            Units = "l/min";

            NMIPinTopPosition = 2;
            NMIClass = "emsflowWater";
        }
    }

    public class emsH2FlowPin : emsFlowPin
    {
        public emsH2FlowPin() : base()
        {
            PinType = eEmsPinTypeName.H2Flow;
            PinName = "H2 In Flow";
            CanConnectToPinType = new List<string>
            {
                eEmsPinTypeName.H2Flow
            };
            Units = "nl/h";

            NMIPinTopPosition = 1;
            NMIClass = "h2flow";
        }
    }
}
