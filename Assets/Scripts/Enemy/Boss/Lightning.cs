using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    public bool isOn;
    public bool isFollowing;
    public int followDistance;
    public float hitDistance;
    public int dmg;

    private Transform playerTransform;
    private Player player;
    private List<Vector3> storedPositions = new();

    private IEnumerator FollowPlayer()
    {
        while (isFollowing)
        {
            storedPositions.Add(playerTransform.position);

            if (storedPositions.Count <= followDistance) yield return null;

            transform.position = storedPositions[0];
            storedPositions.RemoveAt(0);

            yield return null;
        }
    }

    private void Update()
    {
        while (isFollowing)
        {
            storedPositions.Add(playerTransform.position);

            if (storedPositions.Count <= followDistance) return;

            transform.position = storedPositions[0];
            storedPositions.RemoveAt(0);

            return;
        }
    }

    public void Load(Player givenTarget, int givenDmg)
    {
        dmg = givenDmg;
        player = givenTarget;
        playerTransform = player.transform;
        transform.position = playerTransform.position;
        gameObject.SetActive(true);
        isFollowing = true;
        //StartCoroutine(FollowPlayer());
    }

    public void StopFollowing()
    {
        isFollowing = false;

        if (Vector3.Distance(playerTransform.position, transform.position) <= hitDistance)
        {
            player.PlayerHit(dmg, true);
        }
    }
}
