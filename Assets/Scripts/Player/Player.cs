using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int health;
    public float speed;
    public float movementSpeed;
    public bool isDead;

    // Update is called once per frame
    void Update()
    {
        speed = (movementSpeed * Time.deltaTime);
    }
    public void PlayerHit(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            isDead = true;
        }
    }
}
