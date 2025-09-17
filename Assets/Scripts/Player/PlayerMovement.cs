using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Player player;
    public Rigidbody2D rb2d;

    private float speed;

    private void Update()
    {
        speed = player.movementSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.W))
        {
            rb2d.AddForce(transform.up * speed, ForceMode2D.Force);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb2d.AddForce(transform.right * (speed * -1), ForceMode2D.Force);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb2d.AddForce(transform.up * (speed * -1), ForceMode2D.Force);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb2d.AddForce(transform.right * speed, ForceMode2D.Force);
        }
    }
}
