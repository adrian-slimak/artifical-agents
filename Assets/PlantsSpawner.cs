using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantsSpawner : MonoBehaviour
{
    public Transform plantsHolder;
    public GameObject plantPrefab;

    public int plantsOnStart = 100;
    public float plantsPerStep = 0.05f;

    float plantsToSpawn;

    void Start()
    {
        Spawn(plantsOnStart);
    }

    private void FixedUpdate()
    {
        plantsToSpawn += plantsPerStep;
        if(plantsToSpawn >=1f)
        {
            Spawn((int)plantsToSpawn);
            plantsToSpawn -= (int)plantsToSpawn;
        }
    }

    void Spawn(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector2 randomPosition = new Vector2(Random.value * 100f, Random.value * 100f);
            GameObject agent = Instantiate(plantPrefab, randomPosition, Quaternion.identity, plantsHolder.transform);
        }
    }
}
