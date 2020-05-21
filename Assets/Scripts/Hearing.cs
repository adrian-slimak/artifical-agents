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
    int hitsNum = 0;

    float angle;
    int cellNum;
    float sound;
    float distance;

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

        for (int i = 0; i < observationsVector.Length; i++)
            observationsVector[i] = hearingRange;

        for (int i = 0; i < hitsNum; i++)
        {
            if (hits[i].gameObject != this.gameObject)
            {
                angle = Vector2.SignedAngle(arcStart, hits[i].transform.position - transform.position);
                if (angle < 0) angle += 360;

                cellNum = (int)(angle / cellAngle);
                distance = Vector2.Distance(transform.position, hits[i].transform.position);

                distance = 1f - (distance / hearingRange);

                sound = hits[i].gameObject.GetComponent<Animal>().currentSound;
                if (sound == 0)
                    sound = distance * hits[i].gameObject.GetComponent<Animal>().GetSpeed();

                if (sound > observationsVector[cellNum])
                    observationsVector[cellNum] = sound;
            }
        }
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
