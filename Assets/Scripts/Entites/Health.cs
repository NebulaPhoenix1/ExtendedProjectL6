using UnityEngine;
using UnityEngine.Events;


public class Health : MonoBehaviour
{

    [SerializeField] private int maxHealth;

    //UnityEvents so we can keep track of play data for DDA later on
    public UnityEvent OnDeath;
    public UnityEvent OnDamageTaken;
    public UnityEvent OnHeal;

    private int health;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
    }

    public int GetHealth()
    {
        return health;
    }

    //Takes a uint for DMG amount to ensure we never get negative damage (healing)
    public void TakeDamage(uint dmgAmount)
    {
        health -= (int)dmgAmount;
        OnDamageTaken.Invoke();
        if (health <= 0)
        {
            OnDeath.Invoke();
        }
    }

    //Takes a uint for DMG amount to ensure we never get negative healing (damage)
    public void RecieveHealing(uint healAmount)
    {
        health += (int)healAmount;
        if(health > maxHealth)
        {
            health = maxHealth;
        }
        OnHeal.Invoke();
    }
}
