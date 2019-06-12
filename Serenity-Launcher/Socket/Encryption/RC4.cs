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

namespace SteerStone.Encryption
{
    /// <summary>
    /// RC4 Encryptor
    /// </summary>
    public class RC4
    {
        /// <summary>
        /// Set our data which will be decrypted or encrypted
        /// </summary>
        /// <param name="p_Data"></param>
        protected void SetData(string p_Data)
        {
            m_Data = new sbyte[p_Data.Length];
            m_Data = (sbyte[])(Array)Encoding.Default.GetBytes(p_Data);
        }

        /// <summary>
        /// Initialize KSA Encryption
        /// </summary>
        protected void Initialize()
        {
            int l_I = 0;
            for (l_I = 0; l_I < 256; l_I++)
                m_S[l_I] = (char)l_I;

            byte[] l_K = new byte[256];
            for (l_I = 0; l_I < 256; l_I++)
                l_K[l_I] = 0;

            int l_J = 0;
            char l_Temp = '0';
            for (l_I = 0; l_I < 256; l_I++)
            {
                l_J = (l_J + m_S[l_I] + l_K[l_I] + Common.SecretKey[l_I % Common.SecretKey.Length]) % 256;
                l_Temp = m_S[l_I];
                m_S[l_I] = m_S[l_J];
                m_S[l_J] = l_Temp;
            }
        }

        /// <summary>
        /// Decrypt data
        /// </summary>
        /// <param name="p_Data"></param>
        /// <returns></returns>
        protected sbyte[] Decrypt()
        {
            Initialize();

            int l_I = 0;
            int l_J = 0;
            int l_Output = 0;
            char l_Temp = '0';
            for (int l_K = 0; l_K < m_Data.Length; l_K++)
            {
                l_I = (l_I + 1) % 256;
                l_J = (l_J + m_S[l_I]) % 256;

                l_Temp = m_S[l_I];
                m_S[l_I] = m_S[l_J];
                m_S[l_J] = l_Temp;

                l_Output = (m_S[l_I] + m_S[l_J]) % 256;
                m_Data[l_K] ^= (sbyte)m_S[l_Output];
            }

            return m_Data;
        }

        /// <summary>
        /// Encrypt data
        /// </summary>
        /// <param name="p_Data"></param>
        /// <returns></returns>
        protected sbyte[] Encrypt()
        {
            Initialize();

            int l_I = 0;
            int l_J = 0;
            int l_Output = 0;
            char l_Temp = '0';
            for (int l_K = 0; l_K < m_Data.Length; l_K++)
            {
                l_I = (l_I + 1) % 256;
                l_J = (l_J + m_S[l_I]) % 256;

                l_Temp = m_S[l_I];
                m_S[l_I] = m_S[l_J];
                m_S[l_J] = l_Temp;

                l_Output = (m_S[l_I] + m_S[l_J]) % 256;
                m_Data[l_K] ^= (sbyte)m_S[l_Output];
            }

            return m_Data;
        }

        /// <summary>
        /// Variables declarations
        /// </summary>
        private sbyte[] m_Data;
        private char[] m_S = new char[256];
    }
}
