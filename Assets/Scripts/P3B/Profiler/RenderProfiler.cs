using System.Collections.Generic;
using Unity.Profiling;

namespace Cocone.P3B.Test
{
    // https://docs.unity3d.com/2022.2/Documentation/Manual/ProfilerRendering.html
    public enum RenderType
    {
        SetPassCallsCount,
        DrawCallsCount,
        TrianglesCount,
        VerticesCount,
        UsedTexturesCount,
        UsedTexturesBytes,
        RenderTexturesCount,
        RenderTexturesBytes,
        RenderTexturesChangesCount,
        UsedBuffersCount,
        UsedBuffersBytes,
        VertexBufferUploadInFrameCount,
        VertexBufferUploadInFrameBytes,
        IndexBufferUploadInFrameCount,
        IndexBufferUploadInFrameBytes,
    }

    public class RenderProfiler : ProfilerBase
    {
        private static Dictionary<RenderType, string> nameMapping = new Dictionary<RenderType, string>()
        {
            [RenderType.SetPassCallsCount] = "SetPass Calls Count",
            [RenderType.DrawCallsCount] = "Draw Calls Count",
            [RenderType.TrianglesCount] = "Triangles Count",
            [RenderType.VerticesCount] = "Vertices Count",
            [RenderType.UsedTexturesCount] = "Used Textures Count",
            [RenderType.UsedTexturesBytes] = "Used Textures Bytes",
            [RenderType.RenderTexturesCount] = "Render Textures Count",
            [RenderType.RenderTexturesBytes] = "Render Textures Bytes",
            [RenderType.RenderTexturesChangesCount] = "Render Textures Changes Count",
            [RenderType.UsedBuffersCount] = "Used Buffers Count",
            [RenderType.UsedBuffersBytes] = "Used Buffers Bytes",
            [RenderType.VertexBufferUploadInFrameCount] = "Vertex Buffer Upload In Frame Count",
            [RenderType.VertexBufferUploadInFrameBytes] = "Vertex Buffer Upload In Frame Bytes",
            [RenderType.IndexBufferUploadInFrameCount] = "Index Buffer Upload In Frame Count",
            [RenderType.IndexBufferUploadInFrameBytes] = "Index Buffer Upload In Frame Bytes",
        };

        public static Dictionary<RenderType, UnitType> unitMapping = new Dictionary<RenderType, UnitType>()
        {
            [RenderType.SetPassCallsCount] = UnitType.Number,
            [RenderType.DrawCallsCount] = UnitType.Number,
            [RenderType.TrianglesCount] = UnitType.Number,
            [RenderType.VerticesCount] = UnitType.Number,
            [RenderType.UsedTexturesCount] = UnitType.Number,
            [RenderType.UsedTexturesBytes] = UnitType.Byte,
            [RenderType.RenderTexturesCount] = UnitType.Number,
            [RenderType.RenderTexturesBytes] = UnitType.Byte,
            [RenderType.RenderTexturesChangesCount] = UnitType.Number,
            [RenderType.UsedBuffersCount] = UnitType.Number,
            [RenderType.UsedBuffersBytes] = UnitType.Byte,
            [RenderType.VertexBufferUploadInFrameCount] = UnitType.Number,
            [RenderType.VertexBufferUploadInFrameBytes] = UnitType.Byte,
            [RenderType.IndexBufferUploadInFrameCount] = UnitType.Number,
            [RenderType.IndexBufferUploadInFrameBytes] = UnitType.Byte,
        };

        public override string Name => nameMapping[type];
        public override UnitType Unit => unitMapping[type];

        private ProfilerRecorder recorder;
        private RenderType type;

        public RenderProfiler(RenderType type)
        {
            this.type = type;
            recorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, nameMapping[type]);
        }

        protected override long GetCurrentValue()
        {
            return recorder.LastValue;
        }
    }
}