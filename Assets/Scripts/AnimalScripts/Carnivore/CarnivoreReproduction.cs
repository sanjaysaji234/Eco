using UnityEngine;

public class CarnivoreReproduction : MonoBehaviour
{
    AnimalCount animalCount;

    [SerializeField] private GameObject tiger;
    private CarnivoreStats carnstats;
    private AnimalSpawner animalSpawner;
    [SerializeField] float reproductionTime, reproductionTimer;

    private void Start()
    {
        animalCount = FindAnyObjectByType<AnimalCount>();
        animalSpawner = FindAnyObjectByType<AnimalSpawner>();
        reproductionTimer = 0;
        reproductionTime = Random.Range(5 * 60, 8 * 60);
        carnstats = GetComponent<CarnivoreStats>();
    }

    private void Update()
    {
        if (carnstats.currentLifeStage == CarnivoreStats.lifeStage.adult && carnstats.gender == 'F')
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
        reproductionTime = Random.Range(5 * 60, 7.5f * 60);
        HexCell spawnCell = carnstats.FindClosestCell();
        GameObject baby = animalSpawner.SpawnEntity(tiger, spawnCell);
        baby.transform.SetParent(animalSpawner.tigerHolder);
        animalCount.tigerCount++;
        // Assign world reference like AnimalSpawner does
        CarnivoreStats babyStats = baby.GetComponent<CarnivoreStats>();
        CarnivoreActions babyActions = baby.GetComponent<CarnivoreActions>();
        if (babyStats != null)
        {
            babyStats.world = carnstats.world;
            babyStats.currentLifeStage = CarnivoreStats.lifeStage.minor;
            babyStats.lifeTimer = 0f;
        }
        if (babyActions != null) babyActions.world = carnstats.world;
        Debug.Log("Tiger Baby Born :)");
    }
}
