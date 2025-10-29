using UnityEngine;

//These classes keep track of various stats during gameplay, 
//organised by what aspect of gameplay they correlate to

[System.Serializable]
public class PlayerStats
{
    public int healingDone; //Total healing done by the player
    public int damageTaken; //Total damage taken by the player
    public int deathCount; //How many times the player has died
}

[System.Serializable]
public class CombatStats
{
    public int enemiesDefeated; //How many enemies the player has killed
    public int meleeEnemiesDefeated; //How many melee enemies the player has killed
    public int rangedEnemiesDefeated; //How many ranged enemies the player has killed
    public int attacksUsed; //How many times the player attacked
    public int attacksHit; //How many attacks hit something
    public int damageDealt; //Total damage dealt to enemies
}

[System.Serializable]
public class ExplorationStats
{
    public int roomsExplored; //How many rooms the player completed
    public int trapsActivated; //How many traps the player triggered
    public int lootCollected; //How much loot the player has collected
    public float timeSpent; //Total time spent playing
}

//This class has a reference to all the stat categories 
//It is responsible for persisting stats across scenes and updating them
//Other classes invoke unity events which should update the stats here
public class StatTracker : MonoBehaviour
{
    public PlayerStats playerStats = new PlayerStats();
    public CombatStats combatStats = new CombatStats();
    public ExplorationStats explorationStats = new ExplorationStats();

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
