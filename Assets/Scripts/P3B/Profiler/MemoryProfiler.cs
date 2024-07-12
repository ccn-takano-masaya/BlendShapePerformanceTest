using System.Collections.Generic;
using Unity.Profiling;

namespace Cocone.P3B.Test
{
    // https://docs.unity3d.com/2022.2/Documentation/Manual/ProfilerMemory.html
    public enum MemoryType
    {
        SystemUsedMemory,
        TotalUsedMemory,
        TotalReservedMemory,
        GCUsedMemory,
        GCReservedMemory,
        TextureCount,
        TextureMemory,
        MeshCount,
        MeshMemory,
        MaterialCount,
        MaterialMemory,
    }

    public class MemoryProfiler : ProfilerBase
    {
        public static Dictionary<MemoryType, string> nameMapping = new Dictionary<MemoryType, string>()
        {
            [MemoryType.SystemUsedMemory] = "System Used Memory",
            [MemoryType.TotalUsedMemory] = "Total Used Memory",
            [MemoryType.TotalReservedMemory] = "Total Reserved Memory",
            [MemoryType.GCUsedMemory] = "GC Used Memory",
            [MemoryType.GCReservedMemory] = "GC Reserved Memory",
            [MemoryType.TextureCount] = "Texture Count",
            [MemoryType.TextureMemory] = "Texture Memory",
            [MemoryType.MeshCount] = "Mesh Count",
            [MemoryType.MeshMemory] = "Mesh Memory",
            [MemoryType.MaterialCount] = "Material Count",
            [MemoryType.MaterialMemory] = "Material Memory",
        };

        public static Dictionary<MemoryType, UnitType> unitMapping = new Dictionary<MemoryType, UnitType>()
        {
            [MemoryType.SystemUsedMemory] = UnitType.Byte,
            [MemoryType.TotalUsedMemory] = UnitType.Byte,
            [MemoryType.TotalReservedMemory] = UnitType.Byte,
            [MemoryType.GCUsedMemory] = UnitType.Byte,
            [MemoryType.GCReservedMemory] = UnitType.Byte,
            [MemoryType.TextureCount] = UnitType.Number,
            [MemoryType.TextureMemory] = UnitType.Byte,
            [MemoryType.MeshCount] = UnitType.Number,
            [MemoryType.MeshMemory] = UnitType.Byte,
            [MemoryType.MaterialCount] = UnitType.Number,
            [MemoryType.MaterialMemory] = UnitType.Byte,
        };

        public override string Name => nameMapping[type];
        public override UnitType Unit => unitMapping[type];

        private ProfilerRecorder recorder;
        private MemoryType type;

        public MemoryProfiler(MemoryType type)
        {
            this.type = type;
            recorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, nameMapping[type]);
        }

        protected override long GetCurrentValue()
        {
            return recorder.LastValue;
        }
    }
}