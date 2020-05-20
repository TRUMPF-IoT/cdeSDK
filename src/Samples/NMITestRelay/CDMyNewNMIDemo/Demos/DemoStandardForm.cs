// SPDX-FileCopyrightText: Copyright(c) 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
// SPDX-License-Identifier: MPL-2.0

using nsCDEngine.Engines;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.ThingService;

namespace CDMyNewNMIDemo.Demos
{
    class DemoStandardForm : TheDemoBase
    {
        public DemoStandardForm(TheThing tBaseThing, ICDEPlugin pPluginBase) : base(tBaseThing, pPluginBase)
        {
            MyBaseThing.DeviceType = eTheDemoScreensTypes.DemoStandardForm;
            MyBaseThing.FriendlyName = MyBaseThing.DeviceType;

        }

        public override void DoCreateUX()
        {
            var tFlds = TheNMIEngine.AddStandardForm(MyBaseThing, "My Demo Form", 18, null, null, 0, "Demos");
            var mMyForm = tFlds["Form"] as TheFormInfo;

            var ts = TheNMIEngine.AddStatusBlock(MyBaseThing, mMyForm, 10);
            ts["Group"].SetParent(1);

            var tc = TheNMIEngine.AddStartingBlock(MyBaseThing, mMyForm, 200, (pMsg, DoStart) =>
            {
                if (DoStart)
                {
                    //dosomethins starting
                }
                else
                {
                    //do something stoppeing
                }
            });
            tc["Group"].SetParent(1);
        }
    }
}
