// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System.Collections.Generic;
using nsCDEngine.BaseClasses;
using nsCDEngine.Security;
using nsCDEngine.ViewModels;

namespace $safeprojectname$
{
    public class BaseApplication
    {
        private static TheBaseApplication MyBaseApplication;
        public static string strScopeID;

        public static bool Init(string strGuid, string strUserScopeID, string strAppID)
        {
            TheScopeManager.SetApplicationID(strAppID); // SDK Non-Commercial ID. FOR COMMERCIAL APP GET THIS ID FROM C-LABS!

            TheBaseAssets.MyServiceHostInfo = new TheServiceHostInfo(cdeHostType.Application)
            {
                ApplicationName = "$safeprojectname$",      // Friendly Name of Application
                cdeMID = TheCommonUtils.CGuid(strGuid),
                Title = "My-Relay (C) $registeredorganization$ $year$", // Title of this Host Service
                ApplicationTitle = "$safeprojectname$",         // Title visible in the NMI Portal 
                CurrentVersion = 1.0001,                        // Version of this Service, increase this number when you publish a new version that the store displays the correct update icon
                DebugLevel = eDEBUG_LEVELS.OFF,                 // Define a DebugLevel for the SystemLog output.
                SiteName = "http://cloud.c-labs.com",           // Link to the main Cloud Node of this host. this is not required and for documentation only

                ISMMainExecutable = "$safeprojectname$",        // Name of the executable (without .exe)
                IgnoreAdminCheck = true,                        // if set to true, the host will not start if launched without admin priviledges. 

                LocalServiceRoute = "LOCALHOST", // Will be replaced by the full DNS name of the host during startup.
                MyStationPort = 8700,            // Port for REST access to this Host node. If your PC already uses port 80 for another webserver, change this port. We recommend using Port 8700 and higher for UPnP to work properly.
                MyStationWSPort = 8701,          // Enables WebSockets on the station port. If UseRandomDeviceID is false, this Value cannot be changed here once the App runs for the first time. On Windows 8 and higher running under "Adminitrator" you can use the same port
            };

            if (strUserScopeID.Length > 0)
                strScopeID = strUserScopeID;
            else
                strScopeID = TheScopeManager.GenerateNewScopeID();  // TIP: instead of creating a new random ID every time your host starts, you can put a breakpoint in the next line, record the ID and feed it in the "SetScopeIDFromEasyID". Or even set a fixed ScopeID here. (FOR TESTING ONLY!!)
            return true;
        }

        public static bool Start()
        {
            bool bResult = false;

            TheScopeManager.SetScopeIDFromEasyID(strScopeID);   // Set a ScopeID - the security context of this node. You can replace tScope with any random 8 characters or numbers

            Dictionary<string, string> ArgList = new Dictionary<string, string>();

            // Scan command line settings.
            for (int i = 0; i < Program.args.Length; i++)
            {
                string[] tArgs = Program.args[i].Split('=');
                if (tArgs.Length == 2)
                {
                    string key = tArgs[0].ToUpper();
                    ArgList[key] = tArgs[1];
                }
            }
            ArgList["DontVerifyTrust"] = "True";    // NEW: 3.2 If this is NOT set, all plugins have to be signed with the same certificate as the host application or C-DEngine.DLL
            ArgList["UseRandomDeviceID"] = "True";  // ATTENTION: ONLY if you set this to false, some of these parameters will be stored on disk and loaded at a later time. "true" assigns a new node ID everytime the host starts and no configuration data will be cached on disk.
            ArgList["ScopeUserLevel"] = "255";      // Set the Scope Access Level 

            MyBaseApplication = new TheBaseApplication();    // Create a new Base (C-DEngine IoT) Application

            if (MyBaseApplication != null)
            {
                // Start the C-DEngine Application. If a PluginService class is added DIRECTLY 
                // to the host project you can instantiate the Service here replacing the null 
                // with "new cdePluginService1()"
                bResult = MyBaseApplication.StartBaseApplication(null, ArgList);         
            }

            return bResult;
        }

        public static bool Stop()
        {
            if (MyBaseApplication != null)
            {
                MyBaseApplication.Shutdown(true);
                MyBaseApplication = null;
                return true;
            }

            return false;
        }

        public static string FetchUrl()
        {
            return TheBaseAssets.MyServiceHostInfo.MyStationURL + "/nmi";
        }
    }
}
