using UnityEngine;


[System.Serializable]
public class Brain
{
    public string brainName;

    [HideInInspector]
    public int observationsVectorSize;
    public int visionObservationsVectorSize;
    public int hearingObservationsVectorSize;
    public int actionsVectorSize;
    public int fitnessVectorSize = 1;

    [HideInInspector]
    public int agentsCount;

    [HideInInspector]
    public int agentsAlive;
    [HideInInspector]
    public float bestAgentFitness;
    [HideInInspector]
    public Agent bestAgent;

    Memory m_Memory;


    public void CreateMemory(int workerID)
    {
        m_Memory = new Memory(brainName, workerID);
    }

    public void Reset()
    {
        visionObservationsVectorSize = (int)(Academy.Instance.m_ResetParameters[brainName + "_observations_vision_vector_size"] ?? visionObservationsVectorSize);
        hearingObservationsVectorSize = (int)(Academy.Instance.m_ResetParameters[brainName + "_observations_hearing_vector_size"] ?? hearingObservationsVectorSize);
        observationsVectorSize = (int)(Academy.Instance.m_ResetParameters[brainName + "_observations_vector_size"]);

        actionsVectorSize = (int)(Academy.Instance.m_ResetParameters[brainName + "_actions_vector_size"] ?? actionsVectorSize);

        //fitnessVectorSize = (int)(Academy.Instance.m_ResetParameters[brainName + "_fitness_vector_size"] ?? fitnessVectorSize);

        agentsCount = 0;

        m_Memory.Reset();
    }

    public void Init()
    {
        agentsAlive = agentsCount;
        bestAgentFitness = -1;
        bestAgent = null;

        m_Memory.Init(agentsCount, observationsVectorSize, actionsVectorSize, fitnessVectorSize);
    }

    public int SubscribeAgent()
    {
        return agentsCount++;
    }

    public void OnAgentDie()
    {
        agentsAlive--;
    }

    public MMArray GetVisionObservationsArray(int agent_id)
    {
        return m_Memory.GetObservationsMemoryArray(agent_id * observationsVectorSize, visionObservationsVectorSize);
    }

    public MMArray GetHearingObservationsArray(int agent_id)
    {
        return m_Memory.GetObservationsMemoryArray(agent_id * observationsVectorSize + visionObservationsVectorSize, hearingObservationsVectorSize);
    }

    public MMArray GetActionsArray(int agent_id)
    {
        return m_Memory.GetActionsMemoryArray(agent_id * actionsVectorSize, actionsVectorSize);
    }

    internal MMArray GetFitnessArray(int agent_id)
    {
        return m_Memory.GetFitnessMemoryArray(agent_id * fitnessVectorSize, fitnessVectorSize);
    }
}
