// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System;

// TODO: Add reference for C-DEngine.dll
// TODO: Make sure plugin file name starts with either CDMy or C-DMy
using nsCDEngine.BaseClasses;
using nsCDEngine.Engines;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.ThingService;
using nsCDEngine.ViewModels;

namespace $rootnamespace$
{
    [DeviceType(DeviceType = strDeviceType, Description = "This Thing does...", Capabilities = new[] { /* eThingCaps.ConfigManagement */ })]
	class $safeitemrootname$: TheThingBase 
    {
         protected IBaseEngine MyBaseEngine;
        // User-interface definition
        TheFormInfo mMyForm;

        //TODO: set your DeviceType
        public const string strDeviceType = "My Cool Thing Type";

        public $safeitemrootname$(TheThing tBaseThing, ICDEPlugin pPluginBase)
        {
            MyBaseThing = tBaseThing ?? new TheThing();
            MyBaseEngine = pPluginBase.GetBaseEngine();
            MyBaseThing.EngineName = MyBaseEngine.GetEngineName();
            MyBaseThing.SetIThingObject(this);
            MyBaseThing.DeviceType = strDeviceType;
        }

        public override bool Init()
        {
            if (!mIsInitCalled)
            {
                mIsInitCalled = true;
                MyBaseThing.LastMessage = "Thing has started";
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

                var tFlds = TheNMIEngine.AddStandardForm(MyBaseThing, MyBaseThing.FriendlyName);
                mMyForm = tFlds["Form"] as TheFormInfo;
                TheNMIEngine.AddSmartControl(MyBaseThing, mMyForm, eFieldType.SingleEnded, 2, 2, 0, "Sample Value", "SampleProperty", new nmiCtrlSingleEnded { ParentFld=1 });
                TheNMIEngine.AddSmartControl(MyBaseThing, mMyForm, eFieldType.BarChart, 3, 2, 0, "Sample bar chart", "SampleProperty", new nmiCtrlBarChart { ParentFld = 1, MaxValue = 255, TileHeight = 2, Foreground = "blue" });
                mIsUXInitialized = true;
            }
            return true;
        }
    }
}
