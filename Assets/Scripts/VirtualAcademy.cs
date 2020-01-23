using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class VirtualAcademy : Academy
{
    public GameObject PreyAgentInstance;
    public GameObject PreysHolder;

    public override void AcademyReset()
    {
        base.AcademyReset();
        ResetAgents();
    }

    public void ResetAgents()
    {
        Agent[] oldAgents = FindObjectsOfType<Agent>();
        foreach (Agent agent in oldAgents)
            Destroy(agent.gameObject);

        foreach (Brain brain in brains)
            brain.Reset();

        List<Agent> agents = SpawnAgents();
        foreach (Agent agent in agents)
            agent.Subscribe();

        int[] offset = { 0, 0, 0};
        foreach (Brain brain in brains)
        {
            int[] size = brain.Init(offset);
            offset[0] += size[0];
            offset[1] += size[1];
            offset[2] += size[2];
        }

        foreach (Agent agent in agents)
            agent.Init();
    }

    List<Agent> SpawnAgents()
    {
        int numberOfPreys = (int)(GetResetParameter("preys_count")??0);

        List<Agent> agents = new List<Agent>(numberOfPreys);
        for (int i = 0; i < numberOfPreys; i++)
        {
            Vector2 randomPosition = new Vector2((Random.value - 0.5f) * 100f, (Random.value - 0.5f) * 100f);
            Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            GameObject agent = Instantiate(PreyAgentInstance, randomPosition, randomRotation, PreysHolder.transform);
            agents.Add(agent.GetComponent<Agent>());
        }

        return agents;
    }
}
