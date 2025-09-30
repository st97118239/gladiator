using System;
using UnityEngine;

public class Root : MonoBehaviour
{
    public bool isOn;
    public Vector3 scaleExtend;
    public Vector3 moveTo;

    //[SerializeField] private 
    [SerializeField] private Vector3 defaultScale;
    [SerializeField] private float maxDistance;
    [SerializeField] private float distanceToDisable;
    [SerializeField] private float spawnSpeed;
    [SerializeField] private float disableSpeed;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite reverseSprite;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D bc2d;

    private int dmg;
    private float angle;
    private bool shouldReverse;

    private void FixedUpdate()
    {
        if (!isOn) return;

        if (!shouldReverse && spriteRenderer.size.y < maxDistance)
        {
            spriteRenderer.size += new Vector2(0, scaleExtend.y / spawnSpeed * Time.deltaTime);
            transform.localPosition += transform.up * ((moveTo.x / spawnSpeed) * Time.deltaTime);
            bc2d.size = new Vector2(0.5f, spriteRenderer.size.y);
        }
        else
        {
            if (!shouldReverse)
                StartReverse();

            spriteRenderer.size += new Vector2(0, -scaleExtend.y / disableSpeed * Time.deltaTime);
            transform.localPosition += transform.up * ((moveTo.x / disableSpeed) * Time.deltaTime);
            bc2d.size = new Vector2(0.5f, spriteRenderer.size.y);

            if (!(spriteRenderer.size.y < distanceToDisable)) return;

            gameObject.SetActive(false);
            isOn = false;
        }
    }

    private void StartReverse()
    {
        shouldReverse = true;
        spriteRenderer.sprite = reverseSprite;
        spriteRenderer.flipY = true;
    }

    public void Load(int givenDamage, float givenAngle)
    {
        spriteRenderer.sprite = defaultSprite;
        spriteRenderer.flipY = false;
        angle = givenAngle;
        transform.eulerAngles = new Vector3(0, 0, angle);
        //transform.localScale = defaultScale;
        dmg = givenDamage;
        shouldReverse = false;
        isOn = true;
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        CheckHit(hit.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D hit)
    {
        CheckHit(hit.gameObject);
    }

    private void CheckHit(GameObject obj)
    {
        if (obj.CompareTag("Player"))
            obj.GetComponent<Player>().PlayerHit(dmg, true);
        else if (obj.CompareTag("Enemy"))
            obj.GetComponent<EnemyController>().Hit(dmg);
        else if (obj.CompareTag("Wall"))
            StartReverse();
    }
}
