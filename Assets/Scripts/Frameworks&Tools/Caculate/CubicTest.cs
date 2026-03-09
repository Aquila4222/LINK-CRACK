using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubicTest : MonoBehaviour
{
    public Transform p0;
    public Transform p1;
    public Transform p2;
    
    public Transform move;

    private float a3;
    private float a2;
    private float a1;
    private float a0;

    private Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
    }
    
    void FixedUpdate()
    {
        CubicFunction c = new CubicFunction(p0.position.x,p0.position.y , p1.position.x , p1.position.y, p2.position.x , p2.position.y );

        var std = c.GetStandardCoefficients();
        a3 = (float)std.a3;
        a2 = (float)std.a2;
        a1 = (float)std.a1;
        a0 = (float)std.a0;
        
        Vector2 mouseScreenPosition = Input.mousePosition;
        Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        
        move.position = new Vector3(mouseWorldPosition.x, f(mouseWorldPosition.x), move.position.z);
    }

    private float f(float x)
    {
        return a3*x*x*x + a2*x*x + a1*x + a0;
    }
}
