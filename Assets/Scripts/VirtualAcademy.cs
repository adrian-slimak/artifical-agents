using System.Collections.Generic;
using UnityEngine;
using UPC;

public class VirtualAcademy : Academy
{

    public new static VirtualAcademy Instance;

    public GameObject PreyAgentInstance;
    public GameObject PreysHolder;

    public GameObject PredatorAgentInstance;
    public GameObject PredatorsHolder;

    public GameObject WorldGround;

    [Parameter("environment_world_size")]
    public int m_WorldSize = 100;
    public int m_HalfWorldSize;

    public override void OnAwakeAcademyInitialization()
    {
        Instance = Academy.Instance as VirtualAcademy;
    }

    protected override void OnAcademyReset()
    {
        m_ResetParameters.LoadEnvParams(this);

        m_HalfWorldSize = m_WorldSize / 2;
        WorldGround.transform.localScale = new Vector3(m_WorldSize, m_WorldSize, 1);

        PlantsSpawner.Instance.OnReset();

        RemoveAgents();
        SpawnPreys();
    }

    public void RemoveAgents()
    {
        foreach (Transform oldAgent in PreysHolder.transform)
            Destroy(oldAgent.gameObject);
        foreach (Transform oldAgent in PredatorsHolder.transform)
            Destroy(oldAgent.gameObject);

        foreach (Brain brain in brains)
            brain.Reset();
    }

    void SpawnPreys()
    {
        int numberOfPreys = (int)(m_ResetParameters["prey_count"] ?? 0);

        List<Agent> preys = SpawnAgents(PreyAgentInstance, numberOfPreys, PreysHolder);

        m_Brains["prey"].Init();

        foreach (Agent agent in preys)
            agent.Init();
    }

    protected override void PredatorSpawnStep()
    {
        SpawnPredators();
    }

    void SpawnPredators()
    {
        int numberOfPredators = (int)(m_ResetParameters["predator_count"] ?? 0);

        List<Agent> predators = SpawnAgents(PredatorAgentInstance, numberOfPredators, PredatorsHolder);

        m_Brains["predator"].Init();

        foreach (Agent agent in predators)
            agent.Init();
    }

    List<Agent> SpawnAgents(GameObject agentInstance, int count, GameObject holder)
    {
        List<Agent> agents = new List<Agent>(count);

        for (int i = 0; i < count; i++)
        {
            Vector2 randomPosition = new Vector2((Random.value - 0.5f) * m_WorldSize, (Random.value - 0.5f) * m_WorldSize);
            Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            GameObject agent = Instantiate(agentInstance, randomPosition, randomRotation, holder.transform);
            agents.Add(agent.GetComponent<Agent>());
        }

        return agents;
    }
}