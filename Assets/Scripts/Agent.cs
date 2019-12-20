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

    public class Agent : MonoBehaviour
    {
        public String brainName;
        AgentAction m_Action;

        int m_Id;

        public ArraySegment<float> m_ActionsVector;
        public ArraySegment<float> m_ObservationsVector;
        public static int m_observationsSize = 100;
        public static int m_actionsSize = 32;

        int m_StepCount;

        Academy m_Academy;
        Vision m_Vision;
        Animal m_Animal;

        void Awake()
        {
            m_Id = Academy.m_Brains[brainName].SubscribeAgent();
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

            academy.AgentUpdateObservations += UpdateObservations;
            academy.AgentUpdateMovement += AgentStep;
            ResetData();
        }

        void Start()
        {
            m_ObservationsVector = Academy.m_Brains[brainName].GetObservationsVector(m_Id);
            m_ActionsVector = Academy.m_Brains[brainName].GetActionsVector(m_Id);
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
            m_Animal.SetMovement(m_ActionsVector.Array[m_ActionsVector.Offset + 0], m_ActionsVector.Array[m_ActionsVector.Offset + 1]);
        }

        void AgentStep()
        {
            AgentAction();
            m_StepCount += 1;
        }
    }
}
