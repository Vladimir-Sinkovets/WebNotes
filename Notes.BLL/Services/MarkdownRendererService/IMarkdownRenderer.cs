using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.BLL.Services.MarkdownRendererService
{
    public interface IMarkdownRenderer
    {
        IHtmlContent RenderFromMarkdownToHTML(string markdownText);
    }
}
