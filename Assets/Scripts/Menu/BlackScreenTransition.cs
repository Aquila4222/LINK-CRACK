using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BlackScreenTransition : MonoBehaviour
{
    #region Mono饿汉单例代码

    private static BlackScreenTransition _instance;
    public static BlackScreenTransition Instance => _instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitializeRuntime()
    {
        GameObject obj = new GameObject("BlackScreenTransition");
        _instance = obj.AddComponent<BlackScreenTransition>();
        DontDestroyOnLoad(obj);
    }
    
    #endregion
    
    public Image blackScreenImage;
    
    public GameObject blackScreenPrefab;
    
    public GameObject blackScreen;
    
    private bool isTransitioning = false;

    private void Awake()
    {
        blackScreenPrefab = Resources.Load<GameObject>("Prefabs/BlackTransition");
        blackScreen = Instantiate(blackScreenPrefab);
        blackScreenImage = blackScreen.GetComponentInChildren<Image>();
        blackScreen.SetActive(false);
        DontDestroyOnLoad(blackScreen);
    }


    /// <summary>
    /// 黑屏转场，传入时间和
    /// </summary>
    /// <param name="time"></param>
    /// <param name="callback"></param>
    public void Transition(float time , Action callback)
    {
        if (isTransitioning == false)
        {
            isTransitioning = true;
            StopAllCoroutines();
            StartCoroutine(TransitionCoroutine(time,callback));
        }
    }

    IEnumerator TransitionCoroutine(float time, Action callback)
    {
        blackScreen.gameObject.SetActive(true);
        for (float i = 0; i <= time; i += 0.02f)
        {
            blackScreenImage.color = new Color(0, 0, 0, i/time);
            yield return new WaitForSeconds(0.02f);
        }
        blackScreenImage.color = new Color(0, 0, 0, 1);
        
        yield return new WaitForSeconds(0.1f);
        
        callback();
        
        for (float i = time; i >=0; i-= 0.02f)
        {
            blackScreenImage.color = new Color(0, 0, 0, i/time);
            yield return new WaitForSeconds(0.02f);
        }
        blackScreen.gameObject.SetActive(false);
        
        isTransitioning = false;
    }
}
