using UnityEngine;

[CreateAssetMenu(menuName = "Projectile")]
public class Projectile : ScriptableObject
{
    public float speed;
    public float despawnDelay;
    public float spinSpeed;
    public Sprite sprite;
}
