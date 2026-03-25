using UnityEngine;

//Spawns points of interest in rooms

public class PointOfInterestSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] pointsOfInterestPrefabs;
    private bool poiSelected = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{
    //    //Return if no POI prefabs assigned
    //    if (pointsOfInterestPrefabs.Length == 0)
    //    {
    //        Debug.LogWarning("No Points of Interest Prefabs assigned to PointOfInterestSpawner on " + gameObject.name);
    //        return;
    //    }
    //    //Select Random POI
    //    int randomIndex = Random.Range(0, pointsOfInterestPrefabs.Length);
    //    Instantiate(pointsOfInterestPrefabs[randomIndex], transform.position, Quaternion.identity, transform);
    //}

    //Add this as a listener when the parent room controller invokes room unlocked
    //This function spawns a provided POI from POI Spawn Controller
    public void SpawnPOI(GameObject prefabToSpawn)
    {
        //Early returns if we have already spawned prefabs or if the provided prefab is null
        if (poiSelected) return;
        if (prefabToSpawn == null) return;
        Instantiate(prefabToSpawn, transform.position, transform.rotation, transform);
        
    }
}
