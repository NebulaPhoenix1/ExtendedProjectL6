using UnityEngine;

[CreateAssetMenu(fileName = "PointOfInterestData", menuName = "Scriptable Objects/PointOfInterestData]")]
public class PointOfInterestData : ScriptableObject
{
    public int numberOfEnemies;
    public bool isTrapPOI;
    public bool isLootPOI;
}
