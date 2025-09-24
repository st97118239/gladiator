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
    
    private int dmg;
    private float angle;
    private bool shouldReverse;

    private void FixedUpdate()
    {
        if (!isOn) return;

        if (!shouldReverse && transform.localScale.y < maxDistance)
        {
            transform.localScale += scaleExtend * Time.deltaTime;
            transform.localPosition += transform.up * ((moveTo.x / spawnSpeed) * Time.deltaTime);
        }
        else
        {
            if (!shouldReverse)
                shouldReverse = true;

            transform.localScale -= scaleExtend / disableSpeed * Time.deltaTime;
            transform.localPosition += transform.up * ((moveTo.x / disableSpeed) * Time.deltaTime);

            if (!(transform.localScale.y < distanceToDisable)) return;

            gameObject.SetActive(false);
            isOn = false;
        }
    }

    public void Load(int givenDamage, float givenAngle)
    {
        angle = givenAngle;
        transform.eulerAngles = new Vector3(0, 0, angle);
        transform.localScale = defaultScale;
        dmg = givenDamage;
        shouldReverse = false;
        isOn = true;
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit.gameObject.CompareTag("Player"))
            hit.gameObject.GetComponent<Player>().PlayerHit(dmg, true);
        else if (hit.gameObject.CompareTag("Enemy"))
            hit.gameObject.GetComponent<EnemyController>().Hit(dmg);
        else if (hit.gameObject.CompareTag("Wall"))
            shouldReverse = true;
    }
}
