using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Player player;
    public Rigidbody2D rb2d;
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rb2d.AddForce(transform.up * player.speed, ForceMode2D.Force);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb2d.AddForce(transform.right * player.speed * -1, ForceMode2D.Force);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb2d.AddForce(transform.up * player.speed * -1, ForceMode2D.Force);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb2d.AddForce(transform.right * player.speed, ForceMode2D.Force);
        }
    }
}
