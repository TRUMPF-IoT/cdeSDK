// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿/*********************************************************************
*
* Project Name" 185-SenderBase
*
* Description: 
*
* Date of creation: 
*
* Author: 
*
* NOTES:
*               "FldOrder" for UX 10 to 
*********************************************************************/
//#define TESTDIRECTUPDATES
using nsCDEngine.ViewModels;
using nsTheThingToPublish;

namespace nsTheSenderBase
{
    public class TheSenderTSM : TheDataBase
    {
        public string SourceEngineName { get; set; }
        public string TargetEngineName { get; set; }
        public string TXTPattern { get; set; }
        public string AckTXTTemplate { get; set; }
        public string AckPLSTemplate { get; set; }
        public bool AckToAll { get; set; }
        public bool Disable { get; set; }
        public bool SerializeTSM { get; set; }

        // TODO Factor these out into a TheMQTTSenderTSM or TheMSBSenderTSM classes
        public bool SendAsFile { get; set; }
        public string MQTTTopicTemplate { get; set; }

        // override if new properties in a derived class need to be considered in comparisons
        internal virtual bool IsEqual(TheSenderTSM senderThingToAdd)
        {
            return
                senderThingToAdd != null
                && cdeMID == senderThingToAdd.cdeMID
                && Disable == senderThingToAdd.Disable
                && SourceEngineName == senderThingToAdd.SourceEngineName
                && TargetEngineName == senderThingToAdd.TargetEngineName
                && SerializeTSM == senderThingToAdd.SerializeTSM
                && SendAsFile == senderThingToAdd.SendAsFile
                && MQTTTopicTemplate == senderThingToAdd.MQTTTopicTemplate
                && TXTPattern == senderThingToAdd.TXTPattern
                && AckTXTTemplate == senderThingToAdd.AckTXTTemplate
                && AckPLSTemplate == senderThingToAdd.AckPLSTemplate
                && AckToAll == senderThingToAdd.AckToAll
                ;
        }

        public TheSenderTSM() : base()
        {
        }

        // Override this if you add new properties to a derivedclass that need to be initialized from a TheTSMToPublish
        internal virtual void Initialize(TheTSMToPublish senderTSMToAdd)
        {
            cdeMID = senderTSMToAdd.cdeMID;
            SourceEngineName = senderTSMToAdd.SourceEngineName;
            TargetEngineName = senderTSMToAdd.TargetEngineName;
            TXTPattern = senderTSMToAdd.TXTPattern;
            AckTXTTemplate = senderTSMToAdd.AckTXTTemplate;
            AckToAll = senderTSMToAdd.AckToAll;
            AckPLSTemplate = senderTSMToAdd.AckPLSTemplate;
            SerializeTSM = senderTSMToAdd.SerializeTSM;
            SendAsFile = senderTSMToAdd.SendAsFile;
            MQTTTopicTemplate = senderTSMToAdd.MQTTTopicTemplate;
            Disable = false;
        }

    }
    }
