using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trap : MonoBehaviour
{
    [SerializeField] private uint damageAmount = 1;
    [SerializeField] private float damageTickDelay = 0.5f;
    
    public UnityEvent TrapPlayerDamageDealt;
    public UnityEvent TrapEnemyDamageDealt;

    private RoomStats parentRoom;

    //Dictionary to keep track of GameObjects that have been damaged by the trap and the time they can next be damaged
    private Dictionary<GameObject, float> nextDamageTimes = new Dictionary<GameObject, float>();

    void Start()
    {
        parentRoom = GetComponentInParent<RoomStats>();
        if (parentRoom != null)
        {
            TrapPlayerDamageDealt.AddListener(() =>
            {
                parentRoom.GetComponent<RoomStats>().explorationStats.IncrementTrapsPlayerActivated();
            });
            TrapEnemyDamageDealt.AddListener(() =>
            {
                parentRoom.GetComponent<RoomStats>().explorationStats.IncrementTrapsEnemyActivated();
            });
        }
        else
        {
            Debug.LogWarning("Trap could not find parent RoomStats.");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Health targetHealth = other.GetComponent<Health>();
        if(targetHealth)
        {
            if(DealDamageCheck(other.gameObject))
            {
                DealDamage(other.gameObject, targetHealth);
            }
        }
    }

    private bool DealDamageCheck(GameObject target)
    {
        if(!nextDamageTimes.ContainsKey(target) || Time.time >= nextDamageTimes[target])
        {
            return true;
        }
        return false;
    }

    private void DealDamage(GameObject target, Health targetHealth)
    {
        targetHealth.TakeTrapDamage(damageAmount);
        nextDamageTimes[target] = Time.time + damageTickDelay;
        if(target.CompareTag("Player"))
        {
            TrapPlayerDamageDealt.Invoke();
        }
        else
        {
            TrapEnemyDamageDealt.Invoke();
        }
    }

    //Clean up when object steps off trap
    private void OnTriggerExit(Collider other)
    {
        if (nextDamageTimes.ContainsKey(other.gameObject))
        {
            nextDamageTimes.Remove(other.gameObject);
        }
    }
}
