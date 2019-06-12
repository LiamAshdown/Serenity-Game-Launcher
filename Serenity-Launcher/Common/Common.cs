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

namespace SteerStone
{
    /// <summary>
    ///  Auth Results when attempting to login server
    /// </summary>
    enum AuthResult
    {
        WOW_SUCCESS                                  = 0x00,
        WOW_FAIL_BANNED                              = 0x03,
        WOW_FAIL_UNKNOWN_ACCOUNT                     = 0x04,
        WOW_FAIL_INCORRECT_PASSWORD                  = 0x05,
        WOW_FAIL_ALREADY_ONLINE                      = 0x06,
        WOW_FAIL_NO_TIME                             = 0x07,
        WOW_FAIL_DB_BUSY                             = 0x08,
        WOW_FAIL_VERSION_INVALID                     = 0x09,
        WOW_FAIL_VERSION_UPDATE                      = 0x0A,
        WOW_FAIL_INVALID_SERVER                      = 0x0B,
        WOW_FAIL_SUSPENDED                           = 0x0C,
        WOW_FAIL_FAIL_NOACCESS                       = 0x0D,
        WOW_SUCCESS_SURVEY                           = 0x0E,
        WOW_FAIL_PARENTCONTROL                       = 0x0F,
        WOW_FAIL_LOCKED_ENFORCED                     = 0x10,
        WOW_FAIL_TRIAL_ENDED                         = 0x11,
        WOW_FAIL_USE_BATTLENET                       = 0x12,
        WOW_FAIL_ANTI_INDULGENCE                     = 0x13,
        WOW_FAIL_EXPIRED                             = 0x14,
        WOW_FAIL_NO_GAME_ACCOUNT                     = 0x15,
        WOW_FAIL_CHARGEBACK                          = 0x16,
        WOW_FAIL_INTERNET_GAME_ROOM_WITHOUT_BNET     = 0x17,
        WOW_FAIL_GAME_ACCOUNT_LOCKED                 = 0x18,
        WOW_FAIL_UNLOCKABLE_LOCK                     = 0x19,
        WOW_FAIL_CONVERSION_REQUIRED                 = 0x20,
        WOW_FAIL_DISCONNECTED                        = 0xFF,
    };

    /// <summary>
    /// Class which holds global variables
    /// </summary>
    public static class Common
    {
        /// <summary>
        /// Size of our buffer to read
        /// </summary>
        public const int BufferSize = 4096;

        /// <summary>
        /// Used for space between labers
        /// </summary>
        public const int Space = 5;

        /// <summary>
        /// Array size start at 1, but arrays start at 0...
        /// </summary>
        public const int ARRAY_OFFSET = 1;

        /// <summary>
        /// Secret key to decrypt RC4 data
        /// </summary>
        public const string SecretKey = "XGZH2J4M5N6Q8R9SBUCVDXFYGZJ3K4M6P7Q8SATBUCWEXFYH2J3K5N6P7R";

        /// <summary>
        /// Max Header Id we can accept
        /// </summary>
        public const uint MaxHeaderId = 200;

        /// <summary>
        /// Sends Login details to server
        /// </summary>
        public const int CLIENT_LAUNCHER_LOGIN = 0x35;

        /// <summary>
        /// Get launcher information
        /// </summary>
        public const int CLIENT_LAUNCHER_INFO = 0x36;

        /// <summary>
        /// Get launcher notification
        /// </summary>
        public const int CLIENT_LAUNCHER_UPDATE = 0x37;

        /// <summary>
        /// Error in logging into server
        /// </summary>
        public const int SERVER_LAUNCHER_LOGIN = 0x50;
        
        /// <summary>
        /// Get server information; e.g articles
        /// </summary>
        public const int SERVER_LAUNCHER_INFO = 0x51;

        /// <summary>
        /// Get server notification
        /// </summary>
        public const int SERVER_LAUNCHER_UPDATE = 0x52;

        /// <summary>
        /// Convert unix time into date time
        /// </summary>
        /// <param name="unixTime"></param>
        /// <returns></returns>
        public static DateTime UnixTimestampToDateTime(double p_UnixTime)
        {
            DateTime l_UnixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            long unixTimeStampInTicks = (long)(p_UnixTime * TimeSpan.TicksPerSecond);
            return new DateTime(l_UnixStart.Ticks + unixTimeStampInTicks, System.DateTimeKind.Utc);
        }
    }
}
