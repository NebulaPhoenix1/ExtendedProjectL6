using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    /* 
       Simple room spawning system which takes one room prefab and spawns them in a grid Legend of Zelda style
       The first room is spawned in by default for now just to ensure the player does not fall through the floor
       We need to find the inital offset for the first room
       Then spawn, pick a direction to spawn the next, check if its valid and if so repeat
    */

    //Enum so we can pick a direction to spawn the next room in
    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private int numberOfRoomsToGenerate = 10; 
    private Direction nextRoom = Direction.Up;
    private float roomSize = 20f; //Rooms will be square for simplicity
    private Vector2 currentRoomPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentRoomPosition = Vector3.zero;
        //Loop through spawning rooms only up for now as a test 
        for (int i = 0; i < numberOfRoomsToGenerate; i++)
        {
            currentRoomPosition.x += roomSize;
            Instantiate(roomPrefab, new Vector3(currentRoomPosition.x, currentRoomPosition.y, 0), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
