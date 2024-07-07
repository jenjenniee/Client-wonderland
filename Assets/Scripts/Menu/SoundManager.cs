using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    private void Awake()
    { 
        if(instance == null) 
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    public void SFXPlay(string sfxName,AudioClip clip) 
    {
        GameObject go = new GameObject(sfxName + "Sound");
        AudioSource audiosiurce = go.AddComponent<AudioSource>();
        audiosiurce.clip = clip;
        audiosiurce.Play();

        Destroy(go,clip.length);
    }
}
