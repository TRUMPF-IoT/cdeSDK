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
        private static Dictionary<Guid, DateTime> dictValidAccessTokens = null;

        private static void InitAccessTokenTable()
        {
            dictValidAccessTokens = new Dictionary<Guid, DateTime>();
        }

        private static Guid CreateToken(TheRequestData pRequest)
        {
            Guid pNewToken = Guid.NewGuid();

            return pNewToken;
        }

        private static bool SaveToValidTokenTable(Guid pToken)
        {
            bool bSuccess = false;

            try
            {
                if (dictValidAccessTokens == null)
                    InitAccessTokenTable();

                DateTime pNow = DateTime.Now;
                dictValidAccessTokens[pToken] = pNow;

                bSuccess = true;
            }
            catch { }

            return bSuccess;
        }

        public static bool IsTokenValid(Dictionary<string, string> aParameters)
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

                foreach (KeyValuePair<Guid, DateTime> kvp in dictValidAccessTokens)
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

    } // class
} // namespace
