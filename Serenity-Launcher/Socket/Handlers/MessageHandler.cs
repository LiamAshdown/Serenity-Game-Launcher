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

using System.Windows.Forms;
using SteerStone.Packet;
using SteerStone.AccountInfo;
using SteerStone.Entry;
using SteerStone.ArticleInfo;

namespace SteerStone.Handler
{
    /// <summary>
    /// Responsible for executing the server opcode handlers
    /// </summary>
    public sealed class MessageHandler
    {
        /// <summary>
        ///  Private constructor gets called on first creation of MessageHandler
        /// </summary>
        public MessageHandler() { }

        /// <summary>
        /// Initialize our client handlers
        /// </summary>
        public void RegisterServerHandlers()
        {
            m_ServerHandler = new ServerHandler[Common.MaxHeaderId];
            m_ServerHandler[Common.SERVER_LAUNCHER_LOGIN] = new ServerHandler(HandleLogin);
            m_ServerHandler[Common.SERVER_LAUNCHER_INFO] = new ServerHandler(HandleInfo);
            m_ServerHandler[Common.SERVER_LAUNCHER_UPDATE] = new ServerHandler(HandleLauncherUpdate);
        }

        /// <summary>
        /// Execute the server opcode handler
        /// </summary>
        /// <param name="p_HeaderId"></param>
        public void ExecuteServerMessageHandler(string p_Packet)
        {
            /// Create temporary server packet just to get header id
            ServerPacket m_TempServerPacket = new ServerPacket(p_Packet);

            if (m_TempServerPacket.GetHeader() <= Common.MaxHeaderId)
                if (m_ServerHandler[m_TempServerPacket.GetHeader()] != null)
                    m_ServerHandler[m_TempServerPacket.GetHeader()].Invoke(new ServerPacket(p_Packet));
        }

        /// <summary>
        /// Login result
        /// </summary>
        private void HandleLogin(ServerPacket p_ServerPacket)
        {
            int l_ErrorCode = p_ServerPacket.ReadBase64Int();

            string l_Message = "Success";

            switch (l_ErrorCode)
            {
                case (int)AuthResult.WOW_FAIL_SUSPENDED:
                    l_Message = "This account is currently supsended. Please try again later.";
                    break;

                case (int)AuthResult.WOW_FAIL_BANNED:
                    l_Message = "This account is permanently banned.";
                    break;

                case (int)AuthResult.WOW_FAIL_INCORRECT_PASSWORD:
                    l_Message = "Username/Password is incorrect!";
                    break;

                    /// Success! Player successfully logged in!
                case (int)AuthResult.WOW_SUCCESS:
                    {
                        Account.SetAccountId(p_ServerPacket.ReadBase64Uint());
                        Account.SetAccountType(p_ServerPacket.ReadBase64Uint());

                        /// Lets boot up our launcher
                        /// Send packet to server to get launcher information
                        ClientPacket l_ClientPacket = new ClientPacket();
                        l_ClientPacket.AppendInterger(Common.CLIENT_LAUNCHER_INFO);
                        l_ClientPacket.AppendSOH();
                        MainEntry.GetInstance.SendPacket(l_ClientPacket.GetData());
                    }
                    break;
            }

            if (l_Message != "Success")
            {
                MessageBox.Show(l_Message, "Error #0003", MessageBoxButtons.OK, MessageBoxIcon.Error);

                MainEntry.GetInstance.GetLogin().Invoke((MethodInvoker)delegate {
                    MainEntry.GetInstance.GetLogin().Login_Text_Box_Password.Clear();
                    MainEntry.GetInstance.GetLogin().Login_Text_Box_Username.Clear();
                });
            }
        }

        /// <summary>
        /// Get server information
        /// </summary>
        private void HandleInfo(ServerPacket p_ServerPacket)
        {
            uint l_Count = p_ServerPacket.ReadBase64Uint();
      
            /// Loop through and store our articles
            for (int l_I = 0; l_I < l_Count; l_I++)
            {
                Article l_Article = new Article();
                l_Article.NameBy = p_ServerPacket.ReadString();
                l_Article.HeadLine = p_ServerPacket.ReadString();
                l_Article.Content = p_ServerPacket.ReadString();
                l_Article.Date = p_ServerPacket.ReadBase64Uint();

                ArticleAccessor.Articles.Add(l_Article);
            }

            MainEntry.GetInstance.ShowMain();
        }

        /// <summary>
        /// Get server update
        /// </summary>
        private void HandleLauncherUpdate(ServerPacket p_ServerPacket)
        {
            /// Retrieve our notification
            string l_Notification = p_ServerPacket.ReadString();
            uint l_Population = p_ServerPacket.ReadBase64Uint();
            uint l_AccountRegistered = p_ServerPacket.ReadBase64Uint();

            MainEntry.GetInstance.GetMain().UpdateStatus(l_Notification, l_AccountRegistered, l_Population);
        }

        /// <summary>
        /// Variables declarations
        /// </summary>
        private delegate void ServerHandler(ServerPacket p_ServerPacket);
        private ServerHandler[] m_ServerHandler;
    }
}
