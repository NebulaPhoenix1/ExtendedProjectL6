using UnityEngine;

public class MeleeEnemy : BaseEnemy
{
    [Header("Melee Specific Values")]
    [SerializeField] private BoxCollider hitVolume;
    [SerializeField] private float attackRange = 2f;

    protected override void Start()
    {
        base.Start(); //Calls base start in BaseEnemy.cs
        if(parentRoomController != null)
        {
            health.OnDeath.AddListener(() => parentRoomController.GetComponent<RoomStats>().combatStats.IncrementMeleeEnemiesDefeated());
        }
    }

    protected override bool PlayerInAttackRange()
    {
        if(playerTransform == null) { return false; }
        else
        {
            return Vector3.Distance(transform.position, playerTransform.position) <= attackRange;
        }
    }

    protected override void Attack()
    {
        if(currentAttackCooldown > 0) { return;  } //Attack on cooldown we must wait
        else //Not on cooldown
        {
            //Get a list of every collider in the trigger, and check to see if one of them is the player if so process the hit
            Collider[] hitColliders = Physics.OverlapBox(hitVolume.bounds.center, hitVolume.bounds.extents, hitVolume.transform.rotation);
            foreach (Collider collider in hitColliders)
            {
                if (collider.gameObject.CompareTag("Player") && collider.gameObject.TryGetComponent<Health>(out Health playerHealth))
                {
                    playerHealth.TakeDamage((uint)attackDamage);
                }
            }
        }
    }
}
