using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Notes.BLL.Services.MarkdownRendererService
{
    public class MarkdownRenderer : IMarkdownRenderer
    {
        public IHtmlContent RenderFromMarkdownToHTML(string markdownText)
        {
            var htmlResult = string.Empty;

            IEnumerable<string> lines = markdownText.Split(Environment.NewLine);

            lines = SetParagraphs(lines);

            lines = SetHeaders(lines);
                
            foreach (var line in lines)
                htmlResult += line;

            return new HtmlString(htmlResult);
        }

        private IEnumerable<string> SetHeaders(IEnumerable<string> lines)
        {
            for (int i = 0; i < lines.Count(); i++)
            {
                var line = lines.ElementAt(i);
                if (line.StartsWith("# "))
                {
                    line = CreateHtmlHeader(line, 1);
                }
                else if (line.StartsWith("## "))
                {
                    line = CreateHtmlHeader(line, 2);
                }
                else if (line.StartsWith("### "))
                {
                    line = CreateHtmlHeader(line, 3);
                }
                else if (line.StartsWith("#### "))
                {
                    line = CreateHtmlHeader(line, 4);
                }
                else if (line.StartsWith("##### "))
                {
                    line = CreateHtmlHeader(line, 5);
                }
                else if (line.StartsWith("###### "))
                {
                    line = CreateHtmlHeader(line, 6);
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
            var newLines = new List<string>();

            newLines.Add("<p>");

            for (int i = 0; i < lines.Count(); i++)
            {
                if (lines.ElementAt(i) == "")
                {
                    newLines.Add("</p>");
                    newLines.Add("<p>");
                }
                else
                {
                    newLines.Add(lines.ElementAt(i));
                }
            }

            newLines.Add("</p>");

            return newLines;
        }

        private static string Bracket(string text, string tagName)
        {
            return $"<{tagName}>\n{text}\n</{tagName}>\n";
        }
    }
}