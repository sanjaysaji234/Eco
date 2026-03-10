using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    [SerializeField] HexWorldGenerator world;
    [SerializeField] GameObject man,deer;
    [SerializeField] float deerThreshold=0.2f,deerFrequency=10f;
    [SerializeField] float manThreshold=0.3f,ManFrequency=10f;
    [SerializeField] float seed;
    [SerializeField] Transform manHolder, deerHolder;
    private void Start()
    {
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

                if (manValue < manThreshold)
                {
                    GameObject entity=SpawnEntity(man,cell);
                    entity.transform.SetParent(manHolder);
                }
                else if (deerValue < deerThreshold)
                {
                    GameObject entity= SpawnEntity(deer, cell);
                    entity.transform.SetParent(deerHolder);
                }
            }
        }
    }


    private GameObject SpawnEntity(GameObject prefab,HexCell cell)
    {
        GameObject entity = Instantiate(prefab, new Vector3 (cell.transform.position.x,8.1f,cell.transform.position.z), Quaternion.identity);

        Agent agent = entity.GetComponent<Agent>();
        if (agent != null)
        {
            agent.world = this.world;
        }
        return entity;
    }
}
