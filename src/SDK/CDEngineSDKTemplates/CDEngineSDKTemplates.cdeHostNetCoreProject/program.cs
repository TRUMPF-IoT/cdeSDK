// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System;
using System.Collections.Generic;
$if$ ($targetframeworkversion$ >= 3.5)using System.Linq;
$endif$using System.Text;
using System.Threading;

using nsCDEngine.BaseClasses;
using nsCDEngine.Security;
using nsCDEngine.ViewModels;

namespace $safeprojectname$
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
                cdeMID = TheCommonUtils.CGuid("<<CREATE WITH GUID TOOL>>"),     //TODO: Give a Unique ID to this Host Service
                Title = "My-Relay (C) C-Labs 2013-2017",                   //Title of this Host Service
                ApplicationTitle = "My-Relay Portal",                           //Title visible in the NMI Portal 
                CurrentVersion = 1.0001,                                        //Version of this Service, increase this number when you publish a new version that the store displays the correct update icon
                DebugLevel = eDEBUG_LEVELS.OFF,                                 //Define a DebugLevel for the SystemLog output.
                SiteName = "http://cloud.c-labs.com",                           //Link to the main Cloud Node of this host. this is not required and for documentation only

                ISMMainExecutable = "$safeprojectname$",                        //Name of the executable (without .exe)
                IgnoreAdminCheck = true,                                             //if set to true, the host will not start if launched without admin priviledges. 
          
                LocalServiceRoute = "LOCALHOST",                                     //Will be replaced by the full DNS name of the host during startup.

                MyStationPort=80,                   //Port for REST access to this Host node. If your PC already uses port 80 for another webserver, change this port. We recommend using Port 8700 and higher for UPnP to work properly.
                MyStationWSPort=81,                 //Enables WebSockets on the station port. If UseRandomDeviceID is false, this Value cannot be changed here once the App runs for the first time. On Windows 8 and higher running under "Adminitrator" you can use the same port
            };

#region Args Parsing
            Dictionary<string, string> ArgList = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                string[] tArgs = args[i].Split('=');
                if (tArgs.Length == 2)
                {
                    string key = tArgs[0]
                    ArgList[key] = tArgs[1];
                }
            }
#endregion

            ArgList["DontVerifyTrust"] = "True";         // NEW: 3.2 If this is NOT set, all plugins have to be signed with the same certificate as the host application or C-DEngine.DLL

            ArgList["UseRandomDeviceID"] = "true";       // ATTENTION: ONLY if you set this to false, some of these parameters will be stored on disk and loaded at a later time. "true" assigns a new node ID everytime the host starts and no configuration data will be cached on disk.
            ArgList["ScopeUserLevel"] = "255";           // Set the Scope Access Level 
            TheBaseApplication MyBaseApplication = new TheBaseApplication();   // Create a new Base (C-DEngine IoT) Application
            if (!MyBaseApplication.StartBaseApplication(null, ArgList))        // Start the C-DEngine Application. If a PluginService class is added DIRECTLY to the host project you can instantiate the Service here replacing the null with "new cdePluginService1()"
                return;                                                        // If the Application fails to start, quit the app. StartBaseApplication returns very fast as all the C-DEngine code is running asynchronously

            string strScope = TheScopeManager.GenerateNewScopeID();            // TIP: instead of creating a new random ID every time your host starts, you can put a breakpoint in the next line, record the ID and feed it in the "SetScopeIDFromEasyID". Or even set a fixed ScopeID here. (FOR TESTING ONLY!!)
            TheScopeManager.SetScopeIDFromEasyID(strScope);                    // Set a ScopeID - the security context of this node. You can replace tScope with any random 8 characters or numbers

        #region Waiting for ESC Key pressed
        // Set up URL for our node.
        string strStationURL = TheBaseAssets.MyServiceHostInfo.GetPrimaryStationURL(false);
            strStationURL = String.Format("{0}/nmi", strStationURL);

            // User input loop
            while (true)
            {
                Console.WriteLine("\r\nStation URL: " + strStationURL);
                Console.WriteLine("\r\nScope ID: " + strScope);
                Console.WriteLine("\r\n[Esc] key to quit. 'B' (or 'b') to launch browser");

                // Loop until (1) C-DEngine master switch, or (2) Keyboard input
                while (TheBaseAssets.MasterSwitch && Console.KeyAvailable == false)
                {
                    Thread.Sleep(250);
                }

                // Check C-DEngine master switch.
                if (!TheBaseAssets.MasterSwitch)
                {
                    // Exit user input loop.
                    break;
                }

                ConsoleKeyInfo key = Console.ReadKey();
                if (key.Key == ConsoleKey.Escape) break;
                if (key.KeyChar == 'b' || key.KeyChar == 'B')
                {
                    try
                    {
                        System.Diagnostics.Process.Start(String.Format("{0}/nmi", TheBaseAssets.MyServiceHostInfo.GetPrimaryStationURL(false), TheBaseAssets.MyServiceHostInfo.MyStationPort));
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
    }
}
