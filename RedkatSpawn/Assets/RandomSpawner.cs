using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    public GameObject prefab;             // Prefab to instantiate
    public int NumInstances = 10;         // Number of instances

    [Header("Spawn Range Settings")]
    public Vector2 xRange = new Vector2(500f, 550f);  // X coordinate range
    public Vector2 yRange = new Vector2(10f, 100f);   // Y coordinate range
    public Vector2 zRange = new Vector2(500f, 550f);  // Z coordinate range

    void Start()
    {
        for (int i = 0; i < NumInstances; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(xRange.x, xRange.y),
                Random.Range(yRange.x, yRange.y),
                Random.Range(zRange.x, zRange.y)
            );

            Instantiate(prefab, randomPosition, Quaternion.identity);
        }
    }
}
