﻿using UnityEngine;
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

    float meanDensity;
    float meanDispersity;
    public int totalNumberOfAttacks;
    public int totalFoodCollected;


    public void CreateMemory(int workerID)
    {
        m_Memory = new Memory(m_BrainName, workerID);
    }

    public void Reset()
    {
        VirtualAcademy.Instance.m_ResetParameters.LoadEnvParams(this, m_BrainName);

        if (m_BrainName == "predator")
            if ((VirtualAcademy.Instance.m_ResetParameters[$"predator_communication_enabled"] ?? 0) > 0)
                m_ActionsVectorSize += 1;

        m_Agents = new List<Agent>(agentsCount);
        agentsCount = 0;
        meanDensity = 0;
        meanDispersity = 0;
        totalNumberOfAttacks = 0;
        totalFoodCollected = 0;

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
        if (m_StatsVectorArray == null)
            return;

        m_StatsVectorArray.Zero();

        m_StatsVectorArray[0] = agentsAlive;

        foreach (Agent agent in m_Agents)
        {
            int swarmDensity = 0;
            float swarmDispersion = float.MaxValue;

            //foreach (Agent agent2 in m_Agents)
            //{
            //    if (agent != agent2)
            //    {
            //        float distance = Vector2.Distance(agent.transform.position, agent2.transform.position);
            //        if (distance < 30f)
            //            swarmDensity++;
            //        if (distance < swarmDispersion)
            //            swarmDispersion = distance;
            //    }
            //}

            for(int i=0; i<agent.m_Vision.hitsNum; i++)
            {
                if (agent.m_Vision.hitDistances[i] < 30f)
                    swarmDensity++;
                if (agent.m_Vision.hitDistances[i] < swarmDispersion)
                    swarmDispersion = agent.m_Vision.hitDistances[i];
            }

            m_StatsVectorArray[1] += swarmDensity;
            m_StatsVectorArray[2] += swarmDispersion;

            agent.density = swarmDensity;
            agent.dispersion = swarmDispersion;

            m_StatsVectorArray[3] += agent.m_Animal.collectedFood;
        }

        if (agentsAlive > 0)
        {
            m_StatsVectorArray[1] /= agentsAlive;
            meanDensity += m_StatsVectorArray[1];
            m_StatsVectorArray[2] /= agentsAlive;
            meanDispersity += m_StatsVectorArray[2];
            m_StatsVectorArray[3] /= agentsAlive;
        }
        else
        {
            m_StatsVectorArray[1] = 0;
            m_StatsVectorArray[2] = 0;
            m_StatsVectorArray[3] = 0;
        }


        if (m_BrainName == "predator")
        {
            foreach (Agent agent in m_Agents)
                m_StatsVectorArray[4] += ((Predator)agent.m_Animal).numberOfAttacks;
            if (agentsCount > 0)
                m_StatsVectorArray[4] /= agentsAlive;
        }
    }

    public void UpdateStatsLate()
    {
        m_StatsVectorArray.Zero();

        m_StatsVectorArray[0] = agentsAlive;

        if (VirtualAcademy.Instance.m_StepCount > 1)
        {
            m_StatsVectorArray[1] = meanDensity / VirtualAcademy.Instance.m_StepCount;
            m_StatsVectorArray[2] = meanDispersity / VirtualAcademy.Instance.m_StepCount;
        }
        else
        {
            m_StatsVectorArray[1] = meanDensity;
            m_StatsVectorArray[2] = meanDispersity;
        }

        if (agentsCount>0)
        {
            m_StatsVectorArray[3] = totalFoodCollected / agentsCount;
            m_StatsVectorArray[4] = totalNumberOfAttacks / agentsCount;
        }
        else
        {
            m_StatsVectorArray[3] = totalFoodCollected;
            m_StatsVectorArray[4] = totalNumberOfAttacks;
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
