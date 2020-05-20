// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using nsCDEngine.BaseClasses;
using nsCDEngine.Communication;
using nsCDEngine.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace CDMyWebToMeshSample
{
    public partial class utils
    {
        public static int ParseQueryParameters(string pQuery, Dictionary<string, string> pOut)
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

        public static bool CheckCommandLineFlag(string strFlagToCheck)
        {
            bool bResult = false;
            string strValue;
            if (TheBaseAssets.MyCmdArgs.TryGetValue(strFlagToCheck, out strValue))
            {
                bool.TryParse(strValue, out bResult);
            }

            return bResult;
        }

        public static bool QueryIntFromDictionary(Dictionary<string, string> d, string strKey, out int value)
        {
            bool bSuccess = false;
            value = -1;
            if (d.ContainsKey(strKey))
            {
                string strValue = d[strKey];
                if (int.TryParse(strValue, out value))
                {
                    bSuccess = true;
                }
            }

            return bSuccess;
        }

        public static void CreateJsonResponse<T>(TheRequestData pRequest, T responsedata)
        {
            pRequest.ResponseMimeType = "application/json";
            string strJson = TheCommonUtils.SerializeObjectToJSONString<T>(responsedata);
            pRequest.ResponseBuffer = TheCommonUtils.CUTF8String2Array(strJson);
            pRequest.StatusCode = (int)eHttpStatusCode.OK;
            pRequest.DontCompress = true;
            pRequest.AllowStatePush = false;
        }

        public static void CreateEmptyResponse(TheRequestData pRequest)
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
