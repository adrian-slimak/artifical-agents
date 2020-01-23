using System;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;


[System.Serializable]
public class Brain
{
    public string brainName;

    [HideInInspector]
    public int observationsVectorSize;
    public int visionObservationsVectorSize;
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
    public int mmf_offset_fitness;
    [HideInInspector]
    public int mmf_size_fitness;

    [HideInInspector]
    public float[] stackedObservations;
    [HideInInspector]
    public float[] stackedActions;
    [HideInInspector]
    public float[] agentsFitness;

    public void Reset()
    {
        visionObservationsVectorSize = (int)(Academy.Instance.GetResetParameter(brainName + "_vision_observations_vector_size") ?? visionObservationsVectorSize);
        actionsVectorSize = (int)(Academy.Instance.GetResetParameter(brainName + "_actions_vector_size") ?? actionsVectorSize);
        observationsVectorSize = visionObservationsVectorSize;
        agentsCount = 0;
        mmf_offset_actions = -1;
        mmf_offset_observations = -1;
        mmf_offset_fitness = -1;
        mmf_size_actions = -1;
        mmf_size_observations = -1;
        mmf_size_fitness = -1;
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
        mmf_size_fitness = 4 * agentsCount;

        mmf_offset_observations = offset[0];
        mmf_offset_actions = offset[1];
        mmf_offset_fitness = offset[2];

        return new int[] { mmf_size_observations, mmf_size_actions, mmf_size_fitness};
    }

    public int SubscribeAgent()
    {
        return agentsCount++;
    }

    public ArraySegment<float> GetVisionObservationsArray(int agent_id)
    {
        return new ArraySegment<float>(stackedObservations, agent_id * observationsVectorSize, visionObservationsVectorSize);
    }

    public ArraySegment<float> GetActionsVector(int agent_id)
    {
        return new ArraySegment<float>(stackedActions, agent_id * actionsVectorSize, actionsVectorSize);
    }
}
