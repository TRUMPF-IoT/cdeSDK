// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿/*********************************************************************
*
* Project Name: 185-ConnectionBase
*
* Description: 
*
* Date of creation: 
*
* Author: 
*
* NOTES:

*********************************************************************/
//#define TESTDIRECTUPDATES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

using nsCDEngine.Engines;
using nsCDEngine.BaseClasses;
using nsCDEngine.Communication;
using nsCDEngine.ViewModels;
using nsCDEngine.Engines.StorageService;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.ThingService;
using TheCommonMessageContracts;

namespace nsTheConnectionBase
{
    public abstract class TheConnectionBase : ICDEThing
    {
        #region - Rare to Override
        public void SetBaseThing(TheThing pThing)
        {
            MyBaseThing = pThing;
        }
        public TheThing GetBaseThing()
        {
            return MyBaseThing;
        }
        public cdeP GetProperty(string pName, bool DoCreate)
        {
            if (MyBaseThing != null)
                return MyBaseThing.GetProperty(pName, DoCreate);
            return null;
        }
        public cdeP SetProperty(string pName, object pValue)
        {
            if (MyBaseThing != null)
                return MyBaseThing.SetProperty(pName, pValue);
            return null;
        }
        public void RegisterEvent(string pName, Action<ICDEThing, object> pCallBack)
        {
            if (MyBaseThing != null)
                MyBaseThing.RegisterEvent(pName, pCallBack);
        }
        public void UnregisterEvent(string pName, Action<ICDEThing, object> pCallBack)
        {
            if (MyBaseThing != null)
                MyBaseThing.UnregisterEvent(pName, pCallBack);
        }
        public void FireEvent(string pEventName, ICDEThing sender, object pPara, bool FireAsync)
        {
            if (MyBaseThing != null)
                MyBaseThing.FireEvent(pEventName, sender, pPara, FireAsync);
        }
        public bool HasRegisteredEvents(string pEventName)
        {
            if (MyBaseThing != null)
                return MyBaseThing.HasRegisteredEvents(pEventName);
            return false;
        }
        protected TheThing MyBaseThing = null;

        protected bool mIsUXInitCalled = false;
        protected bool mIsUXInitialized = false;
        protected bool mIsInitCalled = false;
        protected bool mIsInitialized = false;
        public bool IsUXInit()
        { return mIsUXInitialized; }
        public bool IsInit()
        { return mIsInitialized; }

        #endregion

        #region ThingProperties

        [ConfigProperty(DefaultValue = false)]
        public bool AutoConnect
        {
            get { return TheThing.GetSafePropertyBool(MyBaseThing, "AutoConnect"); }
            set { TheThing.SetSafePropertyBool(MyBaseThing, "AutoConnect", value); }
        }

        bool _bIsConnected = false; // The real connection status: used to detect/prevent changing the status from outside the plugin
        public bool IsConnected
        {
            get
            {
                return TheThing.GetSafePropertyBool(MyBaseThing, nameof(IsConnected));
            }
            protected set
            {
                _bIsConnected = value;
                TheThing.SetSafePropertyBool(MyBaseThing, nameof(IsConnected), value);
                if (!_bIsConnected)
                {
                    if (MyBaseThing.StatusLevel != 0)
                    {
                        MyBaseThing.SetStatus(0, "Not connected");
                    }
                }
                else
                {
                    MyBaseThing.SetStatus(1, "Connected");
                }
            }
        }

        bool _connecting = false;
        public bool Connecting
        {
            get
            {
                return _connecting;
            }
            protected set
            {
                _connecting = value;
                TheThing.SetSafePropertyBool(MyBaseThing, "Connecting", value);
            }
        }

        bool _disconnecting = false;
        public bool Disconnecting
        {
            get { return _disconnecting; }
            protected set
            {
                _disconnecting = value;
                TheThing.SetSafePropertyBool(MyBaseThing, "Disconnecting", value);
            }
        }

        #endregion

        public
#if !NET40
            async
#endif
            virtual void HandleMessage(ICDEThing sender, object pIncoming)
        {
            TheProcessMessage pMsg = pIncoming as TheProcessMessage;
            if (pMsg == null || pMsg.Message == null) return;

            var cmd = TheCommonUtils.cdeSplit(pMsg.Message.TXT, ":", false, false);

            switch (cmd[0])
            {
                case "RUREADY":
                    if (cmd.Length > 1 && cmd[1] == TheCommonUtils.cdeGuidToString(MyBaseThing.cdeMID))
                    {
                        TheCommCore.PublishToOriginator(pMsg.Message, new TSM(pMsg.Message.ENG, "IS_READY:" + TheCommonUtils.cdeGuidToString(MyBaseThing.cdeMID), mIsInitialized.ToString()) { FLG = 8 }, true);
                    }
                    break;
                case "CONNECT_SERVER":
                    Connect();
                    break;
                case nameof(MsgConnectDisconnect):
                    {
                        var request = TheCommRequestResponse.ParseRequestMessageJSON<MsgConnectDisconnect>(pMsg.Message);
                        var responseMsg = new MsgConnectDisconnectResponse();
                        if (request == null)
                        {
                            responseMsg.Error = "Error parsing request message";
                        }
                        else
                        {
                            try
                            {
                                if (request.Connect.HasValue && request.Reconnect.HasValue)
                                {
                                    responseMsg.Error = "Can specify at most one of Connect Reconnect";
                                }
                                else if (!request.Connect.HasValue && !request.Reconnect.HasValue && !request.AutoConnect.HasValue)
                                {
                                    responseMsg.Error = "Must specify at least one of Connect Reconnect AutoConnect";
                                }
                                else
                                {
                                    if (request.Connect.HasValue)
                                    {
                                        if (request.Connect == true)
                                        {
                                            Connect();
                                        }
                                        else
                                        {
                                            Disconnect(true);
                                        }
                                    }
                                    if (request.Reconnect.HasValue)
                                    {
                                        Disconnect(true);
                                        if (request.WaitTimeBeforeReconnect.HasValue)
                                        {
                                            try
                                            {
#if !NET40
                                                await TheCommonUtils.TaskDelayOneEye(request.WaitTimeBeforeReconnect.Value, 100).ConfigureAwait(false);
#else
                                            TheCommonUtils.TaskDelayOneEye(request.WaitTimeBeforeReconnect.Value, 100).Wait();
#endif
                                            }
                                            catch (System.Threading.Tasks.TaskCanceledException) { }
                                        }
                                        Connect();
                                    }
                                    if (request.AutoConnect.HasValue)
                                    {
                                        AutoConnect = request.AutoConnect.Value;
                                    }
                                    responseMsg.Connected = IsConnected;
                                }
                            }
                            catch (Exception e)
                            {
                                responseMsg.Error = e.Message;
                            }
                        }
                        TheCommRequestResponse.PublishResponseMessageJson(pMsg.Message, responseMsg);
                    }
                    break;
            }
        }


        protected IBaseEngine MyBaseEngine;
        public TheConnectionBase(TheThing pThing, ICDEPlugin pPluginBase)
        {
            if (pThing != null)
                MyBaseThing = pThing;
            else
                MyBaseThing = new TheThing();
            MyBaseEngine = pPluginBase.GetBaseEngine();
            MyBaseThing.SetIThingObject(this);
        }

        protected TheFormInfo MyForm;
        public virtual void CreateUXBase(string formTitle)
        {
            var tFlds = TheNMIEngine.AddStandardForm(MyBaseThing, "FACEPLATE");
            MyForm = tFlds["Form"] as TheFormInfo;

            var ts = TheNMIEngine.AddStatusBlock(MyBaseThing, MyForm,3);
            ts["Group"].SetParent(1);

            var tc=TheNMIEngine.AddConnectivityBlock(MyBaseThing, MyForm, 20,
            (pMsg, bConnect) =>
            {
                if (bConnect)
                {
                    Connect();
                }
                else
                {
                    Disconnect(true);
                }
            });
            tc["Group"].SetParent(1);
        }

        public bool Delete()
        {
            if (OnDelete())
            {
                Disconnect(true);
                mIsInitialized = false;
                // TODO Properly implement delete
                return true;
            }
            return false;
        }

        public virtual bool OnDelete()
        {
            return true;
        }

        public virtual bool Init()
        {
            IsConnected = false;
            MyBaseThing.SetStatus(4, "Initializing.");
            TheThing.SetSafePropertyString(MyBaseThing, "StateSensorIcon", "/Images/iconToplogo.png");
            this.GetProperty(nameof(IsConnected), true).RegisterEvent(eThingEvents.PropertyChanged, sinkPropChanged);
            MyBaseThing.SetPublishThrottle(500);
            return true;
        }

        private void sinkPropChanged(cdeP pPara)
        {
            cdeP tProp = pPara as cdeP;
            if (tProp != null && tProp.Name == nameof(IsConnected))
            {
                if (_bIsConnected != TheCommonUtils.CBool(tProp.Value))
                {
                    TheThing.SetSafePropertyBool(MyBaseThing, nameof(IsConnected), _bIsConnected);
                }
            }
        }

        public abstract bool CreateUX();
        public abstract void Connect();
        public abstract void Disconnect(bool bDrain);

    }
}
