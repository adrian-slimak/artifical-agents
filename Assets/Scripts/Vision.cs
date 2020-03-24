﻿using UnityEngine;
using System;

public class Vision : Sensor
{
    public float visionAngle = 220f;
    public float visionRange = 10f;
    public int visionCellNum = 15;

    Animal m_Animal;

    Vector3 arcStart;
    float cellAngle;
    float nearTargetDistance;

    Collider2D[] hits = new Collider2D[100];
    int hitsNum = 0;
    public LayerMask sensorLayerMask;

    private void Awake()
    {
        m_Animal = GetComponent<Animal>();
        string brainName = GetComponent<Agent>().m_BrainName;

        visionAngle = VirtualAcademy.Instance.m_ResetParameters[$"{brainName}_observations_vision_angle"] ?? visionAngle;
        visionRange = VirtualAcademy.Instance.m_ResetParameters[$"{brainName}_observations_vision_range"] ?? visionRange;
        visionCellNum = (int)(VirtualAcademy.Instance.m_ResetParameters[$"{brainName}_observations_vision_cell_number"] ?? visionCellNum);

        cellAngle = visionAngle / visionCellNum;
        nearTargetDistance = 1f - (0.3f / visionRange);
    }

    public override void UpdateObservations()
    {
        arcStart = rotateByAngle(transform.right, (180f - visionAngle) / 2f);
        DetectVision();
    }

    void DetectVision()
    {
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
            }
        }
    }

    public int GetNearTargetObjects(Transform targetObject, float minDistance, string objectTag)
    {
        int numOfObjects = 0;
        for (int i = 0; i < hitsNum; i++)
        {
            if (hits[i].transform != targetObject && hits[i].tag == objectTag)
            {
                if (Vector2.Distance(targetObject.position, hits[i].transform.position) < minDistance)
                    numOfObjects++;
            }
        }

        return numOfObjects;
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
