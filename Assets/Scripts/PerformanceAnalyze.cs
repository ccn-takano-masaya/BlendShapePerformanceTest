using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1フレームあたりの処理時間を計測するクラス
/// </summary>
public class PerformanceAnalyze : MonoBehaviour
{
    private static PerformanceAnalyze _instance;
    public static PerformanceAnalyze Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject(nameof(PerformanceAnalyze)).AddComponent<PerformanceAnalyze>();
            }
            return _instance;
        }
    }

    /// <summary>
    /// CPUの1フレーム当たりの処理時間
    /// </summary>
    public float CpuFrameTime { get; private set; }
    public float CpuMainThreadFrameTime { get; private set; }
    public float CpuRenderThreadFrameTime { get; private set; }
    public float CpuMainThreadPresentTime { get; private set; }
    /// <summary>
    /// GPUの1フレーム当たりの処理時間
    /// </summary>
    public float GpuFrameTime { get; private set; }

    private FrameTiming[] _frameTimings = new FrameTiming[1];

    [SerializeField] float _captureTimeInterval = 0.5f;
    private float _currentCaputureTime = 0;

    private void Update()
    {
        _currentCaputureTime += Time.deltaTime;
        if (_currentCaputureTime < _captureTimeInterval)
        {
            return;
        }

        // フレーム情報をキャプチャする
        FrameTimingManager.CaptureFrameTimings();

        // 必要なフレーム数分の情報を取得する
        // 戻り値は実際に取得できたフレーム情報の数
        var numFrames = FrameTimingManager.GetLatestTimings((uint)_frameTimings.Length, _frameTimings);
        if (numFrames == 0) // 2020.02.16修正しました
        {
            // 1フレームの情報も得られていない場合はスキップ
            return;
        }

        // CPUの処理時間、CPUの処理時間を格納
        CpuFrameTime = (float)(_frameTimings[0].cpuFrameTime);
        CpuMainThreadFrameTime = (float)(_frameTimings[0].cpuMainThreadFrameTime);
        CpuRenderThreadFrameTime = (float)(_frameTimings[0].cpuRenderThreadFrameTime);
        CpuMainThreadPresentTime = (float)(_frameTimings[0].cpuMainThreadPresentWaitTime);
        GpuFrameTime = (float)(_frameTimings[0].gpuFrameTime);

        _currentCaputureTime = 0;
    }
}

