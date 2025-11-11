using UnityEngine;

public class DoorLockCheck : MonoBehaviour
{
    private RoomController roomController;

    private void Start()
    {
        roomController = GetComponentInParent<RoomController>();
    }

    public void OnTriggerEnter(Collider other)
    {
        //If player is safely in room we set a flag so the previous door can lock
        if (other.gameObject.CompareTag("Player"))
        {
            roomController.LockPreviousDoor();
        }
    }
}
