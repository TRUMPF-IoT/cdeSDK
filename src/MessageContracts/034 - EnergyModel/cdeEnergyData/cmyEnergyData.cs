// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0
using System;
using nsCDEngine.ViewModels;
using nsCDEngine.BaseClasses;

namespace CDMyEnergy.ViewModels
{
    #region Energy Data
    /// <summary>
    /// Data captured and/or calculated from the WattsUp
    /// </summary>
    public class TheEnergyData : TheDataBase
    {
        public TheEnergyData()
        {
            if (TheBaseAssets.IsInDesignMode)
            {
                Random RND=new Random((int)DateTime.Now.Ticks);
                StationName = "StationName";
                Record = "Record";
                Time = DateTime.Now;
                Watts = RND.NextDouble() * 100;
                SolarEnergy = RND.NextDouble() * 100;
                WattHours = RND.NextDouble() * 100;
                this.Amps = RND.NextDouble() * 100;
                this.Cost = RND.NextDouble() * 100;
                this.CostPerMonth = RND.NextDouble() * 100;
                this.Volts = RND.NextDouble() * 100;
                this.WattMax = RND.NextDouble() * 100;
                this.WattMin = RND.NextDouble() * 100;
                this.WHPerMonth = RND.NextDouble() * 100;
            }
            StationID = TheBaseAssets.MyServiceHostInfo.MyDeviceInfo.DeviceID;
        }

        public Guid StationID { get; set; }

        private string myStationName;
        public string StationName
        {
            get { return myStationName; }
            set { myStationName = value; }
        }
        private string myRecord;
        /// <summary> The text string </summary> 
        public string Record
        {
            get { return (myRecord); }
            set { myRecord = value; }
        }
        private DateTimeOffset myTime;
        public DateTimeOffset Time
        {
            get { return (myTime); }
            set 
            { 
                myTime = value;
                //NotifyPropertyChanged("Time");
            }
        }

        private double myWatts = -1;
        /// <summary> Watts </summary> 
        public double Watts
        {
            get { return (myWatts); }
            set
            {
                myWatts = value;
                //NotifyPropertyChanged("Watts");
            }
        }

        private double mySolarEnergy;
        public double SolarEnergy
        {
            get { return mySolarEnergy; }
            set
            {
                mySolarEnergy = value;
                //NotifyPropertyChanged("SolarEnergy");
            }
        }

        private double myWattMax;
        /// <summary> Maximum wattage reported over time </summary> 
        public double WattMax
        {
            get { return myWattMax; }
            set { myWattMax = value; }
        }

        /// <summary> Minimum wattage reported over time </summary> 
        private double myWattMin;
        public double WattMin
        {
            get { return myWattMin; }
            set { myWattMin = value; }
        }


        //Volts
        private double myVolts;
        public double Volts
        {
            get { return myVolts; }
            set { myVolts = value; }
        }

        //Amps
        private double myAmps;
        public double Amps
        {
            get { return myAmps; }
            set { myAmps = value; } //by ten
        }

        private double myWattHours;
        /// <summary> Average (or projected) watts used per hour </summary> 
        public double WattHours
        {
            get { return myWattHours; }
            set { myWattHours = value; }
        }

        private double myCost;
        /// <summary> Total cost of watts used so far </summary> 
        public double Cost
        {
            get { return myCost; }
            set { myCost = value; }
        }

        private double myWHPerMonth;
        /// <summary> Projected / averaged watt hours per month </summary> 
        public double WHPerMonth
        {
            get { return myWHPerMonth; }
            set { myWHPerMonth = value; }
        }

        //CostPerMonth
        private double myCostPerMonth;
        /// <summary> Projected / averaged cost per month </summary> 
        public double CostPerMonth
        {
            get { return myCostPerMonth; }
            set { myCostPerMonth = value; } //Ten
        }

        public double RemainingRuntime { get; set; }
        public double UPSLoad { get; set; }
    }
    #endregion
}
