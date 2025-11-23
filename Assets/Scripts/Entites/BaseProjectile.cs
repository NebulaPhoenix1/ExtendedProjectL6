using UnityEngine;
using UnityEngine.Events;

public abstract class BaseProjectile : MonoBehaviour
{
    [Header("Basic Projectile Values")]
    [SerializeField] protected int dmgAmount;
    protected float projectileLifeTime = 10f; //For some reason if this is serialized but set in inspector it defaults to 0?
    protected Rigidbody rb;
    protected BoxCollider box;
    protected float awakeTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if(!rb)
        {
             rb = this.gameObject.AddComponent<Rigidbody>();
             Debug.LogWarning("Projectile: "  + gameObject.name + " has defaulted rigidbody. Behaviour may not be as expected!");     
        }
        box = GetComponent<BoxCollider>();
        if(!box)
        {
            box = this.gameObject.AddComponent<BoxCollider>();
            Debug.LogWarning("Projectile: "  + gameObject.name + " has defaulted box collider. Behaviour may not be as expected!");   
        }
    }

    //When the projectile hits any game object with a collider it should be destroyed
    //And if that same game object has a health component we should deal damage (this will be done in SimpleStraightProjectile.cs)

    // Update is called once per frame
    protected virtual void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Projectile hit: {collision.gameObject.name} (Tag: {collision.gameObject.tag})");
        Debug.Log("Collision Entered");
        Destroy(this.gameObject);
        Debug.Log("Destroyed Projectile");
    }

    void Update()
    {
        //Tick projectile life time, and ensure projectiles are destroyed after a certain amount of time
        if(awakeTime <= projectileLifeTime)
        {
            awakeTime += Time.deltaTime;
        }
        else
        {
            Debug.Log("Projectile despawned:" + awakeTime + ":" + projectileLifeTime);
            Destroy(this.gameObject);
        }
    }
}
