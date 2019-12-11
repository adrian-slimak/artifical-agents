using UnityEngine;

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

    float[] visionCells;
    Vector3 arcStart;
    float cellAngle;

    Collider2D[] hits = new Collider2D[20];
    int hitsNum = 0;


    private void Awake()
    {
        visionCells = new float[visionCellsNum];
        cellAngle = visionAngle / visionCellsNum;
    }

    void DetectVision()
    {
        hitsNum = Physics2D.OverlapCircleNonAlloc(transform.position, visionDistance, hits);
        for (int j = 0; j < visionCellsNum; j++)
            visionCells[j] = 0;
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
                    if (visionCells[cellNum] % 1 < distance)
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

                        visionCells[cellNum] = distance;
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        arcStart = rotateByAngle(transform.right, (180f - visionAngle) / 2f);
        DetectVision();
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmo) return;
        if(Application.isEditor)
        {
            Awake();
            FixedUpdate();
        }
        UnityEditor.Handles.color = new Color(0f, 0f, 0f, 0.1f);
        UnityEditor.Handles.DrawSolidArc(transform.position, transform.forward, arcStart,
                                                            visionAngle, visionDistance);
        UnityEditor.Handles.color = new Color(0f, 0f, 0f, 0.4f);
        UnityEditor.Handles.DrawWireArc(transform.position, transform.forward, arcStart,
                                                            visionAngle, visionDistance);
        UnityEditor.Handles.DrawLine(transform.position,
                transform.position + Quaternion.AngleAxis(visionAngle, Vector3.forward) * arcStart * visionDistance);

        for (int i = 0; i<visionCellsNum; i++)
        {
            UnityEditor.Handles.color = new Color(0f, 0f, 0f, 0.4f);
            UnityEditor.Handles.DrawLine(transform.position,
                transform.position + Quaternion.AngleAxis(cellAngle * i, Vector3.forward) * arcStart * visionDistance);

            if (visionCells[i] > 0)
            {
                UnityEditor.Handles.color = new Color(1f, 0f, 0f, 0.15f);
                if (visionCells[i] > 2)
                    UnityEditor.Handles.color = new Color(0f, 0f, 1f, 0.15f);
                else if (visionCells[i] > 1)
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

    public float[] GetVisionVector()
    {
        return visionCells;
    }
}
