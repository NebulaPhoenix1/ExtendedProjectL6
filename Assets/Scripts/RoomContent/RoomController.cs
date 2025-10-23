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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
