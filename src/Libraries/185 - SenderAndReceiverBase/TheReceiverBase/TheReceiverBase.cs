// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿/*********************************************************************
*
* Project Name" 185-ReceiverBase
*
* Description: 
*
* Date of creation: 
*
* Author: 
*
* NOTES:
        "FldOrder" for UX 10 to 45 and 100 to 110
*********************************************************************/
//#define TESTDIRECTUPDATES
using System;
using System.Collections.Generic;
using nsCDEngine.Engines;
using nsCDEngine.BaseClasses;
using nsCDEngine.ViewModels;
using nsCDEngine.Engines.StorageService;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.ThingService;
using nsTheConnectionBase;

using nsTheEventConverters;

namespace nsTheReceiverBase
{
    public abstract class TheReceiverBase<TConnectionThing, TConnectionThingParam> : TheConnectionWithThingsBase<TConnectionThing, TConnectionThingParam>
        where TConnectionThing : TheConnectionThing<TConnectionThingParam>, new()
        where TConnectionThingParam : TheConnectionThingParam
    {

        #region ThingProperties

        // KPIs in UI

        public long ReceiveCount
        {
            get { return (long)TheThing.GetSafePropertyNumber(MyBaseThing, nameof(ReceiveCount)); }
            set { TheThing.SetSafePropertyNumber(MyBaseThing, nameof(ReceiveCount), value);
                TheThing.SetSafePropertyNumber(MyBaseThing, "QValue", value);
            }
        }
        public long ReceiveErrorCount
        {
            get { return (long)TheThing.GetSafePropertyNumber(MyBaseThing, nameof(ReceiveErrorCount)); }
            set { TheThing.SetSafePropertyNumber(MyBaseThing, nameof(ReceiveErrorCount), value); }
        }
        public DateTimeOffset LastReceiveTime
        {
            get { return TheThing.GetSafePropertyDate(MyBaseThing, nameof(LastReceiveTime)); }
            set { TheThing.SetSafePropertyDate(MyBaseThing, nameof(LastReceiveTime), value); }
        }

        #endregion

        public override void HandleMessage(ICDEThing sender, object pIncoming)
        {
            TheProcessMessage pMsg = pIncoming as TheProcessMessage;
            if (pMsg == null || pMsg.Message == null) return;

            switch (pMsg.Message.TXT)
            {
                default:
                    base.HandleMessage(sender, pIncoming);
                    break;
            }
        }


        public TheReceiverBase(TheThing pThing, ICDEPlugin pPluginBase) : base (pThing, pPluginBase)
        {
        }

        protected override bool InitBase(string friendlyNamePrefix, string deviceType)
        {
            TheThing.SetSafePropertyString(MyBaseThing, "StateSensorValueName", "Events Received");
            return base.InitBase(friendlyNamePrefix, deviceType);
        }

        public override void CreateUXBase(string formTitle)
        {
            base.CreateUXBase(formTitle);

            // KPI Form
            //tKPIForm = new TheFormInfo(TheThing.GetSafeThingGuid(MyBaseThing, "AzureKPI_ID"), eEngineName.NMIService, "Azure Receiver: KPIs", tDataSource) { IsNotAutoLoading = true };

            //var tFlds = TheNMIEngine.AddStandardForm(MyBaseThing, MyBaseThing.FriendlyName, 18, "AzureKPI_ID");
            //var tKPIForm = tFlds["Form"] as TheFormInfo;
            //(tFlds["DashIcon"] as TheDashPanelInfo).PropertyBag=new ThePropertyBag() { "Visibility=false" };

            TheNMIEngine.AddSmartControl(MyBaseThing, MyForm, eFieldType.CollapsibleGroup, 300, 2, 0, String.Format("Receiver KPIs: {0}", MyBaseThing.FriendlyName), null, new nmiCtrlCollapsibleGroup() { DoClose = true, ParentFld = 3, TileWidth = 6, IsSmall = true });

            TheNMIEngine.AddFields(MyConnectionThingsForm, new List<TheFieldInfo>
            {
                new TheFieldInfo() { FldOrder = 13, DataItem = "EventFormat", Flags = 2, Type = eFieldType.ComboBox, Header = "Data Serializer", FldWidth = 3, PropertyBag = new ThePropertyBag() { "Options=" + TheEventConverters.GetDisplayNamesAsSemicolonSeperatedList() } },
            });


            TheNMIEngine.AddSmartControl(MyBaseThing, MyForm, eFieldType.Number, 308, 0, 0, "Events Received", nameof(ReceiveCount), new ThePropertyBag() { "ParentFld=300", "TileHeight=1", "TileWidth=6", "FldWidth=4" });
            TheNMIEngine.AddSmartControl(MyBaseThing, MyForm, eFieldType.DateTime, 309, 0, 0, "Last Receive", nameof(LastReceiveTime), new ThePropertyBag() { "ParentFld=300", "TileHeight=1", "TileWidth=6", "FldWidth=4" });
            TheNMIEngine.AddSmartControl(MyBaseThing, MyForm, eFieldType.Number, 310, 0, 0, "Receive Errors", nameof(ReceiveErrorCount), new ThePropertyBag() { "ParentFld=300", "TileHeight=1", "TileWidth=6", "FldWidth=4" });

            // Add link to KPIs to AzureConnection form
            //TheNMIEngine.AddSmartControl(MyBaseThing, MyForm , eFieldType.TileButton, 12, 2, 0xF0, "Show KPIs", null, new nmiCtrlTileButton() { ParentFld=3, NoTE=true, OnClick=$"TTS:{tKPIForm.cdeMID}", ClassName="cdeTransitButton"  });

        }

    }

}
