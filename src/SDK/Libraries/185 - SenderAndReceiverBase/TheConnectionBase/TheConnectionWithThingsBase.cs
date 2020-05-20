// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using nsCDEngine.Engines;
using nsCDEngine.Engines.ThingService;
using nsCDEngine.BaseClasses;
using nsCDEngine.Engines.StorageService;
using nsCDEngine.ViewModels;
using System.ComponentModel;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Communication;

namespace nsTheConnectionBase
{
    public abstract class TheConnectionWithThingsBase<TConnectionThing, TConnectionThingParam> : TheConnectionBase 
        where TConnectionThing : TheConnectionThing<TConnectionThingParam>, new()
        where TConnectionThingParam : TheConnectionThingParam
    {
        public TheConnectionWithThingsBase(TheThing pThing, ICDEPlugin pPluginBase) : base(pThing, pPluginBase)
        {
        }

        protected virtual bool InitBase(string friendlyNamePrefix, string deviceType)
        {
            if (TheCommonUtils.CGuid(MyBaseThing.ID) == Guid.Empty)
            {
                MyBaseThing.ID = Guid.NewGuid().ToString();
                if (string.IsNullOrEmpty(MyBaseThing.FriendlyName))
                    MyBaseThing.FriendlyName = $"{friendlyNamePrefix}: {MyBaseThing.ID}";
            }

            MyBaseThing.LastUpdate = DateTimeOffset.Now;

            MyBaseThing.EngineName = MyBaseEngine.GetEngineName();
            MyBaseThing.DeviceType = deviceType;

            TheBaseEngine.WaitForStorageReadiness((pThing, pReady) =>
            {
                if (pReady != null)
                {
                    var receiverThings = new TheStorageMirror<TConnectionThing>(TheCDEngines.MyIStorageService);
                    receiverThings.CacheTableName = nameof(TConnectionThing) + TheThing.GetSafeThingGuid(MyBaseThing, nameof(TConnectionThing));
                    receiverThings.IsRAMStore = true;
                    receiverThings.CacheStoreInterval = 1;
                    receiverThings.IsStoreIntervalInSeconds = true;
                    receiverThings.IsCachePersistent = true;
                    receiverThings.UseSafeSave = true;
                    receiverThings.RegisterEvent(eStoreEvents.StoreReady, (args) =>
                    {
                        var result = args.Para as TSM;
                        if (result != null && result.LVL == eMsgLevel.l1_Error)
                        {
                            MyBaseThing.SetStatus(3, "Error loading things");
                            TheBaseAssets.MySYSLOG.WriteToLog(98201, TSM.L(eDEBUG_LEVELS.ESSENTIALS) ? null : new TSM(MyBaseThing.EngineName, "Initialization", eMsgLevel.l6_Debug, String.Format("Error loading things for connection {0}", this.GetBaseThing().Address)));
                        }
                        else
                        {
                            MyConnectionThings = receiverThings;
                            if (MyConnectionThingsForm != null)
                            {
                                MyConnectionThingsForm.defDataSource = MyConnectionThings.StoreMID.ToString();
                            }
                            TheBaseAssets.MySYSLOG.WriteToLog(95272, TSM.L(eDEBUG_LEVELS.FULLVERBOSE) ? null : new TSM(MyBaseThing.EngineName, "Initialization", eMsgLevel.l6_Debug, String.Format("Things loaded for connection {0}", this.GetBaseThing().Address)));

                            if (AutoConnect)
                            {
                                Connect();
                                if (!IsConnected)
                                {
                                    TheCommonUtils.cdeRunTaskAsync("receiverAutoConnect", async o =>
                                    {
                                        await TheCommonUtils.TaskDelayOneEye(30000, 100).ConfigureAwait(false);
                                        while (!IsConnected && AutoConnect && TheBaseAssets.MasterSwitch)
                                        {
                                            Connect();
                                            await TheCommonUtils.TaskDelayOneEye(30000, 100).ConfigureAwait(false);
                                        }
                                    }).ContinueWith(t => t.Exception);

                                }
                            }
                            mIsInitialized = true;
                            FireEvent(eThingEvents.Initialized, this, true, true);
                            FireEvent("ServerInit", this, IsConnected.ToString(), true);
                        }
                    }
                    );
                    receiverThings.InitializeStore(false, false);
                }
            }, true);
            return mIsInitialized;
        }

        protected TheFormInfo MyConnectionThingsForm;
        public override void CreateUXBase(string formTitle)
        {
            base.CreateUXBase(formTitle);

            // Form for the Things for which events are to be sent to the cloud
            string tDataSource = nameof(TConnectionThing);
            if (MyConnectionThings != null)
                tDataSource = MyConnectionThings.StoreMID.ToString();
            MyConnectionThingsForm = new TheFormInfo(TheThing.GetSafeThingGuid(MyBaseThing, "AzureThings_ID"), eEngineName.NMIService, $"{formTitle}: Things to Connect", tDataSource) { IsNotAutoLoading = true, AddButtonText = "Add new Thing" };
            TheNMIEngine.AddFormToThingUX(MyBaseThing, MyConnectionThingsForm, "CMyTable", $"{formTitle} Thing List", 1, 3, 0xF0, null, null, new ThePropertyBag() { "Visibility=false" });
            TheNMIEngine.AddFields(MyConnectionThingsForm, new List<TheFieldInfo> {
                    new TheFieldInfo() { FldOrder=12,DataItem="ThingMID",Flags=2, cdeA = 0xC0, Type=eFieldType.ThingPicker,Header="Thing to Connect",FldWidth=3, PropertyBag=new nmiCtrlThingPicker() { IncludeEngines=true } },
                        //new TheFieldInfo() { FldOrder=16,DataItem="PartitionKey",Flags=2,Type=eFieldType.ComboBox,Header="Partition Key for EventHub",FldWidth=3,  HelpText="",DefaultValue= TheAzureThing.PartitionKeyChoices.Aggregate ("", (a,p) => a+p+";").TrimEnd(';') },
                    });

            TheNMIEngine.AddFields(MyConnectionThingsForm, new List<TheFieldInfo>
                {
                    // TODO Add Folder button?
                    {  new TheFieldInfo() { cdeA=0x80, FldOrder=100,DataItem="CDE_DELETE",Flags=2,Type=eFieldType.TileButton, TileLeft=0,TileTop=9,TileWidth=1,TileHeight=1 }},
                });


            // Add link to AzureThing list to AzureReceiver form
            TheNMIEngine.AddSmartControl(MyBaseThing, MyForm, eFieldType.TileButton, 11, 2, 0xF0, "Show Receive Thing List", null, new nmiCtrlTileButton() { NoTE=true, OnClick=$"TTS:{MyConnectionThingsForm.cdeMID}", ClassName="cdeTransitButton", ParentFld=3 });
        }

        protected TheStorageMirror<TConnectionThing> MyConnectionThings = null;

        public override void HandleMessage(ICDEThing sender, object pIncoming)
        {
            TheProcessMessage pMsg = pIncoming as TheProcessMessage;
            if (pMsg == null || pMsg.Message == null) return;

            var cmd = TheCommonUtils.cdeSplit(pMsg.Message.TXT, ":", false, false);

            switch (cmd[0])
            {
                case nameof(MsgAddConnectionThing<TConnectionThingParam>):
                    var addMsg = TheCommRequestResponse.ParseRequestMessageJSON<MsgAddConnectionThing<TConnectionThingParam>>(pMsg.Message);

                    var responseMsg = new MsgAddConnectionThingResponse { Error = "Unexpected" };
                    if (addMsg != null)
                    {
                        var thingToAdd = addMsg.ThingToAdd;

                        if (thingToAdd != null)
                        {
                            var currentThing = MyConnectionThings.MyMirrorCache.GetEntryByID(thingToAdd.cdeMID);
                            var newThing = new TConnectionThing();
                            newThing.Initialize(thingToAdd);

                            if (currentThing == null)
                            {
                                MyConnectionThings.AddAnItem(newThing);
                                responseMsg.Error = null;
                            }
                            else
                            {
                                if (!newThing.IsEqual(currentThing))
                                {
                                    UpdateConnectionThing(currentThing, newThing);
                                    MyConnectionThings.UpdateItem(newThing);
                                    Connect();
                                }
                                responseMsg.Error = null;
                            }
                        }
                        else
                        {
                            responseMsg.Error = "INVALIDARGS";
                        }
                    }
                    TheCommRequestResponse.PublishResponseMessageJson(pMsg.Message, responseMsg);
                    break;
                default:
                    base.HandleMessage(sender, pIncoming);
                    break;
            }
        }

        internal abstract void UpdateConnectionThing(TConnectionThing currentThing, TConnectionThing newThing);
    }

    public class MsgAddConnectionThing<TConnectionThingParam> where TConnectionThingParam : TheConnectionThingParam
    {
        public TConnectionThingParam ThingToAdd;
    }

    public class MsgAddConnectionThingResponse
    {
        public string Error;
    }

    public class TheConnectionThing<TConnectionThingParam> : TheDataBase, INotifyPropertyChanged where TConnectionThingParam : TheConnectionThingParam
    {
        public string ThingMID { get; set; }
        public string EventFormat { get; set; }

        public TheConnectionThing(TConnectionThingParam thingParam)
        {
            Initialize(thingParam);
        }

        public virtual void Initialize(TConnectionThingParam thingParam)
        {
            cdeMID = this.cdeMID;
            EventFormat = this.EventFormat;
            ThingMID = this.ThingMID;
        }

        public TheConnectionThing()
        {
        }
        private TheThing _pThing = null;
        public TheThing GetThing()
        {
            if (_pThing == null)
            {
                _pThing = TheThingRegistry.GetThingByMID("*", TheCommonUtils.CGuid(this.ThingMID), true);
            }
            return _pThing;
        }

        internal virtual bool IsEqual(TheConnectionThing<TConnectionThingParam> senderThingToAdd)
        {
            return
                   cdeMID == senderThingToAdd.cdeMID
                && EventFormat == senderThingToAdd.EventFormat
                && ThingMID == senderThingToAdd.ThingMID;
        }

    }

    public class TheConnectionThingParam
    {
        public TheConnectionThingParam()
        {
        }

        public Guid cdeMID { get; set; }
        public string ThingMID { get; set; }
        public string EventFormat { get; set; }
    }

}

