// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using nsCDEngine.BaseClasses;
using nsCDEngine.Communication;
using nsCDEngine.Engines;
using nsCDEngine.Engines.ThingService;
using nsCDEngine.ViewModels;
using System;
using System.Collections.Generic;

namespace CDMyWebToMeshSample
{
    partial class cdePluginService1 : ICDEPlugin, ICDEThing
    {
        /// <summary>
        /// RegisterHttpInterceptor -- Request C-DEngine to call us when there is a
        /// reference to the URL that we provide.
        /// </summary>
        public void RegisterHttpInterceptor()
        {
            TheCommCore.MyHttpService.RegisterHttpInterceptorB4("/api/MileRecordHolder", sinkThingApiInterceptor);
        }

        /// <summary>
        /// UnRegisterHttpInterceptor -- Cancel the request for notifications when
        /// a URL is referenced.
        /// </summary>
        public void UnRegisterHttpInterceptor()
        {
            TheCommCore.MyHttpService.UnregisterHttpInterceptorB4("/api/MileRecordHolder");
        }

        /// <summary>
        /// sinkThingApiInterceptor - Called when our URL is referenced.
        /// </summary>
        /// <param name="pRequest"></param>
        public void sinkThingApiInterceptor(TheRequestData pRequest)
        {
            string strQuery = pRequest.RequestUri.Query;
            if (strQuery.StartsWith("?"))
                strQuery = strQuery.Substring(1);

            string strLocalPathLower = pRequest.RequestUri.LocalPath.ToLower();

            if (strLocalPathLower == "/api/milerecordholder/logon")
            {
                Dictionary<string, string> aParameters = new Dictionary<string, string>();
                utils.ParseQueryParameters(pRequest.RequestUri.Query, aParameters);
                utils.ValidateUserCredentials(pRequest, aParameters);
            }
            else if (strLocalPathLower == "/api/milerecordholder/logoff")
            {
                Dictionary<string, string> aParameters = new Dictionary<string, string>();
                utils.ParseQueryParameters(pRequest.RequestUri.Query, aParameters);
                if (utils.IsTokenValid(aParameters))
                {
                    // TBD
                }
            }
            else if (strLocalPathLower == "/api/milerecordholder/query")
            {
                // Local: validate access token.
                Dictionary<string, string> aParameters = new Dictionary<string, string>();
                utils.ParseQueryParameters(pRequest.RequestUri.Query, aParameters);
                TheRecordHolder trh = null;
                if (utils.IsTokenValid(aParameters))
                {
                    // Local: parse incoming parameters.
                    if (utils.QueryIntFromDictionary(aParameters, "id", out int id))
                    {

                        if (g_EnableMeshDataQuery)
                        {
                            // Remote: query for data.
                            string strEngineName = this.GetBaseEngine().GetEngineName();
                            Guid guidNode = TheBaseAssets.MyServiceHostInfo.MyDeviceInfo.DeviceID;
                            Guid guidThing = this.GetBaseThing().cdeMID;

                            trh = WebToMesh.MeshQueryRecordHolder(id, guidNode, strEngineName, guidThing);
                        }
                        else
                        {
                            // Local: query for data.
                            trh = TheRecordHolder.QueryRecordHolder(id);
                        }
                    }
                }

                if (trh != null)
                {
                    utils.CreateJsonResponse<TheRecordHolder>(pRequest, trh);
                }
                else
                {
                    utils.CreateEmptyResponse(pRequest);
                }
            }
        }


    } // class


} // namespace

