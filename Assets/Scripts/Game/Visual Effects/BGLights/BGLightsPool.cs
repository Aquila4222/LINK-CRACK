using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGLightsPool : ObjectPoolTemplate<BGLightsPool>
{
    public new void Awake()
    {
        warmCount = 10;
        poolSize = 30;
        objectPrefab = Resources.Load<GameObject>("Prefabs/BGLight");
        
        base.Awake();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="duration"></param>
    /// <param name="velocity"></param>
    /// <param name="color"></param>
    public void Play(Vector3 pos ,float duration, Vector2 velocity, Color color,Transform parent = null)
    {
        BGLight effect = GetObject().GetComponent<BGLight>();
        if (effect)
        {
            effect.Play(pos,duration, velocity, color,parent);
        }
    }
}
