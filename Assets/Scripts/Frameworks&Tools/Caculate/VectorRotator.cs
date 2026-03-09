using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorRotator
{
    /// <summary>
    /// 计算从向量 a 到向量 b 的旋转角度，并将该旋转应用于向量 v。
    /// </summary>
    public static Vector2 RotateLike(Vector2 a, Vector2 b, Vector2 v)
    {
        double angleA = Math.Atan2(a.y, a.x);
        double angleB = Math.Atan2(b.y, b.x);
        double theta = angleB - angleA;

        double cos = Math.Cos(theta);
        double sin = Math.Sin(theta);

        double vxRot = v.x * cos - v.y * sin;
        double vyRot = v.x * sin + v.y * cos;

        return new Vector2((float)vxRot, (float)vyRot);
    }
}
