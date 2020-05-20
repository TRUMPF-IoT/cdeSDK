// SPDX-FileCopyrightText: 2017-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

using nsCDEngine.BaseClasses;
using nsCDEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using CU = nsCDEngine.BaseClasses.TheCommonUtils;

namespace nsSample.Security
{
    /// <summary>
    /// Class TheCoreCrypto.
    /// Implements the <see cref="nsCDEngine.Interfaces.ICDECrypto" />
    /// </summary>
    /// <seealso cref="nsCDEngine.Interfaces.ICDECrypto" />
    internal class TheSampleCrypto : ICDECrypto
    {
        private ICDESecrets MySecrets = null;
        private ICDESystemLog MySYSLOG = null;
        public TheSampleCrypto(ICDESecrets pSecrets, ICDESystemLog pSysLog = null)
        {
            MySecrets = pSecrets;
            MySYSLOG = pSysLog;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Encrypts a buffer using AES with an AK and AI. </summary>
        ///
        /// <remarks>   Chris, 4/2/2020. </remarks>
        ///
        /// <param name="toEncrypt">    to encrypt. </param>
        /// <param name="AK">           The ak. </param>
        /// <param name="AI">           The ai. </param>
        ///
        /// <returns>   A byte[]. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public byte[] Encrypt(byte[] toEncrypt, byte[] AK, byte[] AI)
        {
            AesManaged myRijndael = new AesManaged();
            ICryptoTransform encryptor = myRijndael.CreateEncryptor(AK, AI);
            MemoryStream msEncrypt = new MemoryStream();
            CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
            csEncrypt.FlushFinalBlock();
            return msEncrypt.ToArray();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Decrypts a buffer using AES with AI and AK </summary>
        ///
        /// <remarks>   Chris, 4/2/2020. </remarks>
        ///
        /// <param name="toDecrypt">    to decrypt. </param>
        /// <param name="AK">           The ak. </param>
        /// <param name="AI">           The ai. </param>
        ///
        /// <returns>   A byte[]. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public byte[] Decrypt(byte[] toDecrypt, byte[] AK, byte[] AI)
        {
            AesManaged myRijndael = new AesManaged();
            byte[] fromEncrypt;
            ICryptoTransform decryptor = myRijndael.CreateDecryptor(AK, AI);
            MemoryStream msDecrypt = new MemoryStream(toDecrypt);
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            {
                fromEncrypt = new byte[toDecrypt.Length];
                csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);
            }
            return fromEncrypt;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Query if 'OrgBuffer' has buffer correct length according to the buffer type. </summary>
        ///
        /// <remarks>   Chris, 4/2/2020. </remarks>
        ///
        /// <param name="OrgBuffer">    Buffer for organisation data. </param>
        /// <param name="pBufferType">  Type of the buffer. </param>
        ///
        /// <returns>   True if buffer correct length, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool HasBufferCorrectLength(string OrgBuffer, string pBufferType)
        {
            switch (pBufferType)
            {
                case "GUID":
                    return (OrgBuffer.Length == 46 || OrgBuffer.Length == 44);
            }
            return false;
        }

        /// <summary>
        /// decrypts byte array with given string key
        /// </summary>
        /// <param name="pPrivateKey">RSA Key used for decryption</param>
        /// <param name="val">Buffer value to be decrypted</param>
        /// <returns></returns>
        public string RSADecryptWithPrivateKey(byte[] val, string pPrivateKey)
        {
            if (string.IsNullOrEmpty(pPrivateKey)) return "";
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            try
            {
                rsa.FromXmlString(pPrivateKey);
            }
            catch (PlatformNotSupportedException)
            {
                // TODO parse XML
                throw;
            }
            byte[] tBytes = rsa.Decrypt(val, false);
            return Encoding.UTF8.GetString(tBytes, 0, tBytes.Length);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Rsa decrypt a buffer using a custom RSA provider or Private/Public key pair. </summary>
        ///
        /// <remarks>   Chris, 4/2/2020. </remarks>
        ///
        /// <param name="val">          Buffer value to be decrypted. </param>
        /// <param name="rsa">          (Optional) The rsa. </param>
        /// <param name="RSAKey">       (Optional) The rsa key. </param>
        /// <param name="RSAPublic">    (Optional) The rsa public. </param>
        ///
        /// <returns>   A string. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string RSADecrypt(byte[] val, RSACryptoServiceProvider rsa = null, string RSAKey = null, string RSAPublic = null)
        {
            if (rsa == null)
            {
                if (string.IsNullOrEmpty(RSAKey))
                {
                    CreateRSAKeys(out RSAKey, out RSAPublic);
                    if (string.IsNullOrEmpty(RSAKey))
                        return "";
                }
                rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(RSAKey);
            }
#if (!CDE_STANDARD) // RSA Decrypt parameter different (padding enum vs. bool)
                byte[] tBytes = rsa.Decrypt(val, false);
#else
            byte[] tBytes = rsa.Decrypt(val, RSAEncryptionPadding.Pkcs1);
#endif
            return Encoding.UTF8.GetString(tBytes, 0, tBytes.Length);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Rsa encrypt a string with a public key </summary>
        ///
        /// <remarks>   Chris, 4/2/2020. </remarks>
        ///
        /// <param name="val">              String value to be encrypted. </param>
        /// <param name="pRSAPublic">       The rsa public key</param>
        ///
        /// <returns>   An encrypted byte[] buffer. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public byte[] RSAEncrypt(string val, string pRSAPublic)
        {
            if (string.IsNullOrEmpty(pRSAPublic) || string.IsNullOrEmpty(val)) return null;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            string[] rsaP = pRSAPublic.Split(',');
            RSAParameters tP = new RSAParameters()
            {
                Modulus = CU.ToHexByte(rsaP[1]),
                Exponent = CU.ToHexByte(rsaP[0])
            };
            rsa.ImportParameters(tP);
            byte[] tBytes = rsa.Encrypt(CU.CUTF8String2Array(val), false);
            return tBytes;
        }

        static bool _platformDoesNotSupportRSAXMLExportImport;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Creates rsa keys and a RSA provider. </summary>
        ///
        /// <remarks>   Chris, 4/2/2020. </remarks>
        ///
        /// <param name="RSAKey">               [out] The rsa key. </param>
        /// <param name="RSAPublic">            [out] The rsa public. </param>
        /// <param name="createXMLKeyAlways">   (Optional) True to create XML key always. </param>
        ///
        /// <returns>   The new rsa keys. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public RSACryptoServiceProvider CreateRSAKeys(out string RSAKey, out string RSAPublic, bool createXMLKeyAlways = false)
        {
            RSACryptoServiceProvider rsa;
            RSAKey = null;
            if (Environment.OSVersion.Platform == PlatformID.Unix) // CODE REVIEW: Is this really the right check/condition? CM: Better would be to find out if we create this in SW or HW
                rsa = new RSACryptoServiceProvider(512);
            else
                rsa = new RSACryptoServiceProvider();
            if (!_platformDoesNotSupportRSAXMLExportImport)
            {
                try
                {
                    // TODO Decide if we want to switch to MyRSA object for the private key in general, or just for Std/Core
                    RSAKey = rsa.ToXmlString(true);
                }
                catch (PlatformNotSupportedException)
                {
                    _platformDoesNotSupportRSAXMLExportImport = true;
                }
            }
            if (_platformDoesNotSupportRSAXMLExportImport)
            {
                if (!createXMLKeyAlways)
                {
                    RSAKey = "no private key in string format on this platform";
                }
                else
                {
                    RSAKey = GetRSAKeyAsXML(rsa, true);
                }
            }
            RSAParameters param = rsa.ExportParameters(false);
            RSAPublic = CU.ToHexString(param.Exponent) + "," + CU.ToHexString(param.Modulus);
            return rsa;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets rsa keys as XML </summary>
        ///
        /// <remarks>   Chris, 4/2/2020. </remarks>
        ///
        /// <param name="rsa">                      The rsa. </param>
        /// <param name="includePrivateParameters"> True to include, false to exclude the private
        ///                                         parameters. </param>
        ///
        /// <returns>   The rsa key as XML. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string GetRSAKeyAsXML(RSACryptoServiceProvider rsa, bool includePrivateParameters)
        {
            var paramsPrivate = rsa.ExportParameters(includePrivateParameters);

            var sb = new StringBuilder("<RSAKeyValue>");
            sb.Append(CU.cdeCreateXMLElement(nameof(paramsPrivate.Modulus), paramsPrivate.Modulus));
            sb.Append(CU.cdeCreateXMLElement(nameof(paramsPrivate.Exponent), paramsPrivate.Exponent));
            if (includePrivateParameters)
            {
                sb.Append(CU.cdeCreateXMLElement(nameof(paramsPrivate.P), paramsPrivate.P));
                sb.Append(CU.cdeCreateXMLElement(nameof(paramsPrivate.Q), paramsPrivate.Q));
                sb.Append(CU.cdeCreateXMLElement(nameof(paramsPrivate.DP), paramsPrivate.DP));
                sb.Append(CU.cdeCreateXMLElement(nameof(paramsPrivate.DQ), paramsPrivate.DQ));
                sb.Append(CU.cdeCreateXMLElement(nameof(paramsPrivate.InverseQ), paramsPrivate.InverseQ));
                sb.Append(CU.cdeCreateXMLElement(nameof(paramsPrivate.D), paramsPrivate.D));
            }
            sb.Append("</RSAKeyValue>");
            return sb.ToString();
        }

        /// <summary>
        /// Decrypt incoming buffer array to a Dictionary.
        /// </summary>
        /// <param name="pInbuffer">incoming byte arrey to be decrypted</param>
        /// <returns></returns>
        public Dictionary<string, string> DecryptKV(byte[] pInbuffer)
        {
            try
            {
                var fromEncrypt = Decrypt(pInbuffer, MySecrets?.GetAK(), MySecrets?.GetAI());
                string dec = CU.CArray2UnicodeString(fromEncrypt, 0, fromEncrypt.Length);
                dec = dec.TrimEnd('\0');
                return CU.DeserializeJSONStringToObject<Dictionary<string, string>>(dec);
            }
            catch (Exception e)
            {
                TheBaseAssets.MySYSLOG?.WriteToLog(TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM("TheCommonUtils", $"Error during KV descrypt...", eMsgLevel.l1_Error) { PLS = e.ToString() }, 5015);
                return null;
            }
        }

        /// <summary>
        /// Encryptes a dictionary to a byte array
        /// </summary>
        /// <param name="pVal">Dictionary to be encrypted</param>
        /// <returns></returns>
        public byte[] EncryptKV(Dictionary<string, string> pVal)
        {
            try
            {
                string t = CU.SerializeObjectToJSONString(pVal);
                return Encrypt(CU.CUnicodeString2Array(t), MySecrets?.GetAK(), MySecrets?.GetAI());
            }
            catch (Exception)
            {
                TheBaseAssets.MySYSLOG?.WriteToLog(TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM("TheCommonUtils", $"Error during KV encrypt...", eMsgLevel.l1_Error), 5016);
                return null;
            }
        }

        /// <summary>
        /// Decrypts a byte array to a string using internal AES encryption
        /// </summary>
        /// <param name="OrgBuffer">Buffer to be decrypted</param>
        /// <param name="pType">Current only supported type: "AIV1"</param>
        /// <returns></returns>
        public string DecryptToString(string OrgBuffer, string pType)
        {
            switch (pType)
            {
                case "CDEP":
                    if (OrgBuffer.StartsWith("&^CDESP1^&:"))
                    {
                        return CU.cdeDecrypt(OrgBuffer.Substring(11), MySecrets?.GetNodeKey());
                    }
                    break;
            }
            return null;
        }

        /// <summary>
        /// Encrypts an string to an encrypted string.
        /// </summary>
        /// <param name="pInBuffer">The inbuffer.</param>
        /// <param name="pType">Type of the buffer.</param>
        /// <returns>System.String.</returns>
        public string EncryptToString(string pInBuffer, string pType)
        {
            switch (pType)
            {
                case "CDEP":
                    if (!pInBuffer.StartsWith("&^CDESP1^&:"))
                    {
                        return "&^CDESP1^&:" + CU.cdeEncrypt(pInBuffer, MySecrets?.GetNodeKey());
                    }
                    break;
            }
            return pInBuffer;
        }
    }
}
