using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public abstract class BaseEnemy : MonoBehaviour
{
    [Header("Base Enemy References")]
    [SerializeField] protected Transform playerTransform;
    protected NavMeshAgent agent;
    protected Health health;
    protected RoomController parentRoomController;

    [Header("Base Enemy Stats")]
    [SerializeField] protected float attackCooldown = 2f;
    [SerializeField] protected int attackDamage = 1;

    protected float currentAttackCooldown = 0f; //How long until the enemy can attack again
    protected bool isActive = false; //Can the enemy path find to the player

    //Note to self because its been ages since I've done inheritance in unity:
    //Virtual methods can be overwritten by child classes, and add their own logic as well as call the base version
    protected virtual void Start()
    {
        //Get NavMeshAgent component, and if it does not exist create one
        agent = GetComponent<NavMeshAgent>();
        if(agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
            Debug.LogWarning("Added default NavMeshAgent component to GameObject: " + gameObject.name);
        }

        //Get Player
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        //Log error if player can't be found
        if(playerTransform == null) 
        {
            Debug.LogError("BaseEnemy.cs could not find player transform. GameObject: " + gameObject.name);
        }

        //Check if we can actually pathfind. If any of these fail, we will be unable to pathfind
        if (agent != null && agent.isOnNavMesh && playerTransform != null) 
        {
            isActive = true;
        }
        else
        {
            Debug.LogError("BaseEnemy.cs failed to initalise path finding required values. GameObject: " + gameObject.name);
        }

        //Get health component, and if it does not exist create one
        health = GetComponent<Health>();
        if (!health)
        {
            health = this.gameObject.AddComponent<Health>();
        }
        //Set up events for on death
        health.OnDeath.AddListener(OnDeath);
        //Get parent room controller
        parentRoomController = GetComponentInParent<RoomController>();
        if (!parentRoomController)
        {
            Debug.LogWarning("Enemy " + gameObject.name + " has no parent RoomController");
        }
        else
        {
            health.OnDeath.AddListener(() => parentRoomController.updateRoomDataCount(-1, 0, 0));
        }
    }

    protected virtual void Update()
    {
        //Check if we are able to pathfind. If not, we return early
        if(!isActive)
        {
            if(agent.isOnNavMesh) { agent.isStopped = true; } //Make enemy stand still if we can
            return;
        }
        if (parentRoomController.IsPlayerInRoom()) //Only update enemies if player is in the same room
        {
            //Tick attack cooldown if needed
            if (currentAttackCooldown > 0f)
            {
                currentAttackCooldown -= Time.deltaTime;
            }

            //Check if player is in attack range, if so stop moving and attack
            if (PlayerInAttackRange())
            {
                agent.isStopped = true;
                Attack();
            }
            //Else set destination and start moving again
            else
            {
                agent.isStopped = false;
                agent.SetDestination(playerTransform.position);
            }
        }
    }

    protected virtual void OnDeath()
    {
        isActive = false;
        agent.isStopped = true;
        Destroy(this.gameObject, 0.1f); //Destroys enemy after a short delay
    }

    //Abstract methods: these gotta be defined by child classes
    protected abstract bool PlayerInAttackRange();
    protected abstract void Attack();
}
