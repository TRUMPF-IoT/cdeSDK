// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace RESTClient
{
    public partial class Form1 : Form
    {
        public long Address_Loopback = 0x0100007F;
        public ushort usPort = 8700;

        private Dictionary<int, string> dictRequest = new Dictionary<int, string>();
        private Dictionary<int, List<string>> dictResponse = new Dictionary<int, List<string>>();

        public Form1()
        {
            InitializeComponent();
            g_ListViewLog = listLog;
        }

        private void Logon_Click(object sender, EventArgs e)
        {
            string strFunction = "Logon_Click";

            // Assemble a call like this:
            // http://localhost:8700/api/MileRecordHolder?user=MyUser&pwd=Asterisks

            string strURL = textURL.Text;
            int iPathStarts = QueryPathStarts(strURL);
            string strLeft = strURL.Substring(0, iPathStarts);
            string strRight = strURL.Substring(iPathStarts);
            string strPort = QueryPort(textOtherPort.Text, out usPort);
            string strColon = String.IsNullOrEmpty(strPort) ? "" : ":";
            string strRestCall = $"{strLeft}{strColon}{strPort}{strRight}/logon?user={textUID.Text}&pwd={textPWD.Text}";

            // Pull out the local path part.
            iPathStarts = QueryPathStarts(strRestCall);
            string strHost = strRestCall.Substring(0, iPathStarts).Replace("http://", "").Replace("https://", "");
            int iPort = strHost.IndexOf(":");
            if (iPort > -1)
                strHost = strHost.Substring(0, iPort);

            string strPathAndParameters = strRestCall.Substring(iPathStarts);

            // Store in request log.
            dictRequest.Add(iListItemCount, strRestCall);
            AddItemToList($"{strFunction} (Request)", strRestCall);
            textRequest.Text = strRestCall;

            // Intermediate buffer
            List<string> listReceiveBuffer = new List<string>();

            if (httputils.HttpConnectSendReceive("GET", strHost, usPort, strPathAndParameters, listReceiveBuffer))
            {
                dictResponse.Add(iListItemCount, listReceiveBuffer);
                AddItemToList($"{strFunction} (Response)", $"Received Buffer. {listReceiveBuffer.Count} items.");
                ShowResponseItems(listReceiveBuffer);

                bool bHeader = true;
                foreach (string str in listReceiveBuffer)
                {
                    bool bEmpty = String.IsNullOrEmpty(str);
                    if (bHeader)
                    {
                        if (bEmpty)
                            bHeader = false;
                    }
                    else
                    {
                        if (!bEmpty)
                        {
                            bool bFound = false;
                            Guid gTemp;
                            int iStart = str.IndexOf('"');
                            if (iStart > -1)
                            {
                                iStart++;
                                int iStop = str.IndexOf('"', iStart);
                                string strGuid = str.Substring(iStart, iStop - iStart);
                                if (Guid.TryParse(strGuid, out gTemp))
                                {
                                    AddItemToList($"{strFunction}", $"Token received: {gTemp.ToString()}");
                                    textAccessToken.Text = gTemp.ToString();
                                    bFound = true;
                                    break;
                                }
                            }

                            if (bFound == false)
                            {
                                AddItemToList($"{strFunction}", $"Token not found.");
                            }
                        }
                    }
                }
            }
            else
            {
                // Error handling
                AddItemToList($"{strFunction}", "Error - cannot logon.");
            }
        }



        public static int iListItemCount = 1;
        public static ListView g_ListViewLog = null;
        public static void AddItemToList(string strAction, string strDetails)
        {
            string[] item = { iListItemCount.ToString(), strAction, strDetails };
            ListViewItem litem = new ListViewItem(item);
            g_ListViewLog.Items.Add(litem);
            g_ListViewLog.Invalidate();
            g_ListViewLog.Update();
            iListItemCount++;
        }

        private void CmdGET_Click(object sender, EventArgs e)
        {
            string strFunction = "GET_Click";

            // Assemble a call like this:
            // http://localhost:8700/api/MileRecordHolder/query?id=3&key=xxxx-xxxx-xxxx-xxxxxxxxx

            string strURL = textURL.Text;
            int iPathStarts = QueryPathStarts(strURL);
            string strLeft = String.Empty;
            string strRight = String.Empty;
            if (iPathStarts > -1)
            {
                strLeft = strURL.Substring(0, iPathStarts);
                strRight = strURL.Substring(iPathStarts);
            }
            else
            {
                strLeft = strURL;
            }

            string strPort = QueryPort(textOtherPort.Text, out usPort);
            string strColon = String.IsNullOrEmpty(strPort) ? "" : ":";
            string strKeyToken = String.Empty;
            if (!String.IsNullOrEmpty(textAccessToken.Text))
                strKeyToken = $"&key={textAccessToken.Text}";
            string strCommand = textCommand.Text;
            string strParameters = textParameters.Text;
            string strRestCall = $"{strLeft}{strColon}{strPort}{strRight}{strCommand}{strParameters}{strKeyToken}";

            // Split the host name from the path and parameters.
            iPathStarts = QueryPathStarts(strRestCall);
            string strHost = strRestCall.Substring(0, iPathStarts).Replace("http://", "").Replace("https://", "");
            int iPort = strHost.IndexOf(":");
            if (iPort > -1)
                strHost = strHost.Substring(0, iPort);

            string strPathAndParameters = strRestCall.Substring(iPathStarts);

            // Store in request log.
            dictRequest.Add(iListItemCount, strRestCall);
            AddItemToList($"{strFunction} (Request)", strRestCall);
            textRequest.Text = strRestCall;

            // Intermediate buffer
            List<string> listReceiveBuffer = new List<string>();

            if (httputils.HttpConnectSendReceive("GET", strHost, usPort, strPathAndParameters, listReceiveBuffer))
            {
                dictResponse.Add(iListItemCount, listReceiveBuffer);
                AddItemToList($"{strFunction} (Response)", $"Received Buffer. {listReceiveBuffer.Count} items.");
                ShowResponseItems(listReceiveBuffer);

                bool bHeader = true;
                foreach (string str in listReceiveBuffer)
                {
                    bool bEmpty = String.IsNullOrEmpty(str);
                    if (bHeader)
                    {
                        if (bEmpty)
                            bHeader = false;
                    }
                    else
                    {
                        if (!bEmpty)
                        {
                            AddItemToList($"{strFunction}", $"Received: {str}");
                        }
                    }
                }
            }
            else
            {
                // Error handling
                AddItemToList($"{strFunction}", "Error - Request Failed.");
            }

        }

        private void CmdClearLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            listLog.Items.Clear();
            dictRequest = new Dictionary<int, string>();
            dictResponse = new Dictionary<int, List<string>>();
            iListItemCount = 1;
        }

    private void ListLog_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection items = listLog.SelectedItems;

            if (items.Count > 0)
            {
                string strCount = items[0].Text;
                if (strCount.Length > 0)
                {
                    int iItem = 0;
                    if (int.TryParse(strCount, out iItem))
                    {
                        if (dictRequest.ContainsKey(iItem))
                        {
                            string str = dictRequest[iItem];
                            textRequest.Text = str;
                        }

                        if (dictResponse.ContainsKey(iItem))
                        {
                            List<string> list = dictResponse[iItem];
                            ShowResponseItems(list);
                        }
                    }
                }
            }
        }

        public int QueryPathStarts(string strPath)
        {
            // Parse URLs like these
            //   http://mywhatever.com/api
            //   http://128.0.0.1:123/something
            //   http://localhost:80/orother
            //
            // To return index of beginning of non-domain
            int i1 = strPath.IndexOf("//");
            int i2 = i1 + 2;
            int i3 = strPath.IndexOf("/", i2);

            return i3;
        }
        public string QueryPort(string strInPort, out ushort usPort)
        {
            usPort = (ushort)80;
            string strPort = "80";

            if (radio8720.Checked)
            {
                strPort = "8720";
                usPort = (ushort)8720;
            }
            else if (radio8730.Checked)
            {
                strPort = "8730";
                usPort = (ushort)8730;

            }
            else if (radio8740.Checked)
            {
                strPort = "8740";
                usPort = (ushort)8740;

            }
            else if (radioOther.Checked)
            {
                bool bSuccess = false;
                if (!string.IsNullOrEmpty(strInPort))
                {
                    bSuccess = ushort.TryParse(strInPort, out usPort);
                }

                if (!bSuccess)
                {
                    usPort = (ushort)80;
                    strPort = String.Empty;
                }
            }

            return strPort;
        }


        private void ShowResponseItems(List<string> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string str in list)
            {
                sb.AppendLine(str);
            }

            textResponse.Text = sb.ToString();
        }
    } // class

} // namespace
