using UnityEngine;

public class PlantsSpawner : MonoBehaviour
{
    public static PlantsSpawner Instance;
    public Transform plantsHolder;
    public GameObject plantPrefab;

    [Parameter("environment_food_spawn_amount_reset")]
    public int plantsOnReset = 100;
    [Parameter("environment_food_spawn_per_step")]
    public float plantsPerStep = 0.05f;
    [Parameter("environment_food_spawn_grid_step")]
    public float gridStep = 3.5f;
    [Parameter("environment_food_spawn_method")]
    public int spawnMethod = 0;

    float plantsToSpawn;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    public void OnReset()
    {
        Academy.LoadEnvironmentParameters(this);

        foreach (Transform child in plantsHolder.transform)
            Destroy(child.gameObject);

        if (spawnMethod == 0)
            SpawnGrid();
        if (spawnMethod == 1)
            Spawn(plantsOnReset);
    }

    private void FixedUpdate()
    {
        plantsToSpawn += plantsPerStep;
        if (plantsToSpawn >= 1f)
        {
            Spawn((int)plantsToSpawn);
            plantsToSpawn -= (int)plantsToSpawn;
        }
    }

    void Spawn(int amount)
    {
        Debug.Log(VirtualAcademy.Instance.m_WorldSize);
        for (int i = 0; i < amount; i++)
        {
            Vector2 randomPosition = new Vector2((Random.value - 0.5f) * VirtualAcademy.Instance.m_WorldSize, (Random.value - 0.5f) * VirtualAcademy.Instance.m_WorldSize);
            GameObject agent = Instantiate(plantPrefab, randomPosition, Quaternion.identity, plantsHolder.transform);
        }
    }

    void SpawnGrid()
    {
        for (float y=-VirtualAcademy.Instance.m_HalfWorldSize; y<VirtualAcademy.Instance.m_HalfWorldSize; y+=gridStep)
        {

            for (float x=-VirtualAcademy.Instance.m_HalfWorldSize; x< VirtualAcademy.Instance.m_HalfWorldSize; x+=gridStep)
            {
                float noiseX = (Random.value - 0.5f)*0.6f;
                float noiseY = (Random.value - 0.5f)*0.6f;
                Vector2 randomPosition = new Vector2(x+noiseX, y+noiseY);
                GameObject agent = Instantiate(plantPrefab, randomPosition, Quaternion.identity, plantsHolder.transform);
            }
        }
    }
}
