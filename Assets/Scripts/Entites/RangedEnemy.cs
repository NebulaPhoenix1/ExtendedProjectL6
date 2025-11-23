using NUnit.Framework;
using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEngine;

public class RangedEnemy : BaseEnemy
{
    [Header("Ranged Specific Values")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float minimumRange = 2f;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private LayerMask lineOfSightExclusion; 
    [SerializeField] private float maxAttackAngle = 10f; 

    protected override void Start()
    {
        base.Start(); //Calls base start in BaseEnemy.cs
        if(parentRoomController != null)
        {
            health.OnDeath.AddListener(() => parentRoomController.GetComponent<RoomStats>().combatStats.IncrementRangedEnemiesDefeated());
        }
        if(lineOfSightExclusion == 0) { Debug.LogWarning(name + " LayerMask not set. Enemy may block their own LOS."); }
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
                return;
            }
            //Chase if out of range
            else if(distToPlayer > attackRange)
            {
                agent.isStopped = false;
                agent.updateRotation = true; //NavMesh can handle rotate while running
                agent.SetDestination(playerTransform.position);
                return;
            }
            //Attack if within range
            else
            {
                //Stop moving while firing
                agent.isStopped = true; 
                agent.updateRotation = false;

                FacePlayer();
                agent.isStopped = true;
                if(IsFacingPlayer() && PlayerInAttackRange())
                {
                    Attack();
                }
                return;
            }
        }
    }


    //If player is within max and minimum attack range with successful LOS check, return true
    //Else, we need to flee away from the player until we are within the ranged enemy range
    protected override bool PlayerInAttackRange()
    {
        //Debug.Log("Player in range check");
        if(playerTransform == null) { return false; } 

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        //Check to see if distance is within attack ranges, if not just return
        if(distance < minimumRange || distance > attackRange) { return false; }

        //Raycast to see if we will shoot a wall
        Vector3 raycastOrigin = projectileSpawnPoint.position;
        Vector3 raycastTarget = playerTransform.position;
        Vector3 raycastDirction = (raycastTarget - raycastOrigin).normalized;
        RaycastHit hit;

        

        if(Physics.Raycast(raycastOrigin, raycastDirction, out hit, attackRange, lineOfSightExclusion))
        {
            if(hit.transform != playerTransform)
            {
                //Debug.Log("Player not in range");
                return false; //Hit a wall
            }
        }
        return true;
    }

    protected override void Attack()
    {
        if(currentAttackCooldown > 0) { return;  } //Attack on cooldown we must wait
        currentAttackCooldown = attackCooldown;

        Vector3 vectorToPlayer = playerTransform.position - projectileSpawnPoint.position;
        Vector3 direction = vectorToPlayer.normalized;
        //Calculate rotation
        Quaternion bulletRotation = Quaternion.LookRotation(direction);
        //Create projectile
        GameObject newProjectile = Instantiate(projectile, projectileSpawnPoint.position, bulletRotation);
        
        //Ignore collisions between projetile parent and parent
        Collider enemyCollider = GetComponent<Collider>();
        Collider projectileCollider = newProjectile.GetComponent<Collider>();
        if (enemyCollider != null && projectileCollider != null)
        {
            Physics.IgnoreCollision(enemyCollider, projectileCollider);
        }

        //Set projectile velocity
        Rigidbody rb = newProjectile.GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
        }
        else
        {
            Debug.LogWarning("Projectile prefab has no Rigid Body component");
        }
        //Debug.Log("Attacked");
    }

    private void FacePlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        //Debug.Log("Facing player");
    }

    private void FleeFromPlayer()
    {
        agent.isStopped = false;
        agent.updateRotation = true;
        Vector3 directionToPlayer = transform.position - playerTransform.position;
        Vector3 newPosition = transform.position + directionToPlayer.normalized * minimumRange; //Move Away
        agent.SetDestination(newPosition);
        //Debug.Log("Fleeing");
    }

    //Return true/false if the player is within MaxAttackAngle degrees of player
    private bool IsFacingPlayer()
    {
        Vector3 directionToPlayyer = (playerTransform.position - transform.position).normalized;
        return Vector3.Angle(transform.forward, directionToPlayyer) < maxAttackAngle;
    }
}
