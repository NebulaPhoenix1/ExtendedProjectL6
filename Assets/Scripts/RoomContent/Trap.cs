using UnityEngine;
using UnityEngine.Events;

public class Trap : MonoBehaviour
{
    [SerializeField] private uint damageAmount = 1;

    
    public UnityEvent trapPlayerDamageDealt;
    public UnityEvent trapEnemyDamageDealt;

    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        //If other has a health component, deal damage
        //This should repeat if entity stays in trap 
        if(other.GetComponent<Health>() != null)
        {
            other.GetComponent<Health>().TakeDamage(damageAmount);
            Debug.Log(other.gameObject.name + " has triggered a trap and taken " + damageAmount + " damage.");
            //Invoke trap damage dealt if other is player
            if (other.gameObject.CompareTag("Player"))
            {
                trapPlayerDamageDealt.Invoke();
            }
            else
            {
                trapEnemyDamageDealt.Invoke();
            }
        }
    }
}
