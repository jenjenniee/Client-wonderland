using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject polaroid;
    AudioManager audiomanager;
    public AudioClip clip;

    void Start()
    {
        audiomanager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    public void ClickPolaroid()
    {
        if (polaroid != null)
        {
            //Debug.Log($"polaroid On");
            polaroid.SetActive(true);
        }
        audiomanager.PlaySFX(clip);
    }
}
