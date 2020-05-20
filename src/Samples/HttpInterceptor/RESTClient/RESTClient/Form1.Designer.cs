// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿namespace RESTClient
{
    partial class Form1
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
            this.cmdLogon = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textUID = new System.Windows.Forms.TextBox();
            this.textPWD = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textURL = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textRequest = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textAccessToken = new System.Windows.Forms.TextBox();
            this.textResponse = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textParameters = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.radio8720 = new System.Windows.Forms.RadioButton();
            this.radio8730 = new System.Windows.Forms.RadioButton();
            this.radio8740 = new System.Windows.Forms.RadioButton();
            this.cmdGET = new System.Windows.Forms.Button();
            this.listLog = new System.Windows.Forms.ListView();
            this.Id = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Action = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Details = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cmdClearLog = new System.Windows.Forms.LinkLabel();
            this.radioOther = new System.Windows.Forms.RadioButton();
            this.textOtherPort = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.textCommand = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdLogon
            // 
            this.cmdLogon.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cmdLogon.Location = new System.Drawing.Point(391, 4);
            this.cmdLogon.Name = "cmdLogon";
            this.cmdLogon.Size = new System.Drawing.Size(75, 25);
            this.cmdLogon.TabIndex = 0;
            this.cmdLogon.Text = "Log On";
            this.cmdLogon.UseVisualStyleBackColor = true;
            this.cmdLogon.Click += new System.EventHandler(this.Logon_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel5, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.textURL, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.textRequest, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.textAccessToken, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.textResponse, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.label8, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label9, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 1, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.424464F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.8275F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.97478F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.97478F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.424464F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.424464F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 29.94956F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(639, 270);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 5;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.Controls.Add(this.label6, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label7, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.textUID, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.textPWD, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.cmdLogon, 4, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(162, 59);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(469, 34);
            this.tableLayoutPanel2.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 10);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "UID:";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(193, 10);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(36, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "PWD:";
            // 
            // textUID
            // 
            this.textUID.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textUID.Location = new System.Drawing.Point(49, 7);
            this.textUID.Name = "textUID";
            this.textUID.Size = new System.Drawing.Size(100, 20);
            this.textUID.TabIndex = 2;
            this.textUID.Text = "myuser";
            // 
            // textPWD
            // 
            this.textPWD.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textPWD.Location = new System.Drawing.Point(235, 7);
            this.textPWD.Name = "textPWD";
            this.textPWD.Size = new System.Drawing.Size(100, 20);
            this.textPWD.TabIndex = 3;
            this.textPWD.Text = "asterisks";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "REST Server Base URL:";
            // 
            // textURL
            // 
            this.textURL.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textURL.Location = new System.Drawing.Point(162, 3);
            this.textURL.Name = "textURL";
            this.textURL.Size = new System.Drawing.Size(355, 20);
            this.textURL.TabIndex = 1;
            this.textURL.Text = "http://localhost/api/MileRecordHolder";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(77, 142);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Access Token:";
            // 
            // textRequest
            // 
            this.textRequest.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textRequest.Location = new System.Drawing.Point(162, 164);
            this.textRequest.Name = "textRequest";
            this.textRequest.Size = new System.Drawing.Size(474, 20);
            this.textRequest.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(56, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(94, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Response Stream:";
            // 
            // textAccessToken
            // 
            this.textAccessToken.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textAccessToken.Location = new System.Drawing.Point(162, 139);
            this.textAccessToken.Name = "textAccessToken";
            this.textAccessToken.Size = new System.Drawing.Size(229, 20);
            this.textAccessToken.TabIndex = 3;
            // 
            // textResponse
            // 
            this.textResponse.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textResponse.Location = new System.Drawing.Point(162, 189);
            this.textResponse.Multiline = true;
            this.textResponse.Name = "textResponse";
            this.textResponse.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textResponse.Size = new System.Drawing.Size(474, 78);
            this.textResponse.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(94, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Credentials:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(81, 167);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Request URL:";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(66, 109);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(90, 13);
            this.label8.TabIndex = 9;
            this.label8.Text = "Input Parameters:";
            // 
            // textParameters
            // 
            this.textParameters.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textParameters.Location = new System.Drawing.Point(159, 7);
            this.textParameters.Name = "textParameters";
            this.textParameters.Size = new System.Drawing.Size(150, 20);
            this.textParameters.TabIndex = 10;
            this.textParameters.Text = "id=2";
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(127, 34);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(29, 13);
            this.label9.TabIndex = 11;
            this.label9.Text = "Port:";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 5;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.Controls.Add(this.radio8720, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.radio8730, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.radio8740, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.radioOther, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.textOtherPort, 4, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(162, 28);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(347, 25);
            this.tableLayoutPanel3.TabIndex = 12;
            // 
            // radio8720
            // 
            this.radio8720.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radio8720.AutoSize = true;
            this.radio8720.Checked = true;
            this.radio8720.Location = new System.Drawing.Point(3, 4);
            this.radio8720.Name = "radio8720";
            this.radio8720.Size = new System.Drawing.Size(49, 17);
            this.radio8720.TabIndex = 0;
            this.radio8720.TabStop = true;
            this.radio8720.Text = "8720";
            this.radio8720.UseVisualStyleBackColor = true;
            // 
            // radio8730
            // 
            this.radio8730.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radio8730.AutoSize = true;
            this.radio8730.Location = new System.Drawing.Point(72, 4);
            this.radio8730.Name = "radio8730";
            this.radio8730.Size = new System.Drawing.Size(49, 17);
            this.radio8730.TabIndex = 1;
            this.radio8730.Text = "8730";
            this.radio8730.UseVisualStyleBackColor = true;
            // 
            // radio8740
            // 
            this.radio8740.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radio8740.AutoSize = true;
            this.radio8740.Location = new System.Drawing.Point(141, 4);
            this.radio8740.Name = "radio8740";
            this.radio8740.Size = new System.Drawing.Size(49, 17);
            this.radio8740.TabIndex = 2;
            this.radio8740.Text = "8740";
            this.radio8740.UseVisualStyleBackColor = true;
            // 
            // cmdGET
            // 
            this.cmdGET.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cmdGET.Location = new System.Drawing.Point(391, 4);
            this.cmdGET.Name = "cmdGET";
            this.cmdGET.Size = new System.Drawing.Size(75, 25);
            this.cmdGET.TabIndex = 2;
            this.cmdGET.Text = "GET";
            this.cmdGET.UseVisualStyleBackColor = true;
            this.cmdGET.Click += new System.EventHandler(this.CmdGET_Click);
            // 
            // listLog
            // 
            this.listLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Id,
            this.Action,
            this.Details});
            this.listLog.FullRowSelect = true;
            this.listLog.HideSelection = false;
            this.listLog.Location = new System.Drawing.Point(0, 288);
            this.listLog.Name = "listLog";
            this.listLog.Size = new System.Drawing.Size(654, 172);
            this.listLog.TabIndex = 5;
            this.listLog.UseCompatibleStateImageBehavior = false;
            this.listLog.View = System.Windows.Forms.View.Details;
            this.listLog.SelectedIndexChanged += new System.EventHandler(this.ListLog_SelectedIndexChanged);
            // 
            // Id
            // 
            this.Id.Text = "Id";
            this.Id.Width = 50;
            // 
            // Action
            // 
            this.Action.Text = "Action";
            this.Action.Width = 150;
            // 
            // Details
            // 
            this.Details.Text = "Details";
            this.Details.Width = 437;
            // 
            // cmdClearLog
            // 
            this.cmdClearLog.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cmdClearLog.AutoSize = true;
            this.cmdClearLog.Location = new System.Drawing.Point(50, 63);
            this.cmdClearLog.Name = "cmdClearLog";
            this.cmdClearLog.Size = new System.Drawing.Size(52, 13);
            this.cmdClearLog.TabIndex = 6;
            this.cmdClearLog.TabStop = true;
            this.cmdClearLog.Text = "Clear Log";
            this.cmdClearLog.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.CmdClearLog_LinkClicked);
            // 
            // radioOther
            // 
            this.radioOther.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.radioOther.AutoSize = true;
            this.radioOther.Location = new System.Drawing.Point(219, 4);
            this.radioOther.Name = "radioOther";
            this.radioOther.Size = new System.Drawing.Size(54, 17);
            this.radioOther.TabIndex = 3;
            this.radioOther.Text = "Other:";
            this.radioOther.UseVisualStyleBackColor = true;
            // 
            // textOtherPort
            // 
            this.textOtherPort.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textOtherPort.Location = new System.Drawing.Point(279, 3);
            this.textOtherPort.Name = "textOtherPort";
            this.textOtherPort.Size = new System.Drawing.Size(50, 20);
            this.textOtherPort.TabIndex = 4;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 3;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.Controls.Add(this.cmdGET, 2, 0);
            this.tableLayoutPanel4.Controls.Add(this.textParameters, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.textCommand, 0, 0);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(162, 99);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(469, 34);
            this.tableLayoutPanel4.TabIndex = 7;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.cmdClearLog, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 189);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(153, 78);
            this.tableLayoutPanel5.TabIndex = 7;
            // 
            // textCommand
            // 
            this.textCommand.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textCommand.Location = new System.Drawing.Point(3, 7);
            this.textCommand.Name = "textCommand";
            this.textCommand.Size = new System.Drawing.Size(146, 20);
            this.textCommand.TabIndex = 11;
            this.textCommand.Text = "/query?";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(655, 460);
            this.Controls.Add(this.listLog);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "RESTClient";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdLogon;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textURL;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textAccessToken;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textRequest;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textResponse;
        private System.Windows.Forms.Button cmdGET;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textUID;
        private System.Windows.Forms.TextBox textPWD;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListView listLog;
        private System.Windows.Forms.ColumnHeader Id;
        private System.Windows.Forms.ColumnHeader Action;
        private System.Windows.Forms.ColumnHeader Details;
        private System.Windows.Forms.LinkLabel cmdClearLog;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textParameters;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.RadioButton radio8720;
        private System.Windows.Forms.RadioButton radio8730;
        private System.Windows.Forms.RadioButton radio8740;
        private System.Windows.Forms.RadioButton radioOther;
        private System.Windows.Forms.TextBox textOtherPort;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TextBox textCommand;
    }
}

