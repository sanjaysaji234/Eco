using UnityEngine;
using UnityEngine.UIElements;


public class HexWorldGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    public int size = 20;
    public float gridSize = 1f;


    [Header("HexDimensions")]
    float hexWidth;
    float hexHeight;

    [Header("Prefabs")]
    [SerializeField] GameObject hexPrefab;
    [SerializeField] GameObject[] hexTreesPrefab;
    [SerializeField] Transform gridParent;

    [Header("Noise Settings")]
    public float seed = 0;
    public float waterNoiseFrequency = 5f;
    public float waterNoiseThreshold = 0.35f;
    public float treeDensity = 0.2f;
    public float treeNoiseFrequency = 8f;

    GameObject[,] grids;

    enum GridType
    {
        DEFAULT,
        Tree,
        Water
    }

    void Start()
    {
        grids = new GameObject[size, size];
        //seed = Random.Range(0, 10000);
        Renderer r = hexPrefab.GetComponent<Renderer>();
        hexWidth = r.bounds.size.x;
        hexHeight = r.bounds.size.z;

        GenerateHexWorld();
    }


    void GenerateHexWorld()
    {
        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                Vector3 position = GetCalculatedPositionOfGrid(x, z);
                GameObject prefab;
                switch (GetTypeOfGrid(position.x, position.z))
                {
                    case GridType.Water:
                        continue;
                    case GridType.Tree:
                        prefab = hexTreesPrefab[Random.Range(0,hexTreesPrefab.Length)];
                        break;
                    default:
                        prefab=hexPrefab;
                        break;
                }

                GameObject grid = Instantiate(prefab, position, Quaternion.identity);


                grid.transform.localScale = Vector3.one * gridSize;
                grid.transform.Rotate(0, 90, 0);
                grid.transform.SetParent(gridParent);

                grids[x, z] = grid;
            }
        }
    }

    GridType GetTypeOfGrid(float x, float z)
    {
        float waterValue = Mathf.PerlinNoise(
            (x + seed) / waterNoiseFrequency,
            (z + seed) / waterNoiseFrequency
        );

        if (waterValue < waterNoiseThreshold)
            return GridType.Water;

        float treeValue = Mathf.PerlinNoise(
            (x + seed + 100) / treeNoiseFrequency,
            (z + seed + 100) / treeNoiseFrequency
        );

        if (treeValue < treeDensity)
            return GridType.Tree;
        return GridType.DEFAULT;
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
            zPos += verticalDistance / 2f;

        return new Vector3(xPos, 0, zPos);
    }
}