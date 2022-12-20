using Microsoft.AspNetCore.Html;

namespace Notes.BLL.Services.MarkdownRendererService
{
    public interface IMarkdownRenderer
    {
        IHtmlContent RenderFromMarkdownToHTML(string markdownText);
    }
}