using System;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;


[System.Serializable]
public class Brain
{
    public string brainName;

    public int observationsVectorSize;
    public int actionsVectorSize;

    [HideInInspector]
    public int agentsCount;
    [HideInInspector]
    public int mmf_offset_observations;
    [HideInInspector]
    public int mmf_size_observations;
    [HideInInspector]
    public int mmf_offset_actions;
    [HideInInspector]
    public int mmf_size_actions;

    [HideInInspector]
    public float[] stackedObservations;
    [HideInInspector]
    public float[] stackedActions;
    [HideInInspector]
    public float[] agentsFitness;

    public void Reset()
    {
        observationsVectorSize = (int)(Academy.Instance.GetResetParameter(brainName + "_observations_vector_size") ?? observationsVectorSize);
        actionsVectorSize = (int)(Academy.Instance.GetResetParameter(brainName + "_actions_vector_size") ?? actionsVectorSize);
        agentsCount = 0;
        mmf_offset_actions = -1;
        mmf_offset_observations = -1;
        mmf_size_actions = -1;
        mmf_size_observations = -1;
        stackedActions = null;
        stackedObservations = null;
    }

    public int[] Init(int[] offset)
    {
        stackedObservations = new float[agentsCount * observationsVectorSize];
        stackedActions = new float[agentsCount * actionsVectorSize];
        agentsFitness = new float[agentsCount];

        mmf_size_observations = 4 * stackedObservations.Length;
        mmf_size_actions = 4 * stackedActions.Length;

        mmf_offset_observations = offset[0];
        mmf_offset_actions = offset[1];

        return new int[] { mmf_size_observations, mmf_size_actions };
    }

    public int SubscribeAgent()
    {
        return agentsCount++;
    }

    public ArraySegment<float> GetObservationsVector(int agent_id)
    {
        return new ArraySegment<float>(stackedObservations, agent_id * observationsVectorSize, observationsVectorSize);
    }

    public ArraySegment<float> GetActionsVector(int agent_id)
    {
        return new ArraySegment<float>(stackedActions, agent_id * actionsVectorSize, actionsVectorSize);
    }
}
