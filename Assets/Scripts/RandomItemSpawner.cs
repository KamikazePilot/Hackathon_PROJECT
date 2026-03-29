using System.Collections.Generic;
using UnityEngine;

public class RandomItemSpawner : MonoBehaviour
{
    [Header("Item Prefabs")]
    public GameObject StonePrefab;
    public GameObject MarblePrefab;

    [Header("Camp Prefabs")]
    public GameObject CampStonePrefab;
    public GameObject CampMarblePrefab;

    [Header("Spawn Points")]
    public List<Transform> itemSpawnPoints = new List<Transform>();
    public List<Transform> campSpawnPoints = new List<Transform>();

    [Header("Counts")]
    public int StoneCount = 5;
    public int MarbleCount = 5;

    private void Start()
    {
        SpawnItems();
        SpawnCamps();
    }

    void SpawnItems()
    {
        List<Transform> availableItemPoints = new List<Transform>(itemSpawnPoints);

        SpawnMultiple(StonePrefab, StoneCount, availableItemPoints);
        SpawnMultiple(MarblePrefab, MarbleCount, availableItemPoints);
    }

    void SpawnCamps()
    {
        List<Transform> availableCampPoints = new List<Transform>(campSpawnPoints);

        SpawnSingle(CampStonePrefab, availableCampPoints);
        SpawnSingle(CampMarblePrefab, availableCampPoints);
    }

    void SpawnMultiple(GameObject prefab, int count, List<Transform> availablePoints)
    {
        if (prefab == null)
        {
            Debug.LogError("Missing item prefab.");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            if (availablePoints.Count == 0)
            {
                Debug.LogWarning("Not enough item spawn points.");
                return;
            }

            int randomIndex = Random.Range(0, availablePoints.Count);
            Transform chosenPoint = availablePoints[randomIndex];

            Instantiate(prefab, chosenPoint.position, chosenPoint.rotation);
            availablePoints.RemoveAt(randomIndex);
        }
    }

    void SpawnSingle(GameObject prefab, List<Transform> availablePoints)
    {
        if (prefab == null)
        {
            Debug.LogError("Missing camp prefab.");
            return;
        }

        if (availablePoints.Count == 0)
        {
            Debug.LogWarning("Not enough camp spawn points.");
            return;
        }

        int randomIndex = Random.Range(0, availablePoints.Count);
        Transform chosenPoint = availablePoints[randomIndex];

        Instantiate(prefab, chosenPoint.position, chosenPoint.rotation);
        availablePoints.RemoveAt(randomIndex);
    }
}