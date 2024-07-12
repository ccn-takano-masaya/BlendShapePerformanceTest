using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cocone.P3B.Test
{
    public enum ProfilerStatistic
    {
        Min,
        Max,
        OnePercentLow,
        ZeroPointOnePercentLow,
        OnePercentHigh,
        ZeroPointOnePercentHigh,
        Avg = 1 << 4,
    }

    public enum UnitType
    {
        Byte,
        Number,
        Nanosecond,
    }

    public abstract partial class ProfilerBase
    {
        public abstract string Name { get; }
        public abstract UnitType Unit { get; }
        public bool enabled { get; private set; } = false;
        public List<long> values { get; private set; }

        public void Start(int capacity = 0)
        {
            if (values == null || values.Capacity < capacity)
            {
                values = new List<long>(capacity);
            }
            else
            {
                values.Clear();
            }
            enabled = true;
        }

        public void Tick()
        {
            if (enabled)
            {
                values.Add(this.GetCurrentValue());
            }
        }

        public void Stop()
        {
            enabled = false;
        }

        protected abstract long GetCurrentValue();

        public virtual double GetStatisticValue(ProfilerStatistic statistic)
        {
            if (values.Count == 0) return 0;
            switch (statistic)
            {
                case ProfilerStatistic.Min:
                    return values.Min();
                case ProfilerStatistic.Max:
                    return values.Max();
                case ProfilerStatistic.OnePercentLow:
                    {
                        var copied = new List<long>(values);
                        copied.Sort();
                        var sum = 0.0;
                        var count = Mathf.CeilToInt((float)copied.Count / 100);
                        for (int i = 0; i < count; i++)
                        {
                            sum += copied[i];
                        }
                        return sum / count;
                    }
                case ProfilerStatistic.ZeroPointOnePercentLow:
                    {
                        var copied = new List<long>(values);
                        copied.Sort();
                        var sum = 0.0;
                        var count = Mathf.CeilToInt((float)copied.Count / 1000);
                        for (int i = 0; i < count; i++)
                        {
                            sum += copied[i];
                        }
                        return sum / count;
                    }
                case ProfilerStatistic.OnePercentHigh:
                    {
                        var copied = new List<long>(values);
                        copied.Sort((a, b) => b.CompareTo(a));
                        var sum = 0.0;
                        var count = Mathf.CeilToInt((float)copied.Count / 100);
                        for (int i = 0; i < count; i++)
                        {
                            sum += copied[i];
                        }
                        return sum / count;
                    }
                case ProfilerStatistic.ZeroPointOnePercentHigh:
                    {
                        var copied = new List<long>(values);
                        copied.Sort((a, b) => b.CompareTo(a));
                        var sum = 0.0;
                        var count = Mathf.CeilToInt((float)copied.Count / 1000);
                        for (int i = 0; i < count; i++)
                        {
                            sum += copied[i];
                        }
                        return sum / count;
                    }
                case ProfilerStatistic.Avg:
                    return values.Average();
                default:
                    return 0;
            }
        }

        public virtual string GetStatisticValueString(ProfilerStatistic statistic)
        {
            if (Unit == UnitType.Number)
            {
                return FormatNumber(GetStatisticValue(statistic));
            }
            else if (Unit == UnitType.Byte)
            {
                return FormatBytes(GetStatisticValue(statistic));
            }
            else
            {
                return $"{GetStatisticValue(statistic) * 1e-6:F2} ms";
            }
        }
    }

    public partial class ProfilerBase
    {
        private const long BASE = 1000;
        private const long KILO = BASE;
        private const long MEGA = KILO * BASE;
        private const long GIGA = MEGA * BASE;
        private const long TERA = GIGA * BASE;

        public static string FormatNumber(double value)
        {
            return FormatNumber(Convert.ToInt64(value));
        }

        public static string FormatNumber(long value)
        {
            if (value >= TERA)
            {
                return $"{(float)value / TERA:F1}T";
            }
            else if (value >= GIGA)
            {
                return $"{(float)value / GIGA:F1}G";
            }
            else if (value >= MEGA)
            {
                return $"{(float)value / MEGA:F1}M";
            }
            else if (value >= KILO)
            {
                return $"{(float)value / KILO:F1}k";
            }
            return value.ToString();
        }

        private const long BASE_BYTES = 1024;
        private const long KILO_BYTES = BASE_BYTES;
        private const long MEGA_BYTES = KILO_BYTES * BASE_BYTES;
        private const long GIGA_BYTES = MEGA_BYTES * BASE_BYTES;
        private const long TERA_BYTES = GIGA_BYTES * BASE_BYTES;

        public static string FormatBytes(double value)
        {
            return FormatBytes(Convert.ToInt64(value));
        }

        public static string FormatBytes(long value)
        {
            if (value >= TERA_BYTES)
            {
                return $"{(float)value / TERA_BYTES:F2}TB";
            }
            else if (value >= GIGA_BYTES)
            {
                return $"{(float)value / GIGA_BYTES:F2}GB";
            }
            else if (value >= MEGA_BYTES)
            {
                return $"{(float)value / MEGA_BYTES:F2}MB";
            }
            else if (value >= KILO_BYTES)
            {
                return $"{(float)value / KILO_BYTES:F2}KB";
            }
            return $"{value}B";
        }
    }
}