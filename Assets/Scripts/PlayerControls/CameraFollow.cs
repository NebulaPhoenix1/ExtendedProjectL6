using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] float cameraHeight = 22f;
    private Transform playerTransform;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if(playerTransform == null)
        {
            Debug.LogError("Player Transform not found by CameraFollow script.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Camera follows player position on X and Z axis, but keeps a constant height on Y axis
        if (playerTransform != null)
        {
            Vector3 newPosition = new Vector3(playerTransform.position.x, cameraHeight, playerTransform.position.z);
            transform.position = newPosition;
        }
    }
}
