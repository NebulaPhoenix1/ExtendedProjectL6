using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class PlayerHealthHUD : MonoBehaviour
{
    private TMP_Text healthText;
    private GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        healthText = GetComponent<TMP_Text>();
        int maxHealth = player.GetComponent<Health>().getMaxHealth();
        int currentHealth = player.GetComponent<Health>().getHealth();
        healthText.text = "Health: " + maxHealth + " / " + maxHealth;
    }

    public void UpdateHealthDisplay()
    {
        int maxHealth = player.GetComponent<Health>().getMaxHealth();
        int currentHealth = player.GetComponent<Health>().getHealth();
        healthText.text = "Health: " + currentHealth + " / " + maxHealth;
    }
}
