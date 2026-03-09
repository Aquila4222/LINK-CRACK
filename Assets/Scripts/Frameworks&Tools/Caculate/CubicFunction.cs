using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubicFunction
{
    public double X0 { get; private set; }
    public double Y0 { get; private set; }
    public double A { get; private set; }
    public double B { get; private set; }

    /// <summary>
    /// 通过极值点 (x0, y0) 和另外两个点 (x1, y1), (x2, y2) 构造三次函数。
    /// </summary>
    public CubicFunction(double x0, double y0, double x1, double y1, double x2, double y2)
    {
        if (x0 < (x2 - x1) / 3 + x1)
        {
            x0 = (x2 - x1) / 3 + x1;
        }
        if (x0 > (x2 - x1)*2 / 3 + x1)
        {
            x0 = (x2 - x1)*2 / 3 + x1;
        }
        
        
        X0 = x0;
        Y0 = y0;

        double u1 = x1 - x0;
        double u2 = x2 - x0;
        double v1 = y1 - y0;
        double v2 = y2 - y0;

        // 检查输入有效性
        if (Math.Abs(u1) < 1e-12 || Math.Abs(u2) < 1e-12)
            throw new ArgumentException("极值点不能与给定点相同。");
        if (Math.Abs(u1 - u2) < 1e-12)
            throw new ArgumentException("两个给定点不能有相同的 x 坐标（相对于极值点）。");

        // 计算系数 a
        double aNumerator = v1 / (u1 * u1) - v2 / (u2 * u2);
        double aDenominator = u1 - u2;
        A = aNumerator / aDenominator;

        // 计算系数 b（使用第一个点）
        B = (v1 - A * u1 * u1 * u1) / (u1 * u1);
    }

    /// <summary>
    /// 计算函数在 x 处的值。
    /// </summary>
    public double Evaluate(double x)
    {
        double dx = x - X0;
        return A * dx * dx * dx + B * dx * dx + Y0;
    }

    /// <summary>
    /// 获取标准形式系数：f(x) = a3*x^3 + a2*x^2 + a1*x + a0。
    /// </summary>
    public (double a3, double a2, double a1, double a0) GetStandardCoefficients()
    {
        double a3 = A;
        double a2 = -3 * A * X0 + B;
        double a1 = 3 * A * X0 * X0 - 2 * B * X0;
        double a0 = -A * X0 * X0 * X0 + B * X0 * X0 + Y0;
        return (a3, a2, a1, a0);
    }
}
