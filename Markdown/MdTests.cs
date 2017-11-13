using FluentAssertions;
using NUnit.Framework;

namespace Markdown
{
	public class MdTests
	{
		[TestFixture]
		public class Md_ShouldRender
		{
			[TestCase("_Blabla_", ExpectedResult = "<em>Blabla</em>", TestName = "MarkdownText_MakeHTMLTags_GetHTMLWithEmTags")]
			[TestCase("__Blabla__", ExpectedResult = "<strong>Blabla</strong>", TestName = "SimpleStrongTag_CorrecHTMLString")]
			[TestCase("__bla_BLA_bla__", ExpectedResult = "<strong>bla<em>BLA</em>bla</strong>", TestName = "EmInStrongTags")]
			[TestCase("__", ExpectedResult = "__", TestName = "TwoUnderscores_NotTags")]
			[TestCase("____", ExpectedResult = "<strong></strong>", TestName = "FourUnderscopes_TwoStrongTags")]
			[TestCase("___bla_a__", ExpectedResult = "<strong><em>bla</em>a</strong>", TestName = "TripleOpeningUnderscope_StrongThanEmTags")]
			[TestCase("__a_bla___", ExpectedResult = "<strong>a<em>bla</em></strong>", TestName = "TripleClosingUnderscopeEmThanStrongClosingTags")]
			[TestCase("___bla___", ExpectedResult = "<strong><em>bla</em></strong>", TestName = "TripleUnderscopes_EmInStrongTags")]
			[TestCase("_a___b__", ExpectedResult = "<em>a</em><strong>b</strong>", TestName = "Gosha's Test")]
			[TestCase("__a_", ExpectedResult = "__a_", TestName = "UnpairedUnderscores_NotTags")]
			public string GivenMarkdownWithCorrectTags(string markdown)
			{
				return new Md().RenderToHtml(markdown);
			}

			[TestCase("\\_hello_world_", ExpectedResult = "_hello<em>world</em>", TestName = "ShieldedEmTag_isNotTag")]
			[TestCase("\\_\\_hello__world__", ExpectedResult = "__hello<strong>world</strong>",
				TestName = "ShieldedStrongTag_isNotTag")]
			[TestCase("\\__hello_world", ExpectedResult = "_<em>hello</em>world", TestName = "ShieldedStrong_GetEmTag1")]
			[TestCase("_\\_hello_world", ExpectedResult = "<em>_hello</em>world", TestName = "ShieldedStrong_GetEmTag2")]
			[TestCase("\\hello__world__\\", ExpectedResult = "\\hello<strong>world</strong>\\",
				TestName = "SlashWithoutShielding_GetSlash")]
			public string GivenMarkdownWithEscape(string markdown)
			{
				return new Md().RenderToHtml(markdown);
			}

			[TestCase("pretty_1Test_case_", ExpectedResult = "pretty_1Test<em>case</em>",
				TestName = "OneUnderscoreWithDigit_NotTag")]
			[TestCase("pretty__2Test_case", ExpectedResult = "pretty<em>_2Test</em>case",
				TestName = "TwoUnderscoresWithDigit_isEmTag")]
			[TestCase("pretty_2_Test_case", ExpectedResult = "pretty_2<em>Test</em>case",
				TestName = "TwoUnderscoresWithDigit(Between)_isEmTag")]
			public string GivenMarkdownWithDigits(string markdown)
			{
				return new Md().RenderToHtml(markdown);
			}

			[TestCase("_ uni__verse__", ExpectedResult = "_ uni<strong>verse</strong>",
				TestName = "SpaceAfterOpeningEm_isUnderscore")]
			[TestCase("_uni _verse_!", ExpectedResult = "<em>uni _verse</em>!", TestName = "SpaceBeforeClosingEm_isUnderscore")]
			[TestCase("__ universe_", ExpectedResult = "<em>_ universe</em>",
				TestName = "SpaceAfterOpeningStrong_isOpeningAndClosingEmTags")]
			[TestCase("_universe __", ExpectedResult = "<em>universe _</em>", TestName = "SpaceBeforeClosingStrong_isEmTag")]
			public string GivenMarkdownWithSpaces(string markdown)
			{
				return new Md().RenderToHtml(markdown);
			}

			[Test]
			public void MarkdownWithDoubleSelectionInsideSingle_GetOnlyEmTags()
			{
				new Md().RenderToHtml("_go__out__please_").Should().Be("<em>gooutplease</em>");
			}
		}
	}
}