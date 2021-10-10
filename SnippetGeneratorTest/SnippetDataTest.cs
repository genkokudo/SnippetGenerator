using System;
using System.Collections.Generic;
using System.IO;
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
        public static IEnumerable<object[]> TestDataProp2
        {
            get
            {
                yield return new object[] {
                    "title", "author", "description", "shortcut", "code", Language.CSharp, "delimiter", Kind.Any, null, null,
                    "./Sample/TextFile1.snippet"
                };
                yield return new object[] {
                    null, null, null, null, null, Language.CSharp, null, Kind.Any, null, null,
                    "./Sample/TextFile2.snippet"
                };
                yield return new object[] {
                    "title", "author", "description", "shortcut", "code", Language.CSharp, "delimiter", Kind.Any,
                    new List<Literal> { new Literal { Id = "id1", Default = "default1", ToolTip = "tooltip1", Function = Function.None, FunctionValue = null },
                        new Literal { Id = "id2", Default = "default2", ToolTip = "tooltip2", Function = Function.SimpleTypeName, FunctionValue = "fanctionvalue2" } },
                    new List<string> { "import1", "import2", "import3" },
                    "./Sample/TextFile3.snippet"
                };
            }
        }

        [Trait("Category", "スニペット作成")]
        [Theory(DisplayName = "スニペットを作成するテスト")]
        [MemberData(nameof(TestDataProp2))]
        public void WriteSnippetTest(string title, string author, string description, string shortcut, string code, Language language, string delimiter, Kind kind, List<Literal> declarations, List<string> imports, string answerFile)
        {
            var service = new SnippetService();

            var xml = service.MakeSnippetXml(new Snippet(title, author, description, shortcut, code, language, delimiter, kind, declarations, imports));
            var result = xml.ToString();
            output.WriteLine(result);
            var data = File.ReadAllText(answerFile);
            result.Is(data);
        }

        [Trait("Category", "スニペット読み込み")]
        [Theory(DisplayName = "スニペットを読み込むテスト")]
        [InlineData("./Sample/TextFile1.snippet")]
        [InlineData("./Sample/TextFile2.snippet")]
        [InlineData("./Sample/TextFile3.snippet")]
        public void ReadSnippetTest(string filepath)
        {
            var service = new SnippetService();

            var result = service.ReadSnippet(filepath);

            result.IsNotNull();
        }

        [Trait("Category", "スニペット一覧")]
        [Fact(DisplayName = "スニペット一覧を作成する")]
        public void GetSnippetListTest()
        {
            var service = new SnippetService();

            var directory = Path.GetFullPath("./Sample");
            var result = service.GetSnippetList(directory);
            result[Language.JavaScript].IsNotNull();
            result[Language.TypeScript].IsNotNull();
            result[Language.CSharp].IsNull();
        }

    }
}