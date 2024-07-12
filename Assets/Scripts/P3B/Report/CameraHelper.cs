using System.Text;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Cocone.P3B.Test
{
    public static class CameraHelper
    {
        public static void ToMarkdown(MarkdownCreator creator, int size = 2, string title = "Camera")
        {
            creator.Heading(title, size);
            var table = new Table();
            var camera = Camera.main;
            var cameraData = camera.GetUniversalAdditionalCameraData();
            table.CreateRow("requiresColorTexture", cameraData.requiresColorTexture);
            table.CreateRow("requiresDepthTexture", cameraData.requiresDepthTexture);
            table.CreateRow("renderPostProcessing", cameraData.renderPostProcessing);
            table.CreateRow("renderShadows", cameraData.renderShadows);
            table.CreateRow("antialiasing", cameraData.antialiasing);
            if (cameraData.antialiasing != AntialiasingMode.None)
            {
                table.CreateRow("antialiasingQuality", cameraData.antialiasingQuality);
            }
            table.CreateRow("stopNaN", cameraData.stopNaN);
            table.CreateRow("dithering", cameraData.dithering);
            table.CreateRow("allowHDR", camera.allowHDR);
            table.CreateRow("allowMSAA", camera.allowMSAA);
            table.CreateRow("clearFlags", camera.clearFlags);
            table.CreateRow("fieldOfView", camera.fieldOfView);
            table.CreateRow("nearClipPlane", camera.nearClipPlane);
            table.CreateRow("farClipPlane", camera.farClipPlane);

            var sb = new StringBuilder();
            table.BuildString(sb);
            creator.Paragraph(sb.ToString());
        }
    }
}