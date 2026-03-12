using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainVE : MonoBehaviour
{
    public float ringInterval;
    public float PullScale;
    public float MaxPull;
    
    public GameObject ring;
    public GameObject Square;
    
    private GameObject[] chainsRing = new GameObject[20];
    private GameObject[] chainsSquare = new GameObject[19];
    
    

    void Start()
    {
        for (int i = 0; i < chainsRing.Length; i++)
        {
            chainsRing[i] = GameObject.Instantiate(ring);
            chainsRing[i].transform.parent = transform;
            chainsRing[i].transform.localPosition = Vector3.zero;
            chainsRing[i].name = "Ring " + i;
        }
        for (int i = 0; i < chainsSquare.Length; i++)
        {
            chainsSquare[i] = GameObject.Instantiate(Square);
            chainsSquare[i].transform.parent = transform;
            chainsSquare[i].transform.localPosition = Vector3.zero;
            chainsSquare[i].name = "Square " + i;
        }
        
        ring.SetActive(false);
        Square.SetActive(false);
    }

    public void SetChain(Vector2 objectPos, Vector2 playerPos , Vector2 pullPos)
    {
        EnableAll();
        
        Vector2 rotate =  objectPos - playerPos;

        pullPos -= playerPos;
        Vector2 p0 = VectorRotator.RotateLike(rotate, Vector2.right,pullPos);
        p0 = new Vector2((objectPos-playerPos).magnitude-p0.x , -p0.y);
        p0.y *= PullScale;
        
        if (p0.y < -MaxPull) p0.y = -MaxPull;
        if (p0.y > MaxPull) p0.y = MaxPull;
        Vector2 p1 = Vector2.zero;
        Vector2 p2 = Vector2.right*((objectPos-playerPos).magnitude);
        
        CubicFunction c = new CubicFunction(p0.x, p0.y, p1.x, p1.y, p2.x, p2.y);

        //计算旋转前位置
        float x = p2.x;
        for (int i = 0; i < chainsRing.Length; i++)
        {
            if (i == 0)
            {
                chainsRing[i].transform.localPosition = p2;
                continue;
            }

            if (x < 0)
            {
                chainsRing[i].transform.localPosition = p1;
                continue;
            }

            float dis = 0f;
            float x_old = x;
            do
            {
                x -= 0.01f;
                dis = (new Vector2(x,(float)c.Evaluate(x)) - new Vector2(x_old,(float)c.Evaluate(x_old))).magnitude;
            } while (dis < ringInterval && x >= 0);

            if (x < 0)
            {
                chainsRing[i].transform.localPosition = p1;
            }
            else
            {
                chainsRing[i].transform.localPosition = new Vector2(x,(float)c.Evaluate(x));
            }
        }
        
        //计算旋转
        for (int i = 0; i < chainsRing.Length; i++)
        {
            chainsRing[i].transform.localPosition = VectorRotator.RotateLike(Vector2.right, rotate, chainsRing[i].transform.localPosition);
            
            chainsRing[i].transform.localPosition = VectorRotator.RotateLike(Vector2.right, Vector2.left, chainsRing[i].transform.localPosition);
            
        }
        
        for (int i = 0; i < chainsRing.Length-1; i++)
        {
            chainsSquare[i].transform.position = (chainsRing[i].transform.position+chainsRing[i+1].transform.position)/2f;
            chainsSquare[i].transform.right = chainsRing[i].transform.position-chainsRing[i+1].transform.position;
        }
        
    }

    public void EnableAll()
    {
        foreach (var chainRing in chainsRing)
        {
            chainRing.SetActive(true);
        }

        foreach (var chainSquare in chainsSquare)
        {
            chainSquare.SetActive(true);
        }
    }
    
    public void DisableAll()
    {
        foreach (var chainRing in chainsRing)
        {
            chainRing.SetActive(false);
        }
        foreach (var chainSquare in chainsSquare)
        {
            chainSquare.SetActive(false);
        }
    }
    
}
