using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PlayerStats
{
    [SerializeField] private int healingDone; //Total healing done by the player
    [SerializeField] private int damageTaken; //Total damage taken by the player
    [SerializeField] private int deathCount; //How many times the player has died

    public void AddHealingDone(uint amount)
    {
        healingDone += (int)amount;
    }
    public void AddDamageTaken(uint amount)
    {
        damageTaken += (int)amount;
    }
    public void IncrementDeathCount()
    {
        deathCount++;
    }
}

[System.Serializable]
public class CombatStats
{
    [SerializeField] private int enemiesDefeated; //How many enemies the player has killed
    [SerializeField] private int meleeEnemiesDefeated; //How many melee enemies the player has killed
    [SerializeField] private int rangedEnemiesDefeated; //How many ranged enemies the player has killed
    [SerializeField] private int attacksUsed; //How many times the player attacked
    [SerializeField] private int attacksHit; //How many attacks hit something
    [SerializeField] private int damageDealt; //Total damage dealt to enemies

    public void IncrementMeleeEnemiesDefeated()
    {
        meleeEnemiesDefeated++;
        enemiesDefeated++;
    }

    public void IncrementRangedEnemiesDefeated()
    {
        rangedEnemiesDefeated++;
        enemiesDefeated++;
    }

    public void IncrementAttacksUsed()
    {
        attacksUsed++;
    }

    public void IncrementAttacksHit()
    {
        attacksHit++;
    }

    public void AddDamageDealt(uint amount)
    {
        damageDealt += (int)amount;
    }
}

[System.Serializable]
public class ExplorationStats
{
    [SerializeField] private int trapsPlayerActivated; //How many traps the player triggered
    [SerializeField] private int trapsEnemyActivated; //How many traps enemies triggered
    [SerializeField] private int lootCollected; //How much loot the player has collected
    [SerializeField] private float timeSpent; //Total time spent playing

   
    public void IncrementTrapsPlayerActivated()
    {
        trapsPlayerActivated++;
    }
    public void IncrementTrapsEnemyActivated()
    {
        trapsEnemyActivated++;
    }
    public void IncrementLootCollected()
    {
        lootCollected++;
    }
    public void AddTimeSpent(float amount)
    {
        timeSpent += amount;
    }
}

[System.Serializable]
public class RoomContents
{
    [SerializeField] private int startingEnemyCount; //How many enemies were there to start off with in the room?
    [SerializeField] private int trapCount; //How many traps were there in the room?
    [SerializeField] private int lootCount; //How much loot was there in the room?

    public void SetEnemyCount(int count) { startingEnemyCount = count; }
    public void SetTrapCount(int count) { trapCount = count; }
    public void SetLootCount(int count) {  lootCount = count; }

}

public class RoomStats : MonoBehaviour
{
    public PlayerStats playerStats = new PlayerStats();
    public CombatStats combatStats = new CombatStats();
    public ExplorationStats explorationStats = new ExplorationStats();
    public RoomContents roomContents = new RoomContents();

    private RoomController roomController;
    private StatTracker statTracker;

    private void Start()
    {
        roomController = GetComponent<RoomController>();
        roomController.RoomCleared.AddListener(OnRoomCleared);
        
        statTracker = StatTracker.Instance;
        if (statTracker == null)
        {
            Debug.LogError("StatTracker instance not found by RoomStats Component");
        }
        else
        {
            //Set room contents stats just before we add the room stats to the stat tracker
            roomController.RoomDataSave.AddListener(() => roomContents.SetEnemyCount(roomController.getStartingEnemyCount()));
            roomController.RoomDataSave.AddListener(() => roomContents.SetTrapCount(roomController.getTrapCount()));
            roomController.RoomDataSave.AddListener(() => roomContents.SetLootCount(roomController.getLootCount()));
            roomController.RoomDataSave.AddListener(() => StatTracker.Instance.AddRoomStats(this));
        }
    }

    void Update()
    {
        //Update time in room is not cleared and player is in the room
        if(!roomController.getRoomClearStatus() && roomController.IsPlayerInRoom())
        {
            explorationStats.AddTimeSpent(Time.deltaTime);
        }
    }

    //This is called when the room is cleared
    public void OnRoomCleared()
    {

    }
}


//We need a non-MonoBehaviour serializable class to store room stats data for JSON serialization

[System.Serializable]
public class RoomStatsData
{ 
    public PlayerStats playerStats;
    public CombatStats combatStats;
    public ExplorationStats explorationStats;
    public RoomContents roomContents;
    
    public RoomStatsData(RoomStats roomStats)
    {
        //Ensure we make a copy of stats data, not just a reference
        //We serialixe to JSON and back to create a deep copy
        playerStats = JsonUtility.FromJson<PlayerStats>(JsonUtility.ToJson(roomStats.playerStats));
        combatStats = JsonUtility.FromJson<CombatStats>(JsonUtility.ToJson(roomStats.combatStats));
        explorationStats = JsonUtility.FromJson<ExplorationStats>(JsonUtility.ToJson(roomStats.explorationStats));
        roomContents = JsonUtility.FromJson<RoomContents>(JsonUtility.ToJson(roomStats.roomContents));
    }

    public RoomStatsData() { }

}

