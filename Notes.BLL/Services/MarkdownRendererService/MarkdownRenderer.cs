using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Notes.BLL.Services.MarkdownRendererService
{
    public class MarkdownRenderer : IMarkdownRenderer
    {
        public IHtmlContent RenderFromMarkdownToHTML(string markdownText)
        {
            IEnumerable<string> lines = markdownText.Split(Environment.NewLine);

            lines = SetParagraphs(lines);

            lines = SetHeaders(lines);

            lines = SetStrong(lines);

            return new HtmlString(string.Join("", lines));
        }

        private static IEnumerable<string> SetStrong(IEnumerable<string> lines)
        {
            Regex regex = new(@"\*\*.+\*\*");

            for (int i = 0; i < lines.Count(); i++)
            {
                string line = lines.ElementAt(i);

                var matches = regex.Matches(line);

                foreach (Match match in matches)
                {
                    line = line.Replace(match.Value, $"<strong>{match.Value.Replace("**", "")}</strong>");
                }

                yield return line;
            }
        }

        private static IEnumerable<string> SetLineBreakes(IEnumerable<string> lines)
        {
            for (int i = 0; i < lines.Count() - 2; i++)
            {
                string line = lines.ElementAt(i);

                if (string.IsNullOrEmpty(line) == false)
                {
                    yield return line + "<br/>";
                }
            }
        }

        private static IEnumerable<string> SetHeaders(IEnumerable<string> lines)
        {
            for (int i = 0; i < lines.Count(); i++)
            {
                var line = lines.ElementAt(i);
                
                for (int j = 1; j < 7; j++)
                {
                    string markdownHeader = new(Enumerable.Range(0, j).Select(x => '#').ToArray());

                    if (line.StartsWith($"{markdownHeader} "))
                    {
                        line = CreateHtmlHeader(line, j);
                    }
                }
                
                yield return line;
            }
        }

        private static string CreateHtmlHeader(string line, int value)
        {
            line = line.Remove(0, value + 1);
            line = line.Insert(0, $"<h{value}>");

            int length = line.Length;

            line = line.Insert(length, $"</h{value}>");
            return line;
        }

        private static IEnumerable<string> SetParagraphs(IEnumerable<string> lines)
        {
            yield return "<p>";

            for (int i = 0; i < lines.Count(); i++)
            {
                if (lines.ElementAt(i) == "")
                {
                    yield return "</p>";
                    yield return "<p>";
                }
                else
                {
                    yield return lines.ElementAt(i);
                }
            }

            yield return "</p>";
        }
    }
}