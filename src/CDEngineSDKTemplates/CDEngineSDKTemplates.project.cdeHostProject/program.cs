// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

// TODO: Add reference for (1) C-DEngine.dll and (2) CDMyNMIHtml5.dll
using nsCDEngine.BaseClasses;
using nsCDEngine.Security;
using nsCDEngine.ViewModels;

namespace $safeprojectname$
{
    class Program
    {
        private static TheBaseApplication MyBaseApplication;
        private static string strScope;
        private static Dictionary<string, string> ArgList;

        static void Main(string[] args)
        {
            Thread.CurrentThread.Name = "Main thread";  // Helps Debugging

            // SDK Non-Commercial ID. FOR COMMERCIAL APP GET THIS ID FROM C-LABS!
            TheScopeManager.SetApplicationID("/cVjzPfjlO;{@QMj:jWpW]HKKEmed[llSlNUAtoE`]G?"); 

            //
            //  Establish service parameters
            //
            TheBaseAssets.MyServiceHostInfo = new TheServiceHostInfo(cdeHostType.Application)
            {
                // TODO: Generate host service unique ID
                cdeMID = TheCommonUtils.CGuid("{$guid2$}"),   

                // TCP/IP Port Assignments
                MyStationPort=80,        // Port for REST access to this node. 
                                         // If your PC already uses port 80 for another webserver, change this port. 
                                         // We recommend using Port 8700 and higher for UPnP to work properly.

                MyStationWSPort=81,      // Enables WebSockets on the station port. 
                                         // If UseRandomDeviceID is false, this Value cannot be changed here once the 
                                         // app runs for the first time. 
                                         // On Windows 8 and later, MyStationPort and MyStationWSPort 
                                         // can be the same port if running as Administrator.

                IgnoreAdminCheck = true, // If false, the host requires admin priviledges to start

                ISMMainExecutable = "$safeprojectname$",   // Name of the executable (without .exe)
                ApplicationName = "My-Relay",              // Friendly Name of Application
                Title = "My-Relay (c) $year$ $registeredorganization$", // Title of this Host Service
                ApplicationTitle = "My-Relay Portal",      // Title visible in the NMI Portal 

                DebugLevel = eDEBUG_LEVELS.OFF,            // Define a DebugLevel for the SystemLog output.
            };

            //
            // Create dictionary to hold configuration settings.
            //
            ArgList = new Dictionary<string, string>();   
            for (int i = 0; i < args.Length; i++) {
                string[] tArgs = args[i].Split('=');      // Read config settings from command line
                if (tArgs.Length == 2)
                {
                    string key = tArgs[0].ToUpper();
                    ArgList[key] = tArgs[1];
                }
            }

            ArgList["DontVerifyTrust"] = "True";       // When "false", all plugins have to be signed with the 
                                                       // same certificate as the host application or C-DEngine.DLL.
                                                       // When "true", ignore code signing security check (dev only!)

            ArgList["UseRandomDeviceID"] = "True";     // When "true", assigns a new device ID everytime 
                                                       // the host starts. No configuration data retained on disk.
                                                       // When "false", enables persistence between system starts

            ArgList["ScopeUserLevel"] = "255";         // Set the Scope Access Level

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
                    return;
                }

            // Generate random Scope ID every time we run.
            strScope = TheScopeManager.GenerateNewScopeID();     // TIP: instead of creating a new random ID every 
                                                                 // time your host starts, you can put a breakpoint in the 
                                                                 // next line, record the ID and feed it in the "SetScopeIDFromEasyID". 
                                                                 // Or even set a fixed ScopeID here. (FOR TESTING ONLY!!)

            TheScopeManager.SetScopeIDFromEasyID(strScope);      // Set a ScopeID - the security context of this node. 
                                                                 // Replace strScope with any random 8 characters or numbers


            // MyBaseApplication.MyCommonDisco.RegisterUPnPUID("*", null);  // (Optional) Whether to use Universal Plug 
            // and Play (UPnP) to find devices

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

                // Check keyboard input.
                ConsoleKeyInfo key = Console.ReadKey();
                if (key.Key == ConsoleKey.Escape) break;
                if (key.KeyChar == 'b' || key.KeyChar == 'B')
                {
                    try
                    {
                        System.Diagnostics.Process.Start(strStationURL);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error launching browser {0}", e);
                    }
                }
            }
            MyBaseApplication.Shutdown(true);
        }
    }
}
