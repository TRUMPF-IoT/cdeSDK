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

            if (BaseApplication.Init(m_StrGuid, strScopeID, strAppID))
            {
                textScopeID.Text = BaseApplication.strScopeID;
                MessageSuccess("Success initializing Base App.");
                cmdInitBaseApp.Enabled = false;
                cmdStartBaseApp.Enabled = true;
            }
            else
            {
                MessageFailure("Failure initializing Base App.");
            }
        } // cmdInitBaseApp_Click

        // User clicked [Start Base App] button
        private void cmdStartBaseApp_Click(object sender, EventArgs e)
        {
            BaseApplication.strScopeID = textScopeID.Text;
            if (BaseApplication.Start())
            {
                MessageSuccess("Success starting Base App. Click URL to launch browser.");
                textScopeID.Enabled = false;
                cmdStartBaseApp.Enabled = false;
                cmdStopBaseApp.Enabled = true;

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
                cmdInitBaseApp.Enabled = true;
                cmdStartBaseApp.Enabled = false;
                cmdStopBaseApp.Enabled = false;
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

        private void CmdCallHttpIntercepter_Click(object sender, EventArgs e)
        {
            nsCDEngine.ViewModels.TheRequestData p = new nsCDEngine.ViewModels.TheRequestData();
            p.cdeRealPage = "/HelloHTTPInterceptor?TARGET=MyPlugin";
            TheCommonUtils.GetAnyFile(p);
        }
    }
}
