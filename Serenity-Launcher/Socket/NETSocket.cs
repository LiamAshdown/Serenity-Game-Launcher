/*
* Liam Ashdown
* Copyright (C) 2019
*
* This program is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using SteerStone.Encryption;
using SteerStone.Handler;
using SteerStone.Entry;

namespace SteerStone.TCP
{
    /// <summary>
    /// Holds Buffer and size
    /// </summary>
    public class BufferData
    {
        public byte[] Buffer = new byte[Common.BufferSize];
        public StringBuilder BufferString = new StringBuilder();
    }

    /// <summary>
    ///  To Handle incoming data and sending outgoing data to and from server
    /// </summary>
    public class AsyncSocketClient : CryptionCoder
    {
        /// <summary>
        /// Attempt to connect client to server
        /// </summary>
        /// <param name="p_Address"></param>
        /// <param name="p_Port"></param>
        public void StartClient(string p_Address, int p_Port)
        {
            IPHostEntry l_IpHost = Dns.GetHostEntry(p_Address);
            IPAddress l_Ip = l_IpHost.AddressList[0];
            IPEndPoint l_RemoteEndPoint = new IPEndPoint(l_Ip, p_Port);

            m_Socket = new Socket(l_Ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            m_Socket.BeginConnect(l_RemoteEndPoint, new AsyncCallback(ConnectionCallBack), m_Socket);
        }

        /// <summary>
        /// Wait for any incoming data from server
        /// </summary>
        protected void Recieve()
        {
            m_Socket.BeginReceive(m_Buffer.Buffer, 0, Common.BufferSize, SocketFlags.None, new AsyncCallback(RecieveCallBack), m_Buffer);
            m_RecieveCompleted.WaitOne();
        }

        /// <summary>
        /// Read data sent by server
        /// </summary>
        /// <param name="p_Ar"></param>
        private void RecieveCallBack(IAsyncResult p_Ar)
        {
            try
            {
                int l_BytesRecieved = m_Socket.EndReceive(p_Ar);

                if (l_BytesRecieved > 0)
                {
                    m_Buffer.BufferString.Append(Encoding.Default.GetString(m_Buffer.Buffer, 0, l_BytesRecieved));

                    ProcessIncomingData(l_BytesRecieved);

                    /// Continue to read data
                    m_Socket.BeginReceive(m_Buffer.Buffer, 0, Common.BufferSize, SocketFlags.None, new AsyncCallback(RecieveCallBack), m_Buffer);
                }

                m_RecieveCompleted.Set();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error #0004", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Pass the buffer into our derived class to handle
        /// </summary>
        /// <param name="p_Length"></param>
        public virtual void ProcessIncomingData(int p_Length) { }

        /// <summary>
        /// Send data to server
        /// </summary>
        /// <param name="p_Buffer"></param>
        protected void Send(string p_Buffer)
        {
            byte[] l_Temp = new CryptionCoder().EncryptRC4(p_Buffer);
            m_Socket.BeginSend(l_Temp, 0, l_Temp.Length, 0, new AsyncCallback(SendCallBack), m_Socket);
            m_SendCompleted.WaitOne();
        }

        /// <summary>
        /// End of completion of sending data to server
        /// </summary>
        /// <param name="p_Ar"></param>
        private void SendCallBack(IAsyncResult p_Ar)
        {
            try
            {
                m_SendCompleted.Set();

                /// Continue to recieve packets
                Recieve();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error #0002", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Connection call back
        /// </summary>
        /// <param name="p_Ar"></param>
        private void ConnectionCallBack(IAsyncResult p_Ar)
        {
            try
            {
                m_ConnectCompleted.Set();

                if (m_Socket.Connected)
                {
                    /// We've connected! Now login into server
                    MainEntry.GetInstance.GetLogin().LoginIntoServer();
                    m_Open = true;
                }
                else
                    CloseSocket();

                m_Socket.EndConnect(p_Ar);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error #0003", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Close down socket
        /// </summary>
        protected void CloseSocket()
        {
            if (!IsOpen())
                return;

            m_Open = false;
            m_Socket.Shutdown(SocketShutdown.Both);
            m_Socket.Close();
        }

        /// <summary>
        /// Returns our buffer we are currently dealing with
        /// </summary>
        /// <returns></returns>
        protected ref BufferData GetBuffer()
        {
            return ref m_Buffer;
        }

        /// <summary>
        /// Returns if socket is open or closed
        /// </summary>
        /// <returns></returns>
        public bool IsOpen()
        {
            return m_Open;
        }

        /// <summary>
        /// Variables declarations
        /// </summary>
        private Socket m_Socket = null;
        bool m_Open = false;
        private BufferData m_Buffer = new BufferData();
        private readonly ManualResetEvent m_ConnectCompleted = new ManualResetEvent(false);
        private readonly ManualResetEvent m_SendCompleted = new ManualResetEvent(false);
        private readonly ManualResetEvent m_RecieveCompleted = new ManualResetEvent(false);
    }
}
