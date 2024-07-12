using System.Collections.Generic;
using System.Text;
using Cocone.P3B.Test.Chart;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Cocone.P3B.Test
{
    public static class ProfilerHelper
    {
        public static void ToMarkdown(MarkdownCreator creator, ProfilerBase profiler, int size = 2)
        {
            creator.Heading(profiler.Name, size);
            if (profiler.Unit == UnitType.Nanosecond)
            {
                if (profiler is TimeProfiler timeProfiler && timeProfiler.type == TimeType.CPU)
                {
                    var avg = profiler.GetStatisticValue(ProfilerStatistic.Avg) * 1e-6;
                    var onePercentHigh = profiler.GetStatisticValue(ProfilerStatistic.OnePercentHigh) * 1e-6;
                    var zeroPointOnePercentHigh = profiler.GetStatisticValue(ProfilerStatistic.ZeroPointOnePercentHigh) * 1e-6;
                    creator.Table(
                        new string[] { "Average", "1% Low", "0.1% Low" },
                        new string[] {
                            $"{1000 / avg:F0} fps [{avg:F2} ms]",
                            $"{1000 / onePercentHigh:F0} fps [{onePercentHigh:F2} ms]",
                            $"{1000 / zeroPointOnePercentHigh:F0} fps [{zeroPointOnePercentHigh:F2} ms]"
                        });
                }
                else
                {
                    creator.Table(
                    new string[] { "Average", "1% High", "0.1% High" },
                    new string[] {
                        profiler.GetStatisticValueString(ProfilerStatistic.Avg),
                        profiler.GetStatisticValueString(ProfilerStatistic.OnePercentHigh),
                        profiler.GetStatisticValueString(ProfilerStatistic.ZeroPointOnePercentHigh)
                    });
                }  
            }
            else
            {
                creator.Table(
                    new string[] { "Average", "Maximum" },
                    new string[] { profiler.GetStatisticValueString(ProfilerStatistic.Avg), profiler.GetStatisticValueString(ProfilerStatistic.Max) });
            }
        }

        public static void ToMarkdown(MarkdownCreator creator, ProfilerBase[] profilers, string title = null, int size = 2)
        {
            if (!string.IsNullOrEmpty(title))
            {
                creator.Heading(title, size);
            }

            var table = new Table();
            

            if (profilers[0].Unit == UnitType.Nanosecond)
            {
                var row = new TableRow();
                row.AddChild(new TableHeader());
                row.AddChild(new TableHeader("Average"));
                row.AddChild(new TableHeader("1% High"));
                row.AddChild(new TableHeader("0.1% High"));
                table.AddChild(row);

                for (int i = 0; i < profilers.Length; i++)
                {
                    var profiler = profilers[i];
                    row = new TableRow();
                    row.AddChild(new TableHeader(profiler.Name));
                    row.AddChild(new TableCell(profiler.GetStatisticValueString(ProfilerStatistic.Avg)));
                    row.AddChild(new TableCell(profiler.GetStatisticValueString(ProfilerStatistic.OnePercentHigh)));
                    row.AddChild(new TableCell(profiler.GetStatisticValueString(ProfilerStatistic.ZeroPointOnePercentHigh)));
                    table.AddChild(row);
                }
            }
            else
            {
                var row = new TableRow();
                row.AddChild(new TableHeader());
                row.AddChild(new TableHeader("Average"));
                row.AddChild(new TableHeader("Maximum"));
                table.AddChild(row);

                for (int i = 0; i < profilers.Length; i++)
                {
                    var profiler = profilers[i];
                    row = new TableRow();
                    row.AddChild(new TableHeader(profiler.Name));
                    row.AddChild(new TableCell(profiler.GetStatisticValueString(ProfilerStatistic.Avg)));
                    row.AddChild(new TableCell(profiler.GetStatisticValueString(ProfilerStatistic.Max)));
                    table.AddChild(row);
                }
            }

            var sb = new StringBuilder();
            table.BuildString(sb);
            creator.Paragraph(sb.ToString());
        }

        public static readonly Color32 backgroundColor = Color.black;
        public static readonly Color32[] lableColors = new Color32[]
        {
            new Color32(130, 157, 48, 255),
            new Color32(76, 134, 164, 255),
            new Color32(192, 117, 41, 255),
            new Color32(112, 176, 186, 255),
            new Color32(112, 104, 34, 255),
            new Color32(191, 163, 54, 255),
            new Color32(126, 47, 21, 255),
        };
        public static readonly Color32 gridColor = new Color32(102, 102, 102, 255);
        public static readonly Color32 fontColor = new Color32(163, 163, 163, 255);

        public static async UniTask<Texture2D> GetChartImage(ProfilerBase profiler, IList<float> xAxis, int width = 1000, int height = 600)
        {
            var quickChart = new QuickChart<ScatterChart>();
            var scatterChart = quickChart.chart;
            quickChart.width = width;
            quickChart.heigth = height;
            quickChart.SetBackgroundColor(backgroundColor);
            var xAxes = scatterChart.options.scales.xAxes[0];
            xAxes.scaleLabel.SetColor(fontColor);
            xAxes.scaleLabel.labelString = "Time (ms)";
            xAxes.gridLines.SetColor(gridColor);

            var yAxes = scatterChart.options.scales.yAxes[0];
            yAxes.scaleLabel.SetColor(fontColor);
            yAxes.scaleLabel.labelString =
                profiler.Unit == UnitType.Byte ? "MB" :
                profiler.Unit == UnitType.Number ? "Count" : "ms";
            yAxes.gridLines.SetColor(gridColor);

            scatterChart.options.legend.labels.SetColor(fontColor);

            var dataset = new ScatterChart.DataSet();
            dataset.label = profiler.Name;
            dataset.SetBorderColor(lableColors[0]);
            var multiplicand =
                profiler.Unit == UnitType.Byte ? 1 / (1024f * 1024f) :
                profiler.Unit == UnitType.Number ? 1 : 1e-6f;
            for (int i = 0; i < profiler.values.Count; i++)
            {
                dataset.AddData(new Vector2(xAxis[i], profiler.values[i] * multiplicand));
            }
            scatterChart.data.AddDataSet(dataset);

            return await quickChart.GetImage();
        }

        public static async UniTask<Texture2D> GetChartImage(ProfilerBase[] profilers, IList<float> xAxis, int width = 1000, int height = 600)
        {
            var quickChart = new QuickChart<ScatterChart>();
            var scatterChart = quickChart.chart;
            quickChart.width = width;
            quickChart.heigth = height;
            quickChart.SetBackgroundColor(backgroundColor);
            var xAxes = scatterChart.options.scales.xAxes[0];
            xAxes.scaleLabel.SetColor(fontColor);
            xAxes.scaleLabel.labelString = "Time (ms)";
            xAxes.gridLines.SetColor(gridColor);

            var yAxes = scatterChart.options.scales.yAxes[0];
            yAxes.scaleLabel.SetColor(fontColor);
            yAxes.scaleLabel.labelString =
                profilers[0].Unit == UnitType.Byte ? "MB" :
                profilers[0].Unit == UnitType.Number ? "Count" : "ms";
            yAxes.gridLines.SetColor(gridColor);

            scatterChart.options.legend.labels.SetColor(fontColor);

            var multiplicand =
                profilers[0].Unit == UnitType.Byte ? 1 / (1024f * 1024f) :
                profilers[0].Unit == UnitType.Number ? 1 : 1e-6f;
            for (int i = 0; i < profilers.Length; i++)
            {
                var profiler = profilers[i];
                var dataset = new ScatterChart.DataSet();
                dataset.label = profiler.Name;
                dataset.SetBorderColor(lableColors[i % lableColors.Length]);

                for (int j = 0; j < profiler.values.Count; j++)
                {
                    dataset.AddData(new Vector2(xAxis[j], profiler.values[j] * multiplicand));
                }
                scatterChart.data.AddDataSet(dataset);
            }
            return await quickChart.GetImage();
        }
    }
}