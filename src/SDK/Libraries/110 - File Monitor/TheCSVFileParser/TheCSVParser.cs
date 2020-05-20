// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using nsCDEngine.BaseClasses;
using nsCDEngine.Engines.ThingService;

namespace nsTheCSVParser
{
    internal class CSVParserOptions
    {
        public bool QuotedValues { get; set; }
        public bool RemoveRemainingQuotes { get; set; }
        public string Separator { get; set; } = ",";
        public string AlternateSeparator { get; set; } = ";";
        public string LineBreak { get; set; } = "\r\n";
        public bool NameValuePairs { get; set; }
        public string TimeFieldName { get; set; }
    }

    internal class TheCSVParser
    {
        public static Task<bool> ParseCSVData(TheThing tThing, string fileName, string csvData, int delayBetweenRows, CSVParserOptions options, long previousErrorCount, Action<long, long, bool> onLineParsed)
        {
            int lineCount = 0;
            long lineErrorCount = previousErrorCount;

            var result = ParseCSVData(csvData,
                options,
                async (dict, sourceTimestamp) =>
                {
                    lineCount++;

                    if (dict == null || options.NameValuePairs && dict.Count == 0)
                    {
                        lineErrorCount++;
                    }
                    else
                    {
                        try
                        {
                            if (!String.IsNullOrEmpty(fileName))
                            {
                                dict["CurrentFile"] = fileName;
                            }
                            tThing.SetProperties(dict, sourceTimestamp);
                        }
                        catch
                        {
                            lineErrorCount++;
                        }

                        if (delayBetweenRows > 0)
                        {
                            await TheCommonUtils.TaskDelayOneEye(delayBetweenRows, 100);
                        }
                    }
                    onLineParsed?.Invoke(lineCount, lineErrorCount, false);
                });
            onLineParsed?.Invoke(lineCount, lineErrorCount, true);
            return result;
        }

        public static async Task<bool> ParseCSVData(string csvData, CSVParserOptions options,
            Func<Dictionary<string, object>, DateTimeOffset, Task> onLineProcessed = null)
        {
            bool bSuccess = true;
            var lines = csvData.Split(new[] { options.LineBreak }, StringSplitOptions.RemoveEmptyEntries);
            var splitString = new[] { options.QuotedValues ? "\"" + options.Separator + "\"" : options.Separator };

            var columnNames = lines[0].Split(splitString, StringSplitOptions.None);
            if (columnNames.Length <= 1 && !String.IsNullOrEmpty(options.AlternateSeparator))
            {
                splitString = new[] { options.QuotedValues ? "\"" + options.AlternateSeparator + "\"" : options.AlternateSeparator };
                columnNames = lines[0].Split(splitString, StringSplitOptions.None);
            }
            if (options.QuotedValues)
            {
                columnNames[0] = columnNames[0].TrimStart('\"');
                columnNames[columnNames.Length - 1] = columnNames[columnNames.Length - 1].TrimEnd('\"');
            }
            if (options.RemoveRemainingQuotes)
            {
                for (int i = 0; i < columnNames.Length; i++)
                {
                    columnNames[i] = columnNames[i].Trim('\"');
                }
            }

            foreach (var line in lines.Skip(1))
            {
                var sourceTimestamp = DateTimeOffset.MinValue;

                try
                {
                    var values = line.Split(splitString, StringSplitOptions.None);
                    if (options.QuotedValues)
                    {
                        values[0] = values[0].TrimStart('\"');
                        values[values.Length - 1] = values[values.Length - 1].TrimEnd('\"');
                    }

                    var dict = new Dictionary<string, object> { };

                    string nvName = null;
                    string nvValue = null;

                    for (int i = 0; i < values.Length; i++)
                    {
                        if (options.RemoveRemainingQuotes)
                        {
                            values[i] = values[i].Trim('\"');
                        }

                        if (!String.IsNullOrEmpty(options.TimeFieldName) && String.Equals(columnNames[i], options.TimeFieldName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (!DateTimeOffset.TryParse(values[i], out sourceTimestamp))
                            {
                                sourceTimestamp = DateTimeOffset.MinValue;
                            }
                        }
                        if (!options.NameValuePairs)
                        {
                            dict[columnNames[i]] = values[i];
                        }
                        else
                        {
                            // CSV contains name value pairs, i.e. Header is time, name, value
                            if (String.Equals(columnNames[i], "name", StringComparison.InvariantCultureIgnoreCase))
                            {
                                nvName = values[i];
                            }
                            else if (String.Equals(columnNames[i], "value", StringComparison.InvariantCultureIgnoreCase))
                            {
                                nvValue = values[i];
                            }
                        }
                    }

                    if (!TheBaseAssets.MasterSwitch)
                    {
                        bSuccess = false;
                        break;
                    }

                    if (options.NameValuePairs)
                    {
                        if (nvName != null && nvValue != null)
                        {
                            dict[nvName] = nvValue;
                        }
                    }
                    await onLineProcessed(dict, sourceTimestamp);
                }
                catch
                {
                    await onLineProcessed(null, DateTimeOffset.MinValue);
                }
            }
            return bSuccess;
        }

    }
}
