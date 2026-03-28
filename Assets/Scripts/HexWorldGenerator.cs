using System;
using UnityEngine;

public class HexWorldGenerator : MonoBehaviour
{
    public int size = 20;
    public float gridSize = 1f;

    [SerializeField] GameObject hexPrefab, hexWaterPrefab,hexGrassPrefab;
    [SerializeField] GameObject[] hexTreesPrefab;
    [SerializeField] Transform waterGridParent, treeGridParent, landGridParent, grassGridPareent;
    AnimalSpawner animalSpawner;

    public float seed = 0;
    public float waterNoiseFrequency = 5f;
    public float waterNoiseThreshold = 0.35f;
    public float treeDensity = 0.2f;
    public float treeNoiseFrequency = 8f;
    public float grassDensity = 0.2f;
    public float grassNoiseFrequency = 8f;

    HexCell[,] grids;

    enum GridType
    {
        Land,
        Tree,
        Water,
        Grass
    }
    CloudAnimationEvents cloudAnimationEvents;

    public event EventHandler OnMapGenerated;
    void Awake()
    {
        grids = new HexCell[size, size];
        GenerateHexWorld();
        LinkNeighbors();
    }

    private void Start()
    {
        animalSpawner = FindAnyObjectByType<AnimalSpawner>();
        cloudAnimationEvents = FindAnyObjectByType<CloudAnimationEvents>();
        cloudAnimationEvents.OnCloudCovered += CloudAnimationEvents_OnCloudCovered;
    }

    private void CloudAnimationEvents_OnCloudCovered(object sender, EventArgs e)
    {
        foreach (Transform child in landGridParent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in waterGridParent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in grassGridPareent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in treeGridParent)
        {
            Destroy(child.gameObject);
        }
        seed += 200;
        grids = new HexCell[size, size];
        GenerateHexWorld();
        LinkNeighbors();
        OnMapGenerated?.Invoke(this, EventArgs.Empty);
    }


    void GenerateHexWorld()
    {
        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                Vector3 position = GetCalculatedPositionOfGrid(x, z);

                GameObject prefab;
                GridType type = GetTypeOfGrid(x, z);

                switch (type)
                {
                    case GridType.Water:
                        prefab = hexWaterPrefab;
                        break;

                    case GridType.Tree:
                        prefab = hexTreesPrefab[UnityEngine.Random.Range(0, hexTreesPrefab.Length)];
                        break;

                    case GridType.Grass:
                        prefab=hexGrassPrefab;
                        break;
                    default:
                        prefab = hexPrefab;
                        break;
                }

                GameObject grid = Instantiate(prefab, position, Quaternion.identity);

                HexCell cell = grid.AddComponent<HexCell>();
                cell.x = x;
                cell.z = z;

                if (type == GridType.Tree)
                {
                    cell.isTree = true;
                    grid.transform.SetParent(treeGridParent);
                }
                else if (type == GridType.Water)
                {
                    cell.isWater = true;
                    grid.transform.SetParent(waterGridParent);
                }
                else if (type == GridType.Grass)
                {
                    cell.isGrass = true;
                    grid.transform.SetParent(grassGridPareent);
                }
                else
                {
                    cell.isLand = true;
                    grid.transform.SetParent(landGridParent);
                }

                grid.transform.localScale = Vector3.one * gridSize;
                grid.transform.Rotate(0, 90, 0);

                grids[x, z] = cell;
            }
        }
    }

    void LinkNeighbors()
    {
        // Fixed standard odd-q offsets
        int[][] dirsEven = {
            new int[]{+1,  0}, new int[]{+1, -1}, new int[]{ 0, -1},
            new int[]{-1, -1}, new int[]{-1,  0}, new int[]{ 0, +1}
        };

        int[][] dirsOdd = {
            new int[]{+1, +1}, new int[]{+1,  0}, new int[]{ 0, -1},
            new int[]{-1,  0}, new int[]{-1, +1}, new int[]{ 0, +1}
        };

        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                HexCell cell = grids[x, z];
                int[][] dirs = (x % 2 == 0) ? dirsEven : dirsOdd;

                for (int i = 0; i < 6; i++)
                {
                    int nx = x + dirs[i][0];
                    int nz = z + dirs[i][1];

                    if (nx >= 0 && nz >= 0 && nx < size && nz < size)
                    {
                        cell.neighbors[i] = grids[nx, nz];
                    }
                        
                }
            }
        }
    }

    Vector3 GetCalculatedPositionOfGrid(int x, int z)
    {
        float width = 2f * gridSize;
        float height = Mathf.Sqrt(3f) * gridSize;

        float horizontalDistance = width * 0.75f;
        float verticalDistance = height;

        float xPos = x * horizontalDistance;
        float zPos = z * verticalDistance;

        if (x % 2 == 1)
        {
            zPos += verticalDistance / 2f;
        }
            

        return new Vector3(xPos, 0, zPos);
    }

    GridType GetTypeOfGrid(float x, float z)
    {
        float waterValue = Mathf.PerlinNoise(
            (x + seed) / waterNoiseFrequency,
            (z + seed) / waterNoiseFrequency
        );

       

        float treeValue = Mathf.PerlinNoise(
            (x + seed + 100) / treeNoiseFrequency,
            (z + seed + 100) / treeNoiseFrequency
        );

        float grassValue = Mathf.PerlinNoise(
            (x + seed + 200) / grassNoiseFrequency,
            (z + seed + 200) / grassNoiseFrequency
         );
        if (waterValue < waterNoiseThreshold)
            return GridType.Water;

        if (treeValue < treeDensity)
        {
            return GridType.Tree;
        }

        if (grassValue < grassDensity)
        {
            return GridType.Grass;
        }
            

        return GridType.Land;
    }

    public HexCell GetCell(int x, int z)
    {
        if (x < 0 || z < 0 || x >= size || z >= size)
        {
            return null;
        }
            

        return grids[x, z];
    }
}