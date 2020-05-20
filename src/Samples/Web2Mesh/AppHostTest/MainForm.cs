// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System;
using System.Windows.Forms;

using nsCDEngine.BaseClasses;
using nsCDEngine.ViewModels;

namespace AppHostTest
{
    public partial class FormMain : Form
    {
        private const string m_StrGuid = "{D4FBF84C-3558-4E3C-9E1E-83921BF90E65}";

        // SDK Non-Commercial ID. FOR COMMERCIAL APP GET THIS ID FROM C-LABS!
        private const string m_SDKAppID = "/cVjzPfjlO;{@QMj:jWpW]HKKEmed[llSlNUAtoE`]G?";

        public FormMain()
        {
            InitializeComponent();
            cmdInitBaseApp.Enabled = true;
            cmdStartBaseApp.Enabled = false;
            cmdStopBaseApp.Enabled = false;
            cmdCopy.Enabled = true;
        }

        // User clicked [Init Base App] button
        private void cmdInitBaseApp_Click(object sender, EventArgs e)
        {
            string strScopeID = textScopeID.Text;
            string strAppID = m_SDKAppID;

            if (m_StrGuid.Contains("XXXX-XXXX"))
            {
                MessageSuccess("GUID not initialized. Run GuidGen and copy new Guid into code.");
                return;
            }

            BaseApplication.strScopeID = textScopeID.Text;
            bool bEnableRest = checkEnableREST.Checked;
            bool bEnableMeshRequest = checkEnableMeshRequest.Checked;
            bool bEnableMeshResponse = checkEnableMeshResponse.Checked;

            ushort usPort = (radioPort8720.Checked) ? (ushort)8720 : (radioPort8730.Checked ? (ushort)8730 : (ushort)8740);
            string strRoute = (checkNoCloudServiceRoute.Checked ? "" : textCloudServiceRoute.Text);
            if (BaseApplication.Init(m_StrGuid, strScopeID, strAppID, usPort, strRoute, bEnableRest, bEnableMeshRequest, bEnableMeshResponse))
            {
                textScopeID.Text = BaseApplication.strScopeID;
                MessageSuccess("Success initializing Base App.");
                cmdInitBaseApp.Enabled = false;
                cmdStartBaseApp.Enabled = true;
                groupCloudRoute.Enabled = false;
                groupWebMeshSettings.Enabled = false;
            }
            else
            {
                MessageFailure("Failure initializing Base App.");
            }
        } // cmdInitBaseApp_Click

        // User clicked [Start Base App] button
        private void cmdStartBaseApp_Click(object sender, EventArgs e)
        {
            if (BaseApplication.Start())
            {
                MessageSuccess("Success starting Base App. Click URL to launch browser.");
                textScopeID.Enabled = false;
                cmdStartBaseApp.Enabled = false;
                cmdStopBaseApp.Enabled = true;
                groupPortSettings.Enabled = false;

                linkOpenBrowser.Text = BaseApplication.FetchUrl();
            }
            else
            {
                MessageFailure("Failure starting Base App.");
            }
        } // cmdStartBaseApp_Click

        // User clicked [Stop Base App] button
        private void cmdStopBaseApp_Click(object sender, EventArgs e)
        {
            if (BaseApplication.Stop())
            {
                groupPortSettings.Enabled = true;
                cmdInitBaseApp.Enabled = true;
                cmdStartBaseApp.Enabled = false;
                cmdStopBaseApp.Enabled = false;
                groupCloudRoute.Enabled = true;
                groupWebMeshSettings.Enabled = true;
                MessageSuccess("Success stopping Base App.");
            }
            else
            {
                MessageFailure("Failure stopping Base App.");
            }

        } // cmdStopBaseApp_Click

        // User clicked [Copy] button
        private void cmdCopy_Click(object sender, EventArgs e)
        {
            string strUrl = textScopeID.Text;
            if (strUrl.Length > 0)
                Clipboard.SetText(strUrl);
        }

        private TheRequestData newTheRequestData()
        {
            throw new NotImplementedException();
        }

        public void MessageSuccess(string strMessage)
        {
            textLastMessage.Text = strMessage;
            // MessageBox.Show(this, strMessage, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        public void MessageFailure(string strMessage)
        {
            textLastMessage.Text = $"Error: {strMessage}";
            // MessageBox.Show(this, strMessage, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error );
        }

        private void textScopeID_TextChanged(object sender, EventArgs e)
        {
            cmdCopy.Enabled = (textScopeID.Text.Length > 0);
        }

        private void LinkOpenBrowser_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            bool bSuccess = false;
            try
            {
                string strUrl = linkOpenBrowser.Text;
                System.Diagnostics.Process.Start(strUrl);
                bSuccess = true;
            }
            catch
            {

            }

            if (!bSuccess)
                MessageFailure("Failure starting browser");
        }

        private int __iCurrentPort = 8720;
        private int __iCurrentCloudRoute = 8730;
        private void RadioPort_CheckedChanged(object sender, EventArgs e)
        {
            bool bChanged = false;
            if (radioPort8720.Checked && __iCurrentPort != 8720)
            {
                __iCurrentPort = 8720;
                __iCurrentCloudRoute = 8730;
                checkEnableREST.Checked = true;
                checkEnableMeshRequest.Checked = false;
                checkEnableMeshResponse.Checked = false;
                bChanged = true;
            }
            else if (radioPort8730.Checked && __iCurrentPort != 8730)
            {
                __iCurrentPort = 8730;
                __iCurrentCloudRoute = 8740;
                checkEnableREST.Checked = true;
                checkEnableMeshRequest.Checked = true;
                checkEnableMeshResponse.Checked = false;
                bChanged = true;
            }
            else if (radioPort8740.Checked && __iCurrentPort != 8740)
            {
                __iCurrentPort = 8740;
                __iCurrentCloudRoute = 8720;
                checkEnableREST.Checked = false;
                checkEnableMeshRequest.Checked = false;
                checkEnableMeshResponse.Checked = true;
                bChanged = true;
            }

            if (bChanged)
            {
                // Don't update if we are not using this.
                if (!checkNoCloudServiceRoute.Checked)
                {
                    // Only update if user has not put their own value here.
                    if (textCloudServiceRoute.Text.StartsWith("http://localhost:"))
                    {
                        textCloudServiceRoute.Text = $"http://localhost:{__iCurrentCloudRoute}";
                    }
                }

            }
            
        }

        private void CheckNoCloudServiceRoute_CheckedChanged(object sender, EventArgs e)
        {
            if (checkNoCloudServiceRoute.Checked)
            {
                textCloudServiceRoute.Enabled = false;
            }
            else
            {
                textCloudServiceRoute.Enabled = true;
            }
        }
    } // class
} // namespace
