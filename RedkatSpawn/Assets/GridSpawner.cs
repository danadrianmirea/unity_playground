using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    [Header("Prefab and Reference")]
    public GameObject prefab;                     // Prefab to instantiate
    public GameObject prefabReferenceInstance;   // Scene instance of prefab to measure size
    public int totalSpawned { get; private set; }


    [Header("Spawn Range Settings")]
    public Vector2 xRange = new Vector2(0f, 10f);
    public Vector2 yRange = new Vector2(0f, 10f);
    public Vector2 zRange = new Vector2(0f, 10f);

    [Header("Spacing Settings")]
    [Range(0f, 10f)]
    public float spacingMultiplier = 1.2f;

    private int maxInstances = 10000;

    void Start()
    {
        if (prefab == null)
        {
            Debug.LogError("Prefab not assigned.");
            return;
        }

        if (prefabReferenceInstance == null)
        {
            Debug.LogError("Prefab reference instance not assigned.");
            return;
        }

        totalSpawned = 0;

        Vector3 size = GetReferenceSize();
        Vector3 spacing = size * spacingMultiplier;

        int countX = Mathf.Max(1, Mathf.FloorToInt((xRange.y - xRange.x) / spacing.x));
        int countY = Mathf.Max(1, Mathf.FloorToInt((yRange.y - yRange.x) / spacing.y));
        int countZ = Mathf.Max(1, Mathf.FloorToInt((zRange.y - zRange.x) / spacing.z));

        long totalInstances = (long)countX * countY * countZ;

        if (totalInstances > maxInstances)
        {
            Debug.LogWarning($"Total instances ({totalInstances}) exceed maxInstances ({maxInstances}). Clamping counts.");

            // Calculate approximate cube root scale factor to reduce counts proportionally
            float scale = Mathf.Pow(maxInstances / (float)totalInstances, 1f / 3f);

            countX = Mathf.Max(1, Mathf.FloorToInt(countX * scale));
            countY = Mathf.Max(1, Mathf.FloorToInt(countY * scale));
            countZ = Mathf.Max(1, Mathf.FloorToInt(countZ * scale));

            totalInstances = (long)countX * countY * countZ;

            Debug.Log($"Counts after clamping: X={countX}, Y={countY}, Z={countZ} (Total: {totalInstances})");
        }

        Vector3 totalGridSize = new Vector3(
            spacing.x * (countX - 1),
            spacing.y * (countY - 1),
            spacing.z * (countZ - 1)
        );

        // Center X and Z on GameObject, keep Y centered in yRange
        Vector3 startPos = new Vector3(
            transform.position.x - totalGridSize.x / 2f,
            yRange.x + ((yRange.y - yRange.x) - totalGridSize.y) / 2f,
            transform.position.z - totalGridSize.z / 2f
        );

        for (int x = 0; x < countX; x++)
        {
            for (int y = 0; y < countY; y++)
            {
                for (int z = 0; z < countZ; z++)
                {
                    Vector3 position = new Vector3(
                        startPos.x + x * spacing.x,
                        startPos.y + y * spacing.y,
                        startPos.z + z * spacing.z
                    );

                    Instantiate(prefab, position, Quaternion.identity);
                    totalSpawned++;
                }
            }
        }
    }

    Vector3 GetReferenceSize()
    {
        SphereCollider sphere = prefabReferenceInstance.GetComponentInChildren<SphereCollider>();
        if (sphere != null)
        {
            Vector3 scale = sphere.transform.lossyScale;
            float diameterX = sphere.radius * 2f * scale.x;
            float diameterY = sphere.radius * 2f * scale.y;
            float diameterZ = sphere.radius * 2f * scale.z;
            return new Vector3(diameterX, diameterY, diameterZ);
        }

        Collider col = prefabReferenceInstance.GetComponentInChildren<Collider>();
        if (col != null)
        {
            return col.bounds.size;
        }

        Renderer rend = prefabReferenceInstance.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            return rend.bounds.size;
        }

        Debug.LogWarning("Prefab reference instance has no collider or renderer components. Defaulting to Vector3.one.");
        return Vector3.one;
    }
}