﻿using FluentAssertions;
using Notes.BLL.Services.MarkdownRendererService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.BLL.Tests.ServicesTests
{
    public class MarkdownRendererTests
    {
        [Fact]
        public void Should_RenderParagraphs()
        {
            // Arrange
            IMarkdownRenderer renderer = new MarkdownRenderer();
            string text = "The formatting of the output is not important\r\n" +
                          "\r\n" +
                          "good approach to do this";

            // Act
            string htmlText = renderer.RenderFromMarkdownToHTML(text).ToString();

            // Assert
            htmlText.Should().Contain("<p>The formatting of the output is not important</p>")
                .And.Contain("<p>good approach to do this</p>");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        public void Should_RenderHeaderTags(int headerValue)
        {
            // Arrange
            string markdownHeader = new string(Enumerable.Range(0, headerValue)
                .Select(x => '#')
                .ToArray());

            string text = $"{markdownHeader} The formatting of the output is not important\r\n" +
                          "\r\n" +
                          "good approach to do this\r\n" +
                          $"{markdownHeader} good approach to do this";

            IMarkdownRenderer renderer = new MarkdownRenderer();

            // Act
            string htmlText = renderer.RenderFromMarkdownToHTML(text).ToString();

            // Assert
            htmlText.Should().Contain($"<h{headerValue}>The formatting of the output is not important</h{headerValue}>")
                .And
                .Contain($"<h{headerValue}>good approach to do this</h{headerValue}>");
        }
    }
}