// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System;
using System.Windows.Forms;

// TODO: Add reference for (1) C-DEngine.dll and (2) CDMyNMIHtml5.dll
using nsCDEngine.BaseClasses;

namespace $safeprojectname$
{
    public partial class FormMain : Form
    {
        // TODO: Provide a unique GUID for your app.
        // Hint: In Visual Studio: Tools | Create GUID.
        private const string m_StrGuid = "{XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX}";

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
                cmdStopBaseApp.Enabled = true;
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

        public void MessageSuccess(string strMessage)
        {
            textLastMessage.Text = strMessage;
            // MessageBox.Show(this, strMessage, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        public void MessageFailure (string strMessage)
        {
            textLastMessage.Text = $"Error: {strMessage}";
            // MessageBox.Show(this, strMessage, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error );
        }

        private void textScopeID_TextChanged(object sender, EventArgs e)
        {
            cmdCopy.Enabled = (textScopeID.Text.Length > 0);
        }
    }
}
