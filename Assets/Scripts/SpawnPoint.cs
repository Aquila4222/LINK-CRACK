using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public Transform player;

    private void Update()
    {
        if (player.position.x - transform.position.x is > -1 and < 1)
        {
            PlayerLife.SpawnPoint = transform.position;
        }
    }
}
