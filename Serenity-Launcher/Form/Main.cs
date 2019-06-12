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
using SteerStone.ArticleInfo;
using SteerStone.ConfigHandler;
using SteerStone.Entry;
using SteerStone.Packet;
using SteerStone.ZipHandler;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SteerStone
{
    public partial class Main : Form
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
        /// Constructor
        /// </summary>
        public Main()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10)); ///< Add curves around corners to fit with design

            /// Begin timer operation
            Main_Timer_Status_Checker.Start();

            /// Initialize background worker events
            InitializeBackgroundWorker();
        }

        /// <summary>
        /// Set up the BackgroundWorker object by 
        /// </summary>
        private void InitializeBackgroundWorker()
        {
            Main_Background_Thread_Zip.DoWork +=
                new DoWorkEventHandler(DownloadClient);
        }

        #region Labels
        /// <summary>
        /// Intialize content
        /// </summary>
        public void UpdateContent()
        {
            /// Show first article
            /// There must be always an article otherwise we will crash!!!
            Article l_Article = ArticleAccessor.Articles[ArticleAccessor.Index];

            Main_Label_Head_Line.Text = l_Article.HeadLine;
            Main_Label_Version.Text = Assembly.GetEntryAssembly().GetName().Version.ToString();
            Main_Label_Username.Text = Account.GetUsername();
            Main_Label_By_Username.Text = l_Article.NameBy.ToUpper();

            /// If we don't have more than 1 article - then don't show next article arrow
            if (ArticleAccessor.Articles.Count < 2)
                Main_Label_Forward.Visible = false;
            /// Don't show forward button if we reached last article
            else if (ArticleAccessor.Index == ArticleAccessor.Articles.Count - Common.ARRAY_OFFSET)
            {
                Main_Label_Forward.Visible = false;
                Main_Label_Backward.Visible = true;
            }
            /// Don't show backward button if we reached first article
            else if (ArticleAccessor.Index == 0)
            {
                Main_Label_Forward.Visible = true;
                Main_Label_Backward.Visible = false;
            }
            else
            {
                Main_Label_Backward.Visible = true;
                Main_Label_Forward.Visible = true;
            }

            /// Convert unix time into date time
            Main_Label_Head_Line_Date.Text = "ON " + Common.UnixTimestampToDateTime(l_Article.Date).ToShortDateString().ToString();

            /// Align the labels
            /// Don't mess around with the values if you don't know what you're doing
            Main_Label_Username.Location = new Point(Main_Label_Welcome_Back.Right - Common.Space, Main_Label_Welcome_Back.Location.Y);
            Main_Label_Exclamation.Location = new Point(Main_Label_Username.Right - Common.Space, Main_Label_Username.Location.Y);
            Main_Label_Arrow.Location = new Point(Main_Label_Exclamation.Right, Main_Label_Arrow.Location.Y);
            Main_Label_Account_Panel.Location = new Point(Main_Label_Arrow.Right, Main_Label_Account_Panel.Location.Y);
            Main_Label_Split.Location = new Point(Main_Label_Account_Panel.Right, Main_Label_Exclamation.Location.Y);
            Main_Label_Logout.Location = new Point(Main_Label_Split.Right, Main_Label_Account_Panel.Location.Y);
            Main_Label_Split_Head_Line.Location = new Point(Main_Label_Head_Line.Right, Main_Label_Split_Head_Line.Location.Y);
            Main_Label_Head_Line_By.Location = new Point(Main_Label_Split_Head_Line.Right, Main_Label_Head_Line_By.Location.Y);
            Main_Label_By_Username.Location = new Point(Main_Label_Head_Line_By.Right, Main_Label_By_Username.Location.Y);
            Main_Label_Head_Line_Date.Location = new Point(Main_Label_By_Username.Right -Common.Space, Main_Label_Head_Line_Date.Location.Y);

            /// Add a line break if the sentence exceeds 120 characters, so we don't go over our content box
            StringBuilder l_StringBuilder = new StringBuilder(l_Article.Content);
            for (int l_I = 20; l_I < l_Article.Content.Length; l_I++)
            {
                if (l_I % 120 == 0)
                {
                    /// If the character is not a space, then it's a word, add a upper line
                    if (l_StringBuilder[l_I - 1] != ' ' && l_StringBuilder[l_I - 1] != '.' && l_StringBuilder[l_I - 1] != ','
                        && l_StringBuilder[l_I + 2] != ' ')
                        l_StringBuilder.Insert(l_I - 1, '-');
             
                    l_StringBuilder.Insert(l_I, '\n');
                }

            }

            Main_Label_Content.Text = l_StringBuilder.ToString();

            /// Send update launcher only once on this function,
            /// it's better we do it this way instead hovering around waiting for the form to finish loading
            if (m_InitializeOnce)
            {
                SendLauncherUpdate();
                m_InitializeOnce = false;
            }
        }

        /// This section here handles the colour changing of the labels when user hovers and exits the label

        /// <summary>
        /// Open webpage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Label_Home_MouseDown(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.serenity-wow.com/");
        }

        /// <summary>
        /// Open webpage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Label_Discord_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://discord.gg/CDh3Bq");
        }

        /// <summary>
        /// Open WebBrowser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Label_Armory_Click(object sender, EventArgs e)
        {
            m_WebBrowser.GoToArmory();
        }

        /// <summary>
        /// Open webpage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Label_Rating_Click(object sender, EventArgs e)
        {
            m_WebBrowser.GoToRating();
        }

        /// <summary>
        /// Open webpage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Label_Bug_Tracker_Click(object sender, EventArgs e)
        {
            m_WebBrowser.GoToBugTracker();
        }
            
        /// <summary>
        /// Show account panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Label_Account_Panel_Click(object sender, EventArgs e)
        {
            m_WebBrowser.GoToAccountPanel();
        }

        /// <summary>
        /// Leave the main form and show login form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Label_Logout_Click(object sender, EventArgs e)
        {
            MainEntry.GetInstance.ShowLogin();
        }

        /// <summary>
        /// Change colour when cursor hovers over
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Label_Home_MouseEnter(object sender, EventArgs e)
        {
            Main_Label_Home.ForeColor = Color.FromArgb(224, 219, 210);
        }

        /// <summary>
        /// Change colour when cursor hovers over
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Label_Discord_MouseEnter(object sender, EventArgs e)
        {
            Main_Label_Discord.ForeColor = Color.FromArgb(224, 219, 210);
        }

        /// <summary>
        /// Change colour when cursor hovers over
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Label_FAQ_MouseEnter(object sender, EventArgs e)
        {
            Main_Label_Armory.ForeColor = Color.FromArgb(224, 219, 210);
        }

        /// <summary>
        /// Change colour when cursor hovers over
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Label_Change_Log_MouseEnter(object sender, EventArgs e)
        {
            Main_Label_Rating.ForeColor = Color.FromArgb(224, 219, 210);
        }

        /// <summary>
        /// Change colour when cursor hovers over
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Label_Bug_Tracker_MouseEnter(object sender, EventArgs e)
        {
            Main_Label_Bug_Tracker.ForeColor = Color.FromArgb(224, 219, 210);
        }

        /// <summary>
        /// Change colour when cursor hovers over
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Label_Account_Panel_MouseEnter(object sender, EventArgs e)
        {
            Main_Label_Account_Panel.ForeColor = Color.FromArgb(224, 219, 210);
        }

        /// <summary>
        /// Change colour when cursor hovers over
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Label_Logout_MouseEnter(object sender, EventArgs e)
        {
            Main_Label_Logout.ForeColor = Color.FromArgb(224, 219, 210);
        }

        /// <summary>
        /// Change back to original colour when cursor leaves
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Label_Home_MouseLeave(object sender, EventArgs e)
        {
            Main_Label_Home.ForeColor = Color.FromArgb(107, 106, 104);
        }

        /// <summary>
        /// Change back to original colour when cursor leaves
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Label_Discord_MouseLeave(object sender, EventArgs e)
        {
            Main_Label_Discord.ForeColor = Color.FromArgb(107, 106, 104);
        }

        /// <summary>
        /// Change back to original colour when cursor leaves
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Label_FAQ_MouseLeave(object sender, EventArgs e)
        {
            Main_Label_Armory.ForeColor = Color.FromArgb(107, 106, 104);
        }

        /// <summary>
        /// Open settings form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Label_Settings_Click(object sender, EventArgs e)
        {
            m_Settings.ShowDialog();
        }

        /// <summary>
        /// Change back to original colour when cursor leaves
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Label_Change_Log_MouseLeave(object sender, EventArgs e)
        {
            Main_Label_Rating.ForeColor = Color.FromArgb(107, 106, 104);
        }

        /// <summary>
        /// Change back to original colour when cursor leaves
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Label_Bug_Tracker_MouseLeave(object sender, EventArgs e)
        {
            Main_Label_Bug_Tracker.ForeColor = Color.FromArgb(107, 106, 104);
        }

        /// <summary>
        /// Change back to original colour when cursor leaves
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Label_Account_Panel_MouseLeave(object sender, EventArgs e)
        {
            Main_Label_Account_Panel.ForeColor = Color.FromArgb(107, 106, 104);
        }

        /// <summary>
        /// Change back to original colour when cursor leaves
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Label_Logout_MouseLeave(object sender, EventArgs e)
        {
            Main_Label_Logout.ForeColor = Color.FromArgb(107, 106, 104);
        }

        /// <summary>
        /// Change colour when cursor hovers over
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Picture_Box_Vote_MouseEnter(object sender, EventArgs e)
        {
            Main_Picture_Box_Vote.Visible = false;
            Main_Picture_Box_Vote_Hover.Visible = true;
        }

        /// <summary>
        /// Change back to original colour when cursor leaves
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Picture_Box_Vote_Hover_MouseLeave(object sender, EventArgs e)
        {
            Main_Picture_Box_Vote.Visible = true;
            Main_Picture_Box_Vote_Hover.Visible = false;
        }

        /// <summary>
        /// Change colour when cursor hovers over
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Picture_Box_Discord_MouseEnter(object sender, EventArgs e)
        {
            Main_Picture_Box_Discord.Visible = false;
            Main_Picture_Box_Discord_Hover.Visible = true;
        }

        /// <summary>
        /// Change back to original colour when cursor leaves
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Picture_Box_Discord_Hover_MouseLeave(object sender, EventArgs e)
        {
            Main_Picture_Box_Discord.Visible = true;
            Main_Picture_Box_Discord_Hover.Visible = false;
        }

        /// <summary>
        /// Change colour when cursor hovers over
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Picture_Box_Donate_MouseEnter(object sender, EventArgs e)
        {
            Main_Picture_Box_Donate.Visible = false;
            Main_Picture_Box_Donate_Hover.Visible = true;
        }

        /// <summary>
        /// Change back to original colour when cursor leaves
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Picture_Box_Donate_Hover_MouseLeave(object sender, EventArgs e)
        {
            Main_Picture_Box_Donate.Visible = true;
            Main_Picture_Box_Donate_Hover.Visible = false;
        }
        #endregion

        #region PictureBox

        /// <summary>
        /// Enable hover version
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Picture_Box_Read_More_MouseEnter(object sender, EventArgs e)
        {
            Main_Picture_Box_Read_More.Visible = false;
            Main_Picture_Box_Read_More_Hover.Visible = true;
        }

        /// <summary>
        /// Enable non-hover version
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Picture_Box_Read_More_Hover_MouseLeave(object sender, EventArgs e)
        {
            Main_Picture_Box_Read_More.Visible = true;
            Main_Picture_Box_Read_More_Hover.Visible = false;
        }

        /// <summary>
        /// Enable hover version
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Picture_Box_Changelog_MouseEnter(object sender, EventArgs e)
        {
            Main_Picture_Box_Changelog.Visible = false;
            Main_Picture_Box_Changelog_Hover.Visible = true;
        }

        /// <summary>
        /// Enable non-hover version
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Picture_Box_Changelog_Hover_MouseLeave(object sender, EventArgs e)
        {
            Main_Picture_Box_Changelog.Visible = true;
            Main_Picture_Box_Changelog_Hover.Visible = false;
        }

        /// <summary>
        /// Opens webbrowser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Picture_Box_Vote_Hover_MouseDown(object sender, MouseEventArgs e)
        {
            m_WebBrowser.GoToVotePanel();
        }

        /// <summary>
        /// Opens WebBrowser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Picture_Box_Donate_Hover_MouseDown(object sender, MouseEventArgs e)
        {
            m_WebBrowser.GoToDonatePanel();
        }

        /// <summary>
        /// Open discord
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Picture_Box_Discord_Hover_MouseDown(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Process.Start("https://discord.gg/JFAKer");
        }

        #endregion

        #region Article

        /// <summary>
        /// Go to next article
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Label_Forward_Click(object sender, EventArgs e)
        {
            ArticleAccessor.Index++;

            /// Index should never be higher than article count
            if (ArticleAccessor.Index + Common.ARRAY_OFFSET > ArticleAccessor.Articles.Count())
                ArticleAccessor.Index = ArticleAccessor.Articles.Count() - Common.ARRAY_OFFSET;

            /// Update launcher
            UpdateContent();
        }

        /// <summary>
        /// Go to previous article
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Label_Backward_Click(object sender, EventArgs e)
        {
            ArticleAccessor.Index--;

            /// Index should never be below 0
            if (ArticleAccessor.Index < 0)
                ArticleAccessor.Index = 0;

            /// Update launcher
            UpdateContent();
        }

        #endregion

        #region Update

        /// <summary>
        /// Update status box and notification
        /// </summary>
        /// <param name="p_Notification"></param>
        /// <param name="p_AccountRegistered"></param>
        /// <param name="p_Population"></param>
        public void UpdateStatus(string p_Notification, uint p_AccountRegistered, uint p_Population)
        {
            Main_Label_Notification.Text                = p_Notification;
            Main_Label_Accounts_Registered_Dynamic.Text = p_AccountRegistered.ToString();
            Main_Label_Players_Online_Dynamic.Text      = p_Population.ToString();
        }

        /// <summary>
        /// Update status info
        /// </summary>
        private void SendLauncherUpdate()
        {
            using (TcpClient l_TCPClient = new TcpClient())
            {
                try
                {
                    l_TCPClient.Connect("151.228.138.247", 8085);
                    Main_Label_Online_Status.Text = "Online";
                }
                catch (Exception)
                {
                    Main_Label_Online_Status.Text = "Offline";
                }
            }

            /// Get update status
            ClientPacket l_ClientPacket = new ClientPacket();
            l_ClientPacket.AppendInterger(Common.CLIENT_LAUNCHER_UPDATE);
            l_ClientPacket.AppendSOH();
            MainEntry.GetInstance.SendPacket(l_ClientPacket.GetData());
        }

        /// <summary>
        /// Timer on when to execute online status checker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Timer_Status_Checker_Tick(object sender, EventArgs e)
        {
            SendLauncherUpdate();
        }

        #endregion

        #region Panel
        /// <summary>
        /// Allow to move the Main form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Panel_Mover_MouseDown(object sender, MouseEventArgs e)
        {
            m_CanMove = true;
            m_X = e.X;
            m_Y = e.Y;
        }

        /// <summary>
        /// Move the Main move to cursor location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Panel_Mover_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_CanMove)
                this.SetDesktopLocation(MousePosition.X - m_X, MousePosition.Y - m_Y);
        }

        /// <summary>
        /// Set to no longer can move Main form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Panel_Mover_MouseUp(object sender, MouseEventArgs e)
        {
            m_CanMove = false;
        }

        #endregion

        #region Website

        /// <summary>
        /// Login into the website
        /// </summary>
        public void Login()
        {
            m_WebBrowser.ClickLogin();
        }

        #endregion

        #region Close

        /// <summary>
        /// When cursor hovers over, it will activate the hover button effect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Picture_Box_Close_MouseEnter(object sender, EventArgs e)
        {
            Main_Picture_Box_Close.Visible = false;
            Main_Picture_Box_Close_Hover.Visible = true;
        }

        /// <summary>
        /// When cursor hovers over, it will activate the un-hover button effect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Picture_Box_Close_Hover_MouseLeave(object sender, EventArgs e)
        {
            Main_Picture_Box_Close.Visible = true;
            Main_Picture_Box_Close_Hover.Visible = false;
        }

        /// <summary>
        /// Exit the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Picture_Box_Close_Hover_MouseDown(object sender, MouseEventArgs e)
        {
            MainEntry.GetInstance.CloseApplication();
        }

        #endregion

        #region Minimise
        /// <summary>
        /// Enable hover version
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Picture_Box_Minimise_MouseEnter(object sender, EventArgs e)
        {
            Main_Picture_Box_Minimise.Visible = false;
            Main_Picture_Box_Minimise_Hover.Visible = true;
        }

        /// <summary>
        /// Enable non-hover version
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Picture_Box_Minimise_Hover_MouseLeave(object sender, EventArgs e)
        {
            Main_Picture_Box_Minimise.Visible = true;
            Main_Picture_Box_Minimise_Hover.Visible = false;
        }

        /// <summary>
        /// Minimise application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Picture_Box_Minimise_Hover_MouseDown(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        #endregion

        #region Config

        /// <summary>
        /// Check if directory exist and whether we need to show waiting button or play button
        /// </summary>
        public void DoesExecutableExist()
        {
            if (File.Exists(m_Config.GetDirectory()))
            {
                Main_Picture_Box_Play.Visible = true;
                Main_Picture_Box_Waiting_Play.Visible = false;
                Main_Panel_Downloader.Visible = false;
            }
            else
            {
                Main_Picture_Box_Play.Visible = false;
                Main_Picture_Box_Waiting_Play.Visible = true;
                Main_Panel_Downloader.Visible = false;
            }
        }

        #endregion

        #region Downloader

        /// <summary>
        /// Start a background worker thread to download the client(s)
        /// </summary>
        public void PrepareDownloadClient()
        {
            /// If thread is already working then don't do anything and let worker complete its course
            if (!Main_Background_Thread_Zip.IsBusy)
            {
                /// Show download
                Main_Panel_Downloader.Visible = true;

                Main_Background_Thread_Zip.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Download Serenity WoW clients
        /// </summary>
        private void DownloadClient(object p_Sender, DoWorkEventArgs p_WorkEvent)
        {
            BackgroundWorker l_Worker = p_Sender as BackgroundWorker;

            /// Initialize Zip
            m_Zip = new Zip(ref l_Worker, ref p_WorkEvent);

            /// Create a new web client to download the client(s)
            WebClient l_Webclient = new WebClient();
            Stream l_Data = l_Webclient.OpenRead("http://151.228.138.247/Test.zip"); ///< This will be hard coded

            /// Now download the data
            m_Zip.UnzipFromStream(l_Data, m_Config.GetDirectory());
        }

        /// <summary>
        /// Update form on download progress
        /// </summary>
        /// <param name="p_Progress"></param>
        /// <param name="p_FileName"></param>
        public void UpdateDownloadProgress(double p_Progress, double p_TotalProgress, int p_CurrentBytes, int p_MaxBytes, string p_FileName, uint p_CurrentIndex, uint p_TotalIndex)
        {
            /// Update labels to current bytes
            Main_Picture_Box_Progress_Bar.Width = (int)(double)(p_Progress * 7.35);
            Main_Label_File_Name.Text = p_FileName;
            Main_Label_Current_Bytes.Text = p_CurrentBytes.ToString() + "MB";
            Main_Label_Total_Bytes.Text = "/ " + p_MaxBytes.ToString();
            Main_Label_Current_Progress_Percentage.Text = ((int)p_Progress).ToString() + "%";
            Main_Label_Current_Index.Text = p_CurrentIndex.ToString() + " /";
            Main_Label_Total_Index.Text = p_TotalIndex.ToString();
            Main_Label_Total_Progress_Percentage.Text = ((int)p_TotalProgress).ToString() + "%";

            /// Update label position so it looks fluently and clean
            Main_Label_Current_Index.Location = new Point(Main_Label_Downloading_File_Static.Right, Main_Label_Downloading_File_Static.Location.Y);
            Main_Label_Total_Index.Location = new Point(Main_Label_Current_Index.Right, Main_Label_Current_Index.Location.Y);
            Main_Label_Download_Splitter.Location = new Point(Main_Label_Total_Index.Right, Main_Label_Download_Splitter.Location.Y);
            Main_Label_File_Name.Location = new Point(Main_Label_Download_Splitter.Right, Main_Label_File_Name.Location.Y);
            Main_Label_Total_Bytes.Location = new Point(Main_Label_Current_Bytes.Right, Main_Label_Total_Bytes.Location.Y);
            Main_Label_Line_Download.Location = new Point(Main_Label_Total_Bytes.Right, Main_Label_Line_Download.Location.Y);
            Main_Label_Current_Progress_Percentage.Location = new Point(Main_Label_Line_Download.Right, Main_Label_Current_Progress_Percentage.Location.Y);
        }

        /// <summary>
        /// Pause background worker thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Picture_Box_Pause_Download_Click(object sender, EventArgs e)
        {
            SetPause(true);
            Main_Picture_Box_Resume_Download.Visible = true;
            Main_Picture_Box_Pause_Download.Visible = false;
        }

        /// <summary>
        /// Resume background worker thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Picture_Box_Resume_Download_Click(object sender, EventArgs e)
        {
            SetPause(false);
            Main_Picture_Box_Resume_Download.Visible = false;
            Main_Picture_Box_Pause_Download.Visible = true;
        }

        /// <summary>
        /// Determine whether the background worker thread should work or not
        /// </summary>
        /// <param name="p_Pause"></param>
        private void SetPause(bool p_Pause)
        {
            m_IsPaused = p_Pause;
        }

        /// <summary>
        /// Is the background worker thread meant to pause its operation?
        /// </summary>
        /// <returns></returns>
        public bool IsPaused()
        {
            return m_IsPaused;
        }

        /// <summary>
        /// This event handler deals with the results of the background operation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Background_Thread_Zip_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            /// Sucessfully downloaded all files, set our default application
            m_Config.SetDirectory(m_Config.GetDirectory() + "\\" + "SerenityClient-Normal-Models-x86.exe");
            DoesExecutableExist();
        }

        /// <summary>
        /// Launch application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Picture_Box_Play_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(m_Config.GetDirectory());
            }
            catch
            {
                MessageBox.Show("Failure in booting up Client! Please make sure your directory is correct!",
                            "Error #0006s",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                           );
            }
        }

        #endregion

        #region Variables

        /// <summary>
        /// Private variable
        /// </summary>
        private bool m_CanMove;
        private int m_X;
        private int m_Y;

        private bool m_InitializeOnce = true; ///< Used to send launcher update once (then timer will handle sending launcher update on interval events)
        private WebBrowser.WebBrowser m_WebBrowser = new WebBrowser.WebBrowser();
        private SettingsBin.Settings m_Settings = new SettingsBin.Settings();
        private Zip m_Zip;
        private Config m_Config = new Config();
        private bool m_IsPaused = false;
        #endregion
    }
}
