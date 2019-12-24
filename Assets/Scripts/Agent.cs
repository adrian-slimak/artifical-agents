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

        Vision m_Vision;
        Animal m_Animal;

        void Awake()
        {
            m_Vision = GetComponent<Vision>();
            m_Animal = GetComponent<Animal>();
        }

        public void Subscribe(Academy academy)
        {
            m_Id = Academy.m_Brains[m_BrainName].SubscribeAgent();
            academy.AgentUpdateObservations += UpdateObservations;
            academy.AgentUpdateMovement += AgentStep;
        }

        public void Init(Academy academy)
        {
            m_ObservationsVector = Academy.m_Brains[m_BrainName].GetObservationsVector(m_Id);
            m_ActionsVector = Academy.m_Brains[m_BrainName].GetActionsVector(m_Id);

            m_Vision.SetObservationsVectorArray(m_ObservationsVector);
        }

        void UpdateObservations()
        {
             m_Vision.UpdateVisionObservations();
        }

        void AgentStep()
        {
            m_StepCount += 1;

            if (m_ActionsVector == null) return;
            m_Animal.SetMovement(m_ActionsVector.Array[m_ActionsVector.Offset + 0], m_ActionsVector.Array[m_ActionsVector.Offset + 1]);
        }
    }
}
