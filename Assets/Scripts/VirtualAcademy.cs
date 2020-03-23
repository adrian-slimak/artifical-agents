using System.Collections.Generic;
using UnityEngine;
using UPC;

public class VirtualAcademy : Academy
{
    public GameObject PreyAgentInstance;
    public GameObject PreysHolder;

    public GameObject PredatorAgentInstance;
    public GameObject PredatorsHolder;

    public override void AcademyReset()
    {
        base.AcademyReset();
        ResetAgents();
    }

    public void ResetAgents()
    {
        foreach (Transform oldAgent in PreysHolder.transform)
            Destroy(oldAgent.gameObject);
        foreach (Transform oldAgent in PredatorsHolder.transform)
            Destroy(oldAgent.gameObject);

        foreach (Brain brain in brains)
            brain.Reset();

        List<Agent> agents = SpawnAgents();

        foreach (Brain brain in brains)
            brain.Init();

        foreach (Agent agent in agents)
            agent.Init();
    }

    List<Agent> SpawnAgents()
    {
        int numberOfPreys = (int)(m_ResetParameters["prey_count"] ?? 0);

        List<Agent> agents = new List<Agent>(numberOfPreys);
        for (int i = 0; i < numberOfPreys; i++)
        {
            Vector2 randomPosition = new Vector2((Random.value - 0.5f) * 100f, (Random.value - 0.5f) * 100f);
            Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            GameObject agent = Instantiate(PreyAgentInstance, randomPosition, randomRotation, PreysHolder.transform);
            agents.Add(agent.GetComponent<Agent>());
        }

        int numberOfPredators = (int)(m_ResetParameters["predator_count"] ?? 0);

        for (int i = 0; i < numberOfPredators; i++)
        {
            Vector2 randomPosition = new Vector2((Random.value - 0.5f) * 100f, (Random.value - 0.5f) * 100f);
            Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            GameObject agent = Instantiate(PredatorAgentInstance, randomPosition, randomRotation, PredatorsHolder.transform);
            agents.Add(agent.GetComponent<Agent>());
        }

        return agents;
    }
}
