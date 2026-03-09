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

   private ChainVE chainVE;

   void Awake()
   {
      InputController.Instance.RegisterLink(ChainInput);
      InputController.Instance.RegisterUnlink(UnChainInput);
      InputController.Instance.RegisterLinkSwitch(SwitchChainInput);
      
      mainCamera = Camera.main;
      
      chainVE =  GetComponentInChildren<ChainVE>();
   }

   void FixedUpdate()
   {
      if (isChaining && chainedMono != null)
      {
         Vector2 mouseScreenPosition = Input.mousePosition;
         Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
         chainedMono.rb.AddForce(((Vector2)mouseWorldPosition - (Vector2)chainedMono.rb.transform.position)*Force);
         
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
         
         chainVE.SetChain(transform.position,chainedMono.rb.transform.position,mouseWorldPosition);
      }
      else
      {
         chainVE.DisableAll();
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
