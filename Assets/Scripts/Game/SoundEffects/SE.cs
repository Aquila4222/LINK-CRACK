using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SE : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    public void PlaySound(AudioClip clip, float volume = 1)
    {
        audioSource.PlayOneShot(clip, volume);
        StartCoroutine(Distory());
    }

    IEnumerator Distory()
    {
        yield return new WaitForSeconds(1f);
        SEPool.Instance.ReturnObject(gameObject);
    }
}
