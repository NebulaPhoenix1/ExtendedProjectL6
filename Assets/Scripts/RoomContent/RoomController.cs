using UnityEngine;
using UnityEngine.Events;

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

    private int totalEnemies = 0;
    private int totalTraps = 0;
    private int totalLoot = 0;

    private GameObject player;
    private bool playerInRoom = false;

    private RoomGenerator roomGenerator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        statTracker = StatTracker.Instance;
        player = GameObject.FindGameObjectWithTag("Player");

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
                }
            }

        });
        roomGenerator = FindFirstObjectByType<RoomGenerator>();
        if (roomGenerator != null)
        {
            RoomDeleted.AddListener(() => { roomGenerator.RemoveOldestRoom(); });
        }
    }

    public bool getRoomClearStatus()
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
    public void updateRoomDataCount(int enemies, int traps, int loot)
    {
        totalEnemies += enemies;
        totalTraps += traps;
        totalLoot += loot;

        if (totalEnemies == 0 && totalLoot == 0 && IsPlayerInRoom() && !isCleared)
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
        if (totalEnemies == 0 && totalLoot == 0 && IsPlayerInRoom() && !isCleared)
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
}
