using UnityEngine;

public class Player : MonoBehaviour
{
    public int health;
    public float speed;
    public float movementSpeed;
    public bool isDead;

    private void Update()
    {
        speed = movementSpeed * Time.deltaTime;
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
