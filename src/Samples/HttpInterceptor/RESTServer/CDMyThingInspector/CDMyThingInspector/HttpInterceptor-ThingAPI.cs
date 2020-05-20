// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using nsCDEngine.BaseClasses;
using nsCDEngine.Communication;
using nsCDEngine.Engines;
using nsCDEngine.Engines.ThingService;
using nsCDEngine.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CDMyThingInspector
{
    partial class cdePluginService1 : ICDEPlugin, ICDEThing
    {
        /// <summary>
        /// RegisterHttpInterceptor -- Request C-DEngine to call us when there is a
        /// reference to the URL that we provide.
        /// </summary>
        public void RegisterHttpInterceptor()
        {
            TheCommCore.MyHttpService.RegisterHttpInterceptorB4("/ThingApi", sinkThingApiInterceptor);
        }

        /// <summary>
        /// UnRegisterHttpInterceptor -- Cancel the request for notifications when
        /// a URL is referenced.
        /// </summary>
        public void UnRegisterHttpInterceptor()
        {
            TheCommCore.MyHttpService.UnregisterHttpInterceptorB4("/ThingApi");
        }

        /// <summary>
        /// sinkThingApiInterceptor - Called when our URL is referenced.
        /// </summary>
        /// <param name="pRequest"></param>
        public void sinkThingApiInterceptor(TheRequestData pRequest)
        {
            string[] astrURLPath = pRequest.cdeRealPage.Split(new char[1] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            bool bHandled = false;
            if (astrURLPath.Length > 1)
            {
                Dictionary<string, string> aParameters = new Dictionary<string, string>();
                ParseQueryParameters(pRequest.RequestUri.Query, aParameters);

                string strEngineContext = astrURLPath[1];
                string strThingContext = (astrURLPath.Length < 3) ? "" : astrURLPath[2];
                string strPropertyContext = (astrURLPath.Length < 4) ? "" : astrURLPath[3];
                string strPropertyName = (astrURLPath.Length < 5) ? "" : astrURLPath[4];

                // Login is only user action that does not require a login key.
                if (strEngineContext.StartsWith("Login"))
                {
                    // Is user logging in?
                    //
                    // Sample URL: 
                    //     http://c-labs-paul-yao-laptop:8700/ThingApi/Login?User=paul&Pwd=yao
                    //
                    // Sample Return value:
                    //     "a6221f63-c88c-4aa1-8a83-b0fd18a6811e"
                    ValidateUserCredentials(pRequest, astrURLPath, aParameters);
                    bHandled = true;
                }
                else if (IsTokenValid(aParameters))
                {

                    if (strEngineContext.StartsWith("Engines"))
                    {
                        // Is user enumerating engines?
                        //
                        // Sample URL: 
                        //     http://c-labs-paul-yao-laptop:8700/ThingApi/Engines?key=a6221f63-c88c-4aa1-8a83-b0fd18a6811e
                        //
                        // Sample Return value:
                        //     ["NMIService.TheNMIHtml5RT","NMIService","ThingService","CDMyThingInspector.cdePluginService1","ContentService"]

                        List<string> listEngines = TheThingRegistry.GetEngineNames(true);
                        pRequest.ResponseMimeType = "application/json";
                        string strJson = TheCommonUtils.SerializeObjectToJSONString<List<string>>(listEngines);
                        pRequest.ResponseBuffer = TheCommonUtils.CUTF8String2Array(strJson);
                        pRequest.StatusCode = (int)eHttpStatusCode.OK;
                        pRequest.DontCompress = true;
                        pRequest.AllowStatePush = false;
                        bHandled = true;
                    }
                    else
                    {
                        // Is user performing a query on a specific engine?
                        // 
                        // Sample URL: 
                        //     http://c-labs-paul-yao-laptop:8700/ThingApi/NMIService/Things?key=a6221f63-c88c-4aa1-8a83-b0fd18a6811e
                        //
                        // Sample Return value:
                        //     ["b0710f68-5d0f-4e00-93ea-b32c1a5a40c7", "d7bec9f2-4dbc-4119-97e0-f34ed09e5ae1"]
                        //
                        // Sample URL to get all things for all engines: 
                        //     http://c-labs-paul-yao-laptop:8700/ThingApi/*/Things?key=a6221f63-c88c-4aa1-8a83-b0fd18a6811e
                        //
                        bool bEngineFound = IsValidEngine(strEngineContext);
                        if (bEngineFound)
                        {
                            if (!String.IsNullOrEmpty(strThingContext))
                            {
                                if (strThingContext == "Things")
                                {
                                    // Enumerate available things for this engine.
                                    List<TheThing> listThings = TheThingRegistry.GetThingsOfEngine(strEngineContext);
                                    List<string> listReturnValues = new List<string>();
                                    foreach (TheThing t in listThings)
                                    {
                                        listReturnValues.Add(t.cdeMID.ToString());
                                    }

                                    pRequest.ResponseMimeType = "application/json";
                                    string strJson = TheCommonUtils.SerializeObjectToJSONString<List<string>>(listReturnValues);
                                    pRequest.ResponseBuffer = TheCommonUtils.CUTF8String2Array(strJson);
                                    pRequest.StatusCode = (int)eHttpStatusCode.OK;
                                    pRequest.DontCompress = true;
                                    pRequest.AllowStatePush = false;
                                    bHandled = true;
                                }
                                else
                                {
                                    // Is user performing a query on all of the properties of a specific thing?
                                    // 
                                    // Sample URL: 
                                    //     http://c-labs-paul-yao-laptop:8700/ThingApi/*/b0710f68-5d0f-4e00-93ea-b32c1a5a40c7/Properties?key=a6221f63-c88c-4aa1-8a83-b0fd18a6811e
                                    //
                                    // Sample Return value:
                                    //     ["Id", "Value", "FriendlyName"]
                                    //
                                    // Sample URL to get all things for all engines: 
                                    //     http://c-labs-paul-yao-laptop:8700/ThingApi/*/Things?key=a6221f63-c88c-4aa1-8a83-b0fd18a6811e
                                    //
                                    // Value is Guid for a specific thing
                                    List<TheThing> listThings = TheThingRegistry.GetThingsOfEngine(strEngineContext);
                                    List<string> listReturnValues = new List<string>();
                                    foreach (TheThing t in listThings)
                                    {
                                        if (strThingContext == t.cdeMID.ToString())
                                        {
                                            if (strPropertyContext == "Properties")
                                            {
                                                // Enumerate all properties for the thing.

                                                bHandled = true;
                                            }
                                            else if (strPropertyContext == "Property")
                                            {
                                                // Act on an individual property

                                                bHandled = true;
                                            }

                                        }
                                    }

                                }
                            }

                        }
                    }
                }
            }

            if (!bHandled)
            {
                ResponseNotImplemented(pRequest);
            }
        }

        private static bool IsValidEngine(string strEngineContext)
        {
            bool bValidEngine = false;
            if (strEngineContext == "*")
                bValidEngine = true;
            else
            {
                List<string> listEngines = TheThingRegistry.GetEngineNames(true);
                foreach (string strEngineName in listEngines)
                {
                    if (strEngineContext == strEngineName)
                    {
                        bValidEngine = true;
                        break;
                    }
                }
            }

            return bValidEngine;
        }

        /// <summary>
        /// Called using this URL /ThingApi/QueryAccessToken?User=XXX&Pwd=YYY
        /// </summary>
        /// <param name="pRequest"></param>
        /// <param name="astrURLPath">Contains URL for Real Page split into parts.
        /// Example for above sample URL:
        ///            [0] = ThingApi
        ///            [1] = QueryAccessToken?User=XXX&Pwd=YYY
        /// </param>
        private static void ValidateUserCredentials(TheRequestData pRequest, string[] astrURLPath, Dictionary<string, string> aParameters)
        {
            if (astrURLPath.Length != 2)
            {
                ResponseNotImplemented(pRequest);
                return;
            }

            string strUser = aParameters["user"].ToString();
            string strPwd = aParameters["pwd"].ToString();

            if (strUser == "paul" && strPwd == "yao")
            {
                ResponseCreateToken(pRequest);
            }
            else
            {
                ResponseNotImplemented(pRequest);
            }
        }

        private static int ParseQueryParameters(string pQuery, Dictionary<string, string> pOut)
        {
            string[] astrParameters = pQuery.Split(new char[2] { '?', '&' }, StringSplitOptions.RemoveEmptyEntries);
            //foreach (string str in astrParameters)
            for (int i = 0; i < astrParameters.Length; i++)
            {
                string str = astrParameters[i];
                int iEqual = str.IndexOf("=");
                if (iEqual > 0 && (iEqual + 1 < str.Length))
                {
                    string strLeft = str.Substring(0, iEqual).ToLower();
                    string strRight = str.Substring(iEqual + 1).ToLower();
                    pOut[strLeft] = strRight;
                }
            }

            return pOut.Count;
        }

        private static void ResponseNotImplemented(TheRequestData pRequest)
        {
            pRequest.ResponseMimeType = "text/html";
            pRequest.ResponseBuffer = new byte[1];
            pRequest.ResponseBuffer[0] = 0;
            pRequest.StatusCode = (int)eHttpStatusCode.OK;
            pRequest.DontCompress = true;
            pRequest.AllowStatePush = false;
        }

        private static Dictionary<Guid, DateTime> dictValidAccessTokens = null; //  new Dictionary<Guid, DateTime>();

        private static void InitAccessTokenTable()
        {
            dictValidAccessTokens = new Dictionary<Guid, DateTime>();
        }

        private static bool IsTokenValid(Dictionary<string, string> aParameters)
        {
            bool bSuccess = false;

            CleanupOldTokens();

            if (aParameters.TryGetValue("key", out string strToken))
            {
                if (!String.IsNullOrEmpty(strToken))
                {
                    Guid guidToken = TheCommonUtils.CGuid(strToken);
                    DateTime dt;

                    if (dictValidAccessTokens != null)
                    {
                        if (dictValidAccessTokens.TryGetValue(guidToken, out dt))
                            bSuccess = true;
                    }
                }
            }


            return bSuccess;
        }

        private static void CleanupOldTokens()
        {
            if (dictValidAccessTokens != null)
            {
                List<Guid> keysToRemove = new List<Guid>();

                foreach (KeyValuePair<Guid,DateTime> kvp in dictValidAccessTokens)
                {
                    DateTime dt = DateTime.Now;
                    TimeSpan ts = (dt - kvp.Value);
                    if (ts.TotalHours > 4)
                        keysToRemove.Add(kvp.Key);
                }

                foreach (Guid g in keysToRemove)
                {
                    dictValidAccessTokens.Remove(g);
                }
            }
        }

            private static void ResponseCreateToken(TheRequestData pRequest)
        {
            Guid pNewToken = Guid.NewGuid();
            DateTime pNow = DateTime.Now;

            if (dictValidAccessTokens == null)
                InitAccessTokenTable();

            dictValidAccessTokens[pNewToken] = pNow;

            pRequest.ResponseMimeType = "application/json";
            string strJson = TheCommonUtils.SerializeObjectToJSONString<Guid>(pNewToken);
            pRequest.ResponseBuffer = TheCommonUtils.CUTF8String2Array(strJson);
            pRequest.StatusCode = (int)eHttpStatusCode.OK;
            pRequest.DontCompress = true;
            pRequest.AllowStatePush = false;
        }
    } // class
} // namespace
