using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Lightning : MonoBehaviour
{
    public bool isOn;
    public bool isFollowing;
    public int followDistance;
    public float hitDistance;
    public float strike1Delay;
    public float strike2Delay;
    public float strike3Delay;
    public float despawnDelay;
    public Vector2 distanceFromMain;
    public float spinSpeed;

    [SerializeField] private Animator animator1;
    [SerializeField] private Animator animator2;
    [SerializeField] private Animator animator3;
    [SerializeField] private SpriteRenderer indicatorRenderer1;
    [SerializeField] private SpriteRenderer indicatorRenderer2;
    [SerializeField] private SpriteRenderer indicatorRenderer3;
    [SerializeField] private Transform strike2Parent;
    [SerializeField] private Transform strike3Parent;
    [SerializeField] private Transform strike2;
    [SerializeField] private Transform strike3;
    [SerializeField] private Lightning strike2Lighting;
    [SerializeField] private Lightning strike3Lighting;

    private BossStateMachine boss;
    private Transform playerTransform;
    private Player player;
    private readonly List<Vector3> storedPositions = new();
    private int dmg;
    private float followDistanceFromMain2;
    private float followDistanceFromMain3;
    private float angle2;
    private float angle3;
    private int strikeIdx;
    private bool hasHitPlayer;
    private bool isFirstStrike;

    private static readonly int strikeAnim = Animator.StringToHash("Strike");

    private void FixedUpdate()
    {
        while (isFollowing)
        {
            storedPositions.Add(playerTransform.position);

            if (storedPositions.Count <= followDistance) return;

            transform.position = storedPositions[0];
            storedPositions.RemoveAt(0);

            strike2Parent.eulerAngles += Vector3.forward * spinSpeed;
            strike2.eulerAngles += Vector3.forward * -spinSpeed;

            strike3Parent.eulerAngles += Vector3.forward * spinSpeed;
            strike3.eulerAngles += Vector3.forward * -spinSpeed;

            return;
        }
    }

    public void LoadMain(Player givenTarget, int givenDmg, BossStateMachine givenBoss)
    {
        hasHitPlayer = false;
        boss = givenBoss;
        dmg = givenDmg;
        player = givenTarget;
        playerTransform = player.transform;
        transform.position = playerTransform.position;
        strikeIdx = 1;

        followDistanceFromMain2 = Random.Range(distanceFromMain.x, distanceFromMain.y);
        angle2 = Random.Range(0, 360);
        strike2Parent.eulerAngles += Vector3.forward * angle2;
        strike2.eulerAngles += Vector3.forward * -angle2;
        strike2.transform.position += transform.right * followDistanceFromMain2;
        strike2Lighting.LoadRing(player, dmg, hitDistance, boss, 2);

        followDistanceFromMain3 = Random.Range(distanceFromMain.x, distanceFromMain.y);
        angle3 = Random.Range(0, 360);
        strike3Parent.eulerAngles += Vector3.forward * angle3;
        strike3.eulerAngles += Vector3.forward * -angle3;
        strike3.transform.position += transform.right * followDistanceFromMain3;
        strike3Lighting.LoadRing(player, dmg, hitDistance, boss, 3);

        indicatorRenderer1.enabled = true;
        indicatorRenderer2.enabled = true;
        indicatorRenderer3.enabled = true;
        isFollowing = true;
        gameObject.SetActive(true);
    }

    public void LoadRing(Player givenTarget, int givenDmg, float givenHitDistance, BossStateMachine givenBoss, int givenIdx)
    {
        isFirstStrike = true;
        hasHitPlayer = false;
        player = givenTarget;
        playerTransform = player.transform;
        dmg = givenDmg;
        hitDistance = givenHitDistance;
        boss = givenBoss;
        strikeIdx = givenIdx;
    }

    public void StopFollowing()
    {
        isFollowing = false;

        StartCoroutine(StrikeAnim());
    }

    private IEnumerator StrikeAnim()
    {
        yield return new WaitForSeconds(strike1Delay);

        indicatorRenderer1.enabled = false;
        animator1.SetTrigger(strikeAnim);

        if (strike2Delay > 0)
            yield return new WaitForSeconds(strike2Delay);

        indicatorRenderer2.enabled = false;
        animator2.SetTrigger(strikeAnim);

        if (strike3Delay > 0)
            yield return new WaitForSeconds(strike3Delay);

        indicatorRenderer3.enabled = false;
        animator3.SetTrigger(strikeAnim);

        yield return new WaitForSeconds(despawnDelay);

        gameObject.SetActive(false);
    }

    public void Strike()
    {
        if (!hasHitPlayer)
        {
            if (Vector3.Distance(playerTransform.position, transform.position) <= hitDistance)
            {
                player.PlayerHit(dmg, true);
                hasHitPlayer = true;
            }
        }

        if (!isFirstStrike) return;
        boss.bossController.enemyManager.levelManager.uiManager.audioManager.PlayZeusThunderStrike();
        isFirstStrike = false;

        if (strikeIdx == 3)
            boss.ZeusEndAbility();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitDistance);
    }
}
