using System.Text;
using Unity.Profiling;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.MessageBox;

public class PerformanceAnalyzeProfilerRecorder: MonoBehaviour
{
    [SerializeField] int m_fontSize = 32;

    string statsText;
    ProfilerRecorder _totalReservedMemoryRecorder;
    ProfilerRecorder _gcReservedMemoryRecorder;
    ProfilerRecorder _systemUsedMemoryRecorder;
    ProfilerRecorder _textureMemoryRecorder;
    ProfilerRecorder _totalUsedMemoryRecorder;

    //描画処理用
    private ProfilerRecorder _setPassCallsRecorder;
    private ProfilerRecorder _drawCallsRecorder;
    private ProfilerRecorder _trianglesRecorder;
    private ProfilerRecorder _verticesRecorder;

    private GUIStyle _style;
    private GUIStyleState _styleState;

    private void Start()
    {
        _style = new GUIStyle();
        _style.fontSize = m_fontSize;

        _styleState = new GUIStyleState();
        _styleState.textColor = Color.white;
        _style.normal = _styleState;

        Debug.Log($"vSyncCount before={QualitySettings.vSyncCount} targetFrameRate={Application.targetFrameRate}");
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        Debug.Log($"vSyncCount after={QualitySettings.vSyncCount} targetFrameRate={Application.targetFrameRate}");
    }


    void OnEnable()
    {
        _totalReservedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Reserved Memory");
        _gcReservedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory");
        _systemUsedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory"); //アプリ全体のメモリ使用量
        _textureMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Texture Memory");
        _totalUsedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Used Memory");

        _setPassCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count");
        _drawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
        _trianglesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count");
        _verticesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Vertices Count");
    }

    void OnDisable()
    {
        _totalReservedMemoryRecorder.Dispose();
        _gcReservedMemoryRecorder.Dispose();
        _systemUsedMemoryRecorder.Dispose();
        _textureMemoryRecorder.Dispose();
        _totalUsedMemoryRecorder.Dispose();

        _setPassCallsRecorder.Dispose();
        _drawCallsRecorder.Dispose();
        _trianglesRecorder.Dispose();
        _verticesRecorder.Dispose();
    }

    void Update()
    {
#if false
        var sb = new StringBuilder(500);
        if (totalReservedMemoryRecorder.Valid)
            sb.AppendLine($"Total Reserved Memory: {totalReservedMemoryRecorder.LastValue}");
        if (gcReservedMemoryRecorder.Valid)
            sb.AppendLine($"GC Reserved Memory: {gcReservedMemoryRecorder.LastValue}");
        if (systemUsedMemoryRecorder.Valid)
            sb.AppendLine($"System Used Memory: {systemUsedMemoryRecorder.LastValue}");
        statsText = sb.ToString();

        m_totalMemory.text = $"Total Reserved Memory: {totalReservedMemoryRecorder.LastValue / 1024 / 1024}";
#endif
    }

    void OnGUI()
    {
        float base_x = 0, base_y = 320;

        //描画情報の表示
        string graphicAPITypeString = "other";
        switch (SystemInfo.graphicsDeviceType)
        {
            case GraphicsDeviceType.Direct3D11:
                graphicAPITypeString = "Direct3D11";
                break;
            case GraphicsDeviceType.Direct3D12:
                graphicAPITypeString = "Direct3D12";
                break;
            case GraphicsDeviceType.OpenGLES3:
                graphicAPITypeString = "OpenGLES3";
                break;
            case GraphicsDeviceType.OpenGLES2:
                graphicAPITypeString = "OpenGLES2";
                break;
            case GraphicsDeviceType.Vulkan:
                graphicAPITypeString = "Vulkan";
                break;
            case GraphicsDeviceType.Metal:
                graphicAPITypeString = "Metal";
                break;
        }

        //メモリ情報の表示
        int y = 0;
        DispMessage(base_x, base_y + (m_fontSize * y), $"GraphicAPI: {graphicAPITypeString}"); y++;
        DispMessage(base_x, base_y + (m_fontSize * y), $"Total Reserved Memory: {_totalReservedMemoryRecorder.LastValue / 1024.0f / 1024:F2} mb"); y++;
        DispMessage(base_x, base_y + (m_fontSize * y), $"GC Reserved Memory: {_gcReservedMemoryRecorder.LastValue / 1024.0f / 1024:F2} mb"); y++;
        DispMessage(base_x, base_y + (m_fontSize * y), $"System Used Memory: {_systemUsedMemoryRecorder.LastValue / 1024.0f / 1024:F2} mb"); y++;
        DispMessage(base_x, base_y + (m_fontSize * y), $"Texture Memory: {_textureMemoryRecorder.LastValue / 1024.0f / 1024:F2}  mb"); y++;
        DispMessage(base_x, base_y + (m_fontSize * y), $"Total Used Memory: {_totalUsedMemoryRecorder.LastValue / 1024.0f / 1024:F2} mb" ); y++;

        
#if false
        GUI.TextArea(new Rect(10, 30, 250, 50), statsText);
#endif
    }

    private void DispMessage(float x,float y,string message)
    {
        GUI.Label(new Rect(x, y, 600, m_fontSize), message, _style);
    }
}