using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

[System.Serializable]
public class GameSaveData
{
    public int roomsExplored;
    public float playerRank = 50f; //Start player rank smack in the middle (it can be between 0 and 100)
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
    [SerializeField] private int maxRecentRooms = 10;

    private int roomsExplored = 0;
    private float playerRank;
    private string pathToSaveFile;

    //DDA Stuff
    private int totalHitsTaken;
    private int totalTrapDamage;
    private int totalAttacksUsed;
    private int totalAttacksHit;
    private float totalTime;
    private int totalEnemiesKilled;




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

        CalculateAndApplyDDA();
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
                this.playerRank = loadedData.playerRank == 0f ? 50f : loadedData.playerRank ; //Fall back to 50 if old file reading 0
                this.roomsExplored = loadedData.roomsExplored;
                this.recentRoomStats = new Queue<RoomStatsData>(loadedData.recentRoomStatsList);
                Debug.Log("Loaded save data from " + pathToSaveFile);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error loading file: " + pathToSaveFile + ": " + e.Message);
                this.roomsExplored = 0;
                this.playerRank = 50f;
                this.recentRoomStats = new Queue<RoomStatsData>();
            }
        }
        //If not, assume first load and create new save file
        else
        {
            Debug.Log("No save file found at " + pathToSaveFile + ", starting new save data using default class values.");
            this.playerRank = 50f; //Default rank to 50 if no save file
        }
    }
    private void SaveJSON()
    {
        //Create new GameSaveData object
        GameSaveData saveData = new GameSaveData();
        //Populate it with current data
        saveData.roomsExplored = this.roomsExplored;
        saveData.recentRoomStatsList = this.recentRoomStats.ToList();
        saveData.playerRank = this.playerRank;
        //Serialize to JSON and write to file
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(pathToSaveFile, json);
        Debug.Log("Saved play data to " + pathToSaveFile);
    }

    public float GetRank()
    {
        return playerRank;
    }

    public void SetRank(float newRank)
    {
        //Check between 0 and 100
        if (newRank >= 0 && newRank <= 100)
        {
            playerRank = newRank;
        }
    }

    public void CalculateAndApplyDDA()
    {
        if (recentRoomStats.Count == 0) return; //Early return if no data
       
        //Reset all values to 0 to only let recent rooms influence DDA
        totalHitsTaken = 0;
        totalTrapDamage = 0;
        totalAttacksUsed = 0;
        totalAttacksHit = 0;
        totalTime = 0;
        totalEnemiesKilled = 0;

        //Loop through each room and tally totals
        foreach(var room in recentRoomStats)
        {
            totalHitsTaken += room.playerStats.GetMeleeDamageTaken() + room.playerStats.GetRangedDamageTaken();
            totalTrapDamage += room.playerStats.GetTrapDamageTaken();
            totalAttacksUsed += room.combatStats.GetAttacksUsed();
            totalAttacksHit += room.combatStats.GetAttacksHit();
            totalTime += room.explorationStats.GetTimeSpent();
            totalEnemiesKilled += room.combatStats.GetTotalEnemiesDefeated();
        }

        int roomCount = recentRoomStats.Count;
        float averageHitsTaken = (float)totalHitsTaken/roomCount;
        float averageTrapHitsTaken = (float)totalTrapDamage/roomCount;
        float accuracy = totalAttacksUsed > 0 ? ((float)totalAttacksHit / totalAttacksUsed) * 100f : 0f; //Check we actually swung our weapon, if not return 0
        float averageTimePerEnemy = totalEnemiesKilled > 0 ? totalTime / totalEnemiesKilled : 0f; //Check we actually defeated enemies, if not return 0

        RankCalculator rankCalculator = FindFirstObjectByType<RankCalculator>();
        if (rankCalculator != null)
        {
            rankCalculator.CalculateRankDelta(averageHitsTaken, averageTrapHitsTaken, averageTimePerEnemy, accuracy);
        }
        else
        {
            Debug.LogWarning("Could not update DDA as rank calculator was not found."); 
        }

    }
}
