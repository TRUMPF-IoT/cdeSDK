// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

using nsCDEngine.BaseClasses;
using nsCDEngine.Interfaces;
using nsCDEngine.ViewModels;
using System;
using System.Security.Cryptography;
using CU = nsCDEngine.BaseClasses.TheCommonUtils;

namespace nsSample.Security
{
    /// <summary>
    /// The Scope Manager Core
    /// </summary>
    internal class SampleScopeManager : ICDEScopeManager
    {
        private ICDESecrets MySecrets = null;
        private ICDESystemLog MySYSLOG = null;
        public SampleScopeManager(ICDESecrets pSecrets, ICDESystemLog pSysLog = null)
        {
            MySecrets = pSecrets;
            MySYSLOG = pSysLog;
        }

        #region ICDEScopeManager Interfaces
        public void SetMiniHSI(TheServiceHostInfoMini pMini)
        {
            MyServiceHostInfo = pMini;
        }

        private string FastSScopeID = null;
        private string mScId;
        public string ScopeID
        {
            get { return mScId; }
            set { mScId = value; FastSScopeID = null; }
        }
        public bool IsScopingEnabled { get; set; }
        public string FederationID { get; set; }

        public void SetScopeIDFromScrambledID(string pScrambledId)
        {
            if (string.IsNullOrEmpty(pScrambledId))
                return;
            ScopeID = GetRealScopeID(pScrambledId);     //GRSI: Low frequency
            IsScopingEnabled = true;
            eventScopeIDChanged?.Invoke(false);
        }

        public bool IsInScope(string pMsgSID, bool IsRealScopeID)
        {
            string pMsgFID = "";
            bool TestForFID = false;
            if (IsScopingEnabled && !string.IsNullOrEmpty(FederationID) && !string.IsNullOrEmpty(pMsgSID))
            {
                TestForFID = true;
                string[] t = pMsgSID.Split(';');
                if (t.Length > 1)
                {
                    pMsgSID = t[0];
                    pMsgFID = t[1];
                }
            }
            if (IsRealScopeID)
                return (!IsScopingEnabled && string.IsNullOrEmpty(pMsgSID)) ||
                    (!string.IsNullOrEmpty(ScopeID) && ScopeID.Equals(pMsgSID)) ||
                    (TestForFID && !string.IsNullOrEmpty(FederationID) && FederationID.Equals(GetRealScopeID(pMsgFID)));     //GRSI: rare
            else
                return (!IsScopingEnabled && string.IsNullOrEmpty(pMsgSID)) ||
                    (!string.IsNullOrEmpty(ScopeID) && ScopeID.Equals(GetRealScopeID(pMsgSID))) ||     //GRSI: rare
                    (TestForFID && !string.IsNullOrEmpty(FederationID) && FederationID.Equals(GetRealScopeID(pMsgFID)));     //GRSI: rare
        }

        public string GetRealScopeID(string pScramScopeId)
        {
            if (string.IsNullOrEmpty(pScramScopeId)) return "";

            pScramScopeId = pScramScopeId.Split(chunkSeparator, StringSplitOptions.None)[0];
            string[] tStr = CU.cdeDecrypt(pScramScopeId, MySecrets?.GetAI(), true).Split('&');
            MyServiceHostInfo?.MyKPIs?.IncrementKPI(eKPINames.KPI5);
            if (tStr.Length > 1 && MySecrets?.IsInApplicationScope(tStr[1])==true)
                return tStr[1];
            else
                return "";
        }

        public string GetScrambledScopeID()
        {
            if (!IsScopingEnabled) return "";
            if (FastSScopeID != null)
                return FastSScopeID;
            MyServiceHostInfo?.MyKPIs?.IncrementKPI(eKPINames.KPI4);
            string t = CU.cdeEncrypt(CU.GetRandomUInt(0, 1000) + "&" + ScopeID, MySecrets?.GetAI());
            if (MyServiceHostInfo?.EnableFastSecurity == true)
                FastSScopeID = t;
            return t;
        }

        public string GetScrambledScopeID(string pRealScopeID, bool IsRealId)
        {
            if (string.IsNullOrEmpty(pRealScopeID)) return "";

            MyServiceHostInfo?.MyKPIs?.IncrementKPI(eKPINames.KPI4);
            if (IsRealId)
                return CU.cdeEncrypt(CU.GetRandomUInt(0, 1000) + "&" + pRealScopeID, MySecrets?.GetAI());
            else
                return CU.cdeEncrypt(CU.GetRandomUInt(0, 1000) + "&" + GetRealScopeID(pRealScopeID), MySecrets?.GetAI()); //GRSI: Low frequency
        }
        public string GetRealScopeIDFromEasyID(string pEasyID, bool bNoLogging=false, bool bUseEasyScope16=false)
        {
            Guid tG = CU.CGuid(MySecrets?.GetAK());
            string tg = InsertCodeIntoGUID(pEasyID, tG, bNoLogging, bUseEasyScope16).ToString();
            return CU.cdeEncrypt(CU.CGuid(tg.Substring(0, 11) + tG.ToString().Substring(11, "00-0000-0000-000000000000".Length)).ToByteArray(), MySecrets?.GetAI());
        }


        public string GetRealScopeIDFromTopic(string pScopedTopic, out string TopicName)
        {
            TopicName = pScopedTopic;
            if (string.IsNullOrEmpty(pScopedTopic)) return "";
            string[] tt = pScopedTopic.Split('@');
            TopicName = tt[0];
            if (tt.Length > 1)
            {
                var tR = GetRealScopeID(tt[1]);     //GRSI: higher frequency - at least once for each TSM to be published
                if (tR.Length > 0)
                    return tR;
            }
            return "";
        }

        /// <summary>
        /// Adds the ScopeID (Security ID) to the topic.
        /// The Result will look like TOPIC@SCRAMBLEDSCOPEID
        /// </summary>
        /// <param name="pTopic">Topic that will receive the ScopeID</param>
        /// <returns></returns>
        public string AddScopeID(string pTopic)
        {
            string tTSMSID = null;
            return AddScopeID(pTopic, ScopeID, ref tTSMSID, true, true);
        }
        /// <summary>
        /// Adds the ScopeID (Security ID) to the topic(s).
        /// The Result will look like TOPIC@SCRAMBLEDSCOPEID;NEXTTOPIC@SCRAMBLEDID...
        /// </summary>
        /// <param name="pTopics">A list of topics separated by ;</param>
        /// <param name="bFirstTopicOnly">If set to true, only the first topic will receive the ScopeID</param>
        /// <returns></returns>
        public string AddScopeID(string pTopics, bool bFirstTopicOnly)
        {
            string tTSMSID = null;
            return AddScopeID(pTopics, ScopeID, ref tTSMSID, bFirstTopicOnly, true);
        }
        /// <summary>
        /// Adds the ScopeID (Security ID) to the topic(s).
        /// The Result will look like TOPIC@SCRAMBLEDSCOPEID;NEXTTOPIC@SCRAMBLEDID...
        /// </summary>
        /// <param name="pTopics">A list of topics separated by ;</param>
        /// <param name="pRealScope">Allows to provide a scrambled ScopeID that will be added to the Topics</param>
        /// <returns></returns>
        public string AddScopeID(string pTopics, bool bFirstTopicOnly, string pRealScope)
        {
            string tTSMSID = null;
            return AddScopeID(pTopics, pRealScope, ref tTSMSID, bFirstTopicOnly, true);
        }
        /// <summary>
        /// Adds the ScopeID (Security ID) to the topic(s).
        /// The Result will look like TOPIC@SCRAMBLEDSCOPEID;NEXTTOPIC@SCRAMBLEDID...
        /// </summary>
        /// <param name="pTopics">A list of topics separated by ;</param>
        /// <param name="pMessage">If Message is not null and has a ScopeID, the ScopeID of the message is taken instead of the ScopeID of this node
        /// This can be useful when returning a scoped (Secure) message to an Originator that has a different scope than the current node
        /// </param>
        /// <param name="bFirstTopicOnly">If set to true, only the first topic will receive the ScopeID</param>
        /// <returns></returns>
        public string AddScopeID(string pTopics, ref string pMessage, bool bFirstTopicOnly)
        {
            return AddScopeID(pTopics, ScopeID, ref pMessage, bFirstTopicOnly, true);
        }

        /// <summary>
        /// Adds the ScopeID (Security ID) to the topic(s).
        /// The Result will look like TOPIC@SCRAMBLEDSCOPEID;NEXTTOPIC@SCRAMBLEDID...
        /// </summary>
        /// <param name="pTopics">A list of topics separated by ;</param>
        /// <param name="pScramScopeID">Allows to provide a scrambled ScopeID that will be added to the Topics</param>
        /// <param name="pMessage">If Message is not null and has a ScopeID, the ScopeID of the message is taken instead of the ScopeID of this node
        /// This can be useful when returning a scoped (Secure) message to an Originator that has a different scope than the current node
        /// </param>
        /// <param name="bFirstTopicOnly">If set to true, only the first topic will receive the ScopeID</param>
        /// <param name="IsScramRScope">If true the parameter is a real-scopeid</param>
        /// <returns></returns>
        public string AddScopeID(string pTopics, string pScramScopeID, ref string pMessageSID, bool bFirstTopicOnly, bool IsScramRScope)
        {
            if (string.IsNullOrEmpty(pTopics)) return "";
            if (string.IsNullOrEmpty(pScramScopeID) && string.IsNullOrEmpty(pMessageSID) == true)
                return pTopics;
            else
            {
                string tTopic = ""; //perf tuning <- allocation of string on stack costs time
                string tRealS = !string.IsNullOrEmpty(pScramScopeID) ? (IsScramRScope ? pScramScopeID : GetRealScopeID(pScramScopeID)) : GetRealScopeID(pMessageSID);    //GRSI: possible high frequency - needs research
                string[] tTops = pTopics.Split(';');
                string tScrambledID = null;
                foreach (string t in tTops)
                {
                    if (string.IsNullOrEmpty(t)) continue;
                    if (bFirstTopicOnly && tTopic.Length > 0)
                    {
                        tTopic += ";" + t;
                        continue;
                    }

                    if (tTopic.Length > 0)
                        tTopic += ";";
                    if (!string.IsNullOrEmpty(pScramScopeID))
                    {
                        if (tRealS.Length > 0)
                        {
                            if (FindRealScopeID(t).Equals(tRealS))   //GRSI: medium
                                tTopic += t;
                            else
                            {
                                if (!t.Contains("@"))
                                {
                                    if (tScrambledID == null || !MyServiceHostInfo.EnableFastSecurity)    //SECURITY: If EnableFast Security is true, all Topics wil have same SScopeID
                                    {
                                        tScrambledID = GetScrambledScopeID(pScramScopeID, IsScramRScope);   //GRSI: rare
                                    }
                                    if (pMessageSID != null)
                                        pMessageSID = (IsScramRScope ? tScrambledID : pScramScopeID);  //SID in TSM must match Topic SID otherwise QSender will reject message routing
                                    tTopic += t + "@" + (IsScramRScope ? tScrambledID : pScramScopeID);
                                    if (!string.IsNullOrEmpty(FederationID))
                                        tTopic += ":" + FederationID;
                                }
                                else
                                    tTopic += t;
                            }
                        }
                        else
                            tTopic += t;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(tRealS) && !t.Contains("@") && !FindRealScopeID(t).Equals(tRealS))  //GRSI: rare
                        {
                            tTopic += t + "@" + pMessageSID;
                            if (!string.IsNullOrEmpty(FederationID))
                                tTopic += ":" + FederationID;
                        }
                        else
                            tTopic += t;
                    }
                }
                return tTopic;
            }
        }

        public string RemoveScopeID(string pCommand, bool IsUnscopedAllowed, bool IsAllowedForeignProcessing)
        {
            if (string.IsNullOrEmpty(pCommand)) return pCommand;
            if (!IsScopingEnabled)
            {
                if (IsUnscopedAllowed && pCommand.Contains("@"))
                    return pCommand.Substring(0, pCommand.IndexOf("@", StringComparison.Ordinal));
                return pCommand;
            }
            string tRSID = FindRealScopeID(pCommand);      //GRSI: rare
            if (string.IsNullOrEmpty(tRSID))
            {
                if (IsUnscopedAllowed)
                    return pCommand;
                return "SCOPEVIOLATION";
            }
            else
            {
                if (!IsAllowedForeignProcessing && !ScopeID.Equals(tRSID))
                {
                    if (string.IsNullOrEmpty(RProSco) || !tRSID.Equals(RProSco))
                        return "SCOPEVIOLATION";
                }
                string tC = pCommand.Substring(0, pCommand.IndexOf('@'));
                return tC;
            }
        }

        public string GetSessionIDFromISB(string pRealPage)
        {
            var tRealPage = CU.cdeDecrypt(pRealPage, MySecrets?.GetAI(), true);
            if (tRealPage?.StartsWith(MySecrets?.GetApID5()) != true)
                return null;

            string[] tQ = tRealPage.Split('&');

            if (tQ.Length > 3) return tQ[3];
            return null;
        }

        public bool ParseISBPath(string pRealPage, out Guid? pSessionID, out cdeSenderType pType, out long pFID, out string pVersion)
        {
            pSessionID = null;
            pType = cdeSenderType.NOTSET;
            pFID = 0;
            pVersion = "";
            var tRealPage = CU.cdeDecrypt(pRealPage, MySecrets?.GetAI(), true);
            if (tRealPage?.StartsWith(MySecrets?.GetApID5()) != true)
                return false;

            string[] tQ = tRealPage.Split('&');

            pType = tQ.Length == 4 ? (cdeSenderType)(CU.CInt(tQ[2])) : cdeSenderType.NOTSET;
            if (tQ.Length > 1) pFID = CU.CLng(tQ[1]);
            if (tQ.Length > 3) pSessionID = CU.CGuid(tQ[3]);
            if (tQ[0].Length == 7)
                pVersion = tQ[0].Substring(5);
            return true;
        }

        public string GetISBPath(string PathPrefix, cdeSenderType pOriginType, cdeSenderType pDestinationType, long pCounter, Guid pSessionID, bool IsWS)
        {
            if (string.IsNullOrEmpty(MySecrets?.GetApID5())) return "";
            if (string.IsNullOrEmpty(PathPrefix))
                PathPrefix = "/";
            else
            {
                if (!PathPrefix.EndsWith("/")) PathPrefix += "/";
            }
            string pSessID = pSessionID.ToString();
            if (pCounter == 1 && pSessionID == Guid.Empty && !(MyServiceHostInfo?.UseFixedConnectionUrl==true))
                pSessID = "P" + GetCryptoGuid().ToString().Substring(1);
            MyServiceHostInfo?.MyKPIs?.IncrementKPI(eKPINames.KPI4);
            string tPath = PathPrefix + "ISB" + Uri.EscapeUriString(CU.cdeEncrypt($"{MySecrets?.GetApID5()}{(MyServiceHostInfo?.ProtocolVersion == null ? "40" : MyServiceHostInfo?.ProtocolVersion)}&{pCounter}&{((int)(pOriginType))}&{pSessID}", MySecrets?.GetAI()));  //3.083: Must be cdeAI
            if (IsWS && (pDestinationType == cdeSenderType.CDE_CLOUDROUTE || pDestinationType == cdeSenderType.CDE_SERVICE || pDestinationType == cdeSenderType.CDE_CUSTOMISB)) //TODO: Might need this for Local Services that are IIS but not CloudRoutes
                tPath += ".ashx";
            return tPath;
        }

        public void SetProSeScope(string pSScope)
        {
            RProSco = GetRealScopeID(pSScope);   //GRSI: rare
        }

        /// <summary>
        /// Sets the ScopeID of the current node using the EasyScopeID given.
        /// The system does not use the EasyScopeID for any encryption but creates a real-scopeID from the easyID
        /// </summary>
        /// <param name="pEasyID"></param>
        public void SetScopeIDFromEasyID(string pEasyID)
        {
            //if (MyScopeManager == null)
            //{
            //    inScopeID = pEasyID;
            //    return; //NO LONGER ALLOWED TO AUTO CREATE SCOPE MANAGER - Request HSI
            //            //MyScopeManager = new TheScopeManager();
            //}
            bool ReqConfig = false;
            if (!string.IsNullOrEmpty(pEasyID))
            {
                ScopeID = GetRealScopeIDFromEasyID(pEasyID);       //GRSI: rare
                                                                   //MyScopeManager.EasyScope = pEasyID; //SECURITY-REVIEW: No more ScopeID in clear text
                IsScopingEnabled = true;
            }
            else //NEW: RC1.1
            {
                IsScopingEnabled = false;
                ReqConfig = true;
            }
            eventScopeIDChanged?.Invoke(ReqConfig);
        }

        private Action<bool> eventScopeIDChanged;
        public void RegisterScopeChanged(Action<bool> pCallback)
        {
            eventScopeIDChanged += pCallback;
        }
        public void UnregisterScopeChanged(Action<bool> pCallback)
        {
            eventScopeIDChanged -= pCallback;
        }



        /// <summary>
        /// Used to create a random new EasyScopeID.
        /// </summary>
        /// <returns></returns>
        public string GenerateNewScopeID()
        {
            cdeSenderType tSender = cdeSenderType.NOTSET;
            if (MyServiceHostInfo != null)
                tSender = MyServiceHostInfo.MyDeviceSenderType;
            return CalculateRegisterCode(GenerateNewAppDeviceID(tSender));
        }

        /// <summary>
        /// In order to tag data with a given ScopeID (Node Realm), this function can be used to get a Hash of the CurrentScopeID.
        /// This hash is not used for any encryption and a scopeID cannot be recreated from this Hash
        /// </summary>
        /// <returns></returns>
        public string GetTokenFromScopeID()
        {
            if (!IsScopingEnabled) return "";
            return ScopeID.GetHashCode().ToString();
        }
        /// <summary>
        /// Similar to the GetTokenFromScopeID, this function returns a hash from an easy ID given
        /// </summary>
        /// <param name="pEasyID"></param>
        /// <returns></returns>
        public string GetTokenFromEasyScopeID(string pEasyID)
        {
            if (string.IsNullOrEmpty(pEasyID)) return "";
            string tID = GetRealScopeIDFromEasyID(pEasyID);       //GRSI: rare
            return tID.GetHashCode().ToString();
        }
        /// <summary>
        /// Use this function to get the hash of a scopeID if you only have a ScrambledScopeID
        /// </summary>
        /// <param name="SScopeID"></param>
        /// <returns></returns>
        public string GetTokenFromScrambledScopeID(string SScopeID)
        {
            if (string.IsNullOrEmpty(SScopeID)) return "";
            string tID = GetRealScopeID(SScopeID);       //GRSI: Low frequency
            return tID.GetHashCode().ToString();
        }

        /// <summary>
        /// This method creates a new scrambled ScopeID from an Easy ScopeID to allow encryption of data against a ScopeID
        /// </summary>
        /// <param name="pEasyScope"></param>
        /// <returns></returns>
        public string GetScrambledScopeIDFromEasyID(string pEasyScope)
        {
            return GetScrambledScopeIDFromEasyID(pEasyScope, false, MyServiceHostInfo?.UseEasyScope16==true);
        }


        /// <summary>
        /// Return a Scrambled ScopeID from an easy scopeID
        /// </summary>
        /// <param name="pEasyScope">Source EasyScope</param>
        /// <param name="bNoLogging">If true, errors will not be logged</param>
        /// <param name="bUseEasyScope16">if true, the EasyScopeID can have up to 16 characters. Attention: this might not be compatible to existing mesh setups as all nodes in a mesh need to understand 16chars ScopeIDs</param>
        /// <returns></returns>
        public string GetScrambledScopeIDFromEasyID(string pEasyScope, bool bNoLogging, bool bUseEasyScope16)
        {
            if (string.IsNullOrEmpty(pEasyScope)) return null;  //NEW 10/20/2012
            Guid tG = CU.CGuid(MySecrets?.GetAK());
            string tg = InsertCodeIntoGUID(pEasyScope, tG, bNoLogging, bUseEasyScope16).ToString();
            MyServiceHostInfo?.MyKPIs?.IncrementKPI(eKPINames.KPI4);
            return CU.cdeEncrypt(CU.GetRandomUInt(0, 1000) + "&" + CU.cdeEncrypt(CU.CGuid(tg.Substring(0, 11) + tG.ToString().Substring(11, "00-0000-0000-000000000000".Length)).ToByteArray(), MySecrets?.GetAI()), MySecrets?.GetAI());
        }

        /// <summary>
        /// Returns a short hash from a given Topic. If pTopic is null, the hash of the Scope of the current node is used
        /// </summary>
        /// <param name="pTopic">Subscription topic or ScrambledScopeID</param>
        /// <param name="IsSScope">Set to true if pTopic is a scrambled scopeID</param>
        /// <returns></returns>
        public string GetScopeHash(string pTopic, bool IsSScope = false)
        {
            if (string.IsNullOrEmpty(pTopic))
                return ScopeID.ToUpper().Substring(0, 4);
            string t;
            if (IsSScope)
                t = GetRealScopeID(pTopic);    //GRSI: Low frequency
            else
                t = FindRealScopeID(pTopic);
            if (!string.IsNullOrEmpty(t) && t.Length > 4)
                return t.Substring(0, 4).ToUpper();
            return "not scoped";
        }

        /// <summary>
        /// Returns the cdeSenderType of a given NodeID
        /// </summary>
        /// <param name="pNodeID">Source NodeID</param>
        /// <returns></returns>
        public cdeSenderType GetSenderTypeFromDeviceID(Guid pNodeID)
        {
            cdeSenderType tRes = cdeSenderType.NOTSET;
            string tN = pNodeID.ToString();
            if (string.IsNullOrEmpty(AKEnd))
                AKEnd = CU.CGuid(MySecrets?.GetAK()).ToString().Substring(29, 6);
            if (tN.EndsWith(AKEnd))
                tRes = (cdeSenderType)CU.CInt(tN.Substring(29, 1));
            return tRes;
        }

        /// <summary>
        /// Verify if the given ScrambledScopeID is valid in the current scope
        /// </summary>
        /// <param name="pScramScopeID1"></param>
        /// <returns></returns>
        public bool IsValidScopeID(string pScramScopeID1)
        {
            return IsValidScopeID(pScramScopeID1, GetScrambledScopeID());     //GRSI: rare
        }
        /// <summary>
        /// Compare two ScrambledScopeIDs if they are of the same Scope
        /// </summary>
        /// <param name="pScramScopeID1"></param>
        /// <param name="pScramScopeID2"></param>
        /// <returns></returns>
        public bool IsValidScopeID(string pScramScopeID1, string pScramScopeID2)
        {
            if (string.IsNullOrEmpty(pScramScopeID1) && string.IsNullOrEmpty(pScramScopeID2)) return true;
            if (string.IsNullOrEmpty(pScramScopeID1) || string.IsNullOrEmpty(pScramScopeID2)) return false;
            string tRealScopeID1 = GetRealScopeID(pScramScopeID1);   //GRSI: low frequency
            if (string.IsNullOrEmpty(tRealScopeID1)) return false;
            return tRealScopeID1.Equals(GetRealScopeID(pScramScopeID2));  //GRSI: low frequency
        }

        /// <summary>
        /// Used to create a random new EasyScopeID.
        /// </summary>
        /// <returns></returns>
        public string GenerateCode(int pDigits = 8)
        {
            cdeSenderType tSender = cdeSenderType.NOTSET;
            if (MyServiceHostInfo != null)
                tSender = MyServiceHostInfo.MyDeviceSenderType;
            return GetRegisterCode(GenerateNewAppDeviceID(tSender), pDigits);
        }

        /// <summary>
        /// Creates a new DeviceID/NodeID for the current Application Scope
        /// </summary>
        /// <param name="tS">Sender Type of the Node</param>
        /// <returns></returns>
        public Guid GenerateNewAppDeviceID(cdeSenderType tS)
        {
            string tg = GetCryptoGuid().ToString();
            //return new Guid(tg.Substring(0, 11) + ((int)tS).ToString() + (new Guid(TheBaseAssets.cdeAK)).ToString().Substring(12, "0-0000-0000-000000000000".Length));
            string t = tg.Substring(0, 29);
            t += ((int)tS).ToString();
            t += (CU.CGuid(MySecrets?.GetAK())).ToString().Substring(29, 6);
            return CU.CGuid(t);
        }

        public string CalculateRegisterCode(Guid pGuid)
        {
            string tstr = pGuid.ToString().Substring(0, 8) + pGuid.ToString().Substring(9, 2);
            ulong calc = ulong.Parse(tstr, System.Globalization.NumberStyles.AllowHexSpecifier);

            string code = "";
            for (int i = 0; i < 8; i++)
            {
                int tt = (int)(calc >> (i * 5));
                int tShift = tt & 0x1F;
                code = MySecrets?.GetCodeArray()[tShift] + code;
            }

            return code;
        }
        #endregion

        private string RProSco { get; set; }
        private static TheServiceHostInfoMini MyServiceHostInfo = null;
        private static string AKEnd = null;
        private static readonly string[] chunkSeparator = new string[] { ":,:" };

        private static Guid GetCryptoGuid()
        {
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            byte[] data = new byte[16];
            crypto.GetBytes(data);
            return CU.CGuid(data);
        }

        private string GetRegisterCode(Guid pGuid, int pDigits = 8)
        {
            string tstr = pGuid.ToString().Replace("-", "").Substring(0, 16);
            ulong calc = ulong.Parse(tstr, System.Globalization.NumberStyles.AllowHexSpecifier);

            string code = "";
            for (int i = 0; i < pDigits; i++)
            {
                int tt = (int)(calc >> (i * 5));
                int tShift = tt & 0x1F;
                code = MySecrets.GetCodeArray()[tShift] + code;
            }

            return code;
        }

        /// <summary>
        /// Looks in the topics for a RealScopeID and returns the first found
        /// Topics cannot have different RealSCopeIDs!!
        /// </summary>
        /// <param name="tScopedTopics"></param>
        /// <returns></returns>
        private string FindRealScopeID(string tScopedTopics)
        {
            if (string.IsNullOrEmpty(tScopedTopics)) return "";
            string[] t = tScopedTopics.Split(';');
            foreach (string str in t)
            {
                string[] tt = str.Split('@');
                if (tt.Length > 1)
                {
                    string tRSID = GetRealScopeID(tt[1]);       //GRSI: High frequency - needs tuning
                    if (!string.IsNullOrEmpty(tRSID))
                        return tRSID;
                }
            }
            return "";
        }




        private Guid InsertCodeIntoGUID(string pCode, Guid pGuid, bool bNoLogging, bool useEasyScope16)
        {
            long calc = 0;
            if (pCode.Length < 8)
            {
                pCode += "00000000".Substring(0, 8 - pCode.Length);
            }
            else
            {
                if (!bNoLogging && pCode.Length > 8 && MyServiceHostInfo?.UseEasyScope16 != true)
                {
                    MyServiceHostInfo?.MySYSLOG?.WriteToLog(MyServiceHostInfo?.IsCloudService == true ? eDEBUG_LEVELS.VERBOSE : eDEBUG_LEVELS.OFF, 2353, "TheScopeManager", "EasyID was longer than 8 characters: extra characters were ignored.", eMsgLevel.l2_Warning);
                }
            }
            var pCodeUpper = pCode.ToUpper();
            var pCodeNormalized = pCodeUpper.Replace('O', '0').Replace('U', 'V').Replace('I', 'J').Replace('L', 'J');
            if (!bNoLogging && !string.Equals(pCodeNormalized, pCodeUpper, StringComparison.Ordinal))
            {
                //Do not log in cloud! This can fill the log if too many scopeIDs are using these chars (which is not a problem in the cloud)
                MyServiceHostInfo?.MySYSLOG?.WriteToLog(MyServiceHostInfo?.IsCloudService == true ? eDEBUG_LEVELS.VERBOSE : eDEBUG_LEVELS.OFF, 2354, "TheScopeManager", "EasyID contained reserved characters (O, V, I or L) that were normalized to (0, U, or J).", eMsgLevel.l2_Warning);
            }
            char[] tAr = pCodeNormalized.ToCharArray();
            for (int i = 0; i < 8; i++)
            {
                long tt = MySecrets.GetCodeArray().IndexOf(tAr[i]);
                calc <<= 5;
                calc += tt;
            }
            string tg = string.Format("{0:x10}", calc);
            if (useEasyScope16) //Use16 digits ScopeID with new ApplicationID insert
            {
                for (int i = 8; i < pCode.Length; i++)
                {
                    long tt = MySecrets.GetCodeArray().IndexOf(tAr[i]);
                    calc <<= 5;
                    calc += tt;
                }
                tg = string.Format("{0:x16}", calc);
                tg = tg.Substring(0, 8) + "-" + tg.Substring(8, 4) + "-" + tg.Substring(12, 4);
                tg += pGuid.ToString().Substring(18, "-0000-000000000000".Length);
            }
            else
            {
                tg = tg.Substring(0, 8) + "-" + tg.Substring(8, 2);
                tg += pGuid.ToString().Substring(11, "00-0000-0000-000000000000".Length);
            }
            return CU.CGuid(tg);
        }
    }
}