using UnityEngine;

public class Agent : MonoBehaviour
{
    public string m_BrainName;

    Brain m_Brain;
    public int m_ID;

    MMArray m_FitnessArray;

    Animal m_Animal;
    Vision m_Vision;
    Hearing m_Hearing;

    void Awake()
    {
        AttachBrain();

        m_Animal = GetComponent<Animal>();
        m_Vision = GetComponent<Vision>();

        if (m_Brain.hearingObservationsVectorSize > 0)
            m_Hearing = GetComponent<Hearing>();
        else
            Destroy(GetComponent<Hearing>());
    }

    public void AttachBrain()
    {
        m_Brain = Academy.Instance.m_Brains[m_BrainName];
        m_ID = m_Brain.SubscribeAgent();

        Academy.Instance.AgentUpdateObservations += UpdateObservations;
        Academy.Instance.AgentUpdateMovement += AgentStep;
        Academy.Instance.AgentUpdateFitness += UpdateFitness;
    }

    public void Init()
    {
        m_FitnessArray = m_Brain.GetFitnessArray(m_ID);

        m_Animal.SetActionsVector(m_Brain.GetActionsArray(m_ID));
        m_Vision.SetObservationsVector(m_Brain.GetVisionObservationsArray(m_ID));
        if (m_Hearing)
            m_Hearing.SetObservationsVector(m_Brain.GetHearingObservationsArray(m_ID));
    }

    void UpdateObservations()
    {
        m_Vision.UpdateObservations();
        if (m_Hearing)
            m_Hearing.UpdateObservations();
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
        m_Animal.UpdateMovement();
        m_Animal.AnimalStep();

        UpdateFitness();
    }

    internal void OnDie()
    {
        UpdateFitness();
        m_Brain.OnAgentDie();
    }

    private void OnDestroy()
    {
        Academy.Instance.AgentUpdateObservations -= UpdateObservations;
        Academy.Instance.AgentUpdateMovement -= AgentStep;
        Academy.Instance.AgentUpdateFitness -= UpdateFitness;
    }
}
