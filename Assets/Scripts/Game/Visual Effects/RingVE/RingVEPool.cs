using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingVEPool : ObjectPoolTemplate<RingVEPool>
{
    public new void Awake()
    {
        warmCount = 10;
        poolSize = 30;
        objectPrefab = Resources.Load<GameObject>("Prefabs/RingVE");
        
        base.Awake();
    }

    /// <summary>
    /// 播放环形特效
    /// </summary>
    /// <param name="pos">位置</param>
    /// <param name="duration">持续时间</param>
    /// <param name="color">颜色</param>
    /// <param name="scale">整体大小</param>
    /// <param name="endScale">结束大小</param>
    /// <param name="startAlpha"></param>
    /// <param name="parent"></param>
    public void Play(Vector3 pos, float duration, Color color , float scale , float endScale , int startAlpha = 1,Transform parent =  null)
    {
        RingVE effect = GetObject().GetComponent<RingVE>();
        if (effect)
        {
            effect.PlayEffect(pos, duration, color, scale, endScale, startAlpha,parent);
        }
    }
}
