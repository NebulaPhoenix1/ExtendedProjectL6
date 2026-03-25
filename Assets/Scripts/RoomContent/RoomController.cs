using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class RoomController : MonoBehaviour
{
    /* 
        Each room has a controller to manage its contents
        There is a reference to the next and previous rooms so we can navigate between them
        This will keep track of enemies, items, and other interactable objects in the room
    */

    public UnityEvent RoomCleared;
    public UnityEvent RoomDataSave;
    public UnityEvent RoomDeleted;

    private StatTracker statTracker;

    private bool isCleared = false;
    public RoomController previousRoom;
    public RoomController nextRoom;

    [Tooltip("Doors")]
    [SerializeField] private GameObject doorUp;
    [SerializeField] private GameObject doorDown;
    [SerializeField] private GameObject doorLeft;
    [SerializeField] private GameObject doorRight;

    private DoorController nextDoor;
    private DoorController previousDoor;

    //These variables keep track of room contents
    //Each POI has a POIData component which has a POI Data scriptable object assigned to it
    //Each scriptable object stores how many enemies there are, whether its trapped or has loot. 

    private int enemiesRemaining = 0;
    private int startingEnemyCount = 0;
    private int totalTraps = 0;
    private int totalLoot = 0;

    private GameObject player;
    private bool playerInRoom = false;

    private RoomGenerator roomGenerator;
    private OutOfBounds outOfBounds;

    //All POI spawners in this room, we call PointOfInterestSpawner.SpawnPOI() for each POI in the next room on room cleared
    [SerializeField] private PointOfInterestSpawner[] pointsOfInterest;
    //Flag for if the room is the very first room
    [SerializeField] private bool isStartingRoom = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        statTracker = StatTracker.Instance;
        player = GameObject.FindGameObjectWithTag("Player");
        outOfBounds = FindFirstObjectByType<OutOfBounds>();
        if(!outOfBounds)
        {
            Debug.LogWarning("RoomController.cs could not find OutOfBounds.cs in the scene.");
        }

        //Update player current room stats on room cleared when Unity Event is fired
        RoomCleared.AddListener(() =>
        {
            var playerAttack = player.GetComponent<PlayerAttack>();
            if (playerAttack != null)
            {
                if (nextRoom != null)
                {
                    playerAttack.currentRoomStats = nextRoom.GetComponent<RoomStats>();
                    Debug.Log("Player stats current room updated!");
                    if (nextRoom.pointsOfInterest.Length > 0 && PointOfInterestSpawnController.Instance != null)
                    {
                        float currentRank = StatTracker.Instance != null ? StatTracker.Instance.GetRank() : 50f; //Default to 50 if rank cant be got
                        List<GameObject> prefabsForNextRoom = PointOfInterestSpawnController.Instance.SelectPOIsForRoom(currentRank, nextRoom.pointsOfInterest.Length);
                        for (int i = 0; i < prefabsForNextRoom.Count; i++)
                        {
                            nextRoom.pointsOfInterest[i].SpawnPOI(prefabsForNextRoom[i]);
                        }
                    }
                    else if (PointOfInterestSpawnController.Instance == null)
                    {
                        Debug.LogWarning("Could not spawn POIs, POI Spawn Controller is missing");
                    }
                }
            }
        });
        roomGenerator = FindFirstObjectByType<RoomGenerator>();
        if (roomGenerator != null)
        {
            RoomDeleted.AddListener(() => { roomGenerator.RemoveOldestRoom(); });
        }

        //Spawn POIs for starting room
        if(isStartingRoom)
        {
            if(pointsOfInterest.Length > 0 && PointOfInterestSpawnController.Instance != null)
            {
                float currentRank = StatTracker.Instance != null ? StatTracker.Instance.GetRank() : 50f; //Default to 50 if rank cant be got
                List<GameObject> startingPrefabs = PointOfInterestSpawnController.Instance.SelectPOIsForRoom(currentRank, pointsOfInterest.Length);
                for (int i = 0; i < pointsOfInterest.Length; i++)
                {
                    pointsOfInterest[i].SpawnPOI(startingPrefabs[i]);
                }
            }
            else if(PointOfInterestSpawnController.Instance == null)
            {
                Debug.LogWarning("POI Spawn Controller is null! Cannot spawn starting rooms");
            }
        }
    }

    public bool GetRoomClearStatus()
    {
        return isCleared;
    }

    public void DetermineDoorSequence()
    {
        //Calcaulte which doors to enable based on room connections
        //Subtract this room's position from the next and previous rooms to determine direction
        Vector3 currentPos = transform.position;
        if (nextRoom != null)
        {
            Vector3 directionToNext = nextRoom.transform.position - currentPos;
            if (directionToNext.z > 0)
            {
                nextDoor = doorUp.GetComponent<DoorController>();
            }
            else if (directionToNext.z < 0)
            {
                nextDoor = doorDown.GetComponent<DoorController>();
            }
            else if (directionToNext.x < 0)
            {
                nextDoor = doorLeft.GetComponent<DoorController>();
            }
            else if (directionToNext.x > 0)
            {
                nextDoor = doorRight.GetComponent<DoorController>();
            }
            //Disable next door for testing
            //nextDoor.SetActive(false);
        }
        //Subtract this room's position from the previous to determine which door should open when the room unlocks
        if (previousRoom != null)
        {
            Vector3 directionToPrevious = previousRoom.transform.position - currentPos;
            if (directionToPrevious.z > 0)
            {
                previousDoor = doorUp.GetComponent<DoorController>();
            }
            else if (directionToPrevious.z < 0)
            {
                previousDoor = doorDown.GetComponent<DoorController>();
            }
            else if (directionToPrevious.x < 0)
            {
                previousDoor = doorLeft.GetComponent<DoorController>();
            }
            else if (directionToPrevious.x > 0)
            {
                previousDoor = doorRight.GetComponent<DoorController>();
            }
            //Disable previous door for testing
            //previousDoor.SetActive(false);
        }
    }

    // Function to update room content counts, this should be called by POI spawners
    public void UpdateRoomStartingDataCount(int enemies, int traps, int loot)
    {
        enemiesRemaining += enemies;
        startingEnemyCount += enemies;
        totalTraps += traps;
        totalLoot += loot;
    }

    //Call with unity events when an enemy is eliminated in the room
    public void EnemyEliminated()
    {
        enemiesRemaining--;
        if (enemiesRemaining == 0 && totalLoot == 0 && IsPlayerInRoom() && !isCleared)
        {
            isCleared = true;
            UnlockRoom();
        }
    }

    private void UnlockRoom()
    {
        //Disable door to next room (This room's next door and the next room's previous door)
        if (nextDoor && nextRoom)
        {
            nextDoor.UnlockDoor();
            nextRoom.previousDoor.UnlockDoor();
        }
        else { Debug.LogWarning("Next door or next room is null on " + (gameObject.name) + " so room cannot unlock."); }
        RoomCleared.Invoke();
    }

    public void LockPreviousDoor()
    {
        if(previousDoor)
        {
            previousDoor.LockDoor();
            return;
        }
        else
        {
            Debug.LogWarning("RoomController.cs could not lock previous door as it is null." +  gameObject.name);
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Fail safe to unlock room
        if (enemiesRemaining == 0 && totalLoot == 0 && IsPlayerInRoom() && !isCleared)
        {
            isCleared = true;
            UnlockRoom();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == player.transform)
        {
            playerInRoom = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == player.transform)
        {
            playerInRoom = false;
            if(isCleared)
            {
                RoomDataSave.Invoke();
                RoomDeleted.Invoke();
            }
        }
    }

    public bool IsPlayerInRoom()
    {
        return playerInRoom;
    }

    public int GetTrapCount() { return totalTraps; }
    public int GetStartingEnemyCount() { return startingEnemyCount; }
    public int GetLootCount() { return totalLoot; } 
}
