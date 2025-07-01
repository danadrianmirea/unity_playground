using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class TerrainGeneratorEditor : MonoBehaviour
{
    public int terrainWidth = 512;
    public int terrainHeight = 512;
    public int terrainDepth = 100;
    public float scale = 20f;

    public float offsetX = 100f;
    public float offsetY = 100f;

    [ContextMenu("Generate Terrain")]
    public void GenerateTerrain()
    {
        offsetX = Random.Range(0f, 9999f);
        offsetY = Random.Range(0f, 9999f);
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrainData(terrain.terrainData);
        // Shift terrain so center is at (0,0,0)
        transform.position = new Vector3(-terrainWidth / 2f, 0, -terrainHeight / 2f);
        Debug.Log("Terrain generated and centered!");
    }

    TerrainData GenerateTerrainData(TerrainData terrainData)
    {
        terrainData.heightmapResolution = terrainWidth + 1;
        terrainData.size = new Vector3(terrainWidth, terrainDepth, terrainHeight);
        terrainData.SetHeights(0, 0, GenerateHeights());
        return terrainData;
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[terrainWidth, terrainHeight];

        int octaves = 4;
        float persistence = 0.5f;
        float lacunarity = 2f;

        for (int x = 0; x < terrainWidth; x++)
        {
            for (int y = 0; y < terrainHeight; y++)
            {
                float amplitude = 1f;
                float frequency = 1f;
                float noiseHeight = 0f;

                for (int i = 0; i < octaves; i++)
                {
                    float xCoord = ((float)x / terrainWidth) * scale * frequency + offsetX;
                    float yCoord = ((float)y / terrainHeight) * scale * frequency + offsetY;

                    float perlinValue = Mathf.PerlinNoise(xCoord, yCoord) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                heights[x, y] = Mathf.InverseLerp(-1f, 1f, noiseHeight);
            }
        }

        return heights;
    }
}
