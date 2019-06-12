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

using SteerStone.Encryption;
using SteerStone.Handler;
using SteerStone.TCP;
using System.Text;
using System.Windows.Forms;

namespace SteerStone.Entry
{
    /// <summary>
    /// Main entry point to handle all forms
    /// </summary>
    public sealed class MainEntry : AsyncSocketClient
    {
        /// <summary>
        /// Static Constructor used for other forms are accessing this class
        /// </summary>
        static MainEntry() { }

        /// <summary>
        /// Private Constructor
        /// </summary>
        private MainEntry() { }

        /// <summary>
        /// Initialize Opcode handlers
        /// </summary>
        public void BootUp()
        {
            /// Initialize our server handlers
            m_MessageHandler.RegisterServerHandlers();

            /// Boot up our login form for user to login
            m_Login.ShowDialog();
        }

        /// <summary>
        /// Return our singleton class
        /// </summary>
        public static MainEntry GetInstance => m_Instance;

        /// <summary>
        /// Virtual method called when socket recieves data from server
        /// </summary>
        /// <param name="p_Length"></param>
        public override void ProcessIncomingData(int p_Length)
        {
            /// We can potentially recieve multiple packets in same stream, split them up and process from there
            string l_Buffer = Encoding.Default.GetString(DecryptRC4(System.Text.Encoding.Default.GetString(GetBuffer().Buffer))).Substring(0, p_Length); ///< Get rid of junk characters at end of string
            string[] l_Split = l_Buffer.Split('\x1');

            foreach (var l_String in l_Split)
            {
                /// If our string length is less than 1, don't read it
                if (l_String.Length < 1)
                    continue;

                m_MessageHandler.ExecuteServerMessageHandler(l_String);
            }
        }

        /// <summary>
        /// Pass the data into our socket class
        /// </summary>
        /// <param name="p_Buffer"></param>
        public void SendPacket(string p_Buffer)
        {
            Send(p_Buffer);
        }

        /// <summary>
        /// Close down application
        /// </summary>
        public void CloseApplication()
        {
            System.Environment.Exit(0);
        }

        /// <summary>
        /// Returns our login form
        /// </summary>
        /// <returns></returns>
        public ref Login GetLogin()
        {
            return ref m_Login;
        }

        /// <summary>
        /// Returns our main form
        /// </summary>
        /// <returns></returns>
        public ref Main GetMain()
        {
            return ref m_Main;
        }

        /// <summary>
        /// Show main dialogue (gets executed on UI thread)
        /// </summary>
        public void ShowMain()
        {
            m_Login.Invoke((MethodInvoker)delegate {
                m_Login.Hide();
                m_Main.UpdateContent();
                m_Main.DoesExecutableExist();
                m_Main.Login();
                m_Main.ShowDialog();
            });
        }

        /// <summary>
        /// Show login dialogue (gets executed on UI thread)
        /// </summary>
        public void ShowLogin()
        {
            m_Login.Invoke((MethodInvoker)delegate {
                m_Main.Hide();
                m_Login.Show();
            });
        }

        /// <summary>
        /// Variables declarations
        /// </summary>
        private static readonly MainEntry m_Instance = new MainEntry();
        private readonly MessageHandler m_MessageHandler = new MessageHandler();
        private readonly Base64 m_Base64 = new Base64();

        /// Forms
        private Login m_Login = new Login();
        private Main m_Main = new Main();
    }
}
