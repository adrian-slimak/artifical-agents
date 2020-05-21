using UnityEngine;

public class Sensor : MonoBehaviour
{
    public LayerMask sensorLayerMask;

    protected Agent m_Agent;

    public MMArray observationsVector;
    protected int observationsVectorSize;

    protected virtual void Awake()
    {
        m_Agent = GetComponent<Agent>();
        VirtualAcademy.Instance.m_ResetParameters.LoadEnvParams(this, m_Agent.m_BrainName);
    }

    public void SetObservationsVector(MMArray observationsVector)
    {
        this.observationsVector = observationsVector;
        this.observationsVectorSize = observationsVector.Length;
    }

    public virtual void UpdateObservations()
    {   }
}
