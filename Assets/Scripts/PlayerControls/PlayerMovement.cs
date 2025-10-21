using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //Player will move towards world space coordinates of the mouse (so player walks relative to mouse)
    //Using Unity's new Input System

    private Rigidbody rb;
    private InputAction moveAction;
    private Vector2 moveInput;
    private Vector2 mouseScreenPosition;
    private Vector3 mouseWorldPosition;
    [SerializeField] float moveSpeed = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveAction = InputSystem.actions.FindAction("Move");
        //Check if moveAction and RB are found
        if(rb == null)
        {
            Debug.LogError("Rigidbody component not found on PlayerMovement script attached to " + gameObject.name);
        }
        if(moveAction == null)
        {
            Debug.LogError("Move action not found in Input System for PlayerMovement script attached to " + gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>(); //Get what the player is inputting
        //Get Mouse Position
        mouseScreenPosition = Mouse.current.position.ReadValue();
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, Camera.main.transform.position.y - transform.position.y));
        //When walking forward/backward, walk towards/away from mouse position
        Vector3 directionToMouse = (mouseWorldPosition - transform.position).normalized;
        Vector3 rightToMouse = Vector3.Cross(Vector3.up, directionToMouse);
        Vector3 moveDir = (directionToMouse * moveInput.y + rightToMouse * moveInput.x).normalized;
        rb.linearVelocity = new Vector3(moveDir.x * moveSpeed, rb.velocity.y, moveDir.z * moveSpeed);

        
    }
}
