// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System;
using System.Collections.Generic;
$if$ ($targetframeworkversion$ >= 3.5)using System.Linq;
$endif$using System.Text;

using nsCDEngine.Engines;
using nsCDEngine.BaseClasses;
using nsCDEngine.Communication;
using nsCDEngine.ViewModels;
using nsCDEngine.Engines.StorageService;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.ThingService;

namespace $rootnamespace$
{
	class $safeitemrootname$: TheThingBase 
    {
        IBaseEngine MyBaseEngine;
        public $safeitemrootname$(TheThing pThing, ICDEPlugin pPluginBase)
        {
            if (pThing != null)
                MyBaseThing = pThing;
            else
                MyBaseThing = new TheThing();
            MyBaseThing.SetIThingObject(this);
            MyBaseThing.EngineName = pPluginBase.GetBaseEngine().GetEngineName();
            MyBaseEngine = pPluginBase.GetBaseEngine();
            MyBaseThing.DeviceType = "MyDeviceType";
        }

        public override void Init()
        {
            if (!mIsInitCalled)
            {
                mIsInitCalled = true;

                if (TheCommonUtils.CGuid(MyBaseThing.ID) == Guid.Empty)
                    MyBaseThing.ID = Guid.NewGuid().ToString();
                if (string.IsNullOrEmpty(MyBaseThing.FriendlyName))
                    MyBaseThing.FriendlyName = "My Friendly Name";
                mIsInitialized = true;
            }
        }

        public override void CreateUX()
        {
            if (!mIsUXInitCalled)
            {
                mIsUXInitCalled = true;

                TheFormInfo tMyForm = TheNMIEngine.AddForm(new TheFormInfo(MyBaseThing) { FormTitle = "Welcome to my Demo Page", DefaultView = 1 });
                TheNMIEngine.AddFormToThingUX(MyBaseThing, tMyForm, "CMyForm", MyBaseThing.FriendlyName, 3, 3,0,"Samples","FriendlyName",null);
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleEnded, 2, 2, 0, "Sample", "Sample");
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.BarChart, 3, 2, 0, "Sample", "Sample",new List<string> {"TileHeight=2"});
                mIsUXInitialized = true;
            }
        }
    }
}
