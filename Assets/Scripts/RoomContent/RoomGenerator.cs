using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class RoomGenerator : MonoBehaviour
{
    /* 
       Simple room spawning system which takes one room prefab and spawns them in a grid Legend of Zelda style
       The first room is spawned in by default for now just to ensure the player does not fall through the floor
       We need to find the inital offset for the first room
       Then spawn, pick a direction to spawn the next, check if its valid and if so repeat
    */

    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private int numberOfRoomsToGenerate = 10; 
    private float roomSize = 21f; //Rooms will be square for simplicity
    private Vector3 currentRoomPosition = Vector3.zero;

    private RoomController currentRoom;
    private RoomController lastRoom;
    private GameObject instantiatedRoom;
    public UnityEvent InitalRoomsGenerated;

    //This is a list of unique items. We can use this to track where rooms have already been spawned 
    private HashSet<Vector3> usedPositions = new HashSet<Vector3>();

    //A queue to keep track of rooms for removal later on
    private  Queue<RoomController> spawnedRooms = new Queue<RoomController>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Current room is first pre placed room
        currentRoom = FindObjectsByType<RoomController>(FindObjectsSortMode.None)[0];
        spawnedRooms.Enqueue(currentRoom);
        InitalRoomsGenerated.AddListener(currentRoom.DetermineDoorSequence);

        usedPositions.Add(currentRoomPosition);
        //Loop through spawning rooms
        for (int i = 0; i < numberOfRoomsToGenerate;)
        {
            GenerateRoom();
            i++;
        }
        InitalRoomsGenerated.Invoke();
    }

    private void GenerateRoom()
    {
        //Pick a direction and calculate next room position
        int directionIndex = Random.Range(0, 4);
        Vector3 nextPos = currentRoomPosition; //Temporary storage for next room
        switch (directionIndex)
        {
            case 0:
                nextPos.z += roomSize;
                break;
            case 1:
                nextPos.z -= roomSize;
                break;
            case 2:
                nextPos.x -= roomSize;
                break;
            case 3:
                nextPos.x += roomSize;
                break;
        }
        //Invalid position, try again
        if (usedPositions.Contains(nextPos))
        {
            GenerateRoom();
            return;
        }
        //Valid position, add position to usedPositions and spawn room
        currentRoomPosition = nextPos;
        usedPositions.Add(currentRoomPosition);
        instantiatedRoom = Instantiate(roomPrefab, currentRoomPosition, Quaternion.identity);
        InitalRoomsGenerated.AddListener(instantiatedRoom.GetComponent<RoomController>().DetermineDoorSequence);

        //Link Rooms
        lastRoom = currentRoom;
        lastRoom.nextRoom = instantiatedRoom.GetComponent<RoomController>();
        currentRoom = instantiatedRoom.GetComponent<RoomController>();
        currentRoom.previousRoom = lastRoom;

        lastRoom.DetermineDoorSequence();
        currentRoom.DetermineDoorSequence();

        //Add current room to spawned rooms queue
        spawnedRooms.Enqueue(currentRoom);

    }
    
    public void RemoveOldestRoom() 
    {
        //This will be called after generating a new room 
        //We will remove the oldest room and free that position in the hash set
        RoomController roomToRemove = spawnedRooms.Dequeue();
        usedPositions.Remove(roomToRemove.transform.position);
        Destroy(roomToRemove.gameObject);
        GenerateRoom();
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
