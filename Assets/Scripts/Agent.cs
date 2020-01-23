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

        Brain m_Brain;
        int m_Id;

        public ArraySegment<float> m_ActionsVector;
        public ArraySegment<float> m_VisionObservationsVectorArray;

        int m_StepCount;

        Vision m_Vision;
        Animal m_Animal;

        void Awake()
        {
            m_Vision = GetComponent<Vision>();
            m_Animal = GetComponent<Animal>();
        }

        public void Subscribe()
        {
            m_Brain = Academy.Instance.m_Brains[m_BrainName];
            m_Id = m_Brain.SubscribeAgent();
            Academy.Instance.AgentUpdateObservations += UpdateObservations;
            Academy.Instance.AgentUpdateMovement += AgentStep;
            Academy.Instance.AgentUpdateFitness += UpdateFitness;
        }

        public void Init()
        {
            m_VisionObservationsVectorArray = m_Brain.GetVisionObservationsArray(m_Id);
            m_ActionsVector = m_Brain.GetActionsVector(m_Id);

            m_Vision.SetVisionObservationsVectorArray(m_VisionObservationsVectorArray);
        }

        void UpdateObservations()
        {
             m_Vision.UpdateVisionObservations();
        }

        public void UpdateFitness()
        {
            float fitness = m_Animal.collectedFood*10;
            Academy.Instance.m_Brains[m_BrainName].agentsFitness[m_Id] = fitness;
        }

        void AgentStep()
        {
            m_StepCount += 1;

            if (m_ActionsVector == null) return;
            m_Animal.SetMovement(m_ActionsVector.Array[m_ActionsVector.Offset + 0], m_ActionsVector.Array[m_ActionsVector.Offset + 1]);
            m_Animal.AnimalStep();
        }

        private void OnDestroy()
        {
            Academy.Instance.AgentUpdateObservations -= UpdateObservations;
            Academy.Instance.AgentUpdateMovement -= AgentStep;
            Academy.Instance.AgentUpdateFitness -= UpdateFitness;
        }
    }
}
