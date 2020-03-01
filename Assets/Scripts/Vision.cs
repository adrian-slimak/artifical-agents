﻿using UnityEngine;
using System;

public class Vision : MonoBehaviour
{
    public float visionAngle = 220f;
    public float visionDistance = 10f;
    public int visionCellsNum = 15;
    public bool drawGizmo = false;

    Animal m_Animal;

    //float[] observationsVector;
    UPC.MMArray observationsVector;
    Vector3 arcStart;
    float cellAngle;
    float nearTargetDistance;

    Collider2D[] hits = new Collider2D[20];
    int hitsNum = 0;


    private void Awake()
    {
        m_Animal = GetComponent<Animal>();

        //observationsVector = new float[visionCellsNum];
        //cellAngle = visionAngle / visionCellsNum;
        //nearTargetDistance = 1f - (0.3f / visionDistance);
    }

    public void SetVisionVectorArray(UPC.MMArray visionVectorArray)
    {
        observationsVector = visionVectorArray;
        visionCellsNum = visionVectorArray.Length;

        cellAngle = visionAngle / visionCellsNum;
        nearTargetDistance = 1f - (0.3f / visionDistance);
    }

    public void UpdateVisionObservations()
    {
        arcStart = rotateByAngle(transform.right, (180f - visionAngle) / 2f);
        DetectVision();
    }

    void DetectVision()
    {
        hitsNum = Physics2D.OverlapCircleNonAlloc(transform.position, visionDistance, hits, layerMask: 1 << 8);
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


                    //distance = 1f - Mathf.Clamp(distance/visionDistance, 0f, 0.999f);
                    distance = 1f - distance / visionDistance;

                    //if (observationsVector[cellNum] % 1 < distance)
                    if (observationsVector[cellNum] < distance)
                    {
                        if (distance > nearTargetDistance) m_Animal.SetNearObject(hits[i].transform);

                        if (m_Animal.Type == Animal.AnimalType.Prey)
                            if (hits[i].tag == "Plant")
                                observationsVector[cellNum] = distance;

                        if (m_Animal.Type == Animal.AnimalType.Predator)
                            if (hits[i].tag == "Prey")
                                observationsVector[cellNum] = distance;
                        ////if (hits[i].tag == "Prey")
                        ////    distance += 0f;
                        ////else
                        //if (hits[i].tag == "Plant")
                        //    distance += 1f;
                        //else
                        //if (hits[i].tag == "Predator")
                        //    distance += 2f;

                        //observationsVector[cellNum] = distance;
                    }
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

        for (int i = 0; i < observationsVector.Length; i++)
        {
            UnityEditor.Handles.color = new Color(0f, 0f, 0f, 0.4f);
            UnityEditor.Handles.DrawLine(transform.position,
                transform.position + Quaternion.AngleAxis(cellAngle * i, Vector3.forward) * arcStart * visionDistance);

            if (observationsVector[i] > 0)
            {
                UnityEditor.Handles.color = new Color(0f, 0f, 1f, 0.15f);
                if (observationsVector[i] > 2)
                    UnityEditor.Handles.color = new Color(1f, 0f, 0f, 0.15f);
                else if (observationsVector[i] > 1)
                    UnityEditor.Handles.color = new Color(0f, 1f, 0f, 0.15f);

                UnityEditor.Handles.DrawSolidArc(transform.position, transform.forward,
                    rotateByAngle(arcStart, i * cellAngle),
                    cellAngle, visionDistance);
            }
        }

    }
#endif

    Vector3 rotateByAngle(Vector3 vector, float angle)
    {
        return Quaternion.AngleAxis(angle, Vector3.forward) * vector;
    }
}
