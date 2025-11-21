using UnityEditor.UIElements;
using UnityEngine;

public class SimpleStraightProjectile : BaseProjectile
{
    [Header("Simple Straight Projectiles Values")]
    [SerializeField] protected string targetTag;

    protected override void OnCollisionEnter(Collision collision)
    {
        //Check if the game object has a health component
        if(collision.gameObject.CompareTag(targetTag))
        {
            //If so, check if dmgAmount is positive/negative and call deal dmg/healing respectively
            //Only do that if the tag of the gameobject matches the tag property above
            if(collision.gameObject.TryGetComponent<Health>(out Health health))
            {
                if(dmgAmount > 0)
                {
                    health.TakeDamage((uint)dmgAmount);
                }
                else if (dmgAmount < 0)
                {
                    health.RecieveHealing((uint)dmgAmount);
                }
                else {Debug.Log("Projectile: " + gameObject.name + " cannot deal 0 damage/healing."); }
            }
        }
        //Regardless, then call base functionality
        base.OnCollisionEnter(collision);
    }
}
