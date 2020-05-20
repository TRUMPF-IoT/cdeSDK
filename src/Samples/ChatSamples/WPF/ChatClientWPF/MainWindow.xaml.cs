// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


using nsCDEngine.BaseClasses;
using nsCDEngine.Communication;
using nsCDEngine.Engines;
using nsCDEngine.Engines.ThingService;
using nsCDEngine.Security;
using nsCDEngine.ViewModels;

namespace ChatClientWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            // Start the CDEngine communication channels
            StartCDEngineHost();

            TheBaseEngine.WaitForEnginesStartedAsync().ContinueWith(t =>
            {

                try
                {
                    // Express our interest in the EngineName used for the chat, so that a subscription to the cloud is established even though the engine has not matching plug-in on this node
                    TheCDEngines.RegisterNewMiniRelay(strChatEngine);

                    // Intercept any messages sent to the chat engine
                    var chatEngine = TheThingRegistry.GetBaseEngine(strChatEngine);
                    chatEngine.RegisterEvent(eEngineEvents.IncomingMessage, HandleMessage);

                    // Intercept any messages sent to the ContentService (we will use that as the originating thing, and acknowledge messages will come back to it)
                    TheThingRegistry.GetBaseEngineAsThing(eEngineName.ThingService).RegisterEvent(eThingEvents.IncomingMessage, HandleMessage);
                }
                catch
                {
                }

            });
        }

        // Messages for sending through the mesh
        class MsgChatHello
        {
            public string SenderName { get; set; }
            public string Message { get; set; }
        }

        class MsgChatHelloResponse
        {
            public bool Acknowledged { get; set; }
            public string SenderName { get; set; }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            var msgHello = new MsgChatHello
            {
                Message = MessageText.Text,
                SenderName = UserName.Text,
            };

            var target = new TheMessageAddress
            {
                EngineName = strChatEngine, // Send to this engine
                Node = Guid.Empty           // Send to all nodes in the mesh
            };

            var response = TheCommRequestResponse.PublishRequestJSonAsync<MsgChatHello, MsgChatHelloResponse>(
                TheThingRegistry.GetBaseEngineAsThing(eEngineName.ThingService), // Since we have no plug-in, use an arbitrary existing thing in the system as the originator
                target,
                msgHello, 
                new TimeSpan(0, 0, 10)) // Wait 10 seconds for ackknowledge message
                .Result;

            if (response == null)
            {
                MessageBox.Show("Error sending message or no acknowlege received.");
            }
            else
            {
                if (response.Acknowledged)
                {

                }
                else
                {
                    MessageBox.Show("Message not acknowledged.");
                }
            }
        }

        const string strChatEngine = "CHATSTER";
        const string strScope = "pass1234";
        const string strCloudServiceRoute = "wss://cloud.c-labs.com";

        private void HandleMessage(ICDEThing arg1, object arg2)
        {
            var msg = (arg2 as TheProcessMessage).Message;
            if (msg == null)
            {
                return;
            }

            var messageName = TheCommRequestResponse.ParseRequestOrResponseMessage(msg, out var messageParameters, out var correlationToken);

            switch (messageName)
            {
                case nameof(MsgChatHello):
                    {
                        var request = TheCommRequestResponse.ParseRequestMessageJSON<MsgChatHello>(msg);
                        if (request != null)
                        {
                            // Messages come in on a .Net Threadpool thread, so need to dispatch it to the WPF main thread before we can access any WPF controls
                            this.Dispatcher.InvokeAsync(() =>
                            {
                                var chatMessage = new ChatMessage
                                {
                                    MessageId = correlationToken,
                                    SenderName = request.SenderName,
                                    Message = request.Message,
                                    SeenBy = 0,
                                    Sent = msg.TIM,
                                    Received = DateTimeOffset.Now,
                                };
                                MessageList.Items.Insert(0, chatMessage);
                            });
                            TheCommRequestResponse.PublishResponseMessageJson(msg, new MsgChatHelloResponse { Acknowledged = true });
                        }
                        else
                        {
                            TheCommRequestResponse.PublishResponseMessageJson(msg, new MsgChatHelloResponse { Acknowledged = false });
                        }
                    }
                    break;
                case nameof(MsgChatHello)+"_RESPONSE":
                    {
                        var request = TheCommRequestResponse.ParseRequestMessageJSON<MsgChatHelloResponse>(msg);
                        if (request != null)
                        {
                            this.Dispatcher.InvokeAsync(() =>
                            {
                                if (request.Acknowledged)
                                {
                                    int i = 0;
                                    foreach (var item in MessageList.Items)
                                    {
                                        var chatMessage = item as ChatMessage;
                                        if (chatMessage != null)
                                        {
                                            if (chatMessage.MessageId == correlationToken)
                                            {
                                                chatMessage.SeenBy++;
                                                MessageList.Items.RemoveAt(i);
                                                MessageList.Items.Insert(i, chatMessage);
                                                break;
                                            }
                                        }
                                        i++;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show($"Somebody ({msg.ORG}) rejected our message {correlationToken}");
                                }
                            });
                        }
                        else
                        {
                            // Somebody is sending MsgChatHelloResponse messages in an unknown/incompatible format, or somebody rejected a message for some reason
                            this.Dispatcher.InvokeAsync(() =>
                            {
                                MessageBox.Show($"Received invalid MsgChatHelloResponse: {msg}");
                            });
                        }
                    }
                    break;
            }
        }

        // Message to show in the UI
        class ChatMessage
        {
            public Guid MessageId;
            public string Message;
            public string SenderName;
            public int SeenBy;
            public DateTimeOffset Sent;
            public DateTimeOffset Received;

            public override string ToString()
            {
                return $"[{Sent}] {SenderName}: {Message} (Seen By: {SeenBy}, Latency (ms): {(Received - Sent).TotalMilliseconds})";
            }
        }

        private static TheBaseApplication MyBaseApplication;
        private static Dictionary<string, string> ArgList;

        void StartCDEngineHost()
        {
            // SDK Non-Commercial ID. FOR COMMERCIAL APP GET THIS ID FROM C-LABS!
            TheScopeManager.SetApplicationID("/cVjzPfjlO;{@QMj:jWpW]HKKEmed[llSlNUAtoE`]G?");

            //
            //  Establish service parameters
            //
            TheBaseAssets.MyServiceHostInfo = new TheServiceHostInfo(cdeHostType.Device)
            {
                // TODO: Generate host service unique ID
                cdeMID = TheCommonUtils.CGuid("5872AA06-0EAD-4A07-89B9-CC9D632FC50C"),

                IgnoreAdminCheck = true, // If false, the host requires admin priviledges to start

                ISMMainExecutable = "ChatClientWPF",   // Name of the executable (without .exe)
                ApplicationName = "My-ChatClient",              // Friendly Name of Application
                Title = "My-Relay (c) C-Labs 2013-2019",   // Title of this Host Service
                ApplicationTitle = "My-Relay Portal",      // Title visible in the NMI Portal 

                LocalServiceRoute = "LOCALHOST",           // Will be replaced by the full DNS name of the host during startup.
                SiteName = "http://cloud.c-labs.com",      // Link to the main Cloud Node of this host. 
                                                           // Not required, for documentation only

                CurrentVersion = 1.0001,                   // Service version of this Service.
                                                           // Increase when you publish a new version so the 
                                                           // online store can display the correct update icon
                DebugLevel = eDEBUG_LEVELS.VERBOSE,        // Define a DebugLevel for the SystemLog output.
                CloudServiceRoute = strCloudServiceRoute,
            };

            //
            // Create dictionary to hold configuration settings.
            //
            ArgList = new Dictionary<string, string>();

            ArgList.Add("DontVerifyTrust", "True");       // When "false", all plugins have to be signed with the 
                                                          // same certificate as the host application or C-DEngine.DLL.
                                                          // When "true", ignore code signing security check (dev only!)

            ArgList.Add("UseRandomDeviceID", "true");     // When "true", assigns a new device ID everytime 
                                                          // the host starts. No configuration data retained on disk.
                                                          // When "false", enables persistence between system starts

            ArgList.Add("ScopeUserLevel", "255");         // Set the Scope Access Level

            // Create a new Base (C-DEngine IoT) Application
            MyBaseApplication = new TheBaseApplication();

            // Start the C-DEngine Host Application. 
            // If a PluginService class is added DIRECTLY to the host project you can 
            // instantiate the Service here. Replace null with "new cdePluginService1()"
            if (!MyBaseApplication.StartBaseApplication(null, ArgList))
            {
                // If the Application fails to start, quit the app. 
                // StartBaseApplication returns very fast because all 
                // C-DEngine code is running asynchronously
                MessageBox.Show("Failed to start CDEngine.");
                return;
            }

            // Generate random Scope ID every time we run.
            //strScope = TheScopeManager.GenerateNewScopeID();     // TIP: instead of creating a new random ID every 
            // time your host starts, you can put a breakpoint in the 
            // next line, record the ID and feed it in the "SetScopeIDFromEasyID". 
            // Or even set a fixed ScopeID here. (FOR TESTING ONLY!!)

            TheScopeManager.SetScopeIDFromEasyID(strScope);      // Set a ScopeID - the security context of this node. 
                                                                 // Replace strScope with any random 8 characters or numbers



            Task.Delay(-1, TheBaseAssets.MasterSwitchCancelationToken).ContinueWith((t) =>
            {
                MessageBox.Show("CDEngine is shutting down.");
            });

            // MyBaseApplication.MyCommonDisco.RegisterUPnPUID("*", null);  // (Optional) Whether to use Universal Plug 
            // and Play (UPnP) to find devices

            // Set up URL for our node.
            string strStationURL = TheBaseAssets.MyServiceHostInfo.GetPrimaryStationURL(false);
            strStationURL = String.Format("{0}/nmi", strStationURL);
        }

    }
}
