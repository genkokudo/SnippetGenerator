//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace SnippetGenerator.Common
//{
//    /// <summary>
//    /// 没コード
//    /// </summary>
//    class Botsu
//    {
//        //public Snippet ReadSnippet(string filePath)
//        //{
//        //    var settings = new XmlReaderSettings
//        //    {
//        //        IgnoreWhitespace = true,
//        //        IgnoreComments = true
//        //    };

//        //    var list = new List<string>();
//        //    using var reader = XmlReader.Create(filePath, settings);

//        //    var snippetType = string.Empty;
//        //    var title = string.Empty;
//        //    var author = string.Empty;
//        //    var description = string.Empty;
//        //    var helpUrl = string.Empty;
//        //    var shortcut = string.Empty;
//        //    var snippet = string.Empty;
//        //    var code = string.Empty;
//        //    var imports = new List<string>();
//        //    var declarations = new List<Literal>();
//        //    var language = Language.CSharp;
//        //    var delimiter = string.Empty;
//        //    var kind = Kind.Any;

//        //    // reader.Read()の挙動がよく分からないのでこの方法で。
//        //    // reader.ReadString();は、今のカーソルのタグを読んだ後にタグの最後に移動するらしい。

//        //    // 取り敢えず上から呼んで、ヘッダ部分の情報を取り出す
//        //    reader.ReadToFollowing("SnippetType");
//        //    snippetType = reader.ReadString();

//        //    reader.ReadToFollowing("Title");
//        //    title = reader.ReadString();

//        //    reader.ReadToFollowing("Author");
//        //    author = reader.ReadString();

//        //    reader.ReadToFollowing("Description");
//        //    description = reader.ReadString();

//        //    reader.ReadToFollowing("HelpUrl");
//        //    helpUrl = reader.ReadString();

//        //    reader.ReadToFollowing("Shortcut");
//        //    shortcut = reader.ReadString();

//        //    // Imports以下に複数同じタグがあるので、それぞれを読む必要がある
//        //    // ぶっちゃけ正しい方法が分からない
//        //    reader.ReadToFollowing("Imports");
//        //    var importsSubtree = reader.ReadSubtree();  // 取り敢えずサブツリーとして本編の流れとは切り離して読み込む

//        //    while (importsSubtree.Read())
//        //    {
//        //        // Namespaceだけ拾えばよいのでこの方法で。
//        //        if (importsSubtree.ReadToFollowing("Namespace"))
//        //        {
//        //            imports.Add(importsSubtree.ReadString());
//        //        }
//        //    }

//        //    reader.ReadToFollowing("Declarations");
//        //    var declarationsSubtree = reader.ReadSubtree();  // 取り敢えずサブツリーとして本編の流れとは切り離して読み込む
//        //    while (declarationsSubtree.Read())
//        //    {


//        //    }

//        //    reader.ReadToFollowing("Code");
//        //    for (int i = 0; i < reader.AttributeCount; i++)
//        //    {
//        //        reader.MoveToAttribute(i);
//        //        if (reader.HasValue == true)
//        //        {
//        //            switch (reader.Name)
//        //            {
//        //                case "Language":
//        //                    Enum.TryParse(reader.Value, out language);
//        //                    break;
//        //                case "Kind":
//        //                    Enum.TryParse(reader.Value, out kind);
//        //                    break;
//        //                case "Delimiter":
//        //                    delimiter = reader.Value;
//        //                    break;
//        //                default:
//        //                    break;
//        //            }
//        //        }
//        //    }
//        //    code = reader.ReadString();

//        //    var result = new Snippet(title, author, description, shortcut, code, language, delimiter, kind, null, imports);
//        //    return result;
//        //}
//    }
//}
