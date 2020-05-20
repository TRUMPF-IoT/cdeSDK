// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

using nsCDEngine.Interfaces;

namespace nsSample.Security
{
    public class TheSampleCodeSigning : ICDECodeSigning
    {
        private static ICDESecrets MySecrets = null;
        private static ICDESystemLog MySYSLOG = null;
        public TheSampleCodeSigning(ICDESecrets pSecrets, ICDESystemLog pSysLog = null)
        {
            MySecrets = pSecrets;
            MySYSLOG = pSysLog;
        }

        /// <summary>
        /// Gets the application cert.
        /// </summary>
        /// <param name="bDontVerifyTrust">if set to <c>true</c> [b the CDE will not verify the certificate].</param>
        /// <param name="pFromFile">The file from where to load the certificate.</param>
        /// <param name="bVerifyTrustPath">if set to <c>true</c> [b verify trust path].</param>
        /// <param name="bDontVerifyIntegrity">if set to <c>true</c> [b dont verify integrity].</param>
        /// <returns>A thumbprint of the Certificate.</returns>
        public string GetAppCert(bool bDontVerifyTrust=false, string pFromFile = null, bool bVerifyTrustPath=true, bool bDontVerifyIntegrity=false)
        {
            return "nothumb";
        }

        /// <summary>
        /// Determines whether the specified file is trusted.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns><c>true</c> if the specified file name is trusted; otherwise, <c>false</c>.</returns>
        public bool IsTrusted(string fileName)
        {
            return true;
        }
    }
}
