using UnityEngine;

public class Agent : MonoBehaviour
{
    public string m_BrainName;

    internal Brain m_Brain;
    public int m_ID;

    MMArray m_FitnessArray;

    internal Animal m_Animal;
    internal Vision m_Vision;
    internal Hearing m_Hearing;

    public float dispersion = 0f;
    public float density = 0f;

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
        m_ID = m_Brain.SubscribeAgent(this);

        Academy.Instance.AgentUpdateObservations += UpdateObservations;
        Academy.Instance.AgentUpdateMovement += AgentStep;
        Academy.Instance.AgentUpdateFitness += UpdateFitness;
    }

    public void Init()
    {
        m_FitnessArray = m_Brain.GetFitnessArray(m_ID);
        m_FitnessArray.Zero();

        m_Animal.SetActionsVector(m_Brain.GetActionsArray(m_ID));
        m_Vision.SetObservationsVector(m_Brain.GetVisionObservationsArray(m_ID));
        m_Hearing?.SetObservationsVector(m_Brain.GetHearingObservationsArray(m_ID));
    }

    void UpdateObservations()
    {
        using (TimerStack.Instance.Scoped("AgentUpdateObservations"))
            m_Vision.UpdateObservations();
        m_Hearing?.UpdateObservations();
    }

    void UpdateFitness()
    {
        //float fitness = m_Animal.collectedFood;
        if (m_Animal.m_Type == Animal.AnimalType.Prey)
            m_FitnessArray[0] = m_Animal.collectedFood + VirtualAcademy.Instance.m_StepCount / 15f;
        else
        {
            m_FitnessArray[0] = m_FitnessArray[0] + m_Animal.collectedFood;// + ((Predator)m_Animal).numberOfAttacks;
            Debug.Log(m_FitnessArray[0]);
        }

        if(m_FitnessArray[0] > m_Brain.bestAgentFitness)
        {
            m_Brain.bestAgentFitness = m_FitnessArray[0];
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
        m_Brain.UnsubscribeAgent(this);

        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        Academy.Instance.AgentUpdateObservations -= UpdateObservations;
        Academy.Instance.AgentUpdateMovement -= AgentStep;
        Academy.Instance.AgentUpdateFitness -= UpdateFitness;
    }
}
