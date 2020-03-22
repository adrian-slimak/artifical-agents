using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UPC;
using System;

public class HUD : MonoBehaviour
{
    public Text m_FPSText;
    public Text EnvInfo;
    public Text PopInfo;
    Academy m_Academy;

    private void Start()
    {
        m_Academy = FindObjectOfType<Academy>();
        StartCoroutine(FPS());
    }

    public void Update()
    {
        EnvInfo.text = string.Format($"Episode: {m_Academy.m_EpisodeCount}\n" +
                                     $"Steps: {m_Academy.m_StepCount}");

        string text = "";
        string text1 = "\n";
        foreach (Brain brain in m_Academy.brains)
        {
            text += $"Alive {brain.brainName}: {brain.agentsAlive}/{brain.agentsCount}\n";
            text1 += $"Best {brain.brainName}: {brain.bestAgentFitness}\n";
        }

        PopInfo.text = text+text1;
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
}
