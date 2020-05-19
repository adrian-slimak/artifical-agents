using UnityEngine;
using System;

public class Vision : Sensor
{
    [Parameter("observations_vision_angle")]
    public float visionAngle = 220f;
    [Parameter("observations_vision_range")]
    public float visionRange = 10f;
    [Parameter("observations_vision_cell_number")]
    public int visionCellNum = 15;
    [Parameter("distance_to_eat")]
    public float nearbyDistance = 5;

    Animal m_Animal;

    Vector3 arcStart;
    float cellAngle;
    float nearTargetDistance;

    Collider2D[] hits = new Collider2D[100];
    int hitsNum = 0;

    protected override void Awake()
    {
        base.Awake();

        m_Animal = GetComponent<Animal>();

        cellAngle = visionAngle / visionCellNum;
        nearTargetDistance = 1f - (nearbyDistance / visionRange);
    }

    public override void UpdateObservations()
    {
        arcStart = rotateByAngle(transform.right, (180f - visionAngle) / 2f);

        hitsNum = Physics2D.OverlapCircleNonAlloc(transform.position, visionRange, hits, layerMask: sensorLayerMask);
        for (int j = 0; j < observationsVector.Length; j++)
            observationsVector[j] = 0;

        m_Animal.ResetNearObject();

        float angle;
        int cellNum;
        float distance;
        for (int i = 0; i < hitsNum; i++)
        {
            if (hits[i].gameObject != this.gameObject)
            {
                angle = Vector2.SignedAngle(arcStart, hits[i].transform.position - transform.position);
                if (angle < 0) angle += 360;

                if (angle < visionAngle)
                {
                    cellNum = (int)(angle / cellAngle);
                    distance = Vector2.Distance(transform.position, hits[i].transform.position);


                    distance = 1f - distance / visionRange;

                    if (hits[i].tag == "Prey")
                        cellNum += visionCellNum;
                    else if (hits[i].tag == "Predator")
                        cellNum += visionCellNum * 2;


                    if (observationsVector[cellNum] < distance)
                    {
                        if (distance > nearTargetDistance) m_Animal.SetNearObject(hits[i].transform);

                        observationsVector[cellNum] = distance;
                    }
                }
                else
                    hits[i] = null;
            }
        }
    }

    public int GetNearTargetObjects(Transform targetObject, float minDistance, string objectTag) // Tutaj są wszystkie hity, nie tylko w obszarze wzroku!!!
    {
        int numOfObjects = 0;
        for (int i = 0; i < hitsNum; i++)
        {
            if (hits[i] != null && hits[i].tag == objectTag)
            {
                if (Vector2.Distance(targetObject.position, hits[i].transform.position) < minDistance)
                    numOfObjects++;
            }
        }

        return numOfObjects;
    }

    public int GetObjectsInSight(string objectType)
    {
        int amount = 0;

        int k = 0;
        if (objectType == "Prey")
            k = 1;
        else if (objectType == "Predator")
            k = 2;

        for(int i = visionCellNum * k; i < visionCellNum * (k+1); i++)
        {
            if (observationsVector[i] > 0)
                amount++;
        }

        return amount;
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

        //UnityEditor.Handles.color = new Color(0f, 0f, 0f, 0.1f);
        //UnityEditor.Handles.DrawSolidArc(transform.position, transform.forward, arcStart,
        //                                                    visionAngle, visionRange);
        UnityEditor.Handles.color = new Color(0f, 0f, 0f, 0.4f);
        UnityEditor.Handles.DrawWireArc(transform.position, transform.forward, arcStart,
                                                            visionAngle, visionRange);
        UnityEditor.Handles.DrawLine(transform.position,
                transform.position + Quaternion.AngleAxis(visionAngle, Vector3.forward) * arcStart * visionRange);

        for (int i = 0; i < visionCellNum; i++)
        {
            UnityEditor.Handles.color = new Color(0f, 0f, 0f, 0.4f);
            UnityEditor.Handles.DrawLine(transform.position,
                transform.position + Quaternion.AngleAxis(cellAngle * i, Vector3.forward) * arcStart * visionRange);

            UnityEditor.Handles.color = new Color(0f, 0f, 0f, 0.1f);
            if (observationsVector[i] > 0)
                UnityEditor.Handles.color += new Color(0f, 1f, 0f, 0.15f);
            if (observationsVector[i+visionCellNum] > 0)
                UnityEditor.Handles.color += new Color(0f, 0f, 1f, 0.15f);
            if (observationsVector[i+visionCellNum*2] > 0)
                UnityEditor.Handles.color += new Color(1f, 0f, 0f, 0.15f);

            UnityEditor.Handles.DrawSolidArc(transform.position, transform.forward, rotateByAngle(arcStart, i * cellAngle),
                cellAngle, visionRange);
        }
    }
#endif

    Vector3 rotateByAngle(Vector3 vector, float angle)
    {
        return Quaternion.AngleAxis(angle, Vector3.forward) * vector;
    }
}
