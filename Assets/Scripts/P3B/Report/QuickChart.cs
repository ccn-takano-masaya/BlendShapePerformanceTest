using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Cocone.P3B.Test.Chart
{
    // https://quickchart.io/documentation/
    [Serializable]
    public sealed class QuickChart<T> where T : Chart
    {
        public const string BASE_URL = "https://quickchart.io/chart";

        public string version = "2";
        public string backgroundColor = "transparent";
        public int width = 1000;
        public int heigth = 600;
        public string format = "png";
        public T chart;

        public QuickChart()
        {
            chart = Activator.CreateInstance<T>();
        }

        public void SetBackgroundColor(Color color)
        {
            backgroundColor = color.ToHexCode();
        }

        public async UniTask<Texture2D> GetImage()
        {
            var json = JsonUtility.ToJson(this);
            var postData = System.Text.Encoding.UTF8.GetBytes(json);

            using (var request = new UnityWebRequest(BASE_URL, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(postData);
                request.downloadHandler = new DownloadHandlerTexture();
                request.SetRequestHeader("Content-Type", "application/json");

                await request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(request.error);
                    return null;
                }
                else
                {
                    return DownloadHandlerTexture.GetContent(request);
                }
            }  
        }
    }

    [Serializable]
    public class Option
    {
        [Serializable]
        public class Label
        {
            public bool display = false;
            public string labelString;
            public string fontColor = "#666666";

            public void SetColor(Color c)
            {
                fontColor = c.ToHexCode();
                display = true;
            }
        }

        [Serializable]
        public class Scale
        {
            [Serializable]
            public class Axis
            {
                [Serializable]
                public class GridLine
                {
                    public bool display = true;
                    public string color = "#eeeeee";

                    public void SetColor(Color c)
                    {
                        color = c.ToHexCode();
                        display = true;
                    }
                }

                [Serializable]
                public class Tick
                {
                    public bool display = true;
                    public bool beginAtZero = false;
                    public string fontColor = "#666666";

                    public void SetColor(Color c)
                    {
                        fontColor = c.ToHexCode();
                        display = true;
                    }
                }

                public bool display = true;
                public Label scaleLabel = new Label();
                public Tick ticks = new Tick();
                public GridLine gridLines = new GridLine();
            }
            public List<Axis> xAxes;
            public List<Axis> yAxes;

            public Scale()
            {
                xAxes = new List<Axis>() { new Axis() };
                yAxes = new List<Axis>() { new Axis() };
            }
        }

        [Serializable]
        public class Legend
        {
            public bool display = true;
            public Label labels;

            public Legend()
            {
                labels = new Label();
                labels.display = true;
            }
        }

        public Scale scales = new Scale();
        public Legend legend = new Legend();
    }

    [Serializable]
    public abstract class Chart
    {
    }

    [Serializable]
    public sealed class ScatterChart : Chart
    {
        [Serializable]
        public class Data
        {
            [SerializeField] private List<DataSet> datasets = new List<DataSet>();

            public void AddDataSet(DataSet dataSet)
            {
                datasets.Add(dataSet);
            }
        }

        [Serializable]
        public class DataSet
        {
            public string label;
            public bool showLine;
            public bool fill;
            public int lineTension;
            public int borderWidth;
            public string borderColor;
            public int pointRadius;
            [SerializeField] private List<Vector2> data;

            public DataSet()
            {
                label = string.Empty;
                showLine = true;
                fill = false;
                lineTension = 0;
                borderWidth = 2;
                borderColor = "blue";
                pointRadius = 0;
                data = new List<Vector2>();
            }

            public void AddData(Vector2 point)
            {
                data.Add(point);
            }

            public void SetBorderColor(Color color)
            {
                borderColor = color.ToHexCode();
            }
        }

        [SerializeField] private string type = "scatter";
        public Data data = new Data();
        public Option options = new Option();
    }

    public static class ColorExtension
    {
        public static string ToHexCode(this Color color)
        {
            return "#" + ColorUtility.ToHtmlStringRGB(color);
        }

        public static string ToHexCode(this Color32 color)
        {
            return "#" + ColorUtility.ToHtmlStringRGB(color);
        }
    }
}