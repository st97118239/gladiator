using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Player player;
    public InputActionAsset inputActions;
    public Rigidbody2D rb2d;
    public bool canMove;

    public InputAction moveAction;

    [SerializeField] private Camera cam;
    [SerializeField] private Vector2 camClampMin = Vector2.one;
    [SerializeField] private Vector2 camClampMax = Vector2.one;
    [SerializeField] private float speed = 1;

    private float moveSpeed;
    private Vector3 moveAmount;

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }

    private void Update()
    {
        moveSpeed = player.movementSpeed * Time.deltaTime;
        moveAmount = moveAction.ReadValue<Vector2>();

        if (canMove)
            //rb2d.MovePosition(rb2d.position + Vector2.one * moveAmount * (speed * Time.deltaTime));
            rb2d.AddForce(moveAmount * (moveSpeed * speed), ForceMode2D.Force);
    }

    private void FixedUpdate()
    {
        

        Vector3 clampedPosition = transform.position;

        clampedPosition.x = Mathf.Clamp(clampedPosition.x, camClampMin.x, camClampMax.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, camClampMin.y, camClampMax.y);
        clampedPosition.z = -10;

        cam.transform.position = clampedPosition;
    }

    public void SpeedChange(float change)
    {
        speed += change;
    }
}
