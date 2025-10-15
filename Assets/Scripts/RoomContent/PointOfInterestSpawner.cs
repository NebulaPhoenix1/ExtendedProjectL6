using UnityEngine;

//Spawns points of interest in rooms

public class PointOfInterestSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] pointsOfInterestPrefabs;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Return if no POI prefabs assigned
        if (pointsOfInterestPrefabs.Length == 0)
        {
            Debug.LogWarning("No Points of Interest Prefabs assigned to PointOfInterestSpawner on " + gameObject.name);
            return;
        }
        //Select Random POI
        int randomIndex = Random.Range(0, pointsOfInterestPrefabs.Length);
        Instantiate(pointsOfInterestPrefabs[randomIndex], transform.position, Quaternion.identity, transform);
    }
}
