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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using SteerStone.Handler;
using SteerStone.Entry;
using SteerStone.Packet;
using SteerStone.AccountInfo;
using SteerStone.LauncherUpdate;
using SteerStone.ConfigHandler;

namespace SteerStone
{
    public partial class Login : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );

        /// <summary>
        ///  Constructor
        /// </summary>
        public Login()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));

            /// Check if there's any updates we can download
            Updater l_Updater = new Updater();
            l_Updater.CheckForUpdates();
        }

        #region Login

        /// <summary>
        /// When cursor hovers over, it will activate the hover button effect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Picture_Box_MouseEnter(object sender, EventArgs e)
        {
            Login_Picture_Box.Visible = false;
            Login_Picture_Box_Hover.Visible = true;
        }

        /// <summary>
        /// When cursor hovers over, it will activate the un-hover button effect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Picture_Box_Hover_MouseLeave(object sender, EventArgs e)
        {
            Login_Picture_Box.Visible = true;
            Login_Picture_Box_Hover.Visible = false;
        }

        /// <summary>
        /// Login into the account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Picture_Box_Hover_MouseDown(object sender, MouseEventArgs e)
        {
            /// Username and password must not be empty!
            if (!(String.IsNullOrEmpty(Login_Text_Box_Username.Text) || String.IsNullOrEmpty(Login_Text_Box_Password.Text)))
            {
                /// Connect to server
                MainEntry.GetInstance.StartClient("127.0.0.1", 3724); ///< Hard coded address

                /// Save username and password for us to login into website
                Account.SetUsername(Login_Text_Box_Username.Text);
                Account.SetPassword(Login_Text_Box_Password.Text);
            }
            else
            {
                MessageBox.Show("Username or Password is empty!",
                  "Error #0001",
                  MessageBoxButtons.OK,
                  MessageBoxIcon.Error 
                 );
            }
        }

        /// <summary>
        /// Login into server - this is called when BeginConnect callback is called
        /// </summary>
        public void LoginIntoServer()
        {
            /// Send our login details to server
            ClientPacket l_ClientPacket = new ClientPacket();
            l_ClientPacket.AppendInterger(Common.CLIENT_LAUNCHER_LOGIN);
            l_ClientPacket.AppendString(AccountInfo.Account.GetUsername(), false);
            l_ClientPacket.AppendString(AccountInfo.Account.GetPassword(), false);
            l_ClientPacket.AppendSOH();
            MainEntry.GetInstance.SendPacket(l_ClientPacket.GetData());
        }

        #endregion

        #region Register
        /// <summary>
        /// When cursor hovers over, it will activate the hover button effect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Picture_Box_Register_MouseEnter(object sender, EventArgs e)
        {
            Login_Picture_Box_Register.Visible = false;
            Login_Picture_Box_Register_Hover.Visible = true;
        }

        /// <summary>
        /// When cursor hovers over, it will activate the un-hover button effect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Picture_Box_Register_Hover_MouseLeave(object sender, EventArgs e)
        {
            Login_Picture_Box_Register.Visible = true;
            Login_Picture_Box_Register_Hover.Visible = false;
        }

        /// <summary>
        /// Open new web page to reset password
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Picture_Box_Password_Recovery_MouseDown(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.serenity-wow.com/account");
        }

        /// <summary>
        /// Open new web page to create account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Picture_Box_Register_Hover_MouseDown(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.serenity-wow.com/register");
        }

        #endregion

        #region Close

        /// <summary>
        /// When cursor hovers over, it will activate the hover button effect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Picture_Box_Close_MouseEnter(object sender, EventArgs e)
        {
            Login_Picture_Box_Close.Visible = false;
            Login_Picture_Box_Close_Hover.Visible = true;
        }

        /// <summary>
        /// When cursor hovers over, it will activate the un-hover button effect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Picture_Box_Close_Hover_MouseLeave(object sender, EventArgs e)
        {
            Login_Picture_Box_Close.Visible = true;
            Login_Picture_Box_Close_Hover.Visible = false;
        }

        /// <summary>
        /// Exit the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Picture_Box_Close_Hover_MouseDown(object sender, MouseEventArgs e)
        {
            MainEntry.GetInstance.CloseApplication();
        }

        #endregion

        #region Panel
        /// <summary>
        /// Allow to move the login form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Panel_Mover_MouseDown(object sender, MouseEventArgs e)
        {
            m_CanMove = true;
            m_X = e.X;
            m_Y = e.Y;
        }

        /// <summary>
        /// Move the login move to cursor location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Panel_Mover_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_CanMove)
                this.SetDesktopLocation(MousePosition.X - m_X, MousePosition.Y - m_Y);
        }

        /// <summary>
        /// Set to no longer can move login form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Panel_Mover_MouseUp(object sender, MouseEventArgs e)
        {
            m_CanMove = false;
        }

        #endregion

        #region Variables

        private bool m_CanMove;
        private int m_X;
        private int m_Y;
        #endregion
    }
}
