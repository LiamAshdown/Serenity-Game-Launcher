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

using SteerStone.AccountInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SteerStone.Entry;

namespace SteerStone.WebBrowser
{
    public partial class WebBrowser : Form
    {
        public WebBrowser()
        {
            InitializeComponent();
            WebBrowser_Browser.ScriptErrorsSuppressed = true; ///< Don't show script errors
            WebBrowser_Browser.Navigate("https://www.serenity-wow.com/");
        }

        /// <summary>
        /// Login into website
        /// </summary>
        public void Login(object sender,
    WebBrowserDocumentCompletedEventArgs e)
        {
            var l_Elements = WebBrowser_Browser.Document.GetElementsByTagName("input");
            foreach (HtmlElement l_I in l_Elements)
            {
                /// Enter username
                if (l_I.GetAttribute("name").Equals("login_username"))
                    l_I.InnerText = Account.GetUsername();
                else
                /// Enter password
                if (l_I.GetAttribute("name").Equals("login_password"))
                    l_I.InnerText = Account.GetPassword();
                /// Login into website
                else if (l_I.GetAttribute("name").Equals("login_submit"))
                    l_I.InvokeMember("click");
            }
        }

        /// <summary>
        /// Manually click login button; we do this to generatr crsf token due to website not supporting IE correctly 
        /// </summary>
        public void ClickLogin()
        {
            var l_Elements = WebBrowser_Browser.Document.GetElementsByTagName("a");
            foreach (HtmlElement l_I in l_Elements)
            {
                if (l_I.OuterHtml.ToString().Contains("https://serenity-wow.com/login"))
                    l_I.InvokeMember("click");
            }

            /// Process to login when webpage is fully loaded
            WebBrowser_Browser.DocumentCompleted +=
                new WebBrowserDocumentCompletedEventHandler(Login);
        }

        /// <summary>
        /// Navigate to account panel
        /// </summary>
        public void GoToAccountPanel()
        {
            WebBrowser_Browser.Navigate("https://serenity-wow.com/ucp");
            Show();
        }

        /// <summary>
        /// Navigate to bug tracker
        /// </summary>
        public void GoToBugTracker()
        {
            WebBrowser_Browser.Navigate("https://serenity-wow.com/bugtracker");
            Show();
        }

        /// <summary>
        /// Navigate to vote panel
        /// </summary>
        public void GoToVotePanel()
        {
            WebBrowser_Browser.Navigate("https://serenity-wow.com/vote");
            Show();
        }

        /// <summary>
        /// Navigate to donate panel
        /// </summary>
        public void GoToDonatePanel()
        {
            WebBrowser_Browser.Navigate("https://serenity-wow.com/donate");
            Show();
        }

        /// <summary>
        /// Navigate to armory panel
        /// </summary>
        public void GoToArmory()
        {
            WebBrowser_Browser.Navigate("https://serenity-wow.com/armory");
            Show();
        }

        /// <summary>
        /// Navigate to rating
        /// </summary>
        public void GoToRating()
        {
            WebBrowser_Browser.Navigate("https://serenity-wow.com/arena_statistics/1v1?order=&dir=&page=1");
            Show();
        }

        /// <summary>
        /// When user has resized the form, also resize the webbrowser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebBrowser_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                MainEntry.GetInstance.GetMain().Focus();
            else if (this.WindowState == FormWindowState.Maximized)
                this.Focus();
            WebBrowser_Browser.Size = new Size(this.Width, this.Height);
        }

        /// <summary>
        /// Prevent windows from diposing the form, we want it active
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true; ///< This cancels the close event.
        }

        private void WebBrowser_Deactivate(object sender, EventArgs e)
        {
            MainEntry.GetInstance.GetMain().Activate();
        }
    }
}
