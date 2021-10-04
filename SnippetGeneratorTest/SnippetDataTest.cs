using System;
using System.Collections.Generic;
using System.Linq;
using ChainingAssertion;
using SnippetGenerator;
using SnippetGenerator.Common;
using Xunit;
using Xunit.Abstractions;

namespace SnippetGeneratorTest
{
    public class SnippetDataTest
    {
        /// <summary>xUnitの場合、標準出力を用意する必要がある</summary>
        private readonly ITestOutputHelper output;

        public SnippetDataTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        // テストデータ
        public static IEnumerable<object[]> TestDataProp
        {
            get
            {
                yield return new object[] { "title", "author", "description", "shortcut", "code", Language.CSharp, "delimiter", Kind.Any, null, null, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<CodeSnippets xmlns=\"http://schemas.microsoft.com/VisualStudio/2005/CodeSnippet\">\r\n  <CodeSnippet Format=\"1.0.0\">\r\n    <Header>\r\n      <SnippetTypes>\r\n        <SnippetType>Expansion</SnippetType>\r\n      </SnippetTypes>\r\n      <Title>title</Title>\r\n      <Author>author</Author>\r\n      <Description>description</Description>\r\n      <HelpUrl>www.microsoft.com</HelpUrl>\r\n      <Shortcut>shortcut</Shortcut>\r\n    </Header>\r\n    <Snippet>\r\n      <Code Language=\"CSharp\" Kind=\"any\" Delimiter=\"delimiter\"><![CDATA[code]]></Code>\r\n    </Snippet>\r\n  </CodeSnippet>\r\n</CodeSnippets>" };
                yield return new object[] { null, null, null, null, null, Language.CSharp, null, Kind.Any, null, null, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<CodeSnippets xmlns=\"http://schemas.microsoft.com/VisualStudio/2005/CodeSnippet\">\r\n  <CodeSnippet Format=\"1.0.0\">\r\n    <Header>\r\n      <SnippetTypes>\r\n        <SnippetType>Expansion</SnippetType>\r\n      </SnippetTypes>\r\n      <Title>Untitled</Title>\r\n      <Author>Unknown</Author>\r\n      <Description></Description>\r\n      <HelpUrl>www.microsoft.com</HelpUrl>\r\n      <Shortcut></Shortcut>\r\n    </Header>\r\n    <Snippet>\r\n      <Code Language=\"CSharp\" Kind=\"any\"><![CDATA[]]></Code>\r\n    </Snippet>\r\n  </CodeSnippet>\r\n</CodeSnippets>" };
            }
        }


        [Trait("Category", "Arithmetic")]
        [Fact(DisplayName = "1+2=3のはず")]
        public void Test1()
        {
            (1 + 2).Is(3);
            output.WriteLine("Test1を実行");
        }

        [Trait("Category", "スニペット作成")]
        [Theory(DisplayName = "プロパティやメソッドを使ったテスト")]
        [MemberData(nameof(TestDataProp))]
        public void AddTest2(string title, string author, string description, string shortcut, string code, Language language, string delimiter, Kind kind, List<Literal> declarations, List<string> imports, string answer)
        {
            var service = new SnippetService();
            
            var xml = service.MakeSnippetXml(new Snippet(title, author, description, shortcut, code, language, delimiter, kind, declarations, imports));
            var result = xml.ToString();
            output.WriteLine(result);
            result.Is(answer);
            //Assert.Throws<Exception>(() => service.MakeSnippetXml(new Snippet()));
        }
    }
}