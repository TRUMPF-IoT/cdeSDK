// SPDX-FileCopyrightText: Copyright(c) 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
// SPDX-License-Identifier: MPL-2.0

using nsCDEngine.BaseClasses;
using nsCDEngine.Communication;
using nsCDEngine.Engines;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.ThingService;
using nsCDEngine.ViewModels;
using System;

namespace CDMyNewNMIDemo.Demos
{
    class DemoIFrame : TheDemoBase
    {
        public DemoIFrame(TheThing tBaseThing, ICDEPlugin pPluginBase):base(tBaseThing,pPluginBase)
        {
            MyBaseThing.DeviceType = eTheDemoScreensTypes.DemoIFrame;
            MyBaseThing.FriendlyName = MyBaseThing.DeviceType;
        }

        public override void DoCreateUX()
        {
            TheFormInfo tMyLiveForm = new TheFormInfo(new Guid("{CCCCCCCC-978D-4443-BC88-543962366B81}"), eEngineName.NMIService, "BTMid:"+ MyBaseThing.cdeMID.ToString(), null)
            { DefaultView = eDefaultView.IFrame, PropertyBag = new nmiCtrlIFrameView { TileWidth = 18, TileHeight=11, Source= "http://www.chrisan.me:8080", OnIFrameLoaded = "NOWN:IFRA" } };
            TheNMIEngine.AddFormToThingUX(MyBaseThing, tMyLiveForm, "CMyForm", $"IFrame Demo:{MyBaseThing.cdeMID}", 1, 3, 0, "..Demos", null, new ThePropertyBag() {  });
            MyBaseThing.RegisterEvent($"OnLoaded:{tMyLiveForm.cdeMID}:IFRA", (sender, obj) => {
                TheProcessMessage t = obj as TheProcessMessage;
                if (t == null || t.Message.PLS== "http://www.c-labs.com")
                    return;
                TheNMIEngine.SetUXProperty(t.Message.GetOriginator(), tMyLiveForm.cdeMID, "Source=http://www.c-labs.com");
                //TheCommCore.PublishToOriginator(t.Message, new TSM(eEngineName.NMIService, "Something"));
            });
        }
    }
}
