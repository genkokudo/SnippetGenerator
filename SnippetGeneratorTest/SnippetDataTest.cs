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
        /// <summary>xUnit�̏ꍇ�A�W���o�͂�p�ӂ���K�v������</summary>
        private readonly ITestOutputHelper output;

        public SnippetDataTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        // �e�X�g�f�[�^
        public static IEnumerable<object[]> TestDataProp2
        {
            get
            {
                yield return new object[] {
                    "title", "author", "description", "shortcut", "code", Language.CSharp, "delimiter", Kind.Any, null, null,
                    "./Sample/TextFile1.txt"
                };
                yield return new object[] {
                    null, null, null, null, null, Language.CSharp, null, Kind.Any, null, null,
                    "./Sample/TextFile2.txt"
                };
                yield return new object[] {
                    "title", "author", "description", "shortcut", "code", Language.CSharp, "delimiter", Kind.Any,
                    new List<Literal> { new Literal { Id = "id1", Default = "default1", ToolTip = "tooltip1", Function = Function.None, FunctionValue = null },
                        new Literal { Id = "id2", Default = "default2", ToolTip = "tooltip2", Function = Function.SimpleTypeName, FunctionValue = "fanctionvalue2" } },
                    new List<string> { "import1", "import2", "import3" },
                    "./Sample/TextFile3.txt"
                };
            }
        }


        [Trait("Category", "Arithmetic")]
        [Fact(DisplayName = "1+2=3�̂͂�")]
        public void Test1()
        {
            (1 + 2).Is(3);
            output.WriteLine("Test1�����s");
        }

        [Trait("Category", "�X�j�y�b�g�쐬")]
        [Theory(DisplayName = "�X�j�y�b�g���쐬����e�X�g")]
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

        [Trait("Category", "�X�j�y�b�g�ǂݍ���")]
        [Theory(DisplayName = "�X�j�y�b�g��ǂݍ��ރe�X�g")]
        [InlineData("./Sample/TextFile1.txt")]
        [InlineData("./Sample/TextFile2.txt")]
        [InlineData("./Sample/TextFile3.txt")]
        public void ReadSnippetTest(string filepath)
        {
            var service = new SnippetService();

            var result = service.ReadSnippet(filepath);

            result.IsNotNull();

        }
    }
}