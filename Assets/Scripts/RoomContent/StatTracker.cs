using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

[System.Serializable]
public class GameSaveData
{
    public int roomsExplored;
    public List<RoomStatsData> recentRoomStatsList;
}



//This class keeps track of the last 20 room's play stats
//We use a queue for this data so when we have 20 sets of data, we dequeue the oldest one when adding a new one
//This ensures old play data is not affecting current play data analysis too much

[System.Serializable]
public class StatTracker : MonoBehaviour
{
    //Singleton instance
    public static StatTracker Instance { get; private set; }

    private Queue<RoomStatsData> recentRoomStats = new Queue<RoomStatsData>();
    [SerializeField] private int maxRecentRooms = 3;

    private int roomsExplored = 0;
    private string pathToSaveFile;

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
        pathToSaveFile = Path.Combine(Application.persistentDataPath, "Play Time Data.json");
        LoadJSON();
    }

    void Update()
    {
        
    }

    public void AddRoomStats(RoomStats latestRoom)
    {
        if(recentRoomStats.Count >= maxRecentRooms)
        {
            recentRoomStats.Dequeue();
        }
        //Convert latestRoom to RoomStatsData and enqueue it    
        RoomStatsData latestRoomData = new RoomStatsData(latestRoom);
        recentRoomStats.Enqueue(latestRoomData);
        roomsExplored++;

        //Save JSON after every room
        SaveJSON();
    }

    private void LoadJSON()
    {
        //Check if we have a save file
        if (File.Exists(pathToSaveFile))
        {
            //Try/Catch block just in case of file corruption
            try
            {
                //If we do, load it and populate recentRoomStats queue
                string json = File.ReadAllText(pathToSaveFile);
                GameSaveData loadedData = JsonUtility.FromJson<GameSaveData>(json);
                this.roomsExplored = loadedData.roomsExplored;
                this.recentRoomStats = new Queue<RoomStatsData>(loadedData.recentRoomStatsList);
                Debug.Log("Loaded save data from " + pathToSaveFile);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error loading file: " + pathToSaveFile + ": " + e.Message);
                this.roomsExplored = 0;
                this.recentRoomStats = new Queue<RoomStatsData>();
            }
        }
        //If not, assume first load and create new save file
        else
        {
            Debug.Log("No save file found at " + pathToSaveFile + ", starting new save data using default class values.");
        }
    }
    private void SaveJSON()
    {
        //Create new GameSaveData object
        GameSaveData saveData = new GameSaveData();
        //Populate it with current data
        saveData.roomsExplored = this.roomsExplored;
        saveData.recentRoomStatsList = this.recentRoomStats.ToList();
        //Serialize to JSON and write to file
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(pathToSaveFile, json);
        Debug.Log("Saved play data to " + pathToSaveFile);
    }
}
