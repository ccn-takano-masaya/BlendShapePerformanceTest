using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Cocone.P3B.Test
{
    public interface ITestBase
    {
        UniTask<ExitCode> StartTest(string inputJson, string outputPath);
    }

    public abstract class TestBase<T, U> : MonoBehaviour, ITestBase where T : TestInputBase where U : TestOutputBase
    {
        private const string IMAGE_FOLDER = "images";

        private string inputJson, outputPath;
        [NonSerialized] protected T input;
        [NonSerialized] protected U output;
        [NonSerialized] protected List<ProfilerBase> profilers;
        [NonSerialized] protected List<TestCommand> commands;
        [NonSerialized] public bool paused = false;
        public GameObjectController cameraController { get; private set; }
        private Dictionary<string, string> savedImages = new Dictionary<string, string>();
        protected event Action update;

        private string imageFolder => outputPath + "/" + IMAGE_FOLDER;

        public async UniTask<ExitCode> StartTest(string inputJson, string outputPath)
        {
            this.inputJson = inputJson;
            this.outputPath = outputPath;

            var result = await Init();
            if (!result)
            {
                Debug.LogError("Init failed");
                return ExitCode.TestInitFailed;
            }
            result = await RunTest();
            if (!result)
            {
                Debug.LogError("Run Test failed");
                return ExitCode.TestRunFailed;
            }

            result = CreateReport();
            if (!result)
            {
                Debug.LogError("Create Report failed");
                return ExitCode.TestReportFailed;
            }
            Debug.Log("*** end profiler ***");
            return ExitCode.None;
        }

        private void Update()
        {
            if (!paused && profilers != null)
            {
                foreach (var profiler in profilers)
                {
                    profiler.Tick();
                }
            }
            update?.Invoke();
        }

        protected void StartProfiler()
        {
            foreach (var profiler in profilers)
            {
                profiler.Start();
            }
        }

        protected void StopProfiler()
        {
            foreach (var profiler in profilers)
            {
                profiler.Stop();
            }
        }

        protected virtual async UniTask<bool> Init()
        {
            Debug.Log(outputPath);
            if (Directory.Exists(outputPath))
            {
                var di = new DirectoryInfo(outputPath);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            else
            {
                Directory.CreateDirectory(outputPath);
            }
            Directory.CreateDirectory(imageFolder);
            
            input = JsonUtility.FromJson<T>(inputJson);
            output = Activator.CreateInstance<U>();
            return true;
        }

        protected virtual async UniTask<bool> RunTest()
        {
            var camera = Camera.main;
            cameraController = camera.gameObject.AddComponent<GameObjectController>();
            if(input != null)
                input.ApplySettings();
            await UniTask.NextFrame();
            return true;
        }

        protected virtual bool CreateReport()
        {
            output.Save(outputPath);
            return true;
        }

        public async UniTask<string> TakeScreenShot(string name)
        {
            await UniTask.WaitForEndOfFrame(this);
            var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            var bytes = texture.EncodeToPNG();
            var path = $"{imageFolder}/{name}.png";
            File.WriteAllBytes(path, bytes);
            Destroy(texture);
            var relativePath = $"./{IMAGE_FOLDER}/{name}.png";
            savedImages[name] = relativePath;
            return relativePath;
        }

        public string SaveImage(Texture2D texture, string name)
        {
            var bytes = texture.EncodeToPNG();
            var path = $"{imageFolder}/{name}.png";
            File.WriteAllBytes(path, bytes);
            var relativePath = $"./{IMAGE_FOLDER}/{name}.png";
            savedImages[name] = relativePath;
            return relativePath;
        }

        public string GetImagePath(string name)
        {
            if (savedImages.TryGetValue(name, out var path)) {
                return path;
            }
            return null;
        }
    }
}
