using UnityEngine;

namespace Cocone.P3B.Test
{
    public enum TimeType
    {
        CPU,
        GPU,
    }

    public class TimeProfiler : ProfilerBase
    {
        public override string Name => type.ToString();
        public override UnitType Unit => UnitType.Nanosecond;
        public TimeType type { get; private set; }

        private FrameTiming[] frameTimings = new FrameTiming[1];

        public TimeProfiler(TimeType type)
        {
            this.type = type;
#if UNITY_EDITOR
            UnityEditor.PlayerSettings.enableFrameTimingStats = true;
#endif
        }

        protected override long GetCurrentValue()
        {
            FrameTimingManager.CaptureFrameTimings();

            if (FrameTimingManager.GetLatestTimings((uint)frameTimings.Length, frameTimings) > 0)
            {
                if (type == TimeType.CPU)
                {
                    return (long)(frameTimings[0].cpuFrameTime * 1e+6);
                }
                else
                {
                    return (long)(frameTimings[0].gpuFrameTime * 1e+6);
                }
            }
            return 0;
        }
    }
}