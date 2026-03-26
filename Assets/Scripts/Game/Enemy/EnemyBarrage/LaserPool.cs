using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPool : ObjectPoolTemplate<LaserPool>
{
    public new void Awake()
    {
        warmCount = 5;
        poolSize = 50;
        objectPrefab = Resources.Load<GameObject>("Prefabs/Laser");
        
        base.Awake();
    }

    public void Shoot(Vector3 pos, Vector2 direction)
    {
        Laser laser = GetObject().GetComponent<Laser>();
        if (laser)
        {
            laser.Shoot(pos, direction);
        }
    }
}
