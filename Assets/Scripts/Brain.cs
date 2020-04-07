using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Brain
{
    public string m_BrainName;

    [HideInInspector]
    [Parameter("observations_vector_size")]
    public int m_ObservationsVectorSize;
    [Parameter("observations_vision_vector_size")]
    public int visionObservationsVectorSize;
    [Parameter("observations_hearing_vector_size")]
    public int hearingObservationsVectorSize;
    [Parameter("actions_vector_size")]
    public int m_ActionsVectorSize;
    [Parameter("fitness_vector_size")]
    public int m_FitnessVectorSize;
    [Parameter("stats_vector_size")]
    public int m_StatisticsVectorSize;

    public MMArray m_StatsVectorArray;

    [HideInInspector]
    public int agentsCount;

    [HideInInspector]
    public int agentsAlive;
    [HideInInspector]
    public float bestAgentFitness;
    [HideInInspector]
    public Agent bestAgent;

    List<Agent> m_Agents;

    Memory m_Memory;


    public void CreateMemory(int workerID)
    {
        m_Memory = new Memory(m_BrainName, workerID);
    }

    public void Reset()
    {
        VirtualAcademy.Instance.m_ResetParameters.LoadEnvParams(this, m_BrainName);

        if (m_BrainName == "predator")
            if ((VirtualAcademy.Instance.m_ResetParameters[$"predator_confusion_effect_value"] ?? 0) > 0)
                m_ActionsVectorSize += 1;

        m_Agents = new List<Agent>(agentsCount);
        agentsCount = 0;

        m_Memory.Reset();
    }

    public void Init()
    {
        agentsAlive = agentsCount;
        bestAgentFitness = -1;
        bestAgent = null;

        m_Memory.Init(agentsCount, m_ObservationsVectorSize, m_ActionsVectorSize, m_FitnessVectorSize, m_StatisticsVectorSize);
        m_StatsVectorArray = m_Memory.GetStatsMemoryArray();
    }

    public int SubscribeAgent(Agent agent)
    {
        m_Agents.Add(agent);
        return agentsCount++;
    }

    public void UnsubscribeAgent(Agent agent)
    {
        agentsAlive--;
        m_Agents.Remove(agent);
    }

    public void UpdateStats()
    {
        m_StatsVectorArray.Zero();

        m_StatsVectorArray[0] = agentsAlive;

        foreach (Agent agent in m_Agents)
        {
            int swarmDensity = agent.m_Hearing.GetNearObjects(30f, agent.gameObject.tag);
            m_StatsVectorArray[1] += swarmDensity;

            float swarmDispersion = float.MaxValue;
            if(agent.m_Hearing!=null)
                swarmDispersion = agent.m_Hearing.GetDistanceToClosest(agent.gameObject.tag);
            if (swarmDispersion == float.MaxValue)
                foreach(Agent agent2 in m_Agents)
                {
                    float distance = Vector2.Distance(agent.transform.position, agent2.transform.position);
                    if (distance < swarmDispersion)
                        swarmDispersion = distance;
                }
            m_StatsVectorArray[2] += swarmDispersion;
        }

        m_StatsVectorArray[1] /= agentsAlive;
        m_StatsVectorArray[2] /= agentsAlive;

        if (m_BrainName == "predator")
        {
            foreach (Agent agent in m_Agents)
                m_StatsVectorArray[3] += ((Predator)agent.m_Animal).numberOfAttacks;
            m_StatsVectorArray[3] /= agentsAlive;
        }
    }

    public MMArray GetVisionObservationsArray(int agent_id)
    {
        return m_Memory.GetObservationsMemoryArray(agent_id * m_ObservationsVectorSize, visionObservationsVectorSize);
    }

    public MMArray GetHearingObservationsArray(int agent_id)
    {
        return m_Memory.GetObservationsMemoryArray(agent_id * m_ObservationsVectorSize + visionObservationsVectorSize, hearingObservationsVectorSize);
    }

    public MMArray GetActionsArray(int agent_id)
    {
        return m_Memory.GetActionsMemoryArray(agent_id * m_ActionsVectorSize, m_ActionsVectorSize);
    }

    internal MMArray GetFitnessArray(int agent_id)
    {
        return m_Memory.GetFitnessMemoryArray(agent_id * m_FitnessVectorSize, m_FitnessVectorSize);
    }

    MMArray GetStatsArray()
    {
        return m_Memory.GetStatsMemoryArray();
    }
}
