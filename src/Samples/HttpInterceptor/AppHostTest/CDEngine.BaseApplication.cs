// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System.Collections.Generic;
using nsCDEngine.BaseClasses;
using nsCDEngine.Security;
using nsCDEngine.ViewModels;

namespace AppHostTest
{
    public class BaseApplication
    {
        private static TheBaseApplication MyBaseApplication;
        public static string strScopeID;

        public static TheBaseAppSettings settings = null;

        public static bool Init(string strGuid, string strUserScopeID, string strAppID)
        {
            TheScopeManager.SetApplicationID(strAppID); // SDK Non-Commercial ID. FOR COMMERCIAL APP GET THIS ID FROM C-LABS!

            TheBaseAssets.MyServiceHostInfo = new TheServiceHostInfo(cdeHostType.Application)
            {
                ApplicationName = "AppHostTest",      // Friendly Name of Application
                cdeMID = TheCommonUtils.CGuid(strGuid),
                Title = "My-Relay (C)  2019", // Title of this Host Service
                ApplicationTitle = "AppHostTest",         // Title visible in the NMI Portal 
                CurrentVersion = 1.0001,                  // Version of this Service, increase this number when you publish a new version that the store displays the correct update icon
                SiteName = "http://cloud.c-labs.com",     // Link to the main Cloud Node of this host. this is not required and for documentation only

                ISMMainExecutable = "AppHostTest",        // Name of the executable (without .exe)

                ServiceRoute = "https://cloud.c-labs.com",
            };

            strScopeID = strUserScopeID;

            settings = new TheBaseAppSettings();

            // Directly add keys to settings table.
            settings.AddKey("UseTcpListenerInsteadOfHttpListener", true);
            settings.SetKeyUnlessAlreadySet("DEBUGLEVEL", eDEBUG_LEVELS.OFF);

            // Select either user manager or scope manager. But not both!!
            settings.InitUserManager();
            // strScopeID = settings.InitScopeManager(strScopeID, 255);

            // Rely on helper functions to initialize other settings.
            settings.InitEnvironmentVarSettings(true, true);
            settings.DisableCodeSigningValidation(true);
            settings.InitClientBinPersistence(false);

            // Initialize ports for web server and web sockets.
            settings.InitWebPorts(8720, 8721);

            return true;
        }

        public static bool Start()
        {
            bool bResult = false;

            MyBaseApplication = new TheBaseApplication();    // Create a new Base (C-DEngine IoT) Application

            if (MyBaseApplication != null)
            {
                // Start the C-DEngine Application. If a PluginService class is added DIRECTLY 
                // to the host project you can instantiate the Service here replacing the null 
                // with "new cdePluginService1()"
                bResult = MyBaseApplication.StartBaseApplication(null, settings.ArgList);
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
            return TheBaseAssets.MyServiceHostInfo.MyStationURL + "/lnmi";
        }
    }
}
