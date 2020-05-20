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

        public static TheBaseAppSettings MySettings = null;
        public static bool Init(string strGuid, string strUserScopeID, string strAppID, ushort usPort, string strCloudServiceRoute, bool bEnableRest, bool bEnableMeshRequest, bool bEnableMeshResponse)
        {
            TheScopeManager.SetApplicationID(strAppID); // SDK Non-Commercial ID. FOR COMMERCIAL APP GET THIS ID FROM C-LABS!

            TheBaseAssets.MyServiceHostInfo = new TheServiceHostInfo(cdeHostType.Application)
            {
                ApplicationName = "AppHostTest",          // Friendly Name of Application
                cdeMID = TheCommonUtils.CGuid(strGuid),
                Title = "My-Relay",             // Title of this Host Service
                ApplicationTitle = "AppHostTest",         // Title visible in the NMI Portal 
                CurrentVersion = 1.0001,                  // Version of this Service, increase this number when you publish a new version that the store displays the correct update icon
                SiteName = "http://cloud.c-labs.com",     // Link to the main Cloud Node of this host. this is not required and for documentation only

                ISMMainExecutable = "AppHostTest",        // Name of the executable (without .exe)
            };

            strScopeID = strUserScopeID;

            MySettings = new TheBaseAppSettings();

            ushort usWsPort = usPort;
            usWsPort++;

            // Initialize ports for web server and web sockets.
            MySettings.InitWebPorts(usPort, usWsPort);

            MySettings.AddKey("CloudServiceRoute", strCloudServiceRoute);

            MySettings.AddKey("DontVerifyTrust", "true");
            // Web-to-Mesh Settings:
            // CDMyWebToMeshSample_EnableREST 
            //  +++ In CDMyWebToMeshSample plugin, whether to be REST server.
            MySettings.SetKeyUnlessAlreadySet("CDMyWebToMeshSample_EnableREST", bEnableRest);

            // CDMyWebToMeshSample_EnableMeshDataQuery
            //   +++ In CDMyWebToMeshSample plugin, whether to look for 
            //       requested REST data in other nodes or whether to provide 
            //       it locally.
            MySettings.SetKeyUnlessAlreadySet("CDMyWebToMeshSample_EnableMeshDataQuery", bEnableMeshRequest);

            // CDMyWebToMeshSample_EnableMeshDataResponse
            //   +++ In CDMyWebToMeshSample plugin, whether to look for 
            //       requested REST data in other nodes or whether to provide 
            //       it locally.
            MySettings.SetKeyUnlessAlreadySet("CDMyWebToMeshSample_EnableMeshDataResponse", bEnableMeshResponse);

            // Directly add keys to settings table.
            MySettings.SetKeyUnlessAlreadySet("DEBUGLEVEL", eDEBUG_LEVELS.VERBOSE);

            MySettings.SetKeyUnlessAlreadySet("EventViewerScope", strScopeID);
            MySettings.SetKeyUnlessAlreadySet("EventViewerMode", "StandardEventViewer");



            // Rely on helper functions to initialize other settings.
            MySettings.InitEnvironmentVarSettings(true, true);
            MySettings.DisableCodeSigningValidation(true);
            MySettings.InitClientBinPersistence(false);

            return true;
        }

        public static bool Start()
        {
            bool bResult = false;

            if (MyBaseApplication == null)
                MyBaseApplication = new TheBaseApplication();    // Create a new Base (C-DEngine IoT) Application

            if (MyBaseApplication != null)
            {
                // Start the C-DEngine Application. If a PluginService class is added DIRECTLY 
                // to the host project you can instantiate the Service here replacing the null 
                // with "new cdePluginService1()"
                bResult = MyBaseApplication.StartBaseApplication(null, MySettings.ArgList);
                if (bResult)
                {
                    // Select either user manager or scope manager. But not both!!
                    // MySettings.InitUserManager();
                    strScopeID = MySettings.InitScopeManager(strScopeID, 255);
                }
            }

            return bResult;
        }

        public static bool Stop()
        {
            if (MyBaseApplication != null)
            {
                MyBaseApplication.Shutdown(true);
                MyBaseApplication = null;

                // Give C-DEngine time to shutdown.
                System.Threading.Thread.Sleep(1000);
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
