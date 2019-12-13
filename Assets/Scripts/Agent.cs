using System;
using System.Collections.Generic;
using UnityEngine;
using Barracuda;
using UnityEngine.Serialization;

namespace MLAgents
{
    public struct AgentAction
    {
        public float[] vectorActions;
        public float value;
    }

    public class Observation
    {
        public float[] observations;
    }

    public class Agent : MonoBehaviour
    {
        AgentAction m_Action;

        public ArraySegment<float> FloatData;
        public static int m_observationsSize = 20;
        Observation m_Observations;

        int m_StepCount;

        Academy m_Academy;

        Vision m_Vision;
        Animal m_Animal;

        public static int m_TotalAgentsCreated = 0;

        void OnEnable()
        {
            m_Vision = GetComponent<Vision>();
            m_Animal = GetComponent<Animal>();
            m_Academy = FindObjectOfType<Academy>();

            OnEnableHelper(m_Academy);
        }

        void OnEnableHelper(Academy academy)
        {
            m_Action = new AgentAction();
            m_Observations = new Observation();

            if (academy == null)
                throw new Exception("No Academy Component could be found in the scene.");

            academy.m_Agents.Add(this);

            academy.AgentUpdateObservations += UpdateObservations;
            academy.AgentUpdateMovement += AgentStep;
            ResetData();
        }

        void ResetData()
        {
            if (m_Action.vectorActions == null)
            {
                m_Action.vectorActions = new float[20];
            }
        }

        void UpdateObservations()
        {

            using (TimerStack.Instance.Scoped("CollectObservations"))
            {
                CollectObservations();
            }

        }

        public virtual void CollectObservations()
        {
            float[] observations = m_Vision.GetVisionVector();
            FloatData = new ArraySegment<float>(observations, 0, observations.Length);
            m_Observations.observations = observations;
        }

        public void AgentAction(float[] vectorAction)
        {
            if (vectorAction == null) return;
            m_Animal.SetMovement(vectorAction[0], vectorAction[1]);
        }

        public void UpdateAgentAction(AgentAction action)
        {
            m_Action = action;
        }

        void AgentStep()
        {
            AgentAction(m_Action.vectorActions);
            m_StepCount += 1;
        }
    }
}
