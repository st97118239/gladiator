using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Net : MonoBehaviour
{
    public bool isOn;
    public Projectile projectile;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D bc2d;

    private AbilityManager abilityManager;

    public void Load(Vector3 position, float angleZ, AbilityManager givenAbilityManager)
    {
        isOn = true;
        abilityManager = givenAbilityManager;
        projectile = abilityManager.netCollapsedProjectile;
        spriteRenderer.sprite = projectile.sprite;
        transform.position = position;
        transform.rotation.eulerAngles.Set(0, 0, angleZ);

        List<Collider2D> hitColliders = new();
        Physics2D.OverlapBox(position, transform.localScale / 2, transform.rotation.z, abilityManager.player.filter, hitColliders);

        int i = 0;
        while (i < hitColliders.Count)
        {
            if (hitColliders[i].CompareTag("Enemy"))
                hitColliders[i].GetComponent<EnemyController>().Stun(projectile.stunDuration);
            else if (hitColliders[i].CompareTag("Boss"))
                hitColliders[i].GetComponent<BossController>().Stun(projectile.stunDuration / 2);
            i++;
        }

        gameObject.SetActive(true);

        StartCoroutine(Despawn());
    }

    private IEnumerator Despawn()
    {
        yield return new WaitForSeconds(projectile.despawnDelay);

        isOn = false;
        gameObject.SetActive(false);
    }
}
