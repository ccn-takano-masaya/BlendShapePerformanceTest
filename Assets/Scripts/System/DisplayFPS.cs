using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayFPS : MonoBehaviour
{
    [SerializeField] Text _fpsText;

    // �ϐ�
    int frameCount;
    float prevTime;
    float fps;

    // ����������
    void Start()
    {
        frameCount = 0;
        prevTime = 0.0f;
    }

    // �X�V����
    void Update()
    {
        frameCount++;
        float time = Time.realtimeSinceStartup - prevTime;

        if (time >= 0.5f)
        {
            fps = frameCount / time;
            //Debug.Log(fps);

            frameCount = 0;
            prevTime = Time.realtimeSinceStartup;

            _fpsText.text = $"FPS:{fps}";
        }
    }
}
