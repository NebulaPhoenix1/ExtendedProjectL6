using UnityEngine;

public class RoomController : MonoBehaviour
{
    /* 
        Each room has a controller to manage its contents
        There is a reference to the next and previous rooms so we can navigate between them
        This will keep track of enemies, items, and other interactable objects in the room
    */

    private bool isCleared = false;
    public RoomController previousRoom;
    public RoomController nextRoom;

    [Tooltip("Doors")]
    [SerializeField] private GameObject doorUp;
    [SerializeField] private GameObject doorDown;
    [SerializeField] private GameObject doorLeft;
    [SerializeField] private GameObject doorRight;

    private GameObject nextDoor;
    private GameObject previousDoor;

    //These variables keep track of room contents
    //Each POI has a POIData component which has a POI Data scriptable object assigned to it
    //Each scriptable object stores how many enemies there are, whether its trapped or has loot. 

    private int totalEnemies = 0;
    private int totalTraps = 0;
    private int totalLoot = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void DoorDisable()
    {
        //Calcaulte which doors to enable based on room connections
        //Subtract this room's position from the next and previous rooms to determine direction
        Vector3 currentPos = transform.position;
        if (nextRoom != null)
        {
            Vector3 directionToNext = nextRoom.transform.position - currentPos;
            if (directionToNext.z > 0)
            {
                nextDoor = doorUp;
            }
            else if (directionToNext.z < 0)
            {
                nextDoor = doorDown;
            }
            else if (directionToNext.x < 0)
            {
                nextDoor = doorLeft;
            }
            else if (directionToNext.x > 0)
            {
                nextDoor = doorRight;
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
                previousDoor = doorUp;
            }
            else if (directionToPrevious.z < 0)
            {
                previousDoor = doorDown;
            }
            else if (directionToPrevious.x < 0)
            {
                previousDoor = doorLeft;
            }
            else if (directionToPrevious.x > 0)
            {
                previousDoor = doorRight;
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

        if(totalEnemies == 0 && totalLoot == 0)
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
            nextDoor.SetActive(false);
            nextRoom.previousDoor.SetActive(false);
        }
        else { Debug.LogWarning("Next door or next room is null on " + (gameObject.name) + " so room cannot unlock."); }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
