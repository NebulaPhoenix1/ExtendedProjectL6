using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PlayerStats
{
    private int healingDone; //Total healing done by the player
    private int damageTaken; //Total damage taken by the player
    private int deathCount; //How many times the player has died

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
    private int enemiesDefeated; //How many enemies the player has killed
    private int meleeEnemiesDefeated; //How many melee enemies the player has killed
    private int rangedEnemiesDefeated; //How many ranged enemies the player has killed
    private int attacksUsed; //How many times the player attacked
    private int attacksHit; //How many attacks hit something
    private int damageDealt; //Total damage dealt to enemies

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
    private int trapsPlayerActivated; //How many traps the player triggered
    private int trapsEnemyActivated; //How many traps enemies triggered
    private int lootCollected; //How much loot the player has collected
    private float timeSpent; //Total time spent playing

   
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


public class RoomStats : MonoBehaviour
{
    public PlayerStats playerStats = new PlayerStats();
    public CombatStats combatStats = new CombatStats();
    public ExplorationStats explorationStats = new ExplorationStats();

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
            roomController.RoomCleared.AddListener(() => StatTracker.Instance.AddRoomStats(this)); 
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
