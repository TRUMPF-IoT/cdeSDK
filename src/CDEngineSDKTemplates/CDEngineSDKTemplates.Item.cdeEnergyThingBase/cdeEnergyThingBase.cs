// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0
using nsCDEngine.BaseClasses;
using nsCDEngine.Engines;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.ThingService;
using nsCDEngine.ViewModels;
using System;

// TODO: Add reference for C-DEngine.dll
// TODO: Make sure plugin file name starts with either CDMy or C-DMy
using CU = nsCDEngine.BaseClasses.TheCommonUtils;
using NMI = nsCDEngine.Engines.NMIService.TheNMIEngine;
using TCC = nsCDEngine.Communication.TheCommCore;
using TT = nsCDEngine.Engines.ThingService.TheThing;

namespace $rootnamespace$
{
    [DeviceType(DeviceType = e$rootnamespace$DeviceTypes.$safeitemrootname$, Description = "This Thing does...", Capabilities = new[] { eThingCaps.ConfigManagement })]
class $safeitemrootname$: TheThingBase
	{
        // Base object references 
        protected IBaseEngine MyBaseEngine;    // Base engine (service)

// User-interface defintion
protected TheFormInfo MyStatusForm;
protected TheDashPanelInfo MyStatusFormDashPanel;

//
// C-DEngine properties are wrapped inside C# properties.
// This is a recommended practice.
// Also recommended, the use of the 'GetSafe...' and 'SetSafe...' methods.
public bool IsConnected
{
    get { return TT.MemberGetSafePropertyBool(MyBaseThing); }
    set { TT.MemberSetSafePropertyBool(MyBaseThing, value); }
}

[ConfigProperty]
public bool AutoConnect
{
    get { return TT.MemberGetSafePropertyBool(MyBaseThing); }
    set { TT.MemberSetSafePropertyBool(MyBaseThing, value); }
}

[ConfigProperty]
public int UpdateInterval
{
    get { return CU.CInt(TT.MemberGetSafePropertyNumber(MyBaseThing)); }
    set { TT.MemberSetSafePropertyNumber(MyBaseThing, value); }
}

[ConfigProperty]
public int PublishInterval
{
    get { return CU.CInt(TT.MemberGetSafePropertyNumber(MyBaseThing)); }
    set { TT.MemberSetSafePropertyNumber(MyBaseThing, value); }
}

public double Watts
{
    get { return CU.CDbl(TT.MemberGetSafePropertyNumber(MyBaseThing)); }
    set { TT.MemberSetSafePropertyNumber(MyBaseThing, value); }
}
public double Volts
{
    get { return CU.CDbl(TT.MemberGetSafePropertyNumber(MyBaseThing)); }
    set { TT.MemberSetSafePropertyNumber(MyBaseThing, value); }
}
public double Ampere
{
    get { return CU.CDbl(TT.MemberGetSafePropertyNumber(MyBaseThing)); }
    set { TT.MemberSetSafePropertyNumber(MyBaseThing, value); }
}

public $safeitemrootname$(TheThing tBaseThing, ICDEPlugin pPluginBase)
        {
            MyBaseThing = tBaseThing ?? new TheThing();
MyBaseEngine = pPluginBase.GetBaseEngine();
            MyBaseThing.EngineName = MyBaseEngine.GetEngineName();
            MyBaseThing.SetIThingObject(this);

// TODO: Add your DeviceType to the plug-in's e$rootnamespace$DeviceTypes class
MyBaseThing.DeviceType = e$rootnamespace$DeviceTypes.$safeitemrootname$;
}

public override bool Init()
{
    if (!mIsInitCalled)
    {
        mIsInitCalled = true;
        IsConnected = false;
        SetMessage("Thing Ready", DateTimeOffset.Now);
        MyBaseThing.StatusLevel = 0;
        MyBaseEngine.RegisterEvent(eEngineEvents.ShutdownEvent, DoEndMe);
        TheThing.SetSafePropertyString(MyBaseThing, "StateSensorValueName", "Current Consumption");
        TheThing.SetSafePropertyString(MyBaseThing, "StateSensorUnit", "Watts");
        if (UpdateInterval == 0)
            UpdateInterval = 1000;
        DoInit();
        if (AutoConnect)
            Connect(null);
        base.Init();
        mIsInitialized = true;
    }
    return true;
}

protected virtual void DoInit()
{

}

protected virtual void DoEndMe(ICDEThing pEngine, object notused)
{

}


public override bool CreateUX()
{
    if (!mIsUXInitCalled)
    {
        mIsUXInitCalled = true;

        var tHead = NMI.AddStandardForm(MyBaseThing, "FACEPLATE");
        MyStatusForm = tHead["Form"] as TheFormInfo;
        MyStatusForm.PropertyBag = new nmiCtrlFormView { TileWidth = 6 };
        MyStatusFormDashPanel = tHead["DashIcon"] as TheDashPanelInfo;
        //MyStatusFormDashPanel.PropertyBag = new nmiDashboardTile { Caption = $"<span class='TileIcon'>&#xf427;</span></br>{MyBaseThing.FriendlyName}",TileWidth=3, TileHeight=3 };
        var tBlock = NMI.AddStatusBlock(MyBaseThing, MyStatusForm, 2);
        tBlock["Group"].SetParent(1);
        tBlock["Group"].PropertyBag = new nmiCtrlCollapsibleGroup { DoClose = true };

        tBlock = NMI.AddConnectivityBlock(MyBaseThing, MyStatusForm, 120, sinkConnect);
        tBlock["Group"].SetParent(1);
        NMI.AddSmartControl(MyBaseThing, MyStatusForm, eFieldType.Number, 130, 2, 0xc0, "Update every (ms)", nameof(UpdateInterval), new nmiCtrlNumber { MinValue = 200, ParentFld = 120 });
        NMI.AddSmartControl(MyBaseThing, MyStatusForm, eFieldType.Number, 140, 2, 0xc0, "Publish every (sec)", nameof(PublishInterval), new nmiCtrlNumber { MinValue = 0, ParentFld = 120 });

        NMI.AddSmartControl(MyBaseThing, MyStatusForm, eFieldType.TileGroup, 10, 0, 0, null, null, new nmiCtrlTileGroup { TileHeight = 7, ParentFld = 1, TileWidth = 6 });
        NMI.AddSmartControl(MyBaseThing, MyStatusForm, eFieldType.Number, 20, 0, 0, "Current Consumption (Watts)", nameof(Watts), new nmiCtrlNumber { ParentFld = 10 });
        NMI.AddSmartControl(MyBaseThing, MyStatusForm, eFieldType.Number, 21, 0, 0, "Volts", nameof(Volts), new nmiCtrlNumber { ParentFld = 10, TileWidth = 6 });
        NMI.AddSmartControl(MyBaseThing, MyStatusForm, eFieldType.Number, 22, 0, 0, "Amperes", nameof(Ampere), new nmiCtrlNumber { ParentFld = 10, TileWidth = 6 });

        if (TheThingRegistry.IsEngineRegistered("CDMyCharts.TheChartsEngine", true))
        {
            NMI.AddSmartControl(MyBaseThing, MyStatusForm, eFieldType.UserControl, 25, 0, 0, null, nameof(Watts), new ThePropertyBag()
                {
                    "NoTE=true","ParentFld=10",
                    "SetSeries={ \"name\": \"Watts\",\"data\": [80],\"tooltip\": { \"valueSuffix\": \" watt\"}}",
                    "TileWidth=6", "TileHeight=4", "ControlType=cdeNMI.ctrlLivePlot"
                });
        }

        DoCreateUX(tHead["Form"] as TheFormInfo);
        mIsUXInitialized = true;
    }
    return true;
}

protected virtual void DoCreateUX(TheFormInfo pForm)
{

}

void sinkConnect(TheProcessMessage pMsg, bool DoConnect)
{
    if (DoConnect)
        Connect(pMsg);
    else
        Disconnect(pMsg);
}


public virtual void Connect(TheProcessMessage pMsg)
{
    SetMessage("Thing Connected", DateTimeOffset.Now);
    MyBaseThing.StatusLevel = 1;
}

public virtual void Disconnect(TheProcessMessage pMsg)
{
    SetMessage("Thing Disconnected", DateTimeOffset.Now);
    MyBaseThing.StatusLevel = 0;
}

public override bool Delete()
{
    DoEndMe(this, null);
    //TODO: Remove any residuals in the cache folder like StorageMirrors
    //i.e. MyStorageMirror.RemoveStore();
    return true;
}

private DateTimeOffset LastPublish = DateTimeOffset.MinValue;
public virtual void SendEnergyData(double pWatts, double pVolts, double pAmps)
{
    TT.SetSafePropertyNumber(MyBaseThing, "QValue", Math.Round(pWatts, 1));
    if (PublishInterval == 0 || DateTimeOffset.Now.Subtract(LastPublish).TotalSeconds < PublishInterval) return;
    LastPublish = DateTimeOffset.Now;
    TheEnergyData LastEnergyData = new TheEnergyData()
    {
        Watts = pWatts,
        Volts = pVolts,
        Amps = pAmps,
        StationName = $"Sensor: {MyBaseThing.FriendlyName}",
        StationID = MyBaseThing.cdeMID,
        Time = DateTime.Now
    };
    TSM msgEnergy2 = new TSM("CDMyEnergy.TheCDMyEnergyEngine", "NEWENERGYREADING", TheCommonUtils.SerializeObjectToJSONString<TheEnergyData>(LastEnergyData));
    msgEnergy2.SetNoDuplicates(true);
    TCC.PublishCentral(msgEnergy2, true);
}
}

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
            Random RND = new Random((int)DateTime.Now.Ticks);
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
    private DateTime myTime;
    public DateTime Time
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
}
    #endregion
}
