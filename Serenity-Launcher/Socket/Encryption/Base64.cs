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

namespace SteerStone.Encryption
{
    /// <summary>
    /// Base 64 Encryption
    /// </summary>
    class Base64
    {
        /// <summary>
        /// Encrypt Base64 Interger
        /// </summary>
        /// <param name="p_Value"></param>
        /// <param name="p_Length"></param>
        /// <returns></returns>
        public string EncodeIntergerBase64(int p_Value, int p_Length)
        {
            string l_Stack = "";
            for (int l_X = 1; l_X <= p_Length; l_X++)
            {
                int l_Offset = 6 * (p_Length - l_X);
                byte l_Val = (byte)(64 + (p_Value >> l_Offset & 0x3f));
                l_Stack += (char)l_Val;
            }
            return l_Stack;
        }

        /// <summary>
        /// Encrypt Base64 String
        /// </summary>
        /// <param name="p_Data"></param>
        /// <returns></returns>
        public string EncodeStringBase64(string p_Data)
        {
            int p_Value = p_Data.Length;
            int p_Length = 2;
            string l_Stack = "";
            for (int l_X = 1; l_X <= p_Length; l_X++)
            {
                int l_Offset = 6 * (p_Length - l_X);
                byte l_Val = (byte)(64 + (p_Value >> l_Offset & 0x3f));
                l_Stack += (char)l_Val;
            }
            return l_Stack;
        }

        /// <summary>
        /// Decode our Base64
        /// </summary>
        /// <param name="p_Data"></param>
        /// <returns></returns>
        public int DecodeBase64(string p_Data)
        {
            char[] l_Val = p_Data.ToCharArray();
            int l_Data = 0;
            int l_Y = 0;
            for (int l_X = (p_Data.Length - 1); l_X >= 0; l_X--)
            {
                int l_DataTemp = (int)(byte)((l_Val[l_X] - 64));
                if (l_Y > 0)
                {
                    l_DataTemp = l_DataTemp * (int)(Math.Pow(64, l_Y));
                }
                l_Data += l_DataTemp;
                l_Y++;
            }
            return l_Data;
        }
    }
}
