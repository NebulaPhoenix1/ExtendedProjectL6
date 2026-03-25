using UnityEngine;

public class PointOfInterest : MonoBehaviour
{
    [SerializeField] public PointOfInterestData pointOfInterestData;

    private void Start()
    {
        if (pointOfInterestData == null)
        {
            Debug.LogWarning("No PointOfInterestData assigned to PointOfInterest on " + gameObject.name);
        }
        else
        {
            //Inform parent room of contents
            RoomController parentRoom = GetComponentInParent<RoomController>();
            if (parentRoom == null)
            {
                Debug.LogWarning("PointOfInterest " + gameObject.name + " has no parent RoomController");
            }
            else
            {
                parentRoom.UpdateRoomStartingDataCount(pointOfInterestData.numberOfEnemies,
                                              pointOfInterestData.isTrapPOI ? 1 : 0,
                                              pointOfInterestData.isLootPOI ? 1 : 0);
            }
        }
    }
}
