using SnippetGenerator.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace SnippetGenerator
{
    /// <summary>
    /// スニペットファイルを生成する
    /// </summary>
    public interface ISnippetService
    {
        /// <summary>
        /// スニペットXMLを出力する
        /// ToStringでstringを得られる。
        /// </summary>
        /// <param name="Data">スニペットデータ</param>
        /// <returns>StringBuilderを返す</returns>
        public StringWriter MakeSnippetXml(Snippet Data);

        /// <summary>VisualStudioでの各言語フォルダ名を取得します</summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public string GetLanguagePath(Language language);

        /// <summary>
        /// スニペットの読み込み
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public Snippet ReadSnippet(string filePath);

        /// <summary>
        /// 指定ディレクトリ以下の全スニペットの読み込み
        /// 言語別に取得
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns>言語別のスニペットリスト</returns>
        public Dictionary<Language, List<Snippet>> GetSnippetList(string directoryPath);

    }

    /// <summary>
    /// スニペットファイルを生成する
    /// </summary>
    public class SnippetService : ISnippetService
    {
        #region 設定

        private readonly XmlWriterSettings Settings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = ("  "),
            Encoding = Encoding.UTF8
        };

        /// <summary>
        /// 何故かXMLヘッダがUTF-16になるので、UTF8に矯正する
        /// </summary>
        private class StringWriterUTF8 : StringWriter
        {
            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }

        #endregion

        public StringWriter MakeSnippetXml(Snippet Data)
        {
            // 必須項目のnullチェック
            if (Data == null)
            {
                throw new Exception("データが入力されていないため、生成できません。");
            }

            var sw = new StringWriterUTF8();
            using (var w = XmlWriter.Create(sw, Settings))
            {
                // 基本：WriteStartElementとWriteEndElementがセット、タグで囲んだ値はWriteValueを呼んで設定。
                // WriteStartAttributeとWriteEndAttributeもセット、ここの値設定はWriteStringを呼ぶ。

                // <?xml version="1.0" encoding="utf-8"?>
                w.WriteStartDocument();

                // <CodeSnippets xmlns="http://schemas.microsoft.com/VisualStudio/2005/CodeSnippet">
                w.WriteStartElement("CodeSnippets", "http://schemas.microsoft.com/VisualStudio/2005/CodeSnippet");

                // <CodeSnippet Format="1.0.0">
                w.WriteStartElement("CodeSnippet");
                w.WriteStartAttribute("Format", "");
                w.WriteString("1.0.0");
                w.WriteEndAttribute();

                // Header句
                w.WriteStartElement("Header");

                // 可変長の要素になる・・・かな？
                w.WriteStartElement("SnippetTypes");
                w.WriteStartElement("SnippetType");
                w.WriteValue(Data.SnippetType.ToString());  // 改行せずに値を書く
                w.WriteEndElement();
                w.WriteEndElement();

                w.WriteStartElement("Title");
                w.WriteValue(Data.Title);
                w.WriteEndElement();

                w.WriteStartElement("Author");
                w.WriteValue(Data.Author);
                w.WriteEndElement();

                w.WriteStartElement("Description");
                w.WriteValue(Data.Description);
                w.WriteEndElement();

                w.WriteStartElement("HelpUrl");
                w.WriteValue(Data.HelpUrl);
                w.WriteEndElement();

                w.WriteStartElement("Shortcut");
                w.WriteValue(Data.Shortcut);
                w.WriteEndElement();

                w.WriteEndElement();

                // Snippet句
                w.WriteStartElement("Snippet");

                var isImport = Data.Imports.Count > 0;
                if (isImport)
                {
                    //<Imports>
                    //    <Import>
                    //        <Namespace>System.Data</Namespace>
                    //    </Import>
                    //    ...
                    //</Imports>
                    w.WriteStartElement("Imports");
                    foreach (var item in Data.Imports)
                    {
                        w.WriteStartElement("Import");
                        w.WriteStartElement("Namespace");
                        w.WriteValue(item);
                        w.WriteEndElement();
                        w.WriteEndElement();
                    }
                    w.WriteEndElement();
                }

                var isDeclarations = Data.Declarations.Count > 0;
                if (isDeclarations)
                {
                    //<Declarations>
                    //    <Literal>
                    //        <ID>type</ID>
                    //        <ToolTip>プロパティの型</ToolTip>
                    //        <Default>int</Default>
                    //    </Literal>
                    //    <Literal Editable="false">
                    //        <ID>property</ID>
                    //        <ToolTip>プロパティ名</ToolTip>
                    //        <Default>MyProperty</Default>
                    //        <Function>MyProperty</Function>
                    //    </Literal>
                    //</Declarations>
                    w.WriteStartElement("Declarations");
                    foreach (var item in Data.Declarations)
                    {
                        w.WriteStartElement("Literal");
                        w.WriteStartElement("ID");
                        w.WriteValue(item.Id);
                        w.WriteEndElement();
                        if (item.ToolTip != null)
                        {
                            w.WriteStartElement("ToolTip");
                            w.WriteValue(item.ToolTip);
                            w.WriteEndElement();
                        }
                        if (item.Default != null)
                        {
                            w.WriteStartElement("Default");
                            w.WriteValue(item.Default);
                            w.WriteEndElement();
                        }
                        if (item.Function != Function.None)
                        {
                            w.WriteStartElement("Function");
                            if (item.Editable != null)
                            {
                                w.WriteStartAttribute("Editable", "");
                                w.WriteString(item.Editable);
                                w.WriteEndAttribute();
                            }
                            if (!string.IsNullOrWhiteSpace(item.FunctionValue))
                            {
                                w.WriteValue($"{item.Function}({item.FunctionValue})");
                            }
                            else if (item.Function == Function.ClassName)
                            {
                                w.WriteValue($"{item.Function}()");
                            }
                            w.WriteEndElement();
                        }
                        w.WriteEndElement();
                    }
                    w.WriteEndElement();
                }

                w.WriteStartElement("Code");
                w.WriteStartAttribute("Language", "");
                w.WriteString(Data.Language.ToString());
                w.WriteEndAttribute();
                w.WriteStartAttribute("Kind", "");
                w.WriteString(Data.Kind.GetStringValue());
                w.WriteEndAttribute();
                if (!string.IsNullOrWhiteSpace(Data.Delimiter))
                {
                    w.WriteStartAttribute("Delimiter", "");
                    w.WriteString(Data.Delimiter);
                    w.WriteEndAttribute();
                }
                w.WriteCData(Data.Code);

                // Snippet句ここまで
                w.WriteEndElement();

                w.WriteEndElement();
                w.WriteEndElement();

                // 完成
                w.Flush();
            }
            return sw;

        }

        public string GetLanguagePath(Language language)
        {
            if (language == Language.CSharp)
            {
                return "Visual C#\\My Code Snippets";
            }
            else if (language == Language.SQL)
            {
                return "SQL_SSDT\\My Code Snippets";
            }
            else if (language == Language.HTML)
            {
                return "Visual Web Developer\\My HTML Snippets";
            }
            return language.ToString() + "\\My Code Snippets";
        }

        public Snippet ReadSnippet(string filePath)
        {
            //var result = new Snippet();
            // 結局、読み込みってできるの？
            // XMLを読んで、Snippetオブジェクトにする
            // XmlReaderを使う感じ？

            //XmlReaderSettings settings = new XmlReaderSettings();
            //settings.IgnoreWhitespace = true;
            //    settings.IgnoreComments   = true;

            //using var reader = XmlReader.Create(@"filepath", settings);
            //while (reader.Read())
            //{
            //    if (reader.IsStartElement())
            //    {
            //        //return only when you have START tag  
            //        switch (reader.Name.ToString())
            //        {
            //            case "Name":
            //                Console.WriteLine("The Name of the Student is " + reader.ReadString());
            //                break;
            //            case "Grade":
            //                Console.WriteLine("The Grade of the Student is " + reader.ReadString());
            //                break;
            //        }
            //    }
            //}


            // なので、まずC#とそれ以外のスニペットを準備。
            // 文字列データにして、テストプログラムを書く。
            // 出来ればImportsも実装。WPFにもC#限定でImports入力欄作ったらいいと思う。使うかどうかは別として。

            //return result;
            return null;
        }

        public Dictionary<Language, List<Snippet>> GetSnippetList(string directoryPath)
        {
            throw new NotImplementedException();
        }
    }
}
