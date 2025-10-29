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

    private StatTracker statTracker; 

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

        //Get Stat Tracker instance
        statTracker = StatTracker.Instance;
        if (statTracker == null)
        {
            Debug.LogError("StatTracker instance not found by: " + gameObject.name);
        }
        else
        {
            Debug.Log("StatTracker instance found by: " + gameObject.name);
            health.OnDeath.AddListener(() => statTracker.combatStats.IncrementMeleeEnemiesDefeated());
        }

        //Get parent room controller
        parentRoom = GetComponentInParent<RoomController>();
        if (!parentRoom)
        {
            Debug.LogWarning("Enemy " + gameObject.name + " has no parent RoomController");
        }
        else
        {
            health.OnDeath.AddListener(() => parentRoom.updateRoomDataCount(-1, 0, 0));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isActive)
        {
            if(currentState == AIState.Pathing)
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
            //Debug.Log("Attacking Player");
        }
    }

    //Deletes entity (animation in future)
    public void OnDeath()
    {
        Destroy(this.gameObject);
    }
}
