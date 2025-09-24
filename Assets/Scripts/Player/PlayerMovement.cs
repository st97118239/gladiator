using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Player player;
    public InputActionAsset inputActions;
    public Rigidbody2D rb2d;
    public bool canMove;

    public InputAction moveAction;

    [SerializeField] private float speed = 5;

    private float speedMultiplier = 1f;
    private Vector2 moveAmount; 

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }

    private void Update()
    {
        moveAmount = moveAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (canMove)
            rb2d.MovePosition(rb2d.position + Vector2.one * moveAmount * (speed * Time.deltaTime));
    }

    public void SpeedChange(float change)
    {
        speedMultiplier += change;
        speed *= speedMultiplier;
    }
}
