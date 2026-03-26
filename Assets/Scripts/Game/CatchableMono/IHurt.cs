using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHurt 
{
    
    public void Hurt(Vector2 repulseForce,float damage = 1);
}
