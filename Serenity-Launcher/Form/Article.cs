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

namespace SteerStone.ArticleInfo
{
    /// <summary>
    ///  Holds Article information
    /// </summary>
    public struct Article
    {
        public string NameBy;
        public string HeadLine;
        public string Content;
        public uint Date;
    }

    /// <summary>
    /// Holds storage about articles
    /// </summary>
    class ArticleAccessor
    {
        public static List<Article> Articles = new List<Article>(); ///< Storage of articles  
        public static int Index = 0;                                ///< Article index on which article we are currently on
    }
}
