using Microsoft.AspNetCore.Html;
using HeyRed.MarkdownSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.BLL.Services.MarkdownRendererService
{
    public class MarkdownAdapter : IMarkdownRenderer
    {
        public IHtmlContent RenderFromMarkdownToHTML(string markdownText)
        {
            var options = new MarkdownOptions()
            {
                AutoHyperlink = true,
                AutoNewLines = true,
                LinkEmails = true,
                QuoteSingleLine = true,
                StrictBoldItalic = true
            };

            var markdown = new Markdown();

            var html = markdown.Transform(markdownText);

            return new HtmlString(html);
        }
    }
}
