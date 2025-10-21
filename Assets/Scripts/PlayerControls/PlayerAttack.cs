using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private InputAction attackAction;
    [SerializeField] private uint attackDamage = 3;
    [SerializeField] private float attackCooldown = 0.6f;
    private BoxCollider hitDetectionVolume;
    private float currentCooldown = 0f;

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
        //For all entities (excl. player) in the hit detection volume, apply damage
        Collider[] hitColliders = Physics.OverlapBox(hitDetectionVolume.bounds.center, hitDetectionVolume.bounds.extents, hitDetectionVolume.transform.rotation);
        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject != this.gameObject && collider.TryGetComponent<Health>(out Health entityHealth))
            {
                entityHealth.TakeDamage(attackDamage);
            }
        }
        currentCooldown = attackCooldown;
    }
}
