using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PointOfInterestSpawnController : MonoBehaviour
{
    //Singleton so we can easily find this class and becuase having mulitple would be problematic
    public static PointOfInterestSpawnController Instance { get; private set; }

    [Header("Prefab List")]
    [SerializeField] List<GameObject> allPOIPrefabs;
    [Header("Cost Values")]
    [SerializeField] private float minRoomBudget = 10f; //Total budget for rank 0 (awful) player
    [SerializeField] private float maxRoomBudget = 60f; //Total bugdet for rank 100 (amazing) player
    [SerializeField] private float costTolerance = 3f; //How much wiggle room there is when selecting a prefab e.g. +/- 3 cost

    public List<GameObject> SelectPOIsForRoom(float playerRank, int amountToSpawn)
    {
        List<GameObject> selectedPrefabs = new List<GameObject>();
        if (allPOIPrefabs == null || allPOIPrefabs.Count == 0)
        {
            Debug.LogWarning("POI Spawn Controller allPOIPrefabs is null or empty");
            return selectedPrefabs;
        }

        //Convert the player rank to a budget this function can spend when selecting a POI
        float startingBudget = Mathf.Lerp(minRoomBudget, maxRoomBudget, playerRank / 100f);
        float remainingBudget = startingBudget;

        //Loop for how many prefabs we want to spawn (usually 4 for this project due to level design simplicity)
        for (int i = 0; i < amountToSpawn; i++)
        {
            //Calculate ideal cost for this spawn
            int remainingSlots = amountToSpawn - i;
            float targetSpawn = remainingBudget / remainingSlots;
            //Find all prefabs close to target cost
            List<GameObject> validPOIs = allPOIPrefabs.Where(prefab =>
            {
                int score = GetPOICost(prefab);
                return Mathf.Abs(score - targetSpawn) <= costTolerance;
            }).ToList();
            //If no prefabs match exact tolerance, grab the closest one
            if (validPOIs.Count == 0)
            {
                validPOIs.Add(allPOIPrefabs.OrderBy(prefab =>
                    Mathf.Abs(GetPOICost(prefab) - targetSpawn)).First());

            }
            //Pick random prefab from valid pool
            GameObject chosenPrefab = validPOIs[Random.Range(0, validPOIs.Count)];
            selectedPrefabs.Add(chosenPrefab);
            remainingBudget -= GetPOICost(chosenPrefab);
        }
        Debug.Log($"Target budget:{startingBudget} remainingBudget: {remainingBudget}");
        return selectedPrefabs;
    }

    private int GetPOICost(GameObject prefab)
    {
        var poiComponent = prefab.GetComponent<PointOfInterest>();
        if (poiComponent != null && poiComponent.pointOfInterestData != null)
        {
            return poiComponent.pointOfInterestData.threatCost;
        }
        Debug.Log($"Could not find POI cost on {prefab.name} defaulting to cost of 1");
        return 1;
    }

    void Awake()
    {
        //Singleton pattern implementation
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    
}
