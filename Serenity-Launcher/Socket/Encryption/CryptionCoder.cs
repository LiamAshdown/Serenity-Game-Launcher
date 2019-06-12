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
    ///  This class is responsible for encoding and decoding data
    /// </summary>
    public class CryptionCoder : RC4
    {
        /// <summary>
        /// Encrypt the data to be sent to the server
        /// </summary>
        /// <param name="p_Data"></param>
        /// <returns></returns>
        public byte[] EncryptRC4(string p_Data)
        {
            SetData(p_Data);

            return (byte[])(Array)Encrypt();
        }

        /// <summary>
        /// Decrypt the data server sent
        /// </summary>
        /// <param name="p_Data"></param>
        /// <returns></returns>
        public byte[] DecryptRC4(string p_Data)
        {
            SetData(p_Data);

            return (byte[])(Array)Decrypt();
        }
    }
}
