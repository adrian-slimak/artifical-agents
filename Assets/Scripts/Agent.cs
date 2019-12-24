using System;
using System.Collections.Generic;
using UnityEngine;
using Barracuda;
using UnityEngine.Serialization;

namespace MLAgents
{

    public class Agent : MonoBehaviour
    {
        public string m_BrainName;

        int m_Id;

        public ArraySegment<float> m_ActionsVector;
        public ArraySegment<float> m_ObservationsVector;

        int m_StepCount;

        Academy m_Academy;
        Vision m_Vision;
        Animal m_Animal;

        void Awake()
        {
            m_Id = Academy.m_Brains[m_BrainName].SubscribeAgent();
            m_Vision = GetComponent<Vision>();
            m_Animal = GetComponent<Animal>();
            m_Academy = FindObjectOfType<Academy>();
        }

        void OnEnable()
        {
            OnEnableHelper(m_Academy);
        }

        void OnEnableHelper(Academy academy)
        {

            if (academy == null)
                throw new Exception("No Academy Component could be found in the scene.");

            academy.AgentUpdateObservations += UpdateObservations;
            academy.AgentUpdateMovement += AgentStep;
        }

        void Start()
        {
            m_ObservationsVector = Academy.m_Brains[m_BrainName].GetObservationsVector(m_Id);
            m_ActionsVector = Academy.m_Brains[m_BrainName].GetActionsVector(m_Id);
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
