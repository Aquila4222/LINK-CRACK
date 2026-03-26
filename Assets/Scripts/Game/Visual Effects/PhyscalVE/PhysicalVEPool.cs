using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalVEPool : ObjectPoolTemplate<PhysicalVEPool>
{
    public new void Awake()
    {
        warmCount = 40;
        poolSize = 100;
        objectPrefab = Resources.Load<GameObject>("Prefabs/PhysicalVE");
        
        base.Awake();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="time"></param>
    /// <param name="vel"></param>
    /// <param name="scale"></param>
    /// <param name="color"></param>
    public void Play(Vector3 pos, float time, Vector2 vel, Vector3 scale, Color color)
    {
        PhysicalVE effect = GetObject().GetComponent<PhysicalVE>();
        if (effect)
        {
            effect.Play(pos, time, vel, scale, color);
        }
    }
}
