// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System;
using System.Collections.Generic;
using System.Text;
using nsCDEngine.BaseClasses;
using nsCDEngine.Communication;
using nsCDEngine.Engines;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.ThingService;
using nsCDEngine.ViewModels;

// TODO: Add reference for C-DEngine.dll
// TODO: Make sure plugin DLL file name starts with either CDMy or C-DMy
// TODO: Set new GUID engine id value and engine friendly name (see below).

namespace CDMyChat
{
    public class cdePluginService1 : ThePluginBase
    {
        // User-interface defintion
        TheDashboardInfo mMyDashboard;

        // TODO: Set GUID value for InitEngineAssets (in the next block)
        Guid guidEngineID = new Guid("{69C72D60-EF24-44CA-B0B4-F89D87DB99DC}");

        // TODO: Set plugin friendly name for InitEngineAssets (optional)
        String strFriendlyName = "My Chat Service";

        #region ICDEPlugin - interface methods for service (engine)
        /// <summary>
        /// InitEngineAssets - The C-DEngine calls this initialization
        /// function as part of registering this service (engine)
        /// </summary>
        /// <param name="pBase">The C-DEngine creates a base engine object.
        /// This parameter is a reference to that base engine object.
        /// We keep a copy because it will be very useful to us.
        /// </param>
        public override void InitEngineAssets(IBaseEngine pBase)
        {
            base.InitEngineAssets(pBase);
            MyBaseEngine.SetEngineID(guidEngineID);          // Unique identifier for our service (engine)
            MyBaseEngine.SetFriendlyName(strFriendlyName);

            MyBaseEngine.SetPluginInfo("This service...",       // Describe plugin for Plugin Store
                                       0,                       // pPrice - retail price (default = 0)
                                       null,                    // Custom home page - default = /ServiceID
                                       "toplogo-150.png",       // pIcon - custom icon.
                                       "C-Labs",                // pDeveloper - name of the plugin developer.
                                       "http://www.c-labs.com", // pDeveloperUrl - URL to developer home page.
                                       new List<string>() { }); // pCategories - Search categories for service.
        }
        #endregion

        public override bool Init()
        {
            if (!mIsInitCalled)
            {
                mIsInitCalled = true;
                MyBaseThing.RegisterEvent(eEngineEvents.IncomingMessage, HandleMessage);
                // Additional initialization processing goes here

                MyBaseThing.StatusLevel = 1;
                MyBaseThing.LastMessage = "Hello World service has started.";
                mIsInitialized = true;

                TheCDEngines.RegisterNewMiniRelay("CHATSTER");
                var tThing = TheThingRegistry.GetBaseEngine("CHATSTER");
                tThing.RegisterEvent(eEngineEvents.IncomingMessage, HandleMessage);
                MyBaseEngine.ProcessInitialized(); 
            }
            return true;
        }

        public override bool CreateUX()
        {
            if (!mIsUXInitCalled)
            {
                mIsUXInitCalled = true;
                mMyDashboard = TheNMIEngine.AddDashboard(MyBaseThing, new TheDashboardInfo(MyBaseEngine, "Chat Plugin"));

                var tFlds = TheNMIEngine.AddStandardForm(MyBaseThing, "Chat Room");
                var tMyForm = tFlds["Form"] as TheFormInfo;

                // Create NMI controls for chat
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleEnded, 10, 2, 0, "Name", nameof(SenderName), new nmiCtrlSingleEnded { TileWidth = 18, ParentFld = 1 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TextArea, 20, 2, 0, "", nameof(MessageFeed), new nmiCtrlTextArea { TileHeight = 4, TileWidth = 18, NoTE = true, ParentFld = 1 });
                TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.SingleEnded, 30, 2, 0, "Message", nameof(Message), new nmiCtrlSingleEnded { TileWidth = 16, ParentFld = 1 });
                TheFieldInfo sendButton = TheNMIEngine.AddSmartControl(MyBaseThing, tMyForm, eFieldType.TileButton, 40, 2, 0, "SEND", null, new nmiCtrlTileButton { NoTE = true, TileWidth = 2, ParentFld = 1 });
                sendButton.RegisterUXEvent(MyBaseThing, eUXEvents.OnClick, "", OnSendButtonClick);

                TheNMIEngine.AddAboutButton4(MyBaseThing, mMyDashboard, null, true);
                mIsUXInitialized = true;
            }
            return true;
        }

        private void OnSendButtonClick(ICDEThing arg1, object arg2)
        {
            var msgHello = new MsgChatHello
            {
                Message = this.Message,
                SenderName = this.SenderName
            };

            var target = new TheMessageAddress
            {
                EngineName = "CHATSTER", // Send to this engine
                Node = Guid.Empty // Send to all nodes in the mesh
            };
            Message = "";
            var response = TheCommRequestResponse.PublishRequestJSonAsync<MsgChatHello, MsgChatHelloResponse>(
                MyBaseThing,
                target,
                msgHello,
                new TimeSpan(0, 0, 10)) // Wait 10 seconds for acknowledge message
                .Result;

            if(response == null)
            {
                PublishToast("Error sending message or no acknowledge received.");
            }
            else
            {
                if(!response.Acknowledged)
                {
                    PublishToast("Message not acknowledged.");
                }
            }
        }

        //TODO: Step 4: Write your Business Logic

        // C-DEngine properties
        // Name of the sender
        public string SenderName
        {
            get { return TheThing.MemberGetSafePropertyString(MyBaseThing); }
            set { TheThing.MemberSetSafePropertyString(MyBaseThing, value); }
        }

        // Message to be sent
        public string Message
        {
            get { return TheThing.MemberGetSafePropertyString(MyBaseThing); }
            set { TheThing.MemberSetSafePropertyString(MyBaseThing, value); }
        }

        // Running feed of messages. For this demo we will just append to a string that will map to a text box control.
        public string MessageFeed
        {
            get { return TheThing.MemberGetSafePropertyString(MyBaseThing); }
            set { TheThing.MemberSetSafePropertyString(MyBaseThing, value); }
        }

        List<ChatMessage> messageList = new List<ChatMessage>(); // Holds our sent and received messages
        StringBuilder builder;
        public void UpdateMessageFeed()
        {
            if (builder == null)
                builder = new StringBuilder(messageList.Count);
            foreach(var chatMessage in messageList)
            {
                builder.Append(chatMessage + "\n");
            }
            MessageFeed = builder.ToString();
            builder.Clear();
        }

        public void PublishToast(string message)
        {
            TSM toast = new TSM(eEngineName.NMIService, "NMI_TOAST:15000", message);
            toast.SetDoNotRelay(true);
            TheCommCore.PublishCentral(toast, true);
        }

        public override void HandleMessage(ICDEThing sender, object pIncoming)
        {
            var msg = (pIncoming as TheProcessMessage)?.Message;
            if(msg == null)
            {
                return;
            }
            var messageName = TheCommRequestResponse.ParseRequestOrResponseMessage(msg, out var messageParameters, out var correlationToken);

            switch(messageName)
            {
                case nameof(MsgChatHello):
                    // Process the hello message
                    {
                        var request = TheCommRequestResponse.ParseRequestMessageJSON<MsgChatHello>(msg);
                        if (request != null)
                        {
                            var chatMessage = new ChatMessage
                            {
                                MessageID = correlationToken,
                                SenderName = request.SenderName,
                                Message = request.Message,
                                SeenBy = 1,
                                Sent = msg.TIM,
                                Received = DateTimeOffset.Now
                            };
                            messageList.Add(chatMessage);
                            UpdateMessageFeed();
                            TheCommRequestResponse.PublishResponseMessageJson(msg, new MsgChatHelloResponse { Acknowledged = true });
                        }
                        else
                            TheCommRequestResponse.PublishResponseMessageJson(msg, new MsgChatHelloResponse { Acknowledged = false });
                    }
                    break;
                case nameof(MsgChatHello) + "_RESPONSE":
                    // Process the hello response message
                    {
                        var request = TheCommRequestResponse.ParseRequestMessageJSON<MsgChatHelloResponse>(msg);
                        if(request != null)
                        {
                            if(request.Acknowledged)
                            {
                                if(!msg.DoesORGContainLocalHost())
                                {
                                    ChatMessage chatMessage = messageList.Find(m => m.MessageID == correlationToken);
                                    chatMessage.SeenBy++;
                                    UpdateMessageFeed();
                                }
                            }
                            else
                            {
                                // Show error message in NMI
                                PublishToast($"Somebody ({msg.ORG}) rejected our message {correlationToken}");
                            }
                        }
                    }
                    break;
            }
        }
    }
}
