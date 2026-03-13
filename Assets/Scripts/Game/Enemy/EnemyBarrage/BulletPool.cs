using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : ObjectPoolTemplate<BulletPool>
{
    public new void Awake()
    {
        warmCount = 10;
        poolSize = 50;
        objectPrefab = Resources.Load<GameObject>("Prefabs/Bullet");
        
        base.Awake();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="velocity"></param>
    public void Shoot(Vector3 pos, Vector2 velocity)
    {
        Bullet bullet = GetObject().GetComponent<Bullet>();
        if (bullet)
        {
            bullet.Shoot(pos,velocity);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Vector2 mouseScreenPosition = Input.mousePosition;
            Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
            Shoot(new Vector3(0,10,0),(mouseWorldPosition-new Vector2(0, 10)).normalized*5f);
        }
    }
}
