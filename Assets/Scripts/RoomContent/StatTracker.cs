using UnityEngine;

//These classes keep track of various stats during gameplay, 
//organised by what aspect of gameplay they correlate to

//Each stat is private with public methods to modify them to ensure encapsulation

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
    private int roomsExplored; //How many rooms the player completed
    private int trapsPlayerActivated; //How many traps the player triggered
    private int trapsEnemyActivated; //How many traps enemies triggered
    private int lootCollected; //How much loot the player has collected
    private float timeSpent; //Total time spent playing

    public void IncrementRoomsExplored()
    {
        roomsExplored++;
    }
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

//This class has a reference to all the stat categories 
//It is responsible for persisting stats across scenes and updating them
//Other classes invoke unity events which should update the stats here
//This class is a singleton to ensure only one instance exists
public class StatTracker : MonoBehaviour
{
    //Singleton instance
    public static StatTracker Instance { get; private set; }

    public PlayerStats playerStats = new PlayerStats();
    public CombatStats combatStats = new CombatStats();
    public ExplorationStats explorationStats = new ExplorationStats();

    void Awake()
    {
        //Singleton pattern implementation
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        //Update time every frame
        explorationStats.AddTimeSpent(Time.deltaTime);
    }
}
