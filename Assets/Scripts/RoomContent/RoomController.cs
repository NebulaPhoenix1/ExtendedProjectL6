using UnityEngine;

public class RoomController : MonoBehaviour
{
    /* 
        Each room has a controller to manage its contents
        There is a reference to the next and previous rooms so we can navigate between them
        This will keep track of enemies, items, and other interactable objects in the room
    */

    private bool isCleared = false;
    private int enemyCount = 0;
    public RoomController previousRoom;
    public RoomController nextRoom;

    [Tooltip("Doors")]
    [SerializeField] private GameObject doorUp;
    [SerializeField] private GameObject doorDown;
    [SerializeField] private GameObject doorLeft;
    [SerializeField] private GameObject doorRight;

    private GameObject nextDoor;
    private GameObject previousDoor;

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
            nextDoor.SetActive(false);
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
            previousDoor.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
