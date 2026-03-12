using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleVEPool : ObjectPoolTemplate<ParticleVEPool>
{
    public new void Awake()
    {
        warmCount = 100;
        poolSize = 500;
        objectPrefab = Resources.Load<GameObject>("Prefabs/ParticleVE");
        
        base.Awake();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="duration"></param>
    /// <param name="speed"></param>
    /// <param name="direction"></param>
    /// <param name="startScale"></param>
    /// <param name="endScale"></param>
    /// <param name="color"></param>
    /// <param name="dofall"></param>
    public void Play(Vector3 pos, float duration, float speed, Vector2 direction, Vector3 startScale, Vector3 endScale,
        Color color,bool dofall = false)
    {
        ParticleVE effect = GetObject().GetComponent<ParticleVE>();
        if (effect)
        {
            effect.Play(pos, duration,speed ,direction, startScale, endScale, color,dofall);
        }
    }
}
