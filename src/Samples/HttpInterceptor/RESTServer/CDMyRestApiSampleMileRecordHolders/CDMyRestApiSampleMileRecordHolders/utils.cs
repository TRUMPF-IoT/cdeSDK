// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using nsCDEngine.BaseClasses;
using nsCDEngine.Communication;
using nsCDEngine.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace CDMyRestApiSampleMileRecordHolders
{
    public partial class utils
    {
        public static Dictionary<string, string> ParseQueryParameters(string pQuery)
        {
            Dictionary<string, string> pOut = new Dictionary<string, string>();
            string[] astrParameters = pQuery.Split(new char[2] { '?', '&' }, 
                                      StringSplitOptions.RemoveEmptyEntries);

            foreach (string str in astrParameters)
            {
                string[] astrResult = str.Split(new char[1] { '=' }, 
                                      StringSplitOptions.RemoveEmptyEntries);
                if (astrResult.Length == 2)
                {
                    string key = astrResult[0].ToLower();
                    string value = astrResult[1].ToLower();
                    pOut[key] = value;
                }
            }

            return pOut;
        }

        /// <summary>
        /// Called using a URL like this: /xxxx/xxxxx/Logon?user=xxxx&pwd=yyyyy
        /// </summary>
        /// <param name="pRequest">TheRequestData receive in http interceptor.</param>
        /// <param name="aParameters">Parameter values parsed into a dictionary.</param>
        /// 
        public static bool ValidateUserCredentials(TheRequestData pRequest, Dictionary<string, string> aParameters)
        {
            bool bSuccess = false;

            if (aParameters != null & aParameters.Count > 1)
            {
                string strUser = aParameters["user"].ToString();
                string strPwd = aParameters["pwd"].ToString();

                if (strUser != null && strPwd != null)
                {
                    if (strUser == "myuser" && strPwd == "asterisks")
                    {
                        Guid pToken = CreateToken(pRequest);
                        SaveToValidTokenTable(pToken);

                        // Return access token to caller.
                        SetResponseValue(pRequest, pToken);
                        bSuccess = true;
                    }
                }
            }

            if (bSuccess == false)
            {
                SetEmptyResponse(pRequest);
            }

            return bSuccess;
        }


        public static void SetResponseValue(TheRequestData pRequest, Guid guidValue)
        {
            pRequest.ResponseMimeType = "application/json";
            string strJson = TheCommonUtils.SerializeObjectToJSONString<Guid>(guidValue);
            pRequest.ResponseBuffer = TheCommonUtils.CUTF8String2Array(strJson);
            pRequest.StatusCode = (int)eHttpStatusCode.OK;
            pRequest.DontCompress = true;
            pRequest.AllowStatePush = false;
        }

        public static void SetEmptyResponse(TheRequestData pRequest)
        {
            pRequest.ResponseMimeType = "text/html";
            pRequest.ResponseBuffer = new byte[1];
            pRequest.ResponseBuffer[0] = 0;
            pRequest.StatusCode = (int)eHttpStatusCode.OK;
            pRequest.DontCompress = true;
            pRequest.AllowStatePush = false;
        }


    } // class
} // namespace
