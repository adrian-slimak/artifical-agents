using UnityEngine;

public class Sensor : MonoBehaviour
{
    protected MMArray observationsVector;
    protected int observationsVectorSize;

    public void SetObservationsVector(MMArray observationsVector)
    {
        this.observationsVector = observationsVector;
        this.observationsVectorSize = observationsVector.Length;
    }

    public virtual void UpdateObservations()
    {   }
}
