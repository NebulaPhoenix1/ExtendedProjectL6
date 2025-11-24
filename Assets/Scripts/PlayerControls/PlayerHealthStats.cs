using UnityEngine;
using UnityEngine.Events;

public class PlayerHealthStats : MonoBehaviour
{
    private Health playerHealth;
    private PlayerAttack playerAttack;
    private RoomStats currentRoom;

    private PlayerHealthHUD playerHealthHUD;

    //Start is called once before the first execution of Update after the MonoBehaviour is created
    //Adds player specific health event listeners to update room stats and HUD
    void Start()
    {
        playerHealth = GetComponent<Health>();
        playerAttack = GetComponent<PlayerAttack>();
        playerHealthHUD = FindFirstObjectByType<PlayerHealthHUD>();

        playerHealth.OnDeath.AddListener(OnPlayerDeath);
        playerHealth.OnHeal.AddListener(OnPlayerHealed);
        playerHealth.OnMeleeDamageTaken.AddListener(OnPlayerMeleeDamaged);
        playerHealth.OnRangedDamageTaken.AddListener(OnPlayerRangedDamaged);
        playerHealth.OnTrapDamageTaken.AddListener(OnPlayerTrapDamaged);

        playerHealth.OnHeal.AddListener(playerHealthHUD.UpdateHealthDisplay);
        playerHealth.OnMeleeDamageTaken.AddListener(playerHealthHUD.UpdateHealthDisplay);
        playerHealth.OnRangedDamageTaken.AddListener(playerHealthHUD.UpdateHealthDisplay);
        playerHealth.OnTrapDamageTaken.AddListener(playerHealthHUD.UpdateHealthDisplay);
    }

    private void OnPlayerDeath()
    {
        currentRoom = playerAttack.currentRoomStats;
        currentRoom.playerStats.IncrementDeathCount();
    }
    private void OnPlayerHealed()
    {
        currentRoom = playerAttack.currentRoomStats;
        currentRoom.playerStats.AddHealingDone((uint)playerHealth.GetLastHealAmount());
    }
    private void OnPlayerMeleeDamaged()
    {
        currentRoom = playerAttack.currentRoomStats;
        currentRoom.playerStats.AddMeleeDamageTaken((uint)playerHealth.GetLastMeleeDamageAmount());   
    }
    private void OnPlayerRangedDamaged()
    {
        currentRoom = playerAttack.currentRoomStats;
        currentRoom.playerStats.AddRangedDamageTaken((uint)playerHealth.GetLastRangedDamageAmount());
    }
    private void OnPlayerTrapDamaged()
    {
        currentRoom = playerAttack.currentRoomStats;
        currentRoom.playerStats.AddTrapDamageTaken((uint)playerHealth.GetLastTrapDamageAmount());
    }
}
