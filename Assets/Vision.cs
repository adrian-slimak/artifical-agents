using UnityEngine;
using System;

public class Vision : MonoBehaviour
{
    public Animal.AnimalType type;
    public float visionAngle = 120f;
    public float visionDistance = 20f;
    public int visionCellsNum = 10;
    public bool drawGizmo = false;

    [HideInInspector]
    public Transform nearFood;
    [HideInInspector]
    public Transform nearMate;

    ArraySegment<float> observationsVector;
    Vector3 arcStart;
    float cellAngle;

    Collider2D[] hits = new Collider2D[20];
    int hitsNum = 0;


    private void Awake()
    {
        observationsVector = new ArraySegment<float>(new float[visionCellsNum]);
        cellAngle = visionAngle / visionCellsNum;
    }

    //private void FixedUpdate()
    //{
    //    UpdateVisionObservations();
    //}

    public void SetObservationsVectorArray(ArraySegment<float> arraySegment)
    {
        observationsVector = arraySegment;
        visionCellsNum = arraySegment.Count;
    }

    public void UpdateVisionObservations()
    {
        arcStart = rotateByAngle(transform.right, (180f - visionAngle) / 2f);
        DetectVision();
    }

    void DetectVision()
    {
        hitsNum = Physics2D.OverlapCircleNonAlloc(transform.position, visionDistance, hits);
        for (int j = observationsVector.Offset; j < observationsVector.Offset + observationsVector.Count; j++)
            observationsVector.Array[j] = 0;

        nearMate = null;
        nearFood = null;

        for (int i=0; i<hitsNum; i++)
        {
            if (hits[i].gameObject != this.gameObject)
            {
                float angle = Vector2.SignedAngle(arcStart, hits[i].transform.position - transform.position);
                if (angle < 0) angle += 360;

                if (angle < visionAngle)
                {
                    int cellNum = (int)(angle / cellAngle);
                    float distance = Vector2.Distance(transform.position, hits[i].transform.position) / visionDistance;
                    distance = 1f - Mathf.Clamp(distance, 0f, 0.99f);
                    if (observationsVector.Array[observationsVector.Offset + cellNum] % 1 < distance)
                    {
                        if (hits[i].tag == "Predator")
                            if (distance > 0.9f && type == Animal.AnimalType.Fox) nearMate = hits[i].transform;
                            distance += 0f;
                        if (hits[i].tag == "Plant")
                        {
                            if (distance > 0.9f && type == Animal.AnimalType.Bunny) nearFood = hits[i].transform;
                            distance += 1f;
                        }
                        if (hits[i].tag == "Prey")
                        {
                            if (distance > 0.9f)
                                if (type == Animal.AnimalType.Bunny) nearMate = hits[i].transform;
                                else nearFood = hits[i].transform;

                            distance += 2f;
                        }

                        observationsVector.Array[observationsVector.Offset + cellNum] = distance;
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmo) return;
        if(Application.isEditor)
        {
            Awake();
            UpdateVisionObservations();
        }
        UnityEditor.Handles.color = new Color(0f, 0f, 0f, 0.1f);
        UnityEditor.Handles.DrawSolidArc(transform.position, transform.forward, arcStart,
                                                            visionAngle, visionDistance);
        UnityEditor.Handles.color = new Color(0f, 0f, 0f, 0.4f);
        UnityEditor.Handles.DrawWireArc(transform.position, transform.forward, arcStart,
                                                            visionAngle, visionDistance);
        UnityEditor.Handles.DrawLine(transform.position,
                transform.position + Quaternion.AngleAxis(visionAngle, Vector3.forward) * arcStart * visionDistance);

        for (int i = observationsVector.Offset; i< observationsVector.Offset + observationsVector.Count; i++)
        {
            UnityEditor.Handles.color = new Color(0f, 0f, 0f, 0.4f);
            UnityEditor.Handles.DrawLine(transform.position,
                transform.position + Quaternion.AngleAxis(cellAngle * i, Vector3.forward) * arcStart * visionDistance);

            if (observationsVector.Array[i] > 0)
            {
                UnityEditor.Handles.color = new Color(1f, 0f, 0f, 0.15f);
                if (observationsVector.Array[i] > 2)
                    UnityEditor.Handles.color = new Color(0f, 0f, 1f, 0.15f);
                else if (observationsVector.Array[i] > 1)
                    UnityEditor.Handles.color = new Color(0f, 1f, 0f, 0.15f);

                UnityEditor.Handles.DrawSolidArc(transform.position, transform.forward,
                    rotateByAngle(arcStart, i * cellAngle),
                    cellAngle, visionDistance);
            }
        }

    }

    Vector3 rotateByAngle(Vector3 vector, float angle)
    {
        return Quaternion.AngleAxis(angle, Vector3.forward) * vector;
    }
}
