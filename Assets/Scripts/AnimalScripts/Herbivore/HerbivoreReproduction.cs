using UnityEngine;

public class HerbivoreReproduction : MonoBehaviour
{
    AnimalCount animalCount;

    [SerializeField] private GameObject deer;
    private HerbivoreStats herbstats;
    private AnimalSpawner animalSpawner;
    [SerializeField]float reproductionTime,reproductionTimer;

    private void Start()
    {
        animalCount=FindAnyObjectByType<AnimalCount>();
        animalSpawner = FindAnyObjectByType<AnimalSpawner>();
        reproductionTimer = 0;
        reproductionTime = Random.Range(5*60, 8*60);
        herbstats = GetComponent<HerbivoreStats>();
    }

    private void Update()
    {
        if (herbstats.currentLifeStage==HerbivoreStats.lifeStage.adult && herbstats.gender=='F')
        {
            reproductionTimer += Time.deltaTime;
            if (reproductionTimer > reproductionTime)
            {
                Reproduce();
            }
        }
    }

    private void Reproduce()
    {
        reproductionTimer = 0;
        reproductionTime=Random.Range(7*60, 11*60);
        HexCell spawnCell = herbstats.FindClosestCell();
        GameObject baby = animalSpawner.SpawnEntity(deer, spawnCell);
        baby.transform.SetParent(animalSpawner.deerHolder);
        animalCount.deerCount++;


        // Assign world reference like AnimalSpawner does
        HerbivoreStats babyStats = baby.GetComponent<HerbivoreStats>();
        HerbivoreActions babyActions = baby.GetComponent<HerbivoreActions>();
        if (babyStats != null)  
        {
            babyStats.world = herbstats.world;
            babyStats.currentLifeStage = HerbivoreStats.lifeStage.minor;
            babyStats.lifeTimer = 0f;
        }
        if (babyActions != null) babyActions.world = herbstats.world;
        Debug.Log("Deer Baby Born :)");
    }
}
