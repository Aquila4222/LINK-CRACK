using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerChain : MonoBehaviour
{
   // --- 在这里进行参数设置 ---
   [Header("绳子设置")]
   public float maxRopeLength = 5f;   // 绳子的最大长度
   public float pullSpring = 100f;    // 弹簧刚度（力的大小）
   public float pullDamper = 10f;     // 阻尼（减少晃动）
    
   
   public CatchableMono testMono;
   
   public CatchableMono chainedMono;
   
   public float Force;

   public float ChainRange;
   
   private bool isChaining = false;
   
   private Camera mainCamera;

   void Awake()
   {
      InputController.Instance.RegisterLink(ChainInput);
      InputController.Instance.RegisterUnlink(UnChainInput);
      InputController.Instance.RegisterLinkSwitch(SwitchChainInput);
      
      mainCamera = Camera.main;
   }

   void FixedUpdate()
   {

      
      

      if (isChaining && chainedMono != null)
      {
         Vector2 mouseScreenPosition = Input.mousePosition;
         Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
         chainedMono.rb.AddForce(((Vector2)mouseWorldPosition - (Vector2)chainedMono.rb.transform.position)*Force);

         // if ((chainedRB.transform.position - transform.position).magnitude > ChainRange)
         // {
         //    Vector2 b1 = chainedRB.transform.position-transform.position;
         //    Vector2 b = b1.normalized;
         //    Vector2 a = chainedRB.velocity;
         //    
         //    Vector2 c = Vector2.Dot(a,b)*b;
         //    
         //    chainedRB.velocity -= 2*c;
         //    
         //    Vector3 pos = (chainedRB.transform.position-transform.position).normalized*(ChainRange*0.99f);
         //    chainedRB.MovePosition(transform.position + pos);
         // }
         
     
         Vector3 objToAnchor = transform.position- chainedMono.rb.transform.position;
         float currentDistance = objToAnchor.magnitude;

    
         if (currentDistance > maxRopeLength)
         {
   
            float distanceExcess = currentDistance - maxRopeLength; 
     
            Vector3 pullDirection = objToAnchor.normalized; 


            float forceMagnitude = distanceExcess * pullSpring;
   
            Vector3 pullForce = pullDirection * forceMagnitude;


            float velocityAlongRope = Vector3.Dot(chainedMono.rb.velocity, pullDirection);
            Vector3 dampingForce = pullDirection * (velocityAlongRope * pullDamper);

        
            chainedMono.rb.AddForce(pullForce - dampingForce);
         }
      }
       
   }
   
   
   private void UnChain()
   {
      if (isChaining == true && chainedMono != null)
      {
         isChaining = false;
         chainedMono.UnChain();
         chainedMono = null;
      }
   }

   private void TryChain()
   {
      //test
      isChaining = true;
      chainedMono = testMono;
      chainedMono.Chain();
   }
   
   private void UnChainInput()
   {
      UnChain();
   }

   private void ChainInput()
   {
      TryChain();
   }
   
   private void SwitchChainInput()
   {
      if (isChaining)
      {
         UnChain();
      }
      else
      {
         TryChain();
      }
   }
}
