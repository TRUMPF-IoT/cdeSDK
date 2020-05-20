// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿namespace AppHostTest
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmdStartBaseApp = new System.Windows.Forms.Button();
            this.cmdStopBaseApp = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textScopeID = new System.Windows.Forms.TextBox();
            this.cmdInitBaseApp = new System.Windows.Forms.Button();
            this.cmdCopy = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.linkOpenBrowser = new System.Windows.Forms.LinkLabel();
            this.textLastMessage = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdStartBaseApp
            // 
            this.cmdStartBaseApp.Location = new System.Drawing.Point(160, 26);
            this.cmdStartBaseApp.Margin = new System.Windows.Forms.Padding(1);
            this.cmdStartBaseApp.Name = "cmdStartBaseApp";
            this.cmdStartBaseApp.Size = new System.Drawing.Size(129, 34);
            this.cmdStartBaseApp.TabIndex = 1;
            this.cmdStartBaseApp.Text = "Start Base App";
            this.cmdStartBaseApp.UseVisualStyleBackColor = true;
            this.cmdStartBaseApp.Click += new System.EventHandler(this.cmdStartBaseApp_Click);
            // 
            // cmdStopBaseApp
            // 
            this.cmdStopBaseApp.Location = new System.Drawing.Point(307, 26);
            this.cmdStopBaseApp.Margin = new System.Windows.Forms.Padding(1);
            this.cmdStopBaseApp.Name = "cmdStopBaseApp";
            this.cmdStopBaseApp.Size = new System.Drawing.Size(129, 34);
            this.cmdStopBaseApp.TabIndex = 2;
            this.cmdStopBaseApp.Text = "Stop Base App";
            this.cmdStopBaseApp.UseVisualStyleBackColor = true;
            this.cmdStopBaseApp.Click += new System.EventHandler(this.cmdStopBaseApp_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 24);
            this.label1.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Scope ID:";
            // 
            // textScopeID
            // 
            this.textScopeID.Location = new System.Drawing.Point(80, 21);
            this.textScopeID.Margin = new System.Windows.Forms.Padding(1);
            this.textScopeID.Name = "textScopeID";
            this.textScopeID.Size = new System.Drawing.Size(70, 20);
            this.textScopeID.TabIndex = 5;
            this.textScopeID.Text = "TELE9878";
            this.textScopeID.TextChanged += new System.EventHandler(this.textScopeID_TextChanged);
            // 
            // cmdInitBaseApp
            // 
            this.cmdInitBaseApp.Location = new System.Drawing.Point(18, 26);
            this.cmdInitBaseApp.Margin = new System.Windows.Forms.Padding(1);
            this.cmdInitBaseApp.Name = "cmdInitBaseApp";
            this.cmdInitBaseApp.Size = new System.Drawing.Size(129, 34);
            this.cmdInitBaseApp.TabIndex = 0;
            this.cmdInitBaseApp.Text = "Init Base App";
            this.cmdInitBaseApp.UseVisualStyleBackColor = true;
            this.cmdInitBaseApp.Click += new System.EventHandler(this.cmdInitBaseApp_Click);
            // 
            // cmdCopy
            // 
            this.cmdCopy.Location = new System.Drawing.Point(160, 17);
            this.cmdCopy.Margin = new System.Windows.Forms.Padding(1);
            this.cmdCopy.Name = "cmdCopy";
            this.cmdCopy.Size = new System.Drawing.Size(60, 27);
            this.cmdCopy.TabIndex = 6;
            this.cmdCopy.Text = "Copy";
            this.cmdCopy.UseVisualStyleBackColor = true;
            this.cmdCopy.Click += new System.EventHandler(this.cmdCopy_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.linkOpenBrowser);
            this.groupBox1.Controls.Add(this.textLastMessage);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cmdInitBaseApp);
            this.groupBox1.Controls.Add(this.cmdStartBaseApp);
            this.groupBox1.Controls.Add(this.cmdStopBaseApp);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(452, 130);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Mesh Connection";
            // 
            // linkOpenBrowser
            // 
            this.linkOpenBrowser.AutoSize = true;
            this.linkOpenBrowser.Location = new System.Drawing.Point(96, 74);
            this.linkOpenBrowser.Name = "linkOpenBrowser";
            this.linkOpenBrowser.Size = new System.Drawing.Size(87, 13);
            this.linkOpenBrowser.TabIndex = 13;
            this.linkOpenBrowser.TabStop = true;
            this.linkOpenBrowser.Text = "linkOpenBrowser";
            this.linkOpenBrowser.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkOpenBrowser_LinkClicked);
            // 
            // textLastMessage
            // 
            this.textLastMessage.Location = new System.Drawing.Point(94, 104);
            this.textLastMessage.Name = "textLastMessage";
            this.textLastMessage.ReadOnly = true;
            this.textLastMessage.Size = new System.Drawing.Size(348, 20);
            this.textLastMessage.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 107);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Last Message:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmdCopy);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.textScopeID);
            this.groupBox2.Location = new System.Drawing.Point(13, 148);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(452, 58);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Data Security";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(477, 230);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMain";
            this.Text = "Host for Http Interceptor Samples";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdStartBaseApp;
        private System.Windows.Forms.Button cmdStopBaseApp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textScopeID;
        private System.Windows.Forms.Button cmdInitBaseApp;
        private System.Windows.Forms.Button cmdCopy;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textLastMessage;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel linkOpenBrowser;
    }
}

