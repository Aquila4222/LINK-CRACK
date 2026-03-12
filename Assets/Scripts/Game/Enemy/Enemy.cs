using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum EnemyStateType
{
    UnGenerated,
    WarmUp,
    Alive,
    Dead,
}

public class Enemy : MonoBehaviour
{
    protected float Health { get; set; }
}
