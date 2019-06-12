using SteerStone.ConfigHandler;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SteerStone.Entry;

namespace SteerStone.SettingsBin
{
    public partial class Settings : Form
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
        public Settings()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10)); ///< Add curves around corners to fit with design

            /// Check if our config file exists
            m_Config.DoesConfigFileExist();

            /// Set our directory and version, 0 is our default value if user has not specified a directory
            if (m_Config.GetDirectory() == "0")
                Settings_Label_Directory.Text = "DIRECTORY NOT SET";
            else
                Settings_Label_Directory.Text = m_Config.GetDirectory().ToUpper();

            Settings_Label_Version.Text = m_Config.GetVersion();

            if (Settings_Label_Version.Text == "32")
            {
                Settings_Label_Version.Text = "32 Bit";
                Settings_Label_32_Bit.Visible = false;
            }
            else if (Settings_Label_Version.Text == "64")
            {
                Settings_Label_Version.Text = "64 Bit";
                Settings_Label_64_Bit.Visible = false;
            }

            /// Check if executable name is serenity or not
            if (m_Config.GetExecutableName() != "SerenityClient-Normal-Models-x86.exe"  &&
                m_Config.GetExecutableName() != "SerenityClient-WoD-Models-x64.exe"     &&
                m_Config.GetExecutableName() != "SerenityClient-Normal-Models-x64.exe"  &&
                m_Config.GetExecutableName() != "SerenityClient-WoD-Models-x86.exe"     &&
                m_Config.GetExecutableName() != "0") ///< Default value
            {
                Settings_Label_Error.Text = "You've not selected Serenity-WoW client, this may affect your playing experience.\n It's recommended to download our client.";
                Settings_Label_Error.Visible = true;
            }

            /// Save our current version settings
            m_TempVersion = Settings_Label_Version.Text;
        }

        #region Panel

        /// <summary>
        /// Allow to move the Setting form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Panel_Mover_MouseDown(object sender, MouseEventArgs e)
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
        private void Settings_Panel_Mover_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_CanMove)
                this.SetDesktopLocation(MousePosition.X - m_X, MousePosition.Y - m_Y);
        }

        /// <summary>
        /// Set to no longer can move Settings form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Panel_Mover_MouseUp(object sender, MouseEventArgs e)
        {
            m_CanMove = false;
        }

        #endregion

        #region Directory

        /// <summary>
        /// Set new directory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Picture_Box_Search_Icon_Click(object sender, EventArgs e)
        {
            OpenFileDialog l_Folder = new OpenFileDialog();
            l_Folder.Filter = "Exe Files (.exe)|*.exe";
            l_Folder.FilterIndex = 1;
            l_Folder.Multiselect = true;

            if (l_Folder.ShowDialog() == DialogResult.OK)
            {
                m_TempDirectory = l_Folder.FileName;
                m_TempVersion = Settings_Label_Version.Text;
                m_TempExecutableName = l_Folder.SafeFileName;

                Settings_Label_Directory.Text = m_TempDirectory.ToUpper();

                /// Check if executable name is serenity or not
                if (m_TempExecutableName != "SerenityClient-Normal-Models-x86.exe"  &&
                    m_TempExecutableName != "SerenityClient-WoD-Models-x64.exe"     &&
                    m_TempExecutableName != "SerenityClient-Normal-Models-x64.exe"  &&
                    m_TempExecutableName != "SerenityClient-WoD-Models-x86.exe")
                {
                    Settings_Label_Error.Text = "You've not selected Serenity-WoW client, this may affect your playing experience.\n It's recommended to download our client.";
                    Settings_Label_Error.Visible = true;
                }
                else
                {
                    Settings_Label_Error.Visible = false;
                    m_Config.SetDirectory(m_TempDirectory);
                    m_Config.SetVersion(m_TempVersion);
                    m_Config.SetExecutableName(m_TempExecutableName);
                }
            }
            else
            {
                MessageBox.Show("Please locate your World of Warcraft Client!",
                "Error #0004",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
               );
            }
        }

        #endregion

        #region PictureBox

        /// <summary>
        /// Switch to hover button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Picture_Box_Save_Settings_MouseEnter(object sender, EventArgs e)
        {
            Settings_Picture_Box_Save_Settings.Visible = false;
            Settings_Picture_Box_Save_Settings_Hover.Visible = true;
        }

        /// <summary>
        /// Switch to non hover button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Picture_Box_Save_Settings_Hover_MouseLeave(object sender, EventArgs e)
        {
            Settings_Picture_Box_Save_Settings.Visible = true;
            Settings_Picture_Box_Save_Settings_Hover.Visible = false;
        }

        /// <summary>
        /// Save our settings to xml config file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Picture_Box_Save_Settings_Hover_Click(object sender, EventArgs e)
        {
            m_Config.SetDirectory(m_TempDirectory);
            m_Config.SetExecutableName(m_TempExecutableName);
            m_Config.SetVersion(Settings_Label_Version.Text);
            m_TempVersion = Settings_Label_Version.Text;

            MainEntry.GetInstance.GetMain().DoesExecutableExist();
            Hide();
        }

        /// <summary>
        /// Switch to hover button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Picture_Box_Cancel_MouseEnter(object sender, EventArgs e)
        {
            Settings_Picture_Box_Cancel.Visible = false;
            Settings_Picture_Box_Cancel_Hover.Visible = true;
        }

        /// <summary>
        /// Switch to non hover button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Picture_Box_Cancel_Hover_MouseLeave(object sender, EventArgs e)
        {
            Settings_Picture_Box_Cancel.Visible = true;
            Settings_Picture_Box_Cancel_Hover.Visible = false;
        }

        /// <summary>
        /// Set version
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Label_64_Bit_Click(object sender, EventArgs e)
        {
            Settings_Label_Version.Text = "64 Bit";
            Settings_Label_64_Bit.Visible = false;
            Settings_Label_32_Bit.Visible = true;
            Settings_Timer_Drop_Down_Menu.Start();
        }

        /// <summary>
        /// Reset all settings back to default
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Label_Reset_Settings_Click(object sender, EventArgs e)
        {
            m_Config.SetDirectory("0");
            m_Config.SetVersion("32");
            m_Config.SetExecutableName("0");

            Settings_Label_Version.Text = "32 Bit";
            Settings_Label_Directory.Text = "DIRECTORY NOT SET";
            Settings_Label_Error.Visible = false;
            m_TempDirectory = "DIRECTORY NOT SET";
            m_TempExecutableName = "0";
            m_TempVersion = "32 Bit";

            MainEntry.GetInstance.GetMain().DoesExecutableExist();
        }

        #endregion

        #region DropDownMenu

        /// <summary>
        /// Execute drop down menu animation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Timer_Drop_Down_Menu_Tick(object sender, EventArgs e)
        {
            if (m_IsCollapsed)
            {
                Settings_Panel_Drop_Down_Menu.Height += 10;
                if (Settings_Panel_Drop_Down_Menu.Height >= 60)
                {
                    Settings_Timer_Drop_Down_Menu.Stop();
                    Settings_Panel_Drop_Down_Menu.Height = 60;
                    m_IsCollapsed = false;
                }
            }
            else
            {
                Settings_Panel_Drop_Down_Menu.Height -= 10;
                if (Settings_Panel_Drop_Down_Menu.Height <= 20)
                {
                    Settings_Timer_Drop_Down_Menu.Stop();
                    Settings_Panel_Drop_Down_Menu.Height = 22;
                    m_IsCollapsed = true;
                }
            }
        }

        /// <summary>
        /// Start drop down animation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Picture_Box_Drop_Down_Button_Click(object sender, EventArgs e)
        {
            Settings_Timer_Drop_Down_Menu.Start();
        }

        /// <summary>
        /// Close form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Picture_Box_Cancel_Hover_Click(object sender, EventArgs e)
        {
            Hide();
        }

        /// <summary>
        /// Change colour when cursor hovers over
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Label_Version_MouseEnter(object sender, EventArgs e)
        {
            Settings_Label_Version.ForeColor = Color.FromArgb(224, 219, 210);
        }

        /// <summary>
        /// Revert back to original colour when cursor leaves label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Label_Version_MouseLeave(object sender, EventArgs e)
        {
            Settings_Label_Version.ForeColor = Color.FromArgb(126, 123, 118);
        }

        /// <summary>
        /// Change colour when cursor hovers over
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Label_64_Bit_MouseEnter(object sender, EventArgs e)
        {
            Settings_Label_64_Bit.ForeColor = Color.FromArgb(224, 219, 210);
        }

        /// <summary>
        /// Revert back to original colour when cursor leaves label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Label_64_Bit_MouseLeave(object sender, EventArgs e)
        {
            Settings_Label_64_Bit.ForeColor = Color.FromArgb(126, 123, 118);
        }

        /// <summary>
        /// Change colour when cursor hovers over
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Label_32_Bit_MouseEnter(object sender, EventArgs e)
        {
            Settings_Label_32_Bit.ForeColor = Color.FromArgb(224, 219, 210);
        }

        /// <summary>
        /// Change colour when cursor hovers over
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Label_Reset_Settings_MouseEnter(object sender, EventArgs e)
        {
            Settings_Label_Reset_Settings.ForeColor = Color.FromArgb(224, 219, 210);
        }


        /// <summary>
        /// Revert back to original colour when cursor leaves label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Label_Reset_Settings_MouseLeave(object sender, EventArgs e)
        {
            Settings_Label_Reset_Settings.ForeColor = Color.FromArgb(126, 123, 118);
        }

        /// <summary>
        /// Revert back to original colour when cursor leaves label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Label_32_Bit_MouseLeave(object sender, EventArgs e)
        {
            Settings_Label_32_Bit.ForeColor = Color.FromArgb(126, 123, 118);
        }

        /// <summary>
        /// Set version
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Label_32_Bit_Click(object sender, EventArgs e)
        {
            Settings_Label_Version.Text = "32 Bit";
            Settings_Label_64_Bit.Visible = true;
            Settings_Label_32_Bit.Visible = false;
            Settings_Timer_Drop_Down_Menu.Start();
        }

        /// <summary>
        /// Start drop down menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Label_Version_Click(object sender, EventArgs e)
        {
            Settings_Timer_Drop_Down_Menu.Start();
        }

        #endregion

        #region Close
        /// <summary>
        /// Activate hover button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Picture_Box_Close_MouseEnter(object sender, EventArgs e)
        {
            Settings_Picture_Box_Close.Visible = false;
            Settings_Picture_Box_Close_Hover.Visible = true;
        }

        /// <summary>
        /// De activate hover button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Picture_Box_Close_Hover_MouseLeave(object sender, EventArgs e)
        {
            Settings_Picture_Box_Close.Visible = true;
            Settings_Picture_Box_Close_Hover.Visible = false;
        }

        /// <summary>
        /// Close settings form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Picture_Box_Close_Hover_Click(object sender, EventArgs e)
        {
            Hide();
        }

        #endregion

        #region Variables
        /// <summary>
        /// Private variable
        /// </summary>
        private bool m_CanMove;
        private int m_X;
        private int m_Y;

        private bool m_IsCollapsed = true;

        /// <summary>
        /// This is used to hold the new selected directory till user cancels or save settings
        /// </summary>
        private string m_TempDirectory;
        private string m_TempVersion;
        private string m_TempExecutableName;

        private Config m_Config = new Config();
        #endregion

        /// <summary>
        /// Show directory to download the serenity client
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Label_Description_Download_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog l_Folder = new FolderBrowserDialog();
            l_Folder.Description = "Please choose a directory to download Serenity-WoW Client";

            DialogResult l_Result = l_Folder.ShowDialog();

            if (l_Result == DialogResult.OK)
            {
                m_Config.SetDirectory(l_Folder.SelectedPath);
                m_Config.SetExecutableName("SerenityClient-Normal-Models-x86.exe"); ///< Default option
                m_Config.SetVersion("32 Bit");

                Hide();
                MainEntry.GetInstance.GetMain().PrepareDownloadClient();
            }
            else
            {
                MessageBox.Show("Error in choosing directory! Please try again!",
                "Error #0005",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
               );
            }
        }
    }
}
