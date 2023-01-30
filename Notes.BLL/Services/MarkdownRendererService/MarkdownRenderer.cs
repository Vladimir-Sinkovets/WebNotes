using Microsoft.AspNetCore.Html;
using HeyRed.MarkdownSharp;
using Microsoft.Extensions.Options;
using Notes.BLL.Services.MarkdownRendererService.Options;
using System;
using System.Text.RegularExpressions;
using Notes.DAL.Repositories;
using Notes.BLL.Services.NoteManagers;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Notes.BLL.Services.MarkdownRendererService
{
    public class MarkdownRenderer : IMarkdownRenderer
    {
        private readonly MarkdownRendererOptions _options;
        private readonly INoteManager _noteManager;
        private readonly ILogger<MarkdownRenderer> _logger;

        public MarkdownRenderer(IOptions<MarkdownRendererOptions> options, INoteManager noteManager, ILogger<MarkdownRenderer> logger)
        {
            _options = options.Value;

            _noteManager = noteManager;

            _logger = logger;
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

            var htmlWithLinks = RenderLinks(html);

            return new HtmlString(htmlWithLinks);
        }

        private string RenderLinks(string html)
        {
            var linkRgx = new Regex(@"\[\[(/?[^>]+?)]\]");

            var matches = linkRgx.Matches(html);

            foreach (Match match in matches)
            {
                var noteTitle = match.Value[2..^2];

                var note = _noteManager.GetAllNotes()
                    .FirstOrDefault(n => n.Title == noteTitle);

                var href = note != null ? $"/Note/Read/{note?.Id}" : "";

                html = html.Replace(match.Value, $"<a href=\"{href}\">{noteTitle}</a>");
            }

            _logger.LogDebug($"{matches.Count} links were rendered");

            return html;
        }
    }
}
