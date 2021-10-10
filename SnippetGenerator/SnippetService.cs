using SnippetGenerator.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

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
        /// <param name="directoryFillPath">ディレクトリフルパス（相対パスは動作未確認）</param>
        /// <returns>言語別のスニペットリスト</returns>
        public Dictionary<Language, List<SnippetInfo>> GetSnippetList(string directoryFillPath);

        /// <summary>
        /// 指定したディレクトリ以下のスニペットフォルダのフルパスリストをVisualStudio準拠で取得
        /// 存在するものだけ辞書に入れて返す
        /// </summary>
        /// <param name="directoryFullPath">ディレクトリフルパス（相対パスは動作未確認）</param>
        /// <returns></returns>
        public Dictionary<Language, string> GetExistDirectryDictionary(string directoryFullPath);
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

        // 現在の所、1ファイルに複数スニペットが無いものとしている
        // もしそれが必要になったら読み込み用クラスを生成し直す
        public Snippet ReadSnippet(string filePath)
        {
            // 読み込み
            var settings = new XmlReaderSettings
            {
                IgnoreWhitespace = true,
                IgnoreComments = true
            };
            using var reader = XmlReader.Create(filePath, settings);
            var serializer = new XmlSerializer(typeof(CodeSnippets));
            var xml = serializer.Deserialize(reader) as CodeSnippets;

            // 読み込んだものをSnippetクラスに落とし込む
            var language = Language.CSharp;
            var kind = Kind.Any;
            Enum.TryParse(xml.CodeSnippet.Snippet.Code.Language, out language);
            Enum.TryParse(xml.CodeSnippet.Snippet.Code.Kind, out kind);

            // 原始的な方法で移し替える
            var declarations = new List<Literal>();
            if (xml.CodeSnippet.Snippet.Declarations != null)
            {
                foreach (var declaration in xml.CodeSnippet.Snippet.Declarations)
                {
                    // Functionを取り出す
                    Function function = Function.None;
                    var funcStr = string.Empty;

                    // SimpleTypeName(fanctionvalue2)
                    if (declaration.Function != null && !string.IsNullOrWhiteSpace(declaration.Function.Value))
                    {
                        var splited = declaration.Function.Value.Split('(');
                        Enum.TryParse(splited[0], out function);
                        funcStr = splited[1].Trim(')');
                    }

                    var literal = new Literal(declaration.ID, declaration.ToolTip, declaration.Default, function, funcStr);
                    declarations.Add(literal);
                }
            }

            var imports = new List<string>();
            if (xml.CodeSnippet.Snippet.Imports != null)
            {
                foreach (var import in xml.CodeSnippet.Snippet.Imports)
                {
                    imports.Add(import.Namespace);
                }
            }
            
            var result = new Snippet(
                xml.CodeSnippet.Header.Title,
                xml.CodeSnippet.Header.Author,
                xml.CodeSnippet.Header.Description,
                xml.CodeSnippet.Header.Shortcut,
                xml.CodeSnippet.Snippet.Code.Value, 
                language,
                xml.CodeSnippet.Snippet.Code.Delimiter, 
                kind, 
                declarations, 
                imports
            );

            // 必須ではないフィールドも読み込む
            SnippetType snippetType = SnippetType.Expansion;
            result.HelpUrl = xml.CodeSnippet.Header.HelpUrl;
            Enum.TryParse(xml.CodeSnippet.Header.SnippetTypes.SnippetType, out snippetType);

            return result;
        }

        public Dictionary<Language, List<SnippetInfo>> GetSnippetList(string directoryPath)
        {
            var result = new Dictionary<Language, List<SnippetInfo>>();
            
            // 各言語のフォルダを取得
            var dirDict = GetExistDirectryDictionary(directoryPath);

            // 引数のディレクトリ存在チェック
            if (dirDict == null)
            {
                return null;
            }

            // フォルダがある言語だけ処理する
            foreach (var language in dirDict.Keys) {
                var fileList = new List<string>();
                // TODO:DirectoryService.FolderInsiteSearchを使って、指定拡張子のファイルを全て探す
                FolderInsiteSearch(dirDict[language], fileList, new string[] { ".snippet" });
                var infoList = new List<SnippetInfo>();

                foreach (var filePath in fileList)
                {
                    // スニペット読み込み
                    var snippet = ReadSnippet(filePath);
                    infoList.Add(new SnippetInfo { FullPath = filePath, Description = snippet.Description, Title = snippet.Title });
                }
                if (infoList.Count > 0)
                {
                    result.Add(language, infoList);
                }
            }

            return result;
        }

        public Dictionary<Language, string> GetExistDirectryDictionary(string directoryPath)
        {
            // 存在チェック
            if (!Directory.Exists(directoryPath))
            {
                return null;
            }

            // 各言語のフォルダを取得
            var result = new Dictionary<Language, string>();
            var langs = (Language[])Enum.GetValues(typeof(Language));
            foreach (var lang in langs)
            {
                // 各言語について、ディレクトリを取得する
                var langdir = GetLanguagePath(lang);
                var sourceDir = Path.Combine(directoryPath, langdir);

                if (Directory.Exists(sourceDir))
                {
                    result.Add(lang, sourceDir);
                }
            }
            return result;
        }



        // TODO:あとでDirectoryServiceを独立化して、それを使用するように書き直すこと！！！！
        private void FolderInsiteSearch(string folderPath, List<string> fileFullPathList, string[] extensions)
        {
            //現在のフォルダ内の指定拡張子のファイル名をリストに追加
            foreach (var fileName in Directory.EnumerateFiles(folderPath))
                foreach (var endId in extensions)
                    if (fileName.EndsWith(endId))
                        fileFullPathList.Add(fileName);
            //現在のフォルダ内のすべてのフォルダパスを取得
            var dirNames = Directory.EnumerateDirectories(folderPath);
            //フォルダがないならば再帰探索終了し、あるなら各フォルダに対して探索実行
            if (dirNames.Count() == 0)
                return;
            else
                foreach (var dirName in dirNames)
                    FolderInsiteSearch(dirName, fileFullPathList, extensions);
        }
    }





}
