using UnityEngine;

public class PlayerLegs : MonoBehaviour
{
    public Player player;

    private void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit.gameObject.CompareTag("FallTrigger"))
            player.PlayerHit(10000, false);
    }
}
