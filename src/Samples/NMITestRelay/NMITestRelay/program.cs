// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using nsCDEngine.BaseClasses;
using nsCDEngine.Security;
using nsCDEngine.ViewModels;
using nsCDEngine.Engines;
using System.Diagnostics;
using nsCDEngine.Engines.ThingService;
using nsCDEngine.Communication;

namespace SDKTestRelay
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.Name = "Main thread";  //Helps Debugging

            TheScopeManager.SetApplicationID("/cVjzPfjlO;{@QMj:jWpW]HKKEmed[llSlNUAtoE`]G?"); //SDK Non-Commercial ID. FOR COMMERCIAL APP GET THIS ID FROM C-LABS!

            TheBaseAssets.MyServiceHostInfo = new TheServiceHostInfo(cdeHostType.Application)
            {
                ApplicationName = "My-Relay",                                   //Friendly Name of Application
                cdeMID = TheCommonUtils.CGuid("{5BD695B9-C545-4966-8342-E26FC168C7D1}"),     //TODO: Give a Unique ID to this Host Service
                Title = "My-Relay",                   //Title of this Host Service
                ApplicationTitle = "My-Relay Portal",                           //Title visible in the NMI Portal 
                DebugLevel = eDEBUG_LEVELS.OFF,                                 //Define a DebugLevel for the SystemLog output. the higher the more output

                ISMMainExecutable = "SDKTestRelay",                             //Name of the executable (without .exe)

                //CloudServiceRoute = "wss://cloud.c-labs.com",                 //try it with cloud access
                MyStationPort = 8713,                                           //Port for REST access to this Host node. If your PC already uses port 80 for another webserver, change this port. We recommend using Port 8700 and higher for UPnP to work properly.
                MyStationWSPort = 8713,                                         //Enables WebSockets on the station port. If UseRandomDeviceID is false, this Value cannot be changed here once the App runs for the first time. On Windows 8 and higher running under "Adminitrator" you can use the same port
            };

            #region Args Parsing 
            Dictionary<string, string> ArgList = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                string[] tArgs = args[i].Split('=');
                if (tArgs.Length == 2)
                {
                    string key = tArgs[0].ToUpper();
                    ArgList[key] = tArgs[1];
                }
            }
            #endregion

            ArgList.Add("AllowRemoteISBConnect", "true");       //Allows connect from CORS offline/hosted NMI
            ArgList.Add("Access-Control-Allow-Origin", "*");    //Allows CORS access from other domains
            ArgList.Add("DontVerifyTrust", "true");             //No Code Signing check
            ArgList.Add("UseRandomDeviceID", "false");          //Set to true if you dont want to store anything - no settings, ThingRegistry or cache values- on the harddrive
            ArgList.Add("ShowSamples", "true");                 //Show all samples in Plugins
            ArgList.Add("ScopeUserLevel", "255");               //Give logged user the user-level 255 (admin)
            ArgList.Add("EnableKPIs", "true");                  //Enable KPI collection
            ArgList.Add("TokenLifeTime", "180");                //Allow to refresh the browser for 180 seconds 

            //Also keep in mind that setting can be overwritten in the App.Config

            TheBaseApplication MyBaseApplication = new TheBaseApplication();    //Create a new Base (C-DEngine IoT) Application
            TheCDEngines.eventAllEnginesStarted += sinkReady;
            if (!MyBaseApplication.StartBaseApplication(null, ArgList))         //Start the C-DEngine Application. If a PluginService class is added DIRECTLY to the host project you can instantiate the Service here replacing the null with "new cdePluginService1()"
                return;                                                         //If the Application fails to start, quit the app. StartBaseApplication returns very fast as all the C-DEngine code is running asynchronously
                                                                                //MyBaseApplication.MyCommonDisco.RegisterUPnPUID("*", null);     //Only necessary if UPnP is used to find devices
            string strScope = TheScopeManager.GenerateNewScopeID();                 //TIP: instead of creating a new random ID every time your host starts, you can put a breakpoint in the next line, record the ID and feed it in the "SetScopeIDFromEasyID". Or even set a fixed ScopeID here. (FOR TESTING ONLY!!)
            Console.WriteLine($"=======> Node Scope : {strScope}");
            TheScopeManager.SetScopeIDFromEasyID(strScope);                       //Set a ScopeID - the security context of this node. You can replace tScope with any random 8 characters or numbers

            #region Waiting for ESC Key pressed
            string strStationURL = TheBaseAssets.MyServiceHostInfo.GetPrimaryStationURL(false);
            strStationURL = String.Format("{0}/nmi", strStationURL);
            // User input loop
            while (true)
            {
                Console.WriteLine("\r\nStation URL: " + strStationURL);
                Console.WriteLine("\r\nScope ID: " + strScope);
                Console.WriteLine("\r\n[Esc] key to quit. 'B' (or 'b') to launch browser");
                ConsoleKeyInfo key = Console.ReadKey();
                if (key.Key == ConsoleKey.Escape) break;
                if (key.KeyChar == 'b' || key.KeyChar == 'B')
                {
                    try
                    {
                        ProcessStartInfo psi = new ProcessStartInfo(strStationURL);
                        psi.UseShellExecute = true;
                        Process.Start(psi);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error launching browser {0}", e);
                    }
                }
            }
            MyBaseApplication.Shutdown(true);
            #endregion
        }

        static void sinkReady()
        {
            Debug.WriteLine("Registering events for engine... ");

            TheCDEngines.MyContentEngine.RegisterEvent(eEngineEvents.IncomingMessage, sinkIncoming);
            if (TheCDEngines.MyNMIService != null)
            {
                TheCDEngines.MyNMIService.RegisterEvent(eEngineEvents.IncomingMessage, sinkIncoming);
                //TheCDEngines.MyContentEngine.RegisterEvent(eEngineEvents.ChannelConnected, sinkEstablished);
            }
            TheCommonUtils.cdeRunAsync("LoopDiDoop", true, (o) => 
            {
                TheThing tTimer = null;
                TheThing GoogleDNS = null;
                TheThing ThingBar = null;
                while (TheBaseAssets.MasterSwitch)
                {
                    TheCommonUtils.SleepOneEye(5000, 100);

                    if (ThingBar == null)
                    {
                        ThingBar = TheThingRegistry.GetThingByProperty("*", Guid.Empty, "FriendlyName", "ThingBar");
                        if (ThingBar != null)
                        {
                            TheThingRegistry.RegisterEventOfThing(ThingBar, eThingEvents.ValueChanged, (sender, para) =>
                            {
                                TSM tTSM2 = new TSM(eEngineName.ContentService, "UNITY:GAUGE", para.ToString());
                                TheCommCore.PublishCentral(tTSM2);
                            });
                        }
                    }
                    if (tTimer == null)
                    {
                        tTimer = TheThingRegistry.GetThingByProperty("*", Guid.Empty, "DeviceType", "Timer");
                        if (tTimer != null)
                        {
                            TheThingRegistry.RegisterEventOfThing(tTimer, eThingEvents.ValueChanged, (sender, para) => 
                            {
                                TSM tTSM2 = new TSM(eEngineName.ContentService, "UNITY:TIMER", para.ToString());
                                TheCommCore.PublishCentral(tTSM2);
                            });
                        }
                    }
                    if (GoogleDNS == null)
                    {
                        GoogleDNS = TheThingRegistry.GetThingByProperty("*", Guid.Empty, "FriendlyName", "Google DNS");
                        if (GoogleDNS != null)
                        {
                            TheThingRegistry.RegisterEventOfThing(GoogleDNS, eThingEvents.PropertyChanged, (sender, para) =>
                            {
                                cdeP tP = para as cdeP;
                                if (tP != null && tP.Name=="RoundTripTime")
                                {
                                    TSM tTSM2 = new TSM(eEngineName.ContentService, "UNITY:HEART", para.ToString());
                                    TheCommCore.PublishCentral(tTSM2);
                                }
                            });
                        }
                    }
                    if (GoogleDNS != null && tTimer != null && ThingBar != null)
                        break;
                }
            });
        }

        /// <summary>
        /// Primary event handler for incoming messages from CD-Engine
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="Para"></param>
        static void sinkIncoming(ICDEThing sender, object Para)
        {
            //Cast returned message to make strongly typed
            TheProcessMessage tProc = Para as TheProcessMessage;
            if (tProc == null) return;

            ////Skip ContentService messages for now
            //if (tProc.Message.ENG == eEngineName.ContentService)
            //{              
            //    return;
            //}

            //Parse message text into command
            string[] tCmd = tProc.Message.TXT.Split(':');
            switch (tCmd[0])
            {
                default:
                    break;
            
            }
        }
    }
}
