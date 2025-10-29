using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerAttack : MonoBehaviour
{
    private InputAction attackAction;
    [SerializeField] private uint attackDamage = 3;
    [SerializeField] private float attackCooldown = 0.6f;
    private BoxCollider hitDetectionVolume;
    private float currentCooldown = 0f;
    private StatTracker statTracker;


    private uint totalDamage = 0;
    public UnityEvent OnAttack;
    public UnityEvent OnAttackHit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
        //Hit detection volume is a child object with a box collider
        hitDetectionVolume = GetComponentInChildren<BoxCollider>();
        if (attackAction == null)
        {
            Debug.LogError("Attack action not found in Input System for PlayerAttack script attached to " + gameObject.name);
        }
        if (!hitDetectionVolume)
        {
            Debug.LogError("Hit detection volume (BoxCollider) not found in children for PlayerAttack script attached to " + gameObject.name);
        }

        //Get stat tracker instance
        statTracker = StatTracker.Instance;
        if (statTracker == null)
        {
            Debug.LogError("StatTracker instance not found by PlayerAttack Component");
        }
        else
        {
            OnAttack.AddListener(() => statTracker.combatStats.AddDamageDealt(totalDamage));
            OnAttack.AddListener(() => statTracker.combatStats.IncrementAttacksUsed());
            OnAttackHit.AddListener(() => statTracker.combatStats.IncrementAttacksHit());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(attackAction.WasPressedThisFrame() && currentCooldown <= 0)
        {
            ProcessHit();
        }
        else if(currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
        }
    }

    private void ProcessHit()
    {
        //Reset hit tracking variables
        totalDamage = 0;
        //For all entities (excl. player) in the hit detection volume, apply damage
        Collider[] hitColliders = Physics.OverlapBox(hitDetectionVolume.bounds.center, hitDetectionVolume.bounds.extents, hitDetectionVolume.transform.rotation);
        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject != this.gameObject && collider.TryGetComponent<Health>(out Health entityHealth))
            {
                entityHealth.TakeDamage(attackDamage);
                totalDamage += attackDamage;
            }
        }
        currentCooldown = attackCooldown;
        OnAttack.Invoke(); //Invoke attack event after processing hit
        if(totalDamage > 0)
        {
            OnAttackHit.Invoke();
        }
    }
}
