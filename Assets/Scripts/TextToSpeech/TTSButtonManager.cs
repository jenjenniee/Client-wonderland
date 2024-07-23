using GoogleTextToSpeech.Scripts.Data;
using GoogleTextToSpeech.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TTSButtonManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private VoiceScriptableObject voice;
    [SerializeField] private TextToSpeech textToSpeech;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private TextMeshProUGUI inputField;
    AudioManager audiomanager;
    private Action<AudioClip> _audioClipReceived;
    private Action<BadRequestData> _errorReceived;

    // Start is called before the first frame update
    void Start()
    {
        audiomanager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    public void PressBtn()
    {
        _errorReceived += ErrorReceived;
        _audioClipReceived += AudioClipReceived;
        textToSpeech.GetSpeechAudioFromGoogle(inputField.text, voice, _audioClipReceived, _errorReceived);

    }

    private void ErrorReceived(BadRequestData badRequestData)
    {
        Debug.Log($"Error {badRequestData.error.code} : {badRequestData.error.message}");
    }

    private void AudioClipReceived(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
        //audiomanager.PlaySFX(clip);
    }
    
   
}
