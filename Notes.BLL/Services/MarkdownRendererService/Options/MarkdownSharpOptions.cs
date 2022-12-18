namespace Notes.BLL.Services.MarkdownRendererService.Options
{
    public class MarkdownSharpOptions
    {
        public bool AllowTargetBlank { get; set; }
        public bool AllowEmptyLinkText { get; set; } = true;
        public bool DisableImages { get; set; }
        public bool QuoteSingleLine { get; set; }
        public bool AutoHyperlink { get; set; }
        public bool AutoNewLines { get; set; }
        public bool LinkEmails { get; set; } = true;
        public bool StrictBoldItalic { get; set; }
        public bool AsteriskIntraWordEmphasis { get; set; }
        public bool LinkEmailsWithoutAngleBrackets { get; set; }
    }
}
