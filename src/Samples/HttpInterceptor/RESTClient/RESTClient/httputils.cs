// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace RESTClient
{
    public class httputils
    {
        public static bool HttpConnectSendReceive(string strHttpMethod, string strHost, ushort usPort, string strPathAndParameters, List<string> listReceiveBuffer)
        {
            bool bSuccess = false;
            Socket sockSender = null;
            System.Net.IPAddress[] pAddress = null;

            try
            {
                try
                {
                    pAddress = Dns.GetHostAddresses(strHost);
                }
                catch
                {
                    return false;
                }
                // Connect to server.
                // sockSender = ConnectToSocket(pAddress[0].Address, usPort);
                byte[] abAddress = pAddress[0].GetAddressBytes();
                if (strHost.ToLower() == "localhost")
                    abAddress = pAddress[1].GetAddressBytes();

                sockSender = ConnectToSocket(abAddress, usPort);

                if (sockSender != null)
                {
                    System.Threading.Thread.Sleep(500);
                    Form1.AddItemToList("HttpConnectSendReceive", "Connected - Sending Request.");

                    // Send a GET request to server.
                    SendRequestToServer(sockSender, strHost, strHttpMethod, strPathAndParameters);

                    Form1.AddItemToList("HttpConnectSendReceive", "Connected - Receiving Response.");

                    ReceiveResponseFromServer(sockSender, listReceiveBuffer);
                    bSuccess = true;
                }
            }
            catch (Exception ex)
            {
                Form1.AddItemToList("HttpConnectSendReceive", $"{ex.Message}");
            }

            sockSender?.Close();
            sockSender = null;

            return bSuccess;
        }

        // public static Socket ConnectToSocket(long Address, ushort usPort)
        public static Socket ConnectToSocket(byte [] abAddress, ushort usPort)
        {
            // Create a TCP/IP  socket.  
            Socket sockSender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Connect to the server.
            // IPEndPoint remoteEP = new IPEndPoint(Address, usPort);  // 127.0.0.1:8800
            IPAddress ip = new IPAddress(abAddress);
            IPEndPoint remoteEP = new IPEndPoint(ip, usPort);

            // Try to connect. Failure = exception.
            RetryCallUntilBooleanReturn(true, "", (str) =>
            {
                bool bDidConnect = false;
                try
                {
                    sockSender.Connect(remoteEP);
                    if (sockSender.Connected)
                        bDidConnect = true;
                }
                catch (Exception ex)
                {
                    Form1.AddItemToList("Socket Connect", $"{ex.Message}");
                    bDidConnect = false;
                }
                return (bDidConnect);
            }, 5, 10);

            if (sockSender == null || !sockSender.Connected)
                sockSender = null;

            return sockSender;
        }

        public static int SendRequestToServer(Socket sockSender, string strHost, string strHttpMethod, string strPathAndParameters)
        {
            string strHttpMessage = $"{strHttpMethod} {strPathAndParameters} HTTP/1.1\r\n" +
                                    "Accept: text/html,application/xhtml,application/xml,text/plain\r\n" +
                                    "Connection: keep-alive\r\n" +
                                    $"Host: {strHost}\r\n" +
                                    "User-Agent: RestApiProgram" +
                                    "\r\n" +
                                    "\r\n";
            int iSend = sockSender.Send(Encoding.UTF8.GetBytes(strHttpMessage));
            return iSend;
        }

        public static int ReceiveResponseFromServer(Socket sockSender, List<string> listBuffer)
        {
            int cAvailable;
            // Retry waiting incoming bytes.
            // (Max 50 times with 100 ms sleep each time = max 5 seconds.)
            int iCount = 0;
            int iPrev = 0;
            int iCurrent = 0;
            RetryCallUntilBooleanReturn(true, "", (str) =>
            {
                iCount++;

                if (sockSender.Available == 0)
                    return false;

                if (iCurrent == 0)
                {
                    iCurrent = sockSender.Available;
                    return false;
                }

                if (iPrev == 0 || iPrev < iCurrent)
                {
                    iPrev = iCurrent;
                    iCurrent = sockSender.Available;
                    return false;
                }

                return true;
            });

            // Retrieve a response from the user.
            cAvailable = sockSender.Available;
            if (cAvailable > 1)
            {
                byte[] abytes = new byte[cAvailable];
                int cReceiveBytes = sockSender.Receive(abytes);
                string strReceive = Encoding.UTF8.GetString(abytes, 0, cReceiveBytes);

                // Read start of header and check for expected key values.
                using (StreamReader sr = new StreamReader(new MemoryStream(abytes)))
                {
                    bool bContinue = true;
                    while (bContinue)
                    {
                        string strLine = sr.ReadLine();
                        listBuffer.Add(strLine);
                        bContinue = strLine != null;
                    }

                    //for (int i = 0; i < listBuffer.Length; i++)
                    //{
                    //    listBuffer[i] = sr.ReadLine();
                    //}
                    //--------------------------------------------------------
                    // Return header might look like this:
                    //
                    //    HTTP/1.1 200 OK
                    //    Server: C-Labs.C-DEngine / 2.2061
                    //    cdeDeviceID: f9d272b0-3682-997b-cd5c-c884511e43c5
                    //    Content-Type: text/html
                    //--------------------------------------------------------
                    //    Return header might look like this:
                    //
                    //    HTTP/1.1 200 OK
                    //    Cache-Control: no-cache
                    //    Content-Length: 4123
                    //    Content-Type: text/html
                    //    Server: C-DEngine V4 Microsoft-HTTPAPI/2.0
                    //    Set-Cookie: aa216d51-413b-401a-80bc-baa04814145dCDESEID=5WMx...<snip>...2tZn4dpY%3D
                    //    cdeDeviceID: beffcdee-1b24-774c-d1d5-9032111e43c5
                    //--------------------------------------------------------
                    //    Return header could also look like this:
                    //
                    //       "HTTP/1.1 404 Not Found"
                    //       "Cache-Control: no-cache"
                    //       "Content-Length: 465"
                    //       "Content-Type: text/html"
                    //       "Server: C-DEngine V4 Microsoft-HTTPAPI/2.0"
                    //       "cdeDeviceID: 8e8e189a-d513-7b40-2f78-d993712c83dd"
                    //       "Date: Mon, 26 Aug 2019 20:58:33 GMT"
                }
            }

            return cAvailable;
        }



        // Helpers to retry calling functions.
        // Built in retry count. Built in sleep.
        // See example, below...
        public delegate bool RetryCallback(string strInput);
        public const int RETRY_SLEEP_MILLISECONDS = 100;
        public const int RETRY_MAX_COUNT = 50;
        public static bool RetryCallUntilBooleanReturn(bool bUntil, string strInput, RetryCallback cb, int cMaxTimes = RETRY_MAX_COUNT, int msSleepTime = RETRY_SLEEP_MILLISECONDS)
        {
            bool bFlag = !bUntil;
            for (int iRetry = 0; iRetry < cMaxTimes; iRetry++)
            {
                bFlag = cb(strInput);
                if (bFlag == bUntil)
                    break;
                System.Threading.Thread.Sleep(msSleepTime);
            }
            return bFlag;
        }

    } // class
} // namespace
