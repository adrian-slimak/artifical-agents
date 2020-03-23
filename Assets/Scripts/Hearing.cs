using UnityEngine;
using System;

public class Hearing : Sensor
{
    public float hearingAngle = 360f;
    public float hearingRange = 30f;
    public int hearingCellNum = 15;
    public bool drawGizmo = false;

    Animal m_Animal;

    Vector3 arcStart;
    float cellAngle;

    Collider2D[] hits = new Collider2D[30];
    int hitsNum = 0;
    public LayerMask sensorLayerMask;


    private void Awake()
    {
        m_Animal = GetComponent<Animal>();
        string brainName = GetComponent<Agent>().m_BrainName;

        hearingAngle = VirtualAcademy.Instance.m_ResetParameters[$"{brainName}_observations_hearing_angle"] ?? hearingAngle;
        hearingRange = VirtualAcademy.Instance.m_ResetParameters[$"{brainName}_observations_hearing_range"] ?? hearingRange;
        hearingCellNum = (int)(VirtualAcademy.Instance.m_ResetParameters[$"{brainName}_observations_hearing_cell_number"] ?? hearingCellNum);

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
        float distance;
        for (int i = 0; i < hitsNum; i++)
        {
            if (hits[i].gameObject != this.gameObject)
            {
                angle = Vector2.SignedAngle(arcStart, hits[i].transform.position - transform.position);
                if (angle < 0) angle += 360;

                if (angle < hearingAngle)
                {
                    cellNum = (int)(angle / cellAngle);
                    distance = Vector2.Distance(transform.position, hits[i].transform.position);

                    distance = 1f - distance / hearingRange;

                    if (observationsVector[cellNum] < distance)
                        observationsVector[cellNum] = distance * hits[i].gameObject.GetComponent<Animal>().GetSpeed();
                }
            }
        }
    }

#if UNITY_EDITOR
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
