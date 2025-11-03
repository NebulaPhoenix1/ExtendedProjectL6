using System.Collections.Generic;
using UnityEngine;

//This class keeps track of the last 20 room's play stats
//We use a queue for this data so when we have 20 sets of data, we dequeue the oldest one when adding a new one
//This ensures old play data is not affecting current play data analysis too much

[System.Serializable]
public class StatTracker : MonoBehaviour
{
    //Singleton instance
    public static StatTracker Instance { get; private set; }

    private Queue<RoomStats> recentRoomStats = new Queue<RoomStats>();
    [SerializeField] private int maxRecentRooms = 20;

    private int roomsExplored = 0;
    

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

    void Update()
    {
        
    }

    public void AddRoomStats(RoomStats latestRoom)
    {
        if(recentRoomStats.Count >= maxRecentRooms)
        {
            recentRoomStats.Dequeue();
        }
        recentRoomStats.Enqueue(latestRoom);
        roomsExplored++;
       
    }
}
