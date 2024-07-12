using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Cocone.P3B.Test
{
    public sealed class MarkdownCreator
    {
        private const int INDENT_SIZE = 4;
        private StringBuilder sb;
        private int indent = 0;
        private string indentSpace => new string(' ', indent * INDENT_SIZE);

        public MarkdownCreator()
        {
            sb = new StringBuilder();
        }

        public void TOC()
        {
            sb.AppendLine("[TOC]");
        }

        public void Heading(string text, int level)
        {
            level = Mathf.Clamp(level, 1, 4);
            sb.AppendLine(new string('#', level) + " " + text);
        }

        public void HorizontalLine()
        {
            sb.AppendLine("----");
        }

        public void Paragraph(string text)
        {
            var tokens = text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (var token in tokens)
            {
                sb.Append(indentSpace).AppendLine(token);
            }
            sb.AppendLine();
        }

        public string Bold(string text)
        {
            return "**" + text + "**";
        }

        public string Link(string text, string url)
        {
            return $"[{text}]({url})";
        }

        public string InlineImage(string src, string alt = "", int width = 0, int height = 0)
        {
            var sb = new StringBuilder();
            sb.Append($"<img src=\"{src}\" alt=\"{alt}\"");
            if (width > 0) sb.Append($" width=\"{width}\"");
            if (height > 0) sb.Append($" height=\"{height}\"");
            sb.Append(">");
            return sb.ToString();
        }

        public void Image(string src, string alt = "", int width = 0, int height = 0)
        {
            sb.Append(indentSpace).AppendLine(InlineImage(src, alt, width, height)).AppendLine();
        }

        public void UnorderedListItem(string text)
        {
            sb.AppendLine(indentSpace + "- " + text);
        }

        public void OrderedListItem(string text)
        {
            sb.AppendLine(indentSpace + "1. " + text);
        }

        public void Table(IList<string> headers, IList<string> cells)
        {
            sb.Append(indentSpace).Append("| ");
            for (int i = 0; i < headers.Count; i++)
            {
                sb.Append(headers[i]).Append(" |");
            }
            sb.AppendLine();

            sb.Append(indentSpace).Append("|");
            for (int i = 0; i < headers.Count; i++)
            {
                sb.Append(" --- |");
            }
            
            for (int i = 0; i < cells.Count; i++)
            {
                if (i % headers.Count == 0)
                {
                    sb.AppendLine().Append(indentSpace).Append("| ");
                }
                sb.Append(cells[i]).Append(" |");
            }
            sb.AppendLine().AppendLine();
        }

        public string InLineCode(string code)
        {
            return "`" + code + "`";
        }

        public void Code(string lang, string code)
        {
            sb.AppendLine(indentSpace + "```" + lang);
            sb.AppendLine(indentSpace + code);
            sb.AppendLine(indentSpace + "```");
            sb.AppendLine();
        }

        public void NewLine()
        {
            sb.AppendLine();
        }

        public string BuildString()
        {
            return sb.ToString();
        }
    }
}