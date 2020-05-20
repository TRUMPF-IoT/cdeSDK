// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using nsCDEngine.BaseClasses;
using nsCDEngine.Communication;
using nsCDEngine.Engines;
using nsCDEngine.Engines.ThingService;
using nsCDEngine.Security;
using nsCDEngine.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace CDMyRestApiSampleMileRecordHolders
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
        /// <param name="pRequest">HTTP request and response fields.</param>
        public void sinkThingApiInterceptor(TheRequestData pRequest)
        {
            string strQuery = pRequest.RequestUri.Query;
            Dictionary<string, string> aParameters = utils.ParseQueryParameters(strQuery);
            string strLocalPathLower = pRequest.RequestUri.LocalPath.ToLower();

            if (strLocalPathLower == "/api/milerecordholder/logon")
            {
                utils.ValidateUserCredentials(pRequest, aParameters);
            }
            else if (strLocalPathLower == "/api/milerecordholder/query")
            {
                if (utils.IsTokenValid(aParameters))
                {
                    if (aParameters.ContainsKey("id"))
                    {
                        string strValue = aParameters["id"];
                        int id;
                        if (int.TryParse(strValue, out id))
                        {
                            if (id > 0 && id <= TheRecordHolder.aData.Length)
                            {
                                ProvideResponseData(pRequest, id);
                            }
                        }
                    }
                }
            }
        }

        public void ProvideResponseData(TheRequestData p, int id)
        {
            p.ResponseMimeType = "application/json";
            TheRecordHolder trh = TheRecordHolder.aData[id - 1];
            string strJson = TheCommonUtils.SerializeObjectToJSONString<TheRecordHolder>(trh);
            p.ResponseBuffer = TheCommonUtils.CUTF8String2Array(strJson);
            p.StatusCode = (int)eHttpStatusCode.OK;
            p.DontCompress = true;
            p.AllowStatePush = false;
        }

    } // class


} // namespace

