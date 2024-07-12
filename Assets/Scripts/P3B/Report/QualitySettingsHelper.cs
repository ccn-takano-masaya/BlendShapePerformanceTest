using System.Text;
using UnityEngine;

namespace Cocone.P3B.Test
{
    public static class QualitySettingsHelper
    {
        public static void ToMarkdown(MarkdownCreator creator, int size = 2, string title = "QualitySettings")
        {
            creator.Heading(title, size);
            var table = new Table();
            table.CreateRow("anisotropicFiltering", QualitySettings.anisotropicFiltering);
            table.CreateRow("globalTextureMipmapLimit", QualitySettings.globalTextureMipmapLimit);
            table.CreateRow("skinWeights", QualitySettings.skinWeights);
            table.CreateRow("activeColorSpace", QualitySettings.activeColorSpace);

            var sb = new StringBuilder();
            table.BuildString(sb);
            creator.Paragraph(sb.ToString());
        }
    }
}