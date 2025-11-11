using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private GameObject doorFrame;
    [SerializeField] private GameObject leftDoorPanel;
    [SerializeField] private GameObject rightDoorPanel;

    private RoomController parentRoomController;
    private void Start()
    {
        parentRoomController = GetComponentInParent<RoomController>();
    }

    public void UnlockDoor()
    {
        leftDoorPanel.SetActive(false);
        rightDoorPanel.SetActive(false);
    }

    public void LockDoor()
    {
        leftDoorPanel.SetActive(true);
        rightDoorPanel.SetActive(true);
    }
}
