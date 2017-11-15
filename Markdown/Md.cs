using System;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace Markdown
{
	public class Md
	{
		public string RenderToHtml(string markdown)
		{
			var result = new StringBuilder();
			var isOpenedUnderscore = false;
			var isOpenedDoubleUnderscore = false;
			for (int i = 0; i < markdown.Length; i++)
			{
				bool isDoubleUnderscore = IsCorrectDoubleUnderscore(markdown, i);
				bool isUnderscore = IsCorrectUnderscore(markdown, i);

				if (IsCorrectTripleUnderscore(markdown, i))
					isDoubleUnderscore = !isOpenedUnderscore;

				if (isDoubleUnderscore)
				{
					if (!isOpenedUnderscore)
					{
						result.Append(GetHtmlTagsInsteadMarkdownUnderscopes("__", !isOpenedDoubleUnderscore));
						isOpenedDoubleUnderscore = !isOpenedDoubleUnderscore;
					}
					i++;
				}
				else if (isUnderscore)
				{
					result.Append(GetHtmlTagsInsteadMarkdownUnderscopes("_", !isOpenedUnderscore));
					isOpenedUnderscore = !isOpenedUnderscore;
				}
				else
				{
					if (IsEscape(markdown, i))
						continue;
					result.Append(markdown[i]);
				}
			}

			return UndoLastTagsIfNotClosed(result.ToString(), isOpenedUnderscore, isOpenedDoubleUnderscore);
		}

		private static bool IsEscape(string text, int index)
		{
			return text[index] == '\\' && index + 1 < text.Length && text[index + 1] == '_';
		}

		private static bool IsCorrectUnderScoreCommonRules(string text, int index, int secondIndex)
		{
			if (text[index] != '_') return false;
			if (index - 1 >= 0 && text[index - 1] == '\\') return false;
			if (secondIndex < text.Length && Char.IsDigit(text[secondIndex])) return false;
			if (secondIndex < text.Length && text[secondIndex] == ' ') return false;
			if (index - 1 >= 0 && text[index - 1] == ' ') return false;
			return true;
		}

		private static bool IsCorrectUnderscore(string text, int index)
		{
			return IsCorrectUnderScoreCommonRules(text, index, index + 1);
		}

		private static bool IsCorrectDoubleUnderscore(string text, int index)
		{
			return index + 1 < text.Length && text[index + 1] == '_' && IsCorrectUnderScoreCommonRules(text, index, index + 2);
		}

		private static bool IsCorrectTripleUnderscore(string text, int index)
		{
			return index + 2 < text.Length && text[index + 1] == '_' && text[index + 2] == '_'
			       && IsCorrectUnderScoreCommonRules(text, index, index + 3);
		}

		private static string UndoLastTagsIfNotClosed(string str, bool isOpenedUnderscore, bool isOpenedDoubleUnderscore)
		{
			if (isOpenedUnderscore)
			{
				int index = str.LastIndexOf("<em>");
				str = str.Remove(index, 4).Insert(index, "_");
			}
			if (isOpenedDoubleUnderscore)
			{
				int index = str.LastIndexOf("<strong>");
				str = str.Remove(index, 8).Insert(index, "__");
			}
			return str;
		}

		private static string GetHtmlTagsInsteadMarkdownUnderscopes(string markdown, bool isOpening)
		{
			switch (markdown)
			{
				case "_":
					return isOpening ? "<em>" : "</em>";
				case "__":
					return isOpening ? "<strong>" : "</strong>";
			}
			return "";
		}
	}
}