using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerformanceDisp : MonoBehaviour
{
    public Text m_cpuFrameTimeText;
    public Text m_cpuMainThreadFrameText;
    public Text m_cpuRenderThreadFrameText;
    public Text m_cpuMainThreadPresentTimeText;
    public Text m_gpuFrameTimeText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_cpuFrameTimeText.text = $"cpuFrame:{PerformanceAnalyze.Instance.CpuFrameTime:F3} ms";
        m_cpuMainThreadFrameText.text = $"cpuMainThread:{PerformanceAnalyze.Instance.CpuMainThreadFrameTime:F3} ms";
        m_cpuRenderThreadFrameText.text = $"cpuRenderThread:{PerformanceAnalyze.Instance.CpuRenderThreadFrameTime:F3} ms";
        m_cpuMainThreadPresentTimeText.text = $"cpuPresentTime:{PerformanceAnalyze.Instance.CpuMainThreadPresentTime:F3} ms";
        m_gpuFrameTimeText.text = $"gpuTime:{PerformanceAnalyze.Instance.GpuFrameTime:F2} ms";
    }
}
