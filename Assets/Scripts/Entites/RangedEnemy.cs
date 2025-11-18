using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class RangedEnemy : BaseEnemy
{
    [Header("Melee Specific Values")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float minimumRange = 2f;

    protected override void Start()
    {
        base.Start(); //Calls base start in BaseEnemy.cs
        if(parentRoomController != null)
        {
            health.OnDeath.AddListener(() => parentRoomController.GetComponent<RoomStats>().combatStats.IncrementRangedEnemiesDefeated());
        }
    }

    protected override void Update()
    {
        if(!isActive)
        {
            if(agent.isOnNavMesh) { agent.isStopped = true; } //Make enemy stand still if we can
            return;
        }
        //Only update enemy if player is in the same room
        if(parentRoomController.IsPlayerInRoom())
        {
            //Tick down attack cooldown if needed
            if(currentAttackCooldown > 0f)
            {
                currentAttackCooldown -= Time.deltaTime;
            }
            float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            //Flee if too close
            if(distToPlayer < minimumRange)
            {
                FleeFromPlayer();
            }
            //Chase if out of range
            if(distToPlayer > attackRange)
            {
                agent.isStopped = false;
                agent.SetDestination(playerTransform.position);
            }
            //Attack if within range
            else
            {
                FacePlayer();
                agent.isStopped = true;
                if(PlayerInAttackRange())
                {
                    Attack();
                }
            }
        }

    }

    protected override bool PlayerInAttackRange()
    {
        if(playerTransform == null) { return false; } 
        //If player is greater than minimum distance away from player, return true
        //Else, we need to flee away from the player until we are within the ranged enemy range.
        
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        //Raycast to see if we will shoot a wall
        RaycastHit hit;
        Vector3 direction = (playerTransform.position - transform.position).normalized;

        if(Physics.Raycast(transform.position, direction, out hit, attackRange))
        {
            if(hit.transform != playerTransform)
            {
                return false; //Hit a wall
            }
        }
        //If within optimial range, return true
        return distance <= attackRange && distance >= minimumRange;
    }

    protected override void Attack()
    {
        if(currentAttackCooldown > 0) { return;  } //Attack on cooldown we must wait
        currentAttackCooldown = attackCooldown;
        GameObject newProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        //Set projectile velocity
        Rigidbody rb = newProjectile.GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
            //Make projectile face target
            newProjectile.transform.forward = direction;
        }
        else
        {
            Debug.LogWarning("Projectile prefab has no Rigid Body component");
        }
    }

    private void FacePlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void FleeFromPlayer()
    {
        agent.isStopped = false;
        Vector3 directionToPlayer = transform.position - playerTransform.position;
        Vector3 newPosition = transform.position + directionToPlayer.normalized * minimumRange; //Move Away
        agent.SetDestination(newPosition);
    }
}
