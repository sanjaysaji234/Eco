using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    AnimalCount animalCount;
    [SerializeField] HexWorldGenerator world;
    [SerializeField] GameObject man,deer,tiger;
    [SerializeField] float deerThreshold=0.2f,deerFrequency=10f;
    [SerializeField] float manThreshold=0.3f,ManFrequency=10f; 
    [SerializeField] float tigerThreshold = 0.3f, tigerFrequency = 10f;
    [SerializeField] float seed;
    public Transform manHolder, deerHolder,tigerHolder;

    private void Start()
    {
        animalCount = FindAnyObjectByType<AnimalCount>();
        spawnAnimals();
        world.OnMapGenerated += World_OnMapGenerated;
    }

    private void World_OnMapGenerated(object sender, System.EventArgs e)
    {
        foreach (Transform child in manHolder)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in deerHolder)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in tigerHolder)
        {
            Destroy(child.gameObject);
        }
        animalCount.deerCount = 0;
        animalCount.tigerCount = 0;
        seed += 100;
        spawnAnimals();
    }

    private void spawnAnimals()
    {
        for(int x = 0;x<world.size;x++)
        {
            for(int z=0; z < world.size; z++)
            {
                HexCell cell=world.GetCell(x,z);
                if (cell == null || cell.isWater || cell.isTree)
                {
                    continue;
                }

                float manValue = Mathf.PerlinNoise((x + seed) / ManFrequency, (z + seed) / ManFrequency);
                float deerValue = Mathf.PerlinNoise((x + seed+1000) / deerFrequency, (z + seed+1000) / deerFrequency);
                float tigerValue= Mathf.PerlinNoise((x + seed + 2000) / tigerFrequency, (z + seed + 2000) / tigerFrequency);

                //if (manValue < manThreshold)
                //{
                //    GameObject entity=SpawnEntity(man,cell);
                //    entity.transform.SetParent(manHolder);
                //}
                if (deerValue < deerThreshold)
                {
                    GameObject entity= SpawnEntity(deer, cell);
                    animalCount.deerCount++;
                    entity.transform.SetParent(deerHolder);
                    HerbivoreStats stats = entity.GetComponent<HerbivoreStats>();
                    HerbivoreActions actions = entity.GetComponent<HerbivoreActions>();
                    if (stats != null)
                    {
                        stats.world = this.world;
                        stats.lifeTimer = Random.Range(0f, 1000f);
                    }
                    if (actions != null)
                    {
                        actions.world = this.world;

                    }
                }
                else if (tigerValue < tigerThreshold)
                {
                    GameObject entity = SpawnEntity(tiger, cell);
                    animalCount.tigerCount++;
                    entity.transform.SetParent(tigerHolder);

                    CarnivoreStats stats = entity.GetComponent<CarnivoreStats>();
                    CarnivoreActions actions = entity.GetComponent<CarnivoreActions>();
                    if (stats != null)
                    {
                        stats.world = this.world;
                        stats.lifeTimer = Random.Range(0f, 1000f);
                    }
                    if (actions != null)
                    {
                        actions.world = this.world;

                    }
                }
            }
        }
    }


    public GameObject SpawnEntity(GameObject prefab,HexCell cell)
    {
        GameObject entity = Instantiate(prefab, new Vector3 (cell.transform.position.x,8.1f,cell.transform.position.z), Quaternion.identity);
        return entity;
    }
}
