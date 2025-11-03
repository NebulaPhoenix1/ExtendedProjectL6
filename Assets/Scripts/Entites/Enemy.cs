using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

//Use NavMesh for pathfinding
//Enemies have 3 difficulties
//State Machine for AI behaviour

public class Enemy : MonoBehaviour
{
    enum AIState
    {
        Pathing,
        Attacking,
    }

    [SerializeField] private Transform playerTransform;
    private bool isActive = false; //If enemy can pathfind to player
    private NavMeshAgent agent;
    private AIState currentState = AIState.Pathing;

    private Health health;
    private RoomController parentRoom; //Each enemy belong to a room, we need a reference to it to update enemy count on death

    private float attackCooldown = 2f;
    private float currentAttackCooldown = 0f;
    [SerializeField] private BoxCollider hitVolume;
    [SerializeField] private int attackDamage = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {        
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if (agent != null && agent.isOnNavMesh && playerTransform != null) { isActive = true; } 
        
        //Get health component, and if it does not exist create one
        health = GetComponent<Health>();
        if(!health )
        {
            health = this.gameObject.AddComponent<Health>();
        }
        //Set up events for on death
        health.OnDeath.AddListener(OnDeath);


        //Get parent room controller
        parentRoom = GetComponentInParent<RoomController>();
        if (!parentRoom)
        {
            Debug.LogWarning("Enemy " + gameObject.name + " has no parent RoomController");
        }
        else
        {
            health.OnDeath.AddListener(() => parentRoom.updateRoomDataCount(-1, 0, 0));
            health.OnDeath.AddListener(() => parentRoom.GetComponent<RoomStats>().combatStats.IncrementMeleeEnemiesDefeated());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isActive)
        {
            //Tick attack cooldown
            if(currentAttackCooldown > 0f)
            {
                currentAttackCooldown -= Time.deltaTime;
            }
            //Update States
            if (currentState == AIState.Pathing)
            {
                EnemyPath();
            }
            else
            {
                Attack();
            }
        }
    }

    void EnemyPath()
    {
        if(Vector3.Distance(transform.position, playerTransform.position) > 2f)
        {
            agent.SetDestination(playerTransform.position);
        }
        else
        {
            currentState = AIState.Attacking;
        }
    }

    void Attack()
    {
        if(Vector3.Distance(transform.position, playerTransform.position) > 2f)
        {
            currentState = AIState.Pathing;
        }
        else
        {
            //Attack logic here
            //Each melee enemy has a cube with a box colldier with is trigger
            //If player is in this trigger, deal damage to player
            if(currentAttackCooldown <= 0f)
            {
                currentAttackCooldown = attackCooldown;
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

    //Deletes entity (animation in future)
    public void OnDeath()
    {
        Destroy(this.gameObject);
    }
}
