using System.Text;
using UnityEngine;

namespace Cocone.P3B.Test
{
    public static class ApplicationHelper
    {
        public static void ToMarkdown(MarkdownCreator creator, int size = 2, string title = "Application")
        {
            creator.Heading(title, size);
            var table = new Table();
            table.CreateRow("unityVersion", Application.unityVersion);
            table.CreateRow("version", Application.version);
            table.CreateRow("platform", Application.platform);
            table.CreateRow("identifier", Application.identifier);
            table.CreateRow("systemLanguage", Application.systemLanguage);
            table.CreateRow("targetFrameRate", Application.targetFrameRate);

            var sb = new StringBuilder();
            table.BuildString(sb);
            creator.Paragraph(sb.ToString());
        }
    }
}