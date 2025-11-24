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
    public UnityEvent OnMeleeDamageTaken;
    public UnityEvent OnRangedDamageTaken;
    public UnityEvent OnTrapDamageTaken;
    public UnityEvent OnHeal;

    private int health;
    private bool isInvincible = false;

    private int lastHealAmount = 0;
    private int lastMeleeDamageAmount = 0;
    private int lastRangedDamageAmount = 0;
    private int lastTrapDamageAmount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
    }

    //Takes a uint for DMG amount to ensure we never get negative damage (healing)
    public void TakeMeleeDamage(uint dmgAmount)
    {
        if(!isInvincible)
        {
            lastMeleeDamageAmount = (int)dmgAmount;
            if (health - (int)dmgAmount <= 0)
            {
                //Debug.Log(gameObject.name + " took " + dmgAmount + " damage and has died.");
                health = 0;
                OnDeath.Invoke();
            }
            else
            {
                health -= (int)dmgAmount;
                OnMeleeDamageTaken.Invoke();
                //Debug.Log(gameObject.name + " took " + dmgAmount + " damage. Current Health: " + health);
                StartCoroutine(Invincibility());
            }
        }
    }
    //Ranged Damage
    public void TakeRangedDamage(uint dmgAmount)
    {
        if (!isInvincible)
        {
            lastRangedDamageAmount = (int)dmgAmount;
            if (health - (int)dmgAmount <= 0)
            {
                //Debug.Log(gameObject.name + " took " + dmgAmount + " damage and has died.");
                health = 0;
                OnDeath.Invoke();
            }
            else
            {
                health -= (int)dmgAmount;
                OnRangedDamageTaken.Invoke();
                //Debug.Log(gameObject.name + " took " + dmgAmount + " damage. Current Health: " + health);
                StartCoroutine(Invincibility());
            }
        }
    }

    //Trap Damage
    public void TakeTrapDamage(uint dmgAmount)
    {
        if (!isInvincible)
        {
            lastTrapDamageAmount = (int)dmgAmount;
            if (health - (int)dmgAmount <= 0)
            {
                //Debug.Log(gameObject.name + " took " + dmgAmount + " damage and has died.");
                health = 0;
                OnDeath.Invoke();
            }
            else
            {
                health -= (int)dmgAmount;
                OnTrapDamageTaken.Invoke();
                //Debug.Log(gameObject.name + " took " + dmgAmount + " damage. Current Health: " + health);
                StartCoroutine(Invincibility());
            }
        }
    }

    //Takes a uint for DMG amount to ensure we never get negative healing (damage)
    public void RecieveHealing(uint healAmount)
    {
        lastHealAmount = (int)healAmount;
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
        //Debug.Log(gameObject.name + " is now invincible for " + invincibilityTime + " seconds.");
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
        //Debug.Log(gameObject.name + " is no longer invincible.");
    }

    public int GetLastHealAmount()
    {
        return lastHealAmount;
    }

    public int GetLastMeleeDamageAmount()
    {
        return lastMeleeDamageAmount;
    }

    public int GetLastRangedDamageAmount()
    {
        return lastRangedDamageAmount;
    }
    public int GetLastTrapDamageAmount()
    {
        return lastTrapDamageAmount;
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}
