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
using System.Text;
using System.Threading.Tasks;
using SteerStone.Encryption;

namespace SteerStone.Packet
{
    /// <summary>
    /// Constructs a string buffer to be sent to server
    /// Uses base64 to encrypt our data
    /// </summary>
    class ClientPacket : Base64
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ClientPacket()
        {
            m_Byte = new byte[Common.BufferSize];
        }

        /// <summary>
        /// Add string into our buffer
        /// </summary>
        /// <param name="message"></param>
        public void AppendString(string p_Message, bool p_Delimeter = true)
        {
            string l_Message = EncodeStringBase64(p_Message);
            var l_Bytes = Encoding.Default.GetBytes(EncodeStringBase64(p_Message) + p_Message);
            m_Storage.AddRange(l_Bytes);

            if (p_Delimeter)
                AppendSOT();
        }

        /// <summary>
        /// Encode interger
        /// </summary>
        /// <param name="p_Value"></param>
        public void AppendInterger(int p_Value)
        {
            string l_Message = EncodeIntergerBase64(p_Value, sizeof(int));
            var l_Bytes = Encoding.Default.GetBytes(l_Message);
            m_Storage.AddRange(l_Bytes);
        }

        /// <summary>
        /// Append Start of Text after each string is appended
        /// </summary>
        public void AppendSOT()
        {
            string l_Message = EncodeStringBase64("\x2");
            var l_Bytes = Encoding.Default.GetBytes(l_Message);
            m_Storage.AddRange(l_Bytes);
        }

        /// <summary>
        /// Append Start of Text after each string is appended
        /// </summary>
        public void AppendSOH()
        {
            string l_Message = "\x1";
            var l_Bytes = Encoding.Default.GetBytes(l_Message);
            m_Storage.AddRange(l_Bytes);
        }

        /// <summary>
        /// Get Buffer Storage
        /// </summary>
        /// <returns></returns>
        public List<byte> GetStorage()
        {
            return m_Storage;
        }

        /// <summary>
        /// Get our data in our storage
        /// </summary>
        /// <returns></returns>
        public string GetData()
        {
            ASCIIEncoding l_ToString = new ASCIIEncoding();
            return l_ToString.GetString(m_Storage.ToArray());
        }

        /// <summary>
        /// Clear Buffer
        /// </summary>
        public void Clear()
        {
            Array.Clear(m_Byte, 0, m_Byte.Length);
            m_Storage.Clear();
        }

        /// <summary>
        /// Variables declarations
        /// </summary>
        private List<byte> m_Storage = new List<byte> { };
        private byte[] m_Byte;
    }
}
