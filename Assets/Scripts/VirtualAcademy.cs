using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class VirtualAcademy : Academy
{
    public GameObject PreyAgentInstance;
    public int numberOfPreys = 100;
    public GameObject PreysHolder;

    public override void AcademyReset()
    {
        //GameObject parent = new GameObject("Preys Holder");
        //for (int i = 0; i < numberOfPreys; i++)
        //{
        //    Vector2 randomPosition = new Vector2(Random.value * 100f, Random.value * 100f);
        //    Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        //    Instantiate(PreyAgentInstance, randomPosition, randomRotation, parent.transform);
        //}
    }

    public override void AcademyInitialization()
    {
        for (int i = 0; i < numberOfPreys; i++)
        {
            Vector2 randomPosition = new Vector2(Random.value * 100f, Random.value * 100f);
            Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            Instantiate(PreyAgentInstance, randomPosition, randomRotation, PreysHolder.transform);
        }
    }
}
