using System;
using UnityEngine;

namespace UPC
{

    public class Agent : MonoBehaviour
    {
        public string m_BrainName;

        Brain m_Brain;
        public int m_Id;

        MMArray m_ActionsVector;
        MMArray m_VisionObservationsVectorArray;
        MMArray m_FitnessArray;

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
            m_ActionsVector = m_Brain.GetActionsArray(m_Id);
            m_FitnessArray = m_Brain.GetFitnessArray(m_Id);

            m_Vision.SetVisionVectorArray(m_VisionObservationsVectorArray);
        }

        void UpdateObservations()
        {
            m_Vision.UpdateVisionObservations();
        }

        public void UpdateFitness()
        {
            float fitness = m_Animal.collectedFood;
            m_FitnessArray[0] = fitness;
            if(fitness>m_Brain.bestAgentFitness)
            {
                m_Brain.bestAgentFitness = fitness;
                m_Brain.bestAgent = this;
            }
        }

        void AgentStep()
        {
            m_StepCount += 1;

            if (m_ActionsVector == null) return;
            m_Animal.SetMovement(m_ActionsVector[0], m_ActionsVector[1]);
            m_Animal.AnimalStep();

            UpdateFitness();
        }

        internal void OnDie()
        {
            m_Brain.agentsAlive--;
            Destroy(this.gameObject);
        }

        private void OnDestroy()
        {
            Academy.Instance.AgentUpdateObservations -= UpdateObservations;
            Academy.Instance.AgentUpdateMovement -= AgentStep;
            Academy.Instance.AgentUpdateFitness -= UpdateFitness;
        }
    }
}
