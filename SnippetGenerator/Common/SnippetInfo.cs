using System;
using System.Collections.Generic;
using System.Text;

namespace SnippetGenerator.Common
{
    /// <summary>
    /// スニペット一覧管理用情報
    /// </summary>
    public class SnippetInfo
    {
        /// <summary>一覧にしたときの見出し、ファイル名</summary>
        public string Title { get; set; }

        /// <summary>説明</summary>
        public string Discription { get; set; }

        /// <summary>ファイルのフルパス</summary>
        public string FullPath { get; set; }
    }
}
