// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0
using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using nsCDEngine.BaseClasses;
using nsCDEngine.ISM;

namespace cdeTelnet
{
    public class TheTelnet : IDisposable
    {
        readonly TCPConnection telnetClient;
        readonly ASCIIEncoding encoder;
        Thread socketThread;

        /// <summary>
        /// Fires when a string was read from the server
        /// </summary>
        public Action<string> eventReadString;

        /// <summary>
        /// Fires when bytes were read from the server
        /// </summary>
        public Action<byte[]> eventReadBytes;
        /// <summary>
        /// Fires when the server was closed - locally or remotely
        /// </summary>
        public Action<string> eventClosed;

        public string LastMessage;

        public TheTelnet()
        {
            telnetClient = new TCPConnection();
            encoder = new ASCIIEncoding();
        }

        public void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (telnetClient != null) telnetClient.Dispose();
            }
        }

#if !CDE_NET35 && !CDE_NET4
        /// <summary>
        /// Connects to a telnet server and starts a polling thread
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public async System.Threading.Tasks.Task<bool> ConnectToServerAsync(string host, int port)
        {
            if (telnetClient.Connected) return true;
            if (await telnetClient.ConnectAsync(host, port))
            {
                socketThread = new Thread(PollTelnetServer);
                socketThread.Start();
                return true;
            }
            else
            {
                LastMessage = telnetClient.LastMessage;
            }
            return false;
        }
#endif
#if !CDE_CORE   //Legacy Sync methods 
        /// <summary>
        /// Connects to a telnet server and starts a polling thread
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public bool ConnectToServer(string host, int port)
        {
            if (telnetClient.Connected) return true;
            if (telnetClient.Connect(host, port))
            {
                socketThread = new Thread(PollTelnetServer);
                socketThread.Start();
                return true;
            }
            else
            {
                LastMessage = telnetClient.LastMessage;
            }
            return false;
        }
#endif

        /// <summary>
        /// Disconnects from the server
        /// </summary>
        public void Disconnect()
        {
            if (telnetClient != null)
                telnetClient.Disconnect();
        }

        private void PollTelnetServer()
        {
            TheDiagnostics.SetThreadName("Telnet-Server", true);
            while (telnetClient.Connected && TheBaseAssets.MasterSwitch)
            {
                PollServer();
                Thread.Sleep(50);
            }
            if (telnetClient.Connected)
                telnetClient.Disconnect();
            eventClosed?.Invoke("Ended");
        }

        /// <summary>
        /// Sends a command to the Telnet Server
        /// </summary>
        /// <param name="cmd"></param>
        public void SendCmd(string cmd)
        {
            if (telnetClient == null) return;
            byte[] tCmd = encoder.GetBytes(cmd);
            telnetClient.SendData(tCmd);
        }

        public void SendCmd(byte[] tCmd)
        {
            if (telnetClient == null || tCmd == null) return;
            telnetClient.SendData(tCmd);
        }

        void PollServer()
        {
            try
            {
                if (!telnetClient.Connected || !telnetClient.DataAvailable)
                {
                    return;
                }

                var dataLength = telnetClient.ReceiveData();
                var dataBytes = telnetClient.GetData();


                eventReadBytes?.Invoke(dataBytes);
                if (eventReadString != null)
                {
                    string t = encoder.GetString(dataBytes, 0, dataLength);
                    eventReadString(t);
                }
            }
            catch
            {
                //ignored
            }
        }


        /// <summary>
        /// True if the server is connected
        /// </summary>
        public bool Connected
        {
            get
            {
                return telnetClient.Connected;
            }
        }
    }


    class TCPConnection : IDisposable
    {
        TcpClient tcpClient;
        NetworkStream networkStream;
        readonly byte[] bufferReceived;
        int bytesReceived;
        public string LastMessage;

        public TCPConnection()
        {
            tcpClient = new TcpClient();
            bufferReceived = new byte[8192];
            bytesReceived = 0;
        }

        public TCPConnection(string host, int port)
        {
            tcpClient = new TcpClient();
            bufferReceived = new byte[8192];
            bytesReceived = 0;
#if CDE_CORE   //Legacy Sync methods 
            ConnectAsync(host, port);  
#else
            Connect(host, port);
#endif
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
#if CDE_CORE //Dispose instead close
                if (tcpClient != null) tcpClient.Dispose();
#else
                if (tcpClient != null) tcpClient.Close();
#endif
            }
        }

#if !CDE_NET35 && !CDE_NET4
        public async System.Threading.Tasks.Task<bool> ConnectAsync(string host, int port)
        {
            try
            {
                await tcpClient.ConnectAsync(host, port);
                networkStream = tcpClient.GetStream(); 
            }
            catch (ObjectDisposedException)
            {
                tcpClient = new TcpClient();
                await ConnectAsync(host, port);
            }
            catch (Exception e)
            {
                LastMessage = e.Message;
                TheBaseAssets.MySYSLOG.WriteToLog(234, new TSM("telnet", "Error during Connect", eMsgLevel.l6_Debug, e.ToString()));
            }
            return tcpClient.Connected;
        }
#endif

#if !CDE_CORE   //Legacy Sync methods 
        public bool Connect(string host, int port)
        {
            try
            {
                tcpClient.Connect(host, port);
                networkStream = tcpClient.GetStream();
            }
            catch (ObjectDisposedException)
            {
                tcpClient = new TcpClient();
                Connect(host, port);
            }
            catch (Exception e)
            {
                LastMessage = e.Message;
                TheBaseAssets.MySYSLOG.WriteToLog(234, new TSM("telnet", "Error during Connect", eMsgLevel.l6_Debug, e.ToString()));
            }
            return tcpClient.Connected;
        }
#endif

        public bool Disconnect()
        {
            try
            {
                if (tcpClient.Connected)
                {
#if CDE_CORE    //Dispose instead close
                    networkStream.Dispose();
                    tcpClient.Dispose();
#else
                    networkStream.Close();
                    tcpClient.Close();
#endif
                }
            }
            catch (Exception e)
            {
                TheBaseAssets.MySYSLOG.WriteToLog(234, new TSM("telnet", "Error during Disconnect", eMsgLevel.l6_Debug, e.ToString()));
            }
            return tcpClient.Connected;
        }

        public bool SendData(byte[] data)
        {
            try
            {
                if (networkStream != null && data != null)
                    networkStream.Write(data, 0, data.Length);
            }
            catch (Exception e)
            {
                TheBaseAssets.MySYSLOG.WriteToLog(234, new TSM("telnet", "Error during Send", eMsgLevel.l6_Debug, e.ToString()));
            }
            return true;
        }

        public int ReceiveData()
        {
            try
            {
                bytesReceived = networkStream.Read(bufferReceived, 0, 8192);
            }
            catch (Exception e)
            {
                TheBaseAssets.MySYSLOG.WriteToLog(234, TSM.L(eDEBUG_LEVELS.VERBOSE) ? null : new TSM("telnet", "Error during Receive Data", eMsgLevel.l6_Debug, e.ToString()));
                Disconnect();
            }
            return bytesReceived;
        }

        public byte[] GetData()
        {
            return bufferReceived;
        }

        public int GetBytesReceived()
        {
            return bytesReceived;
        }

        public bool Connected
        {
            get
            {
                return tcpClient.Connected;
            }
        }

        public bool DataAvailable
        {
            get
            {
                return networkStream.DataAvailable;
            }
        }
    }

}
