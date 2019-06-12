using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteerStone.AccountInfo
{
    enum AccountTypes
    {
        SEC_PLAYER              = 0,
        SEC_VIP                 = 1,
        SEC_MODERATOR           = 2,
        SEC_GAMEMASTER          = 3,
        SEC_HEAD_GAMEMASTER     = 4,
        SEC_ADMINISTRATOR       = 5,
    }

    public class Account
    {

        /// <summary>
        /// Returns account username
        /// </summary>
        /// <returns></returns>
        public static string GetUsername()
        {
            return m_Username;
        }

        /// <summary>
        /// Returns account password
        /// </summary>
        /// <returns></returns>
        public static string GetPassword()
        {
            return m_Password;
        }

        /// <summary>
        /// Sets account username
        /// </summary>
        /// <param name="p_UserName"></param>
        public static void SetUsername(string p_UserName)
        {
            m_Username = p_UserName;
        }

        /// <summary>
        /// Sets account password
        /// </summary>
        /// <param name="p_Password"></param>
        public static void SetPassword(string p_Password)
        {
            m_Password = p_Password;
        }

        /// <summary>
        /// Sets account Id
        /// </summary>
        /// <param name="p_AccountId"></param>
        public static void SetAccountId(UInt32 p_AccountId)
        {
            m_AccountId = p_AccountId;
        }

        /// <summary>
        /// Returns account Id
        /// </summary>
        /// <returns></returns>
        public static UInt32 GetAccountId()
        {
            return m_AccountId;
        }

        /// <summary>
        /// Sets account type
        /// </summary>
        /// <param name="p_AccountType"></param>
        public static void SetAccountType(UInt32 p_AccountType)
        {
            m_AccountType = p_AccountType;
        }

        /// <summary>
        /// Returns account Type
        /// </summary>
        /// <returns></returns>
        public static UInt32 GetAccountType()
        {
            return m_AccountType;
        }

        private static UInt32 m_AccountId;
        private static string m_Username;
        private static string m_Password;
        private static UInt32 m_AccountType;
    }
}
