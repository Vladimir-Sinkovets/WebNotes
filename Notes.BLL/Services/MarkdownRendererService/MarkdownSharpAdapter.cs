using Microsoft.AspNetCore.Html;
using HeyRed.MarkdownSharp;
using Microsoft.Extensions.Options;
using Notes.BLL.Services.MarkdownRendererService.Options;

namespace Notes.BLL.Services.MarkdownRendererService
{
    public class MarkdownSharpAdapter : IMarkdownRenderer
    {
        private readonly MarkdownSharpOptions _options;

        public MarkdownSharpAdapter(IOptions<MarkdownSharpOptions> options)
        {
            _options = options.Value;
        }

        public IHtmlContent RenderFromMarkdownToHTML(string markdownText)
        {
            var options = new MarkdownOptions()
            {
                AutoHyperlink = _options.AutoHyperlink,
                AutoNewLines = _options.AutoNewLines,
                LinkEmails = _options.LinkEmails,
                QuoteSingleLine = _options.QuoteSingleLine,
                StrictBoldItalic = _options.StrictBoldItalic,
                AllowTargetBlank = _options.AllowTargetBlank,
                AllowEmptyLinkText = _options.AllowEmptyLinkText,
                DisableImages = _options.DisableImages,
                AsteriskIntraWordEmphasis = _options.AsteriskIntraWordEmphasis,
                LinkEmailsWithoutAngleBrackets = _options.LinkEmailsWithoutAngleBrackets,
            };

            var markdown = new Markdown(options);

            var html = markdown.Transform(markdownText);

            return new HtmlString(html);
        }
    }
}
