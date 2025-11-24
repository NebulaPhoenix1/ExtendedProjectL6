using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    //This script is attached to a isTrigger collider underneath the player that follows it on the X/Z plane
    //If the player falls off the map and collides with this collider, they are reset and teleported to the center of the current room
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector3 respawnPointOffset;
    private PlayerAttack playerAttack;
    private RoomStats currentRoomStats;
    private GameObject currentRoom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(!playerTransform)
        {
            Debug.LogWarning("Player Transform not assigned in OutOfBounds script on " + gameObject.name);
        }
        else
        {
            playerAttack = playerTransform.GetComponent<PlayerAttack>();
            currentRoomStats = playerAttack.currentRoomStats;
            currentRoom = currentRoomStats.gameObject;
        }
    }

    void Update()
    {
        //Follow player on X/Z plane
        if(playerTransform)
        {
            transform.position = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
        }

    }

    public void UpdateCurrentRoom()
    {
        if(playerAttack)
        {
            currentRoomStats = playerAttack.currentRoomStats;
            currentRoom = currentRoomStats.gameObject;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform == playerTransform)
        {
            //Teleport player to center of current room
            Vector3 roomCenter = (currentRoom.transform.position + respawnPointOffset);
            playerTransform.position = new Vector3(roomCenter.x, roomCenter.y, roomCenter.z);
            Debug.Log("Reset player");
        }

    }
}
