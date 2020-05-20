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
            this.textLastMessage = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.linkOpenBrowser = new System.Windows.Forms.LinkLabel();
            this.groupScopeID = new System.Windows.Forms.GroupBox();
            this.groupWebMeshSettings = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.checkEnableREST = new System.Windows.Forms.CheckBox();
            this.checkEnableMeshResponse = new System.Windows.Forms.CheckBox();
            this.checkEnableMeshRequest = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textCloudServiceRoute = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupCloudRoute = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.checkNoCloudServiceRoute = new System.Windows.Forms.CheckBox();
            this.groupPortSettings = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.radioPort8720 = new System.Windows.Forms.RadioButton();
            this.radioPort8740 = new System.Windows.Forms.RadioButton();
            this.radioPort8730 = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupScopeID.SuspendLayout();
            this.groupWebMeshSettings.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupCloudRoute.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupPortSettings.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
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
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Value:";
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
            this.groupBox1.Controls.Add(this.textLastMessage);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cmdInitBaseApp);
            this.groupBox1.Controls.Add(this.cmdStartBaseApp);
            this.groupBox1.Controls.Add(this.cmdStopBaseApp);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(452, 112);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Mesh Connection";
            // 
            // textLastMessage
            // 
            this.textLastMessage.Location = new System.Drawing.Point(88, 81);
            this.textLastMessage.Name = "textLastMessage";
            this.textLastMessage.ReadOnly = true;
            this.textLastMessage.Size = new System.Drawing.Size(348, 20);
            this.textLastMessage.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 84);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Last Message:";
            // 
            // linkOpenBrowser
            // 
            this.linkOpenBrowser.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.linkOpenBrowser.AutoSize = true;
            this.linkOpenBrowser.Location = new System.Drawing.Point(39, 20);
            this.linkOpenBrowser.Name = "linkOpenBrowser";
            this.linkOpenBrowser.Size = new System.Drawing.Size(87, 13);
            this.linkOpenBrowser.TabIndex = 13;
            this.linkOpenBrowser.TabStop = true;
            this.linkOpenBrowser.Text = "linkOpenBrowser";
            this.linkOpenBrowser.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkOpenBrowser_LinkClicked);
            // 
            // groupScopeID
            // 
            this.groupScopeID.Controls.Add(this.cmdCopy);
            this.groupScopeID.Controls.Add(this.label1);
            this.groupScopeID.Controls.Add(this.textScopeID);
            this.groupScopeID.Location = new System.Drawing.Point(13, 294);
            this.groupScopeID.Name = "groupScopeID";
            this.groupScopeID.Size = new System.Drawing.Size(245, 58);
            this.groupScopeID.TabIndex = 10;
            this.groupScopeID.TabStop = false;
            this.groupScopeID.Text = "Scope ID / Security ID";
            // 
            // groupWebMeshSettings
            // 
            this.groupWebMeshSettings.Controls.Add(this.tableLayoutPanel3);
            this.groupWebMeshSettings.Location = new System.Drawing.Point(13, 358);
            this.groupWebMeshSettings.Name = "groupWebMeshSettings";
            this.groupWebMeshSettings.Size = new System.Drawing.Size(451, 103);
            this.groupWebMeshSettings.TabIndex = 11;
            this.groupWebMeshSettings.TabStop = false;
            this.groupWebMeshSettings.Text = "Web to Mesh Settings";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Controls.Add(this.checkEnableREST, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.checkEnableMeshResponse, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.checkEnableMeshRequest, 0, 1);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(41, 19);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(287, 73);
            this.tableLayoutPanel3.TabIndex = 3;
            // 
            // checkEnableREST
            // 
            this.checkEnableREST.AutoSize = true;
            this.checkEnableREST.Checked = true;
            this.checkEnableREST.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkEnableREST.Location = new System.Drawing.Point(3, 3);
            this.checkEnableREST.Name = "checkEnableREST";
            this.checkEnableREST.Size = new System.Drawing.Size(201, 17);
            this.checkEnableREST.TabIndex = 0;
            this.checkEnableREST.Text = "Enable REST (/api/milerecordholder)";
            this.checkEnableREST.UseVisualStyleBackColor = true;
            // 
            // checkEnableMeshResponse
            // 
            this.checkEnableMeshResponse.AutoSize = true;
            this.checkEnableMeshResponse.Location = new System.Drawing.Point(3, 51);
            this.checkEnableMeshResponse.Name = "checkEnableMeshResponse";
            this.checkEnableMeshResponse.Size = new System.Drawing.Size(180, 17);
            this.checkEnableMeshResponse.TabIndex = 2;
            this.checkEnableMeshResponse.Text = "Enable Mesh Response for Data";
            this.checkEnableMeshResponse.UseVisualStyleBackColor = true;
            // 
            // checkEnableMeshRequest
            // 
            this.checkEnableMeshRequest.AutoSize = true;
            this.checkEnableMeshRequest.Location = new System.Drawing.Point(3, 27);
            this.checkEnableMeshRequest.Name = "checkEnableMeshRequest";
            this.checkEnableMeshRequest.Size = new System.Drawing.Size(172, 17);
            this.checkEnableMeshRequest.TabIndex = 1;
            this.checkEnableMeshRequest.Text = "Enable Mesh Request for Data";
            this.checkEnableMeshRequest.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.Location = new System.Drawing.Point(3, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 31);
            this.label2.TabIndex = 2;
            this.label2.Text = "Path:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textCloudServiceRoute
            // 
            this.textCloudServiceRoute.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textCloudServiceRoute.Location = new System.Drawing.Point(49, 9);
            this.textCloudServiceRoute.Name = "textCloudServiceRoute";
            this.textCloudServiceRoute.Size = new System.Drawing.Size(240, 20);
            this.textCloudServiceRoute.TabIndex = 3;
            this.textCloudServiceRoute.Text = "http://localhost:8730";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.groupCloudRoute);
            this.groupBox4.Controls.Add(this.groupPortSettings);
            this.groupBox4.Controls.Add(this.linkOpenBrowser);
            this.groupBox4.Location = new System.Drawing.Point(12, 143);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(452, 145);
            this.groupBox4.TabIndex = 14;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Network Settings";
            // 
            // groupCloudRoute
            // 
            this.groupCloudRoute.Controls.Add(this.tableLayoutPanel1);
            this.groupCloudRoute.Location = new System.Drawing.Point(113, 45);
            this.groupCloudRoute.Name = "groupCloudRoute";
            this.groupCloudRoute.Size = new System.Drawing.Size(323, 94);
            this.groupCloudRoute.TabIndex = 4;
            this.groupCloudRoute.TabStop = false;
            this.groupCloudRoute.Text = "Service Route";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 85F));
            this.tableLayoutPanel1.Controls.Add(this.checkNoCloudServiceRoute, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.textCloudServiceRoute, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 23);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(311, 65);
            this.tableLayoutPanel1.TabIndex = 14;
            // 
            // checkNoCloudServiceRoute
            // 
            this.checkNoCloudServiceRoute.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkNoCloudServiceRoute.AutoSize = true;
            this.checkNoCloudServiceRoute.Location = new System.Drawing.Point(49, 43);
            this.checkNoCloudServiceRoute.Name = "checkNoCloudServiceRoute";
            this.checkNoCloudServiceRoute.Size = new System.Drawing.Size(111, 17);
            this.checkNoCloudServiceRoute.TabIndex = 4;
            this.checkNoCloudServiceRoute.Text = "No Service Route";
            this.checkNoCloudServiceRoute.UseVisualStyleBackColor = true;
            this.checkNoCloudServiceRoute.CheckedChanged += new System.EventHandler(this.CheckNoCloudServiceRoute_CheckedChanged);
            // 
            // groupPortSettings
            // 
            this.groupPortSettings.Controls.Add(this.tableLayoutPanel2);
            this.groupPortSettings.Location = new System.Drawing.Point(18, 42);
            this.groupPortSettings.Name = "groupPortSettings";
            this.groupPortSettings.Size = new System.Drawing.Size(79, 97);
            this.groupPortSettings.TabIndex = 3;
            this.groupPortSettings.TabStop = false;
            this.groupPortSettings.Text = "Port:";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Controls.Add(this.radioPort8720, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.radioPort8740, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.radioPort8730, 0, 1);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(6, 14);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(67, 79);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // radioPort8720
            // 
            this.radioPort8720.AutoSize = true;
            this.radioPort8720.Checked = true;
            this.radioPort8720.Location = new System.Drawing.Point(3, 3);
            this.radioPort8720.Name = "radioPort8720";
            this.radioPort8720.Size = new System.Drawing.Size(49, 17);
            this.radioPort8720.TabIndex = 0;
            this.radioPort8720.TabStop = true;
            this.radioPort8720.Text = "8720";
            this.radioPort8720.UseVisualStyleBackColor = true;
            this.radioPort8720.CheckedChanged += new System.EventHandler(this.RadioPort_CheckedChanged);
            // 
            // radioPort8740
            // 
            this.radioPort8740.AutoSize = true;
            this.radioPort8740.Location = new System.Drawing.Point(3, 55);
            this.radioPort8740.Name = "radioPort8740";
            this.radioPort8740.Size = new System.Drawing.Size(49, 17);
            this.radioPort8740.TabIndex = 2;
            this.radioPort8740.Text = "8740";
            this.radioPort8740.UseVisualStyleBackColor = true;
            this.radioPort8740.CheckedChanged += new System.EventHandler(this.RadioPort_CheckedChanged);
            // 
            // radioPort8730
            // 
            this.radioPort8730.AutoSize = true;
            this.radioPort8730.Location = new System.Drawing.Point(3, 29);
            this.radioPort8730.Name = "radioPort8730";
            this.radioPort8730.Size = new System.Drawing.Size(49, 17);
            this.radioPort8730.TabIndex = 1;
            this.radioPort8730.Text = "8730";
            this.radioPort8730.UseVisualStyleBackColor = true;
            this.radioPort8730.CheckedChanged += new System.EventHandler(this.RadioPort_CheckedChanged);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 473);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupWebMeshSettings);
            this.Controls.Add(this.groupScopeID);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMain";
            this.Text = "App Host for Web 2 Mesh Samples";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupScopeID.ResumeLayout(false);
            this.groupScopeID.PerformLayout();
            this.groupWebMeshSettings.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupCloudRoute.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupPortSettings.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupScopeID;
        private System.Windows.Forms.TextBox textLastMessage;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel linkOpenBrowser;
        private System.Windows.Forms.GroupBox groupWebMeshSettings;
        private System.Windows.Forms.CheckBox checkEnableREST;
        private System.Windows.Forms.CheckBox checkEnableMeshRequest;
        private System.Windows.Forms.TextBox textCloudServiceRoute;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupPortSettings;
        private System.Windows.Forms.RadioButton radioPort8720;
        private System.Windows.Forms.RadioButton radioPort8740;
        private System.Windows.Forms.RadioButton radioPort8730;
        private System.Windows.Forms.CheckBox checkNoCloudServiceRoute;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.CheckBox checkEnableMeshResponse;
        private System.Windows.Forms.GroupBox groupCloudRoute;
    }
}

