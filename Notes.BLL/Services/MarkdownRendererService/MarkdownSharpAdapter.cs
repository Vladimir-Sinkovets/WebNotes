using Microsoft.AspNetCore.Html;
using HeyRed.MarkdownSharp;

namespace Notes.BLL.Services.MarkdownRendererService
{
    public class MarkdownSharpAdapter : IMarkdownRenderer
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
