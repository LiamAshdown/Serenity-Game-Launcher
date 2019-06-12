using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteerStone.Encryption;

namespace SteerStone.Packet
{
    class ServerPacket : Base64
    {
        /// <summary>
        ///  Constructor
        /// </summary>
        /// <param name="p_Contents"></param>
        public ServerPacket(string p_Contents)
        {
            m_Content = p_Contents;
            m_Header = ReadBase64Int();
        }

        /// <summary>
        /// Returns decoded string
        /// </summary>
        /// <returns></returns>
        public string ReadString()
        {
            int l_Length = DecodeBase64(m_Content.Substring(m_ReadPosition, 2));
            string l_Temp = m_Content.Substring(2 + m_ReadPosition, l_Length);
            m_ReadPosition += (l_Length + 2);

            return l_Temp;
        }

        /// <summary>
        /// Returns decoded signed int
        /// </summary>
        /// <returns></returns>
        public Int32 ReadBase64Int()
        {
            int l_Length = DecodeBase64(m_Content.Substring(m_ReadPosition, 2));
            string l_Temp = m_Content.Substring(2 + m_ReadPosition, l_Length);
            m_ReadPosition += (l_Length + 2);

            return Convert.ToInt32(l_Temp);
        }

        /// <summary>
        /// Returns decoded unsigned int
        /// </summary>
        /// <returns></returns>
        public UInt32 ReadBase64Uint()
        {
            int l_Length = DecodeBase64(m_Content.Substring(m_ReadPosition, 2));
            string l_Temp = m_Content.Substring(2 + m_ReadPosition, l_Length);
            m_ReadPosition += (l_Length + 2);

            return Convert.ToUInt32(l_Temp);
        }

        /// <summary>
        /// Returns decoded bool
        /// </summary>
        /// <returns></returns>
        public bool ReadBase64Bool()
        {
            int l_Length = DecodeBase64(m_Content.Substring(m_ReadPosition, 2));
            string l_Temp = m_Content.Substring(2 + m_ReadPosition, l_Length);
            m_ReadPosition += (l_Length + 2);

            if (l_Temp == "1")
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns packet Id
        /// </summary>
        /// <returns></returns>
        public int GetHeader()
        {
            return m_Header;
        }

        /// <summary>
        /// Variables
        /// </summary>
        private int m_ReadPosition = 0;
        private string m_Content;
        private int m_Header;
    }
}
