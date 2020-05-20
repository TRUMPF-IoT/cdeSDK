// SPDX-FileCopyrightText: Copyright(c) 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
// SPDX-License-Identifier: MPL-2.0

using System;

// TODO: Add reference for C-DEngine.dll
// TODO: Make sure plugin file name starts with either CDMy or C-DMy
using nsCDEngine.BaseClasses;
using nsCDEngine.Engines;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.ThingService;
using nsCDEngine.ViewModels;

namespace CDMyNewNMIDemo.Demos
{
    class TheDemoBase : ICDEThing
    {
        // Base object references 
        protected TheThing MyBaseThing;      // Base thing
        private IBaseEngine MyBaseEngine;    // Base engine (service)

        // Initialization flags
        protected bool mIsInitStarted = false;
        protected bool mIsInitCompleted = false;
        protected bool mIsUXInitStarted = false;
        protected bool mIsUXInitCompleted = false;

        public TheDemoBase()
        {
        }

        public TheDemoBase(TheThing tBaseThing, ICDEPlugin pPluginBase)
        {
            MyBaseThing = tBaseThing ?? new TheThing();
            MyBaseEngine = pPluginBase.GetBaseEngine();
            MyBaseThing.EngineName = MyBaseEngine.GetEngineName();
            MyBaseThing.SetIThingObject(this);
            TheThingRegistry.RegisterThing(this);
        }

        #region - ICDEThing interface methods (rare to override)
        public bool IsInit()
        {
            return mIsInitCompleted;
        }
        public bool IsUXInit()
        {
            return mIsUXInitCompleted;
        }

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
            return MyBaseThing?.GetProperty(pName, DoCreate);
        }
        public cdeP SetProperty(string pName, object pValue)
        {
            return MyBaseThing?.SetProperty(pName, pValue);
        }

        public void RegisterEvent(string pName, Action<ICDEThing, object> pCallBack)
        {
            MyBaseThing?.RegisterEvent(pName, pCallBack);
        }
        public void UnregisterEvent(string pName, Action<ICDEThing, object> pCallBack)
        {
            MyBaseThing?.UnregisterEvent(pName, pCallBack);
        }
        public void FireEvent(string pEventName, ICDEThing sender, object pPara, bool FireAsync)
        {
            MyBaseThing?.FireEvent(pEventName, sender, pPara, FireAsync);
        }
        public bool HasRegisteredEvents(string pEventName)
        {
            if (MyBaseThing != null)
                return MyBaseThing.HasRegisteredEvents(pEventName);
            return false;
        }
        #endregion

        public bool Init()
        {
            if (!mIsInitStarted)
            {
                mIsInitStarted = true;
                MyBaseEngine.RegisterEvent(eEngineEvents.ShutdownEvent, OnShutdown);
                MyBaseThing.LastMessage = "Thing has started";
                MyBaseThing.StatusLevel = 1;
                DoInit();
                mIsInitCompleted = true;
            }
            return true;
        }

        protected virtual void DoInit()
        {

        }

        protected virtual void OnShutdown(ICDEThing pEngine, object notused)
        {

        }

        public bool CreateUX()
        {
            if (!mIsUXInitStarted)
            {
                mIsUXInitStarted = true;

                DoCreateUX();
                mIsUXInitCompleted = true;
            }
            return true;
        }

        public virtual void DoCreateUX()
        {

        }

        public virtual bool Delete()
        {
            return true;
        }

        #region Message Handling
        public virtual void HandleMessage(ICDEThing sender, object pIncoming)
        {
            TheProcessMessage pMsg = pIncoming as TheProcessMessage;
            if (pMsg == null) return;

            string[] cmd = pMsg.Message.TXT.Split(':');
            switch (cmd[0])
            {
                default:
                    break;
            }
        }
        #endregion
    }
}
