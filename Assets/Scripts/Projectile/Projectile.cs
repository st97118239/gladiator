using UnityEngine;

[CreateAssetMenu(menuName = "Projectile")]
public class Projectile : ScriptableObject
{
    public bool playerProj;
    public int damage;
    public float speed;
    public float despawnDelay;
    public Sprite sprite;
}
