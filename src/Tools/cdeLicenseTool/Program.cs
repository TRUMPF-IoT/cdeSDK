// SPDX-FileCopyrightText: Copyright(c) 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
// SPDX-License-Identifier: MPL-2.0

//using CDEngine.CDUtils.Zlib;
using nsCDEngine.Activation;
using nsCDEngine.ActivationKey;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CU = nsCDEngine.BaseClasses.TheCommonUtils;
using CUJS = nsCDEngine.BaseClasses.TheCommonUtils;

namespace cdeLicenseTool
{
    class Program
    {
        public static Version ToolVersion { get
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                return version;
            } 
        }

        static int Main(string[] args)
        {
            if (args.Count() == 0)
            {
                Usage();
                return 0;
            }
            switch (args[0].ToUpper())
            {
                // Create Activation Key
                // GenerateActivationKey A450A7E9-8510-4312-89C5-A1471A73DAE6 asdfasdfsadf 0 ..\..\testfiles\licensecontainers
                case "GENERATEACTIVATIONKEY":
                    {
                        if (args.Length < 4 || args.Length > 6)
                        {
                            GenerateActivationKeyUsage();
                            return -1;
                        }
                        Guid deviceId;
                        if (!Guid.TryParse(args[1], out deviceId))
                        {
                            GenerateActivationKeyUsage();
                            return -1;
                        }
                        string signingKey = args[2];
                        int flags;
                        if (!int.TryParse(args[3], out flags))
                        {
                            GenerateActivationKeyUsage();
                            return -1;
                        }

                        string licensePublicKeyFilePath;
                        if (args.Length > 4)
                        {
                            licensePublicKeyFilePath = args[4];
                        }
                        else
                        {
                            GenerateActivationKeyUsage();
                            return -1;
                        }
                        string licenseDirectory;
                        if (args.Length > 5)
                        {
                            licenseDirectory = args[5];
                        }
                        else
                        {
                            licenseDirectory = Environment.CurrentDirectory;
                        }

                        string activationKeyFile = null;
                        if (args.Length > 6)
                        {
                            activationKeyFile = args[6];
                        }

                        // TODO Add expirationDate as command line parameter
                        DateTime expirationDate = DateTime.UtcNow + new TimeSpan(365, 0, 0, 0);

                        byte[] licenseSignerPublicKey = null;
                        try
                        {
                            licenseSignerPublicKey = File.ReadAllBytes(licensePublicKeyFilePath);
                        }
                        catch (Exception ee)
                        {
                            Console.WriteLine("Failed to load public key for validating licenses:" + ee.ToString());
                            return -1;
                        }
                        List<TheLicense> licenses = new List<TheLicense>();
                        try
                        {
                            if (!TheActivationKeyGenerator.ReadLicensesForActivationKey(licenseDirectory, new List<byte[]> { licenseSignerPublicKey }, out licenses))
                            {
                                throw new Exception("Error reading licenses");
                            }
                        }
                        catch (Exception ee)
                        {
                            Console.WriteLine("Failed to extract license:" + ee.ToString());
                            return -1;
                        }

                        List<TheLicenseParameter> parameters;
                        List<TheLicense> licenseForParameters;

                        if (TheActivationKeyGenerator.GetParametersForActivationKey(licenses, out parameters, out licenseForParameters))
                        {
                            string activationKey = TheActivationKeyGenerator.GenerateActivationKey(deviceId, signingKey, expirationDate, licenses.ToArray(), parameters, (ActivationFlags)0);

                            if (activationKeyFile != null)
                            {
                                try
                                {
                                    File.WriteAllText(activationKeyFile, activationKey);
                                    Console.WriteLine("Wrote activation key to file {0}", activationKeyFile);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Error writing actication key to file {0}: {1}", activationKeyFile, e.ToString());
                                }
                            }
                            else
                            {
                                Console.WriteLine("Activation Key: {0}", activationKey);
                            }
                        }
                    }
                    break;

                // Sign license file / create .cdex file
                // CreateLicense ..\..\testfiles\licenses ..\..\testfiles\test.snk mylicense.cdex
                case "CREATELICENSE":
                    {
                        if (args.Length < 4 || args.Length > 4)
                        {
                            CreateLicenseUsage();
                            return -1;
                        }
                        try
                        {
                            string licenseDirectory = args[1];
                            string rsaPrivateKeyFile = args[2];
                            string licenseOutputFile = args[3];

                            byte[] rsaPrivateKeyBytes= File.ReadAllBytes(rsaPrivateKeyFile);

                            bool useZip = licenseOutputFile.EndsWith(".cdex");

                            List<TheLicense> licenses = new List<TheLicense>();
                            var licenseTemplateFilePaths = Directory.EnumerateFiles(licenseDirectory, "*.cdelt");

                            ZipArchive zip = null;
                            if (useZip)
                            {
                                zip = new ZipArchive(new FileStream(licenseOutputFile, FileMode.Create, FileAccess.ReadWrite), ZipArchiveMode.Create);
                            }
                            foreach (var licenseTemplateFilePath in licenseTemplateFilePaths)
                            {
                                var licenseFileName = $"{Path.GetFileNameWithoutExtension(licenseTemplateFilePath)}.cdel";

                                string licenseJson = File.ReadAllText(licenseTemplateFilePath);
                                licenseJson = ReplaceVersionVariables(licenseJson);
                                var license = CUJS.DeserializeJSONStringToObject<TheLicense>(licenseJson);
                                license.AddSignature(rsaPrivateKeyBytes);
                                // TODO add additional signatures
                                var signedLicenseJson = CUJS.SerializeObjectToJSONString(license);
                                var signedLicenseBytes = CU.CUTF8String2Array(signedLicenseJson);
                                if (!license.ValidateSignature(new List<byte[]> { rsaPrivateKeyBytes }))
                                {
                                    Console.WriteLine("Error validating signatures on file {0}", licenseFileName);
                                    return -1;
                                }
                                if (!useZip)
                                {
                                    File.WriteAllBytes(Path.Combine(licenseOutputFile, licenseFileName), signedLicenseBytes);
                                }
                                else
                                {
                                    var zipEntry = zip.CreateEntry(Path.GetFileName(licenseFileName));
                                    using (var zipStream = zipEntry.Open())
                                    {
                                        zipStream.Write(signedLicenseBytes, 0, signedLicenseBytes.Length);
                                        zipStream.Close();
                                    }
                                }
                            }
                            if (zip != null)
                            {
                                zip.Dispose();
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error creating license: {0}", e.ToString());
                        }
                    }
                    break;
                default:
                    Usage();
                    return -1;

            }
            return 0;
        }

        private static string ReplaceVersionVariables(string licenseJson)
        {
            licenseJson = ReplaceVersionVariable(licenseJson, "Major", $"{ToolVersion.Major}");
            licenseJson = ReplaceVersionVariable(licenseJson, "Minor", $"{ToolVersion.Minor:000}");
            return licenseJson;
        }

        private static string ReplaceVersionVariable(string licenseJson, string name, string fallbackValue)
        {
            var value = Environment.GetEnvironmentVariable($"GitVersion_{name}") ?? fallbackValue;
            if (value != null)
            {
                var pattern = $@"\$\(GITVERSION[_\.]{name}\)";
                licenseJson = Regex.Replace(licenseJson, pattern, value, RegexOptions.IgnoreCase);
            }
            return licenseJson;
        }

        static void Usage()
        {
            Console.WriteLine("Usage:");
            GenerateActivationKeyUsage();
            CreateLicenseUsage();
        }

        static void GenerateActivationKeyUsage()
        {
            Console.WriteLine(@"cdeLicenseTool generateActivationKey <deviceId> <signingKey> <flags> <licensePublicKeyFilePath> [ <licenseFileDirectory> [ <activationKeyFilePath> ]]");
        }

        static void CreateLicenseUsage()
        {
            Console.WriteLine(@"cdeLicenseTool createLicense <licenseFileDirectory> <rsaPrivateKeyFile> <licenseCDEXOutputFile>");
        }
    }
}
