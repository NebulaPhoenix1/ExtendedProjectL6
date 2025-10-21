using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Health : MonoBehaviour
{

    [SerializeField] private int maxHealth;
    [SerializeField] private float invincibilityTime = 0.8f;
    

    //UnityEvents so we can keep track of play data for DDA later on
    public UnityEvent OnDeath;
    public UnityEvent OnDamageTaken;
    public UnityEvent OnHeal;

    private int health;
    private bool isInvincible = false;

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
        if(!isInvincible)
        {
            health -= (int)dmgAmount;
            OnDamageTaken.Invoke();
            if (health <= 0)
            {
                OnDeath.Invoke();
            }
            Debug.Log(gameObject.name + " took " + dmgAmount + " damage. Current Health: " + health);
            StartCoroutine(Invincibility());
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

    //Wait IEnumerator for invincibility frames
    private IEnumerator Invincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
    }
}
