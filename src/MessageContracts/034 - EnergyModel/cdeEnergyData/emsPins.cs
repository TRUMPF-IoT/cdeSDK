﻿using nsCDEngine.BaseClasses;
using nsCDEngine.Engines.ThingService;
using System.Collections.Generic;

namespace cdeEnergyBase
{
    public class eEmsPinTypeName : ePinTypeName
    {
        public const string GenericEMS = "Generic EMS Pin";
        public const string GenericPower = "nsu=http://c-labs.com/UA/Energy;i=2003"; // "Generic EMS Power"
        public const string GenericFlow = "nsu=http://c-labs.com/UA/Energy;i=2000"; // "Generic EMS Flow"
        public const string GenericStorage = "Generic EMS Storage";
        public const string ACPowerFlow = "nsu=http://c-labs.com/UA/Energy;i=2004"; // "AC Power Flow"
        public const string DCPowerFlow = "nsu=http://c-labs.com/UA/Energy;i=2006"; //"DC Power Flow"
        public const string H2Flow = "nsu=http://c-labs.com/UA/Energy;i=2002"; //"H2 Flow"
        public const string WaterFlow = "nsu=http://c-labs.com/UA/Energy;i=2005"; //"Water Flow"
        public const string WaterStorage = "Water Storage";
        public const string H2Storage = "H2 Storage";
        public const string ACPowerStorage = "AC Battery";
        public const string DCPowerStorage = "DC Battery";
        public const string O2Flow = "nsu=http://c-labs.com/UA/Energy;i=2007"; //O2 Flow
        public const string Pressure = "Pressure Pin";
    }

    public class TheEMSPin : ThePin
    {
        public TheEMSPin()
        {
            PinType = ePinTypeName.Generic;
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
            MyPins = new List<ThePin>
            {
                new ThePin { PinName = "VA", Units = "VA", PinProperty="emsVA", NMIPinPosition=-1 },
                new ThePin { PinName = "Volts", Units = "v", PinProperty="emsVolts", NMIPinPosition=-1, EURange = new UARange { High = 240, Low = 110 } },
                new ThePin { PinName = "Ampere", Units = "a", PinProperty="emsAmps", NMIPinPosition=-1 },
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

            NMIPinLocation = ePinLocation.Right;
            NMIPinPosition = 5;
            NMIPinWidth = 64;
            MyPins = new List<ThePin>
            {
                new ThePin { PinName = "Volts", Units = "v", PinProperty="emsVolts", NMIPinPosition=-1, EURange = new UARange { High = 48, Low = 24 } },
                new ThePin { PinName = "Ampere", Units = "a", PinProperty="emsAmps", NMIPinPosition=-1  },
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
            Units = "l/h H2O";

            NMIPinPosition = 2;
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
            Units = "nl/h H2";

            NMIPinPosition = 1;
        }
    }
}
