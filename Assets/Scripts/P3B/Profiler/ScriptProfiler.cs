using UnityEngine.Profiling;

namespace Cocone.P3B.Test
{
    public class ScriptProfiler : ProfilerBase
    {
        public override string Name => marker;
        public override UnitType Unit => UnitType.Nanosecond;

        private string marker;
        private Recorder recorder;

        public ScriptProfiler(string marker)
        {
            this.marker = marker;
            recorder = Recorder.Get(marker);
            recorder.enabled = true;
        }

        protected override long GetCurrentValue()
        {
            return recorder.elapsedNanoseconds;
        }
    }
}