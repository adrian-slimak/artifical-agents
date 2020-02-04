using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantsSpawner : MonoBehaviour
{
    public static PlantsSpawner Instance;
    public Transform plantsHolder;
    public GameObject plantPrefab;

    public int plantsOnReset = 100;
    public float plantsPerStep = 0.05f;
    private float gridStep = 3.5f;

    float plantsToSpawn;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    private void Start()
    {
        //OnReset();
    }

    public void OnReset()
    {
        foreach (Transform child in plantsHolder.transform)
            Destroy(child.gameObject);

        SpawnGrid();
    }

    private void FixedUpdate()
    {
        //plantsToSpawn += plantsPerStep;
        //if (plantsToSpawn >= 1f)
        //{
        //    Spawn((int)plantsToSpawn);
        //    plantsToSpawn -= (int)plantsToSpawn;
        //}
    }

    void Spawn(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector2 randomPosition = new Vector2((Random.value - 0.5f) * 100f, (Random.value - 0.5f) * 100f);
            GameObject agent = Instantiate(plantPrefab, randomPosition, Quaternion.identity, plantsHolder.transform);
        }
    }

    void SpawnGrid()
    {
        int count = 0;
        for (float y=-50f; y<50f; y+=gridStep)
        {

            for (float x=-50f; x<50f; x+=gridStep)
            {
                float noiseX = (Random.value - 0.5f)*0.6f;
                float noiseY = (Random.value - 0.5f)*0.6f;
                count++;
                Vector2 randomPosition = new Vector2(x+noiseX, y+noiseY);
                GameObject agent = Instantiate(plantPrefab, randomPosition, Quaternion.identity, plantsHolder.transform);
            }
        }
        //Debug.Log(count);
    }
}
