using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UPC;
using System;

public class HUD : MonoBehaviour
{
    public Text m_FPSText;
    public Text text;
    float deltaTime = 0.0f;
    Academy m_Academy;

    private void Start()
    {
        m_Academy = FindObjectOfType<Academy>();
        StartCoroutine(FPS());
    }

    public void Update()
    {
        text.text = string.Format($"Episode: {m_Academy.m_EpisodeCount}\n" +
                                  $"Steps: {m_Academy.m_StepCount}");
    }

    private IEnumerator FPS()
    {
        for (; ; )
        {
            // Capture frame-per-second
            int lastSteps = m_Academy.m_StepCount;
            long lastTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            yield return new WaitForSeconds(0.5f);
            long deltaTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - lastTime;
            int deltaSteps = m_Academy.m_StepCount - lastSteps;

            // Display it
            float fps = deltaSteps / (deltaTime/1000f);
            m_FPSText.text = string.Format("{0:0.0} steps/s", fps);
        }
    }

    void UpdateFPS()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        float msec = deltaTime * 1000.0f;
        string text = string.Format("{0:0.0} ms ({1:0.0} fps)", msec, fps);
        m_FPSText.text = text;
    }
}
