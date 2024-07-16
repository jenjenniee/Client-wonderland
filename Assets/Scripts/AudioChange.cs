using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioChange : MonoBehaviour
{

    AudioManager audiomanager;
    public AudioClip clip;
    // Start is called before the first frame update
    void Start()
    {
        audiomanager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        audiomanager.ChangeBackground(clip);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
