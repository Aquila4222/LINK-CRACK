using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEPool : ObjectPoolTemplate<SEPool>
{
    private Dictionary<string, AudioClip> SEClips = new Dictionary<string, AudioClip>();

    public AudioClip[] audioClips;
    
    public new void Awake()
    {
        warmCount = 10;
        poolSize = 50;
        objectPrefab = Resources.Load<GameObject>("Prefabs/SE");
        
        base.Awake();
        
        audioClips = Resources.LoadAll<AudioClip>("SE");
        foreach (AudioClip clip in audioClips)
        {
            SEClips.Add(clip.name, clip);
        }
    }

    public void PlaySE(string clipName, float volume)
    {
        if (!SEClips.ContainsKey(clipName))
        {
            return;
        }
        
        SE effect = GetObject().GetComponent<SE>();
        if (effect)
        {
            effect.PlaySound(SEClips[clipName], volume);
        }
    }
}
