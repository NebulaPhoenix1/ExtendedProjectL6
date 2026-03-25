using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "PointOfInterestData", menuName = "Scriptable Objects/PointOfInterestData]")]
public class PointOfInterestData : ScriptableObject
{
    [Header("Enemy and Trap Counts")]
    public int easyEnemies = 0;
    public int normalEnemies = 0;
    public int hardEnemies = 0;
    public int traps = 0;

    [Header("Other Contents")]
    public bool isLootPOI;

    [Header("Budget Spawning System")]
    [Tooltip("These are auto calculated, do NOT change")]
    public int threatCost;

    //Auto calculating values
    public int numberOfEnemies
    {
        get
        {
            return easyEnemies + normalEnemies + hardEnemies;
        }
    }

    public bool isTrapPOI
    {
        get
        {
            return traps > 0 ? true : false;
        }
    }

    //Cost weights
    private const int easyEnemyWeight = 2;
    private const int normalEnemyWeight = 4;
    private const int hardEnemyWeight = 6;
    private const int trapWeight = 1;
    private const int lootWeight = -1;

    public int AutoCalculatedCost
    {
        get
        {
            int score = 0;
            score += (easyEnemies * easyEnemyWeight);
            score += (normalEnemies * normalEnemyWeight);
            score += (hardEnemies * hardEnemyWeight);
            score += (traps * trapWeight);
            if (isLootPOI) score += lootWeight;
            return Mathf.Max(score,1); //returns cost of 1 to prevent infinite loops
        }
    }


    //Update cost in real time
    private void OnValidate()
    {
        threatCost = AutoCalculatedCost;
    }
}
