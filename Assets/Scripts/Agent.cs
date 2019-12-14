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

        int m_Id;

        public ArraySegment<float> m_ActionsVector;
        public ArraySegment<float> m_ObservationsVector;
        public static int m_observationsSize = 20;
        public static int m_actionsSize = 32;

        int m_StepCount;

        Academy m_Academy;
        Vision m_Vision;
        Animal m_Animal;

        public static int m_TotalAgentsCreated = 0;

        void Awake()
        {
            m_Id = m_TotalAgentsCreated;
            m_TotalAgentsCreated++;
        }

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

            if (academy == null)
                throw new Exception("No Academy Component could be found in the scene.");

            academy.m_Agents.Add(this);

            academy.AgentUpdateObservations += UpdateObservations;
            academy.AgentUpdateMovement += AgentStep;
            ResetData();
        }

        void Start()
        {
            m_ObservationsVector = new ArraySegment<float>(m_Academy.m_StackedObservations, m_Id * m_observationsSize, m_observationsSize);
            m_ActionsVector = new ArraySegment<float>(m_Academy.m_StackedActions, m_Id * m_actionsSize, m_actionsSize);
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
                m_Vision.UpdateVisionObservations();
            }

        }

        public void AgentAction()
        {
            if (m_ActionsVector == null) return;
            Debug.Log(m_ActionsVector.Array[m_ActionsVector.Offset + 0]);
            m_Animal.SetMovement(m_ActionsVector.Array[m_ActionsVector.Offset + 0], m_ActionsVector.Array[m_ActionsVector.Offset + 1]);
        }

        void AgentStep()
        {
            AgentAction();
            m_StepCount += 1;
        }
    }
}
