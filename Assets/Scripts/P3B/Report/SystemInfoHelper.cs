using System.Text;
using UnityEngine;

namespace Cocone.P3B.Test
{
    public static class SystemInfoHelper
    {
        public static void ToMarkdown(MarkdownCreator creator, int size = 2, string title = "SystemInfo")
        {
            creator.Heading(title, size);
            var table = new Table();
            table.CreateRow("deviceModel", SystemInfo.deviceModel);
            table.CreateRow("deviceType", SystemInfo.deviceType);
            table.CreateRow("operatingSystem", SystemInfo.operatingSystem);
            table.CreateRow("processorType", SystemInfo.processorType);
            table.CreateRow("processorFrequency", SystemInfo.processorFrequency);
            table.CreateRow("processorCount", SystemInfo.processorCount);
            table.CreateRow("systemMemorySize", SystemInfo.systemMemorySize);
            table.CreateRow("graphicsDeviceVendor", SystemInfo.graphicsDeviceVendor);
            table.CreateRow("graphicsDeviceName", SystemInfo.graphicsDeviceName);
            table.CreateRow("graphicsDeviceVersion", SystemInfo.graphicsDeviceVersion);
            table.CreateRow("graphicsDeviceType", SystemInfo.graphicsDeviceType);
            table.CreateRow("graphicsMemorySize", SystemInfo.graphicsMemorySize);
            table.CreateRow("graphicsShaderLevel", SystemInfo.graphicsShaderLevel);
            table.CreateRow("maxTextureSize", SystemInfo.maxTextureSize);
            table.CreateRow("supportsComputeShaders", SystemInfo.supportsComputeShaders);
            table.CreateRow("supportedRenderTargetCount", SystemInfo.supportedRenderTargetCount);
            table.CreateRow("supportsInstancing", SystemInfo.supportsInstancing);
            table.CreateRow("supports3DTextures", SystemInfo.supports3DTextures);
            table.CreateRow("supportsShadows", SystemInfo.supportsShadows);
            table.CreateRow("supportsSparseTextures", SystemInfo.supportsSparseTextures);
            table.CreateRow("resolution", $"{Display.main.systemWidth}x{Display.main.systemHeight}");
            table.CreateRow("IL2CPP",
#if ENABLE_IL2CPP
                true
#else
                false
#endif
            );

            var sb = new StringBuilder();
            table.BuildString(sb);
            creator.Paragraph(sb.ToString());
        }
    }
}