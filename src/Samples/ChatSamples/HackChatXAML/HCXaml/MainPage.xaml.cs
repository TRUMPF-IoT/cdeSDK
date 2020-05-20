// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

using CDMyChat;
using nsCDEngine.BaseClasses;
using nsCDEngine.Communication;
using nsCDEngine.Engines;
using nsCDEngine.Engines.ThingService;
using nsCDEngine.Security;
using nsCDEngine.ViewModels;
using System;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HCXaml
{
    class MsgChatHello
    {
        public string SenderName { get; set; }
        public string Message { get; set; }
        public DateTimeOffset TimeSent { get; set; }
    }

    class MsgChatAck
    {
        public bool Acknowledged { get; set; }
    }


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        public async void ConnectToCloud(string pScopeID)
        {
            if (TheBaseAssets.MasterSwitch)
            {
                await UpdateLog("SYSTEM: Already Connected");
                return;
            }
            TheScopeManager.SetApplicationID("/cVjzPfjlO;{@QMj:jWpW]HKKEmed[llSlNUAtoE`]G?"); //SDK Non-Commercial ID. FOR COMMERCIAL APP GET THIS ID FROM C-LABS!

            TheBaseAssets.MyServiceHostInfo = new TheServiceHostInfo(cdeHostType.Application)
            {
                ApplicationName = "My-Relay",                                   //Friendly Name of Application
                cdeMID = TheCommonUtils.CGuid("{4DB8E613-093F-4581-8E84-FE60FFA05A8E}"),     //TODO: Give a Unique ID to this Host Service
                Title = "HackChatXAML (C) C-Labs. Corp 2019",                   //Title of this Host Service
                ApplicationTitle = "HackChat",                           //Title visible in the NMI Portal 
                CurrentVersion = 1.0001,                                        //Version of this Service, increase this number when you publish a new version that the store displays the correct update icon
                DebugLevel = eDEBUG_LEVELS.VERBOSE,                                 //Define a DebugLevel for the SystemLog output.
                SiteName = "http://cloud.c-labs.com",                           //Link to the main Cloud Node of this host. this is not required and for documentation only

                ISMMainExecutable = "HCXaml",                        //Name of the executable (without .exe)
            };

            #region Args Parsing
            Dictionary<string, string> ArgList = new Dictionary<string, string>();
            #endregion

            ArgList.Add("ServiceRoute", "wss://cloud.c-labs.com");
            ArgList.Add("UseRandomDeviceID", "true");   //Set the Scope Access Level 
            ArgList.Add("ScopeUserLevel", "255");   //Set the Scope Access Level 
            TheBaseApplication MyBaseApplication = new TheBaseApplication();    //Create a new Base (C-DEngine IoT) Application
            TheCDEngines.eventAllEnginesStarted += sinkReady;
            var tChatPlugin = new cdePluginService1();
            if (!MyBaseApplication.StartBaseApplication(tChatPlugin, ArgList))         //Start the C-DEngine Application. If a PluginService class is added DIRECTLY to the host project you can instantiate the Service here replacing the null with "new cdePluginService1()"
            {
                await UpdateLog("SYSTEM: Start BaseApp failed!");
                return;                                                         //If the Application fails to start, quit the app. StartBaseApplication returns very fast as all the C-DEngine code is running asynchronously
            }
            else
            {
                TheScopeManager.SetScopeIDFromEasyID(pScopeID);
                await UpdateLog("SYSTEM: Start BaseApp started!");
            }
        }

        async void sinkReady()
        {
            TheCDEngines.RegisterNewMiniRelay("CHATSTER");
            var tThing=TheThingRegistry.GetBaseEngine("CHATSTER");
            tThing.RegisterEvent(eEngineEvents.IncomingMessage, HandleMessageAsync);
            await UpdateLog("SYSTEM: Connected to Cloud");
        }

        async void HandleMessageAsync(ICDEThing pSender, object para)
        {
            TheProcessMessage tMsg = para as TheProcessMessage;

            if (tMsg == null || tMsg.Message == null)
                return;

            var tCmd = tMsg.Message.TXT.Split(':');

            switch (tCmd[0])
            {
                case "MsgChatHello":
                    try
                    {
                        var t = TheCommonUtils.DeserializeJSONStringToObject<MsgChatHello>(tMsg.Message.PLS);
                        await UpdateLog($"{t.SenderName}: {t.Message}");
                    } catch (Exception)
                    {
                        await UpdateLog($"SOMEBODY SENT ILLEGAL JSON");
                    }
                    break;
            }
        }

        private async System.Threading.Tasks.Task UpdateLog(string t)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                lstMessages.Text += $"{Environment.NewLine}{t}";
            });
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            ConnectToCloud(tbScopeID.Text);
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            MsgChatHello tMsg = new MsgChatHello { SenderName="Chris", Message=txtMsgText.Text };
            TheCommCore.PublishCentral(new TSM("CHATSTER", $"MsgChatHello:{Guid.NewGuid()}", TheCommonUtils.SerializeObjectToJSONString(tMsg)), true);
        }
    }
}