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
   
   private bool isChaining = false;
   
   private Camera mainCamera;

   private ChainVE chainVE;
   
   public Transform AimTransform;

   public float AimDistance;
   
   public Vector3 AimPos;
   
   public float AimMoveSpeed;
      
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
        
         chainedMono.rb.AddForce(((Vector2)AimPos - (Vector2)chainedMono.rb.transform.position)*Force);
         
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
         
         
         chainVE.SetChain(transform.position,chainedMono.rb.transform.position,AimPos);
         
         if ((chainedMono.transform.position - transform.position).magnitude > maxRopeLength * 1.5f)
         {
            UnChain();
         }
      }
      else
      {
         chainVE.DisableAll();
      }
   }

   void Update()
   {
      if (InputController.Instance.GetScheme() == 0)
      {
         AimTransform.gameObject.SetActive(true);
         Vector2 mouseScreenPosition = Input.mousePosition;
         Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
         if ((mouseWorldPosition - (Vector2)transform.position).magnitude < AimDistance)
         {
            AimTransform.position = mouseWorldPosition;
            AimPos = AimTransform.position;
         }
         else
         {
            AimTransform.position = (mouseWorldPosition - (Vector2)transform.position).normalized*AimDistance + (Vector2)transform.position;
            AimPos = AimTransform.position;
         }
      }
      else if (InputController.Instance.GetScheme() == 1)
      {
         if (InputController.Instance.AimInput != Vector2.zero)
         {
            AimTransform.gameObject.SetActive(true);
            Vector3 targetPos = InputController.Instance.AimInput.normalized * AimDistance;
            AimTransform.localPosition += (targetPos-AimTransform.localPosition).normalized * (Time.deltaTime * AimMoveSpeed);
            
         }
         else
         {
            AimTransform.gameObject.SetActive(false);
         }
     
         AimPos = AimTransform.position;
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
      Collider2D[] hits =  Physics2D.OverlapCircleAll(AimPos,1);
      isChaining = false;
      chainedMono = null;
      foreach (Collider2D hit in hits)
      {
         CatchableMono c =  hit.GetComponent<CatchableMono>();
         if (c)
         {
            isChaining = true;
            chainedMono = c;
            chainedMono.Chain();
            break;
         }
      }
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
