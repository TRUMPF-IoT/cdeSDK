// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

using System;

// TODO: Add reference for C-DEngine.dll
// TODO: Make sure plugin file name starts with either CDMy or C-DMy
using NMI = nsCDEngine.Engines.NMIService.TheNMIEngine;

namespace $rootnamespace$
{
    [DeviceType(DeviceType = e$rootnamespace$DeviceTypes.$safeitemrootname$, Description = "This Thing does...", Capabilities = new eThingCaps[] { /* eThingCaps.ConfigManagement */ })]
class $safeitemrootname$: TheThingBase 
    {
        // Base object references 
        protected IBaseEngine MyBaseEngine;    // Base engine (service)

// User-interface definition
TheFormInfo mMyForm;

public $safeitemrootname$(TT tBaseThing, IBaseEngine pPluginBase)
        {
            MyBaseThing = tBaseThing ?? new TT();
MyBaseEngine = pPluginBase;
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
        SetMessage("Thing has started", DateTimeOffset.Now);
        MyBaseThing.StatusLevel = 1;
        mIsInitialized = true;
    }
    return true;
}

public override bool CreateUX()
{
    if (!mIsUXInitCalled)
    {
        mIsUXInitCalled = true;

        var tFlds = NMI.AddStandardForm(MyBaseThing, MyBaseThing.FriendlyName);
        mMyForm = tFlds["Form"] as TheFormInfo;
        NMI.AddSmartControl(MyBaseThing, mMyForm, eFieldType.SingleEnded, 2, 2, 0, "Sample Value", "SampleProperty", new nmiCtrlSingleEnded { ParentFld = 1 });
        NMI.AddSmartControl(MyBaseThing, mMyForm, eFieldType.BarChart, 3, 2, 0, "Sample bar chart", "SampleProperty", new nmiCtrlBarChart { ParentFld = 1, MaxValue = 255, TileHeight = 2, Foreground = "blue" });
        mIsUXInitialized = true;
    }
    return true;
}
}
}
