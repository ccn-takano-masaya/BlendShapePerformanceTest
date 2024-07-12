using System;
using System.IO;
using UnityEngine;
using Ftol.Avatar;

namespace Cocone.P3B.Test
{
    public enum TestType
    {
        CollectionRoom,
        Field,
        ArtToy,
        LightCrossfade,
        MetallicShader,
    }

    public enum ExitCode
    {
        None = 0,
        ParseArgumentsFailed = 1,
        ParseTestTypeFailed = 2,
        TestTypeNotSupported = 3,

        TestInitFailed = 10,
        TestRunFailed = 11,
        TestReportFailed = 12,
    }

    public sealed class TestController : MonoBehaviour
    {
        private const string INPUT_KEY = "input";
        private const string OUTPUT_KEY = "output";

        [Serializable]
        private class InputType
        {
            public string type;
        }

        public GameObject worldCameraSet;
        public UnityEngine.Rendering.Volume[] volumes;

        public static TestController Instance;

        private void Awake()
        {
            Instance = this;
        }

        private async void Start()
        {
            string inputJson = "", outputPath = "";
            if (!ParseArguments(out inputJson, out outputPath))
            {
                Exit(ExitCode.ParseArgumentsFailed);
                return;
            }

            var inputType = JsonUtility.FromJson<InputType>(inputJson);
            if (!Enum.TryParse<TestType>(inputType.type, out var selectedType))
            {
                Exit(ExitCode.ParseTestTypeFailed);
                return;
            }

            //アバターデータのロード、クリエイト
            await AvatarData.LoadAvatarDatas();
            var ftolFashionManager = FtolFashionManager.GetInstance();
            for (int i = 0; i < 3; i++)
                ftolFashionManager.AddAvator();

            ITestBase test;
            switch(selectedType)
            {
                case TestType.CollectionRoom:
                    test = gameObject.AddComponent<CollectionRoomTest>();
                    break;
                case TestType.Field:
                    test = gameObject.AddComponent<FieldTest>();
                    break;
                case TestType.LightCrossfade:
                    test = gameObject.AddComponent<LightCrossfadeTest>();
                    break;
                case TestType.MetallicShader:
                    test = gameObject.AddComponent<MetallicShaderTest>();
                    break;
                default:
                    Exit(ExitCode.TestTypeNotSupported);
                    return;
            }
            //test = gameObject.AddComponent<CollectionRoomTest>();

            var exitCode = await test.StartTest(inputJson, outputPath);
            Exit(exitCode);
        }

        private bool ParseArguments(out string inputJson, out string outputPath)
        {
#if UNITY_EDITOR
#if (PROFILE_COMMENT)
            var inputPath = "D:/work/project/unity/BlendShapePerformanceTest/Assets/Config/input.json";
            outputPath = "D:/work/output";
#else
            var inputPath = "/Users/kuo_ming-nsun/Projects/p3b-performance-test/input.json";
            outputPath = "/Users/kuo_ming-nsun/Projects/p3b-performance-test/output";
#endif
#elif UNITY_ANDROID
            var UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var intent = currentActivity.Call<AndroidJavaObject>("getIntent");
            var inputFile = intent.Call<string>("getStringExtra", INPUT_KEY);
            var inputPath = Application.temporaryCachePath + "/" + inputFile;
            var outputFolder = intent.Call<string>("getStringExtra", OUTPUT_KEY);
            outputPath = Application.temporaryCachePath + "/" + outputFolder;
#elif UNITY_IOS
            var inputFile = Environment.GetEnvironmentVariable(INPUT_KEY);
            var outputFolder = Environment.GetEnvironmentVariable(OUTPUT_KEY);
            if (string.IsNullOrEmpty(inputFile) || string.IsNullOrEmpty(outputFolder))
            {
                inputJson = null;
                outputPath = null;
                return false;
            }
            var inputPath = Application.temporaryCachePath + "/" + inputFile;
            outputPath = Application.temporaryCachePath + "/" + outputFolder;
#else
            string inputPath = null;
            inputJson = null;
            outputPath = null;
            Debug.LogError($"'{Application.platform}' is not supported");
            return false;
#endif
            inputJson = File.ReadAllText(inputPath);
            return true;
        }

        private void Exit(ExitCode exitCode = ExitCode.None)
        {
            if (exitCode != ExitCode.None)
            {
                Debug.LogError("Exit Code " + exitCode);
            }
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit((int)exitCode);
#endif
        }
    }
}