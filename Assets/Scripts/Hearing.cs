using UnityEngine;

public class Hearing : Sensor
{
    [Parameter("observations_hearing_angle")]
    public float hearingAngle = 360f;
    [Parameter("observations_hearing_range")]
    public float hearingRange = 30f;
    [Parameter("observations_hearing_cell_number")]
    public int hearingCellNum = 15;

    Animal m_Animal;

    Vector3 arcStart;
    float cellAngle;

    Collider2D[] hits = new Collider2D[50];
    float[] hitDistances = new float[50];
    int hitsNum = 0;

    protected override void Awake()
    {
        base.Awake();

        m_Animal = GetComponent<Animal>();

        cellAngle = hearingAngle / hearingCellNum;
    }

    public override void UpdateObservations()
    {
        arcStart = rotateByAngle(transform.right, (180f - hearingAngle) / 2f);
        DetectVision();
    }

    void DetectVision()
    {
        hitsNum = Physics2D.OverlapCircleNonAlloc(transform.position, hearingRange, hits, layerMask: sensorLayerMask);
        for (int j = 0; j < observationsVector.Length; j++)
            observationsVector[j] = 0;

        float angle;
        int cellNum;
        float normalizedDistance;
        float sound;
        for (int i = 0; i < hitsNum; i++)
        {
            if (hits[i].gameObject != this.gameObject)
            {
                angle = Vector2.SignedAngle(arcStart, hits[i].transform.position - transform.position);
                if (angle < 0) angle += 360;

                if (angle < hearingAngle)
                {
                    cellNum = (int)(angle / cellAngle);
                    hitDistances[i] = Vector2.Distance(transform.position, hits[i].transform.position);

                    normalizedDistance = 1f - hitDistances[i] / hearingRange;

                    sound = hits[i].gameObject.GetComponent<Animal>().currentSound;
                    if (sound == 0)
                        sound = normalizedDistance * hits[i].gameObject.GetComponent<Animal>().GetSpeed();

                    if (observationsVector[cellNum] < sound)
                        observationsVector[cellNum] = sound;
                }
            }
            else
            {
                hitDistances[i] = float.MaxValue;
            }
        }
    }

    public int GetNearObjects(float minDistance, string objectTag)
    {
        int numOfObjects = -1;
        for (int i = 0; i < hitsNum; i++)
            if (hits[i].tag == objectTag && hitDistances[i] < minDistance)
                numOfObjects++;

        return numOfObjects;
    }

    public float GetDistanceToClosest(string objectTag)
    {
        float minDistance = float.MaxValue;

        for (int i = 0; i < hitsNum; i++)
            if (hits[i].tag == objectTag && hitDistances[i] < minDistance)
                minDistance = hitDistances[i];

        return minDistance;
    }

#if UNITY_EDITOR
    public bool drawGizmo = false;

    private void OnDrawGizmos()
    {
        if (!drawGizmo) return;
        if (Application.isEditor)
        {
            //Awake();
            UpdateObservations();
        }

        UnityEditor.Handles.color = new Color(0f, 0f, 0f, 0.4f);
        UnityEditor.Handles.DrawWireArc(transform.position, transform.forward, arcStart,
                                                            hearingAngle, hearingRange);
        UnityEditor.Handles.DrawLine(transform.position,
                transform.position + Quaternion.AngleAxis(hearingAngle, Vector3.forward) * arcStart * hearingRange);

        for (int i = 0; i < observationsVectorSize; i++)
        {
            UnityEditor.Handles.color = new Color(0f, 0f, 0f, 0.4f);
            UnityEditor.Handles.DrawLine(transform.position,
                transform.position + Quaternion.AngleAxis(cellAngle * i, Vector3.forward) * arcStart * hearingRange);

            UnityEditor.Handles.color = new Color(0f, 0f, 0f, 0.1f);

            if (observationsVector[i] > 0)
                UnityEditor.Handles.color = new Color(1f, 1f, 0f, observationsVector[i] / 2f);
            if (observationsVector[i] > 5)
                UnityEditor.Handles.color = new Color(1f, 1f, 0f, observationsVector[i] / 2f);

            UnityEditor.Handles.DrawSolidArc(transform.position, transform.forward,
                rotateByAngle(arcStart, i * cellAngle),
                cellAngle, hearingRange);
        }

    }
#endif

    Vector3 rotateByAngle(Vector3 vector, float angle)
    {
        return Quaternion.AngleAxis(angle, Vector3.forward) * vector;
    }
}
