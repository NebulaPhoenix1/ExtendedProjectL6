using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //Simple WASD Movement Script
    //Using Unity's new Input System

    private Rigidbody rb;
    private InputAction moveAction;
    private Vector2 moveInput;
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
        rb.linearVelocity = new Vector3(moveInput.x * moveSpeed, rb.linearVelocity.y, moveInput.y * moveSpeed);
        //Rotate the player to face mouse
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f))
        {
            Vector3 lookPoint = hitInfo.point;
            lookPoint.y = transform.position.y; //Keep the y position the same to avoid tilting
            transform.LookAt(lookPoint);
        }
    }
}
