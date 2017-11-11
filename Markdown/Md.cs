using System;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.Linq.Expressions;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Markdown
{
	public class Md
	{
		public string RenderToHtml(string markdown)
		{
			var result = new StringBuilder();
			var isOpened_ = false;
			var isOpened__ = false;
			for (int i = 0; i < markdown.Length; i++)
			{
				bool is__ = i + 1 < markdown.Length && markdown[i] == '_' && markdown[i + 1] == '_' && __isCorrect(markdown, i);
				bool is_ = markdown[i] == '_' && _isCorrect(markdown, i);
				// case if both underscopes are opened => second is always _
				if (is_ && is__ && isOpened_ && isOpened__)
					is__ = false;
				if (is__)
				{
					if (!isOpened_)
					{
						if (!isOpened__)
							result.Append("<strong>");
						else
							result.Append("</strong>");
						isOpened__ = !isOpened__;
					}
					i++;
				}
				else if (is_)
				{
					if (!isOpened_)
						result.Append("<em>");
					else
						result.Append("</em>");
					isOpened_ = !isOpened_;
				}
				else
				{
					if (markdown[i] == '\\' && i + 1 < markdown.Length && markdown[i + 1] == '_')
						continue;
					result.Append(markdown[i]);
				}
			}
			return result.ToString();
		}

		private bool _isCorrect(string text, int index)
		{
			if (index - 1 >= 0 && text[index - 1] == '\\') return false;
			if (index + 1 < text.Length && '0' <= text[index + 1] && text[index + 1] <= '9') return false;
			if (index + 1 < text.Length && text[index + 1] == ' ') return false;
			if (index - 1 >= 0 && text[index - 1] == ' ') return false;
			return true;
		}

		private bool __isCorrect(string text, int index)
		{
			// index of first 
			if (index - 1 >= 0 && text[index - 1] == '\\') return false; // case '\__' == '_' 
			if (index + 2 < text.Length && '0' <= text[index + 2] && text[index + 2] <= '9') return false;
			if (index + 2 < text.Length && text[index + 2] == ' ') return false;
			if (index - 1 >= 0 && text[index - 1] == ' ') return false;
			return true;
		}
	}

	[TestFixture]
	public class Md_ShouldRender
	{
		[TestCase("_Blabla_", ExpectedResult = "<em>Blabla</em>", TestName = "SimpleEmTag_CorrecHTMLString")]
		[TestCase("__Blabla__", ExpectedResult = "<strong>Blabla</strong>", TestName = "SimpleStrongTag_CorrecHTMLString")]
		[TestCase("__bla_BLA_bla__", ExpectedResult = "<strong>bla<em>BLA</em>bla</strong>", TestName = "EmInStrongTags")]
		[TestCase("____", ExpectedResult = "<strong></strong>", TestName = "MarkdownWithFourUnderscopes_TwoStrongTags")]
		[TestCase("___bla___", ExpectedResult = "<strong><em>bla</em></strong>", TestName = "EmTagInStrongTagWithoutGap_")]
		public string GivenMarkdownWithCorrectTags(string markdown)
		{
			return new Md().RenderToHtml(markdown);
		}

		[TestCase("\\_hello_world_", ExpectedResult = "_hello<em>world</em>", TestName = "ShieldedEmTag_NotTag")]
		[TestCase("\\_\\_hello__world__", ExpectedResult = "__hello<strong>world</strong>",
			TestName = "ShieldedStrongTag_NotTag")]
		[TestCase("\\__hello_world", ExpectedResult = "_<em>hello</em>world", TestName = "ShieldedStrong_GetEmTag1")]
		[TestCase("_\\_hello_world", ExpectedResult = "<em>_hello</em>world", TestName = "ShieldedStrong_GetEmTag2")]
		[TestCase("\\hello__world__\\", ExpectedResult = "\\hello<strong>world</strong>\\",
			TestName = "SlashWithoutShielding_GetSlash")]
		public string GivenMarkdownWithShielding(string markdown)
		{
			return new Md().RenderToHtml(markdown);
		}

		[TestCase("pretty_1Test_case_", ExpectedResult = "pretty_1Test<em>case</em>",
			TestName = "OneUnderscopeWithDigit_NotTag")]
		[TestCase("pretty__2Test_case", ExpectedResult = "pretty<em>_2Test</em>case",
			TestName = "TwoUnderscopesWithDigit_isEmTag")]
		[TestCase("pretty_2_Test_case", ExpectedResult = "pretty_2<em>Test</em>case",
			TestName = "TwoUnderscopesWithDigit(Between)_isEmTag")]
		public string GivenMarkdownWithDigits(string markdown)
		{
			return new Md().RenderToHtml(markdown);
		}

		[TestCase("_ uni__verse__", ExpectedResult = "_ uni<strong>verse</strong>",
			TestName = "SpaceAfterOpeningEm_isUnderscope")]
		[TestCase("_uni _verse_!", ExpectedResult = "<em>uni _verse</em>!", TestName = "SpaceBeforeClosingEm_isUnderscope")]
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