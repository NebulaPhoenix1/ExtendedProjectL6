using UnityEngine;
using UnityEngine.Events;
public class PlayerHealthStats : MonoBehaviour
{
    private Health playerHealth;
    private PlayerAttack playerAttack;
    private RoomStats currentRoom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerHealth = GetComponent<Health>();
        playerAttack = GetComponent<PlayerAttack>();
        playerHealth.OnDeath.AddListener(OnPlayerDeath);
        playerHealth.OnHeal.AddListener(OnPlayerHealed);
        playerHealth.OnDamageTaken.AddListener(OnPlayerDamaged);
    }

    private void OnPlayerDeath()
    {
        currentRoom = playerAttack.currentRoomStats;
        currentRoom.playerStats.IncrementDeathCount();
    }

    private void OnPlayerHealed()
    {
        currentRoom = playerAttack.currentRoomStats;
        currentRoom.playerStats.AddHealingDone((uint)playerHealth.getLastHealAmount());
    }

    private void OnPlayerDamaged()
    {
        currentRoom = playerAttack.currentRoomStats;
        currentRoom.playerStats.AddDamageTaken((uint)playerHealth.getLastDamageAmount());
    }
}
