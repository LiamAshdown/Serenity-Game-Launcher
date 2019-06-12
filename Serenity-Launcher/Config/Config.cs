using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System.Xml;

namespace SteerStone.ConfigHandler
{
    class Config
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Config()
        {

        }

        /// <summary>
        /// Check if the config file exists
        /// </summary>
        public void DoesConfigFileExist()
        {
            /// Does the file exist in directory where launcher is?
            if (File.Exists("Config.xml"))
            {
                m_Document = new XmlDocument();
                m_Document.Load("config.xml");
            }
            else
            {
                /// If not, then create a new config file
                m_XDocument = new XDocument(new XElement("config", new XElement("directory", "0"), new XElement("version", "32"), new XElement("failsafe", "0")));
                m_XDocument.Save("config.xml");
            }
        }

        /// <summary>
        /// Return directory
        /// </summary>
        /// <returns></returns>
        public string GetDirectory()
        {
            return m_Document.SelectSingleNode("config/directory").InnerText;
        }

        /// <summary>
        /// Return version
        /// </summary>
        /// <returns></returns>
        public string GetVersion()
        {
            return m_Document.SelectSingleNode("config/version").InnerText;
        }

        /// <summary>
        /// Return executable name
        /// </summary>
        /// <returns></returns>
        public string GetExecutableName()
        {
            return m_Document.SelectSingleNode("config/failsafe").InnerText;
        }

        /// <summary>
        /// Set new directory path
        /// </summary>
        /// <param name="p_Directory"></param>
        public void SetDirectory(string p_Directory)
        {
            XmlNode l_Node = m_Document.SelectSingleNode("config/directory");
            l_Node.InnerText = p_Directory;

            m_Document.Save("config.xml");
        }

        /// <summary>
        /// Set Version
        /// </summary>
        /// <param name="p_Version"></param>
        public void SetVersion(string p_Version)
        {
            XmlNode l_Node = m_Document.SelectSingleNode("config/version");
            l_Node.InnerText = p_Version;

            m_Document.Save("config.xml");
        }

        /// <summary>
        /// Set executable name
        /// </summary>
        /// <param name="p_Version"></param>
        public void SetExecutableName(string p_ExecutableName)
        {
            XmlNode l_Node = m_Document.SelectSingleNode("config/failsafe");
            l_Node.InnerText = p_ExecutableName;

            m_Document.Save("config.xml");
        }

        #region Variables
        private static XmlDocument m_Document;
        private static XDocument m_XDocument;
        #endregion
    }
}
