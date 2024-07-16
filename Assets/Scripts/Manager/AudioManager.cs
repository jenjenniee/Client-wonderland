using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [Header("--------- Audio -----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    [Header("--------- Audio Clip -----------")]
    public AudioClip background;
    public AudioClip click;
    public AudioClip attack;
    public AudioClip boom;
    public AudioClip hide;
    public AudioClip Bubble;
    public AudioClip Jungle;
    public AudioClip Sea;

    [SerializeField] private float transitionTime = 2f; // 전환 시간 (초)

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
        musicSource.clip = background;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void ChangeBackground(AudioClip newClip)
    {
        StartCoroutine(TransitionMusic(newClip));
    }

    private IEnumerator TransitionMusic(AudioClip newClip)
    {
        Debug.Log("nuna"); 
        // 현재 음악의 초기 볼륨 저장
        float startVolume = musicSource.volume;

        // 현재 음악 페이드 아웃
        for (float t = 0; t < transitionTime; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / transitionTime);
            yield return null;
        }

        // 새로운 음악으로 변경
        musicSource.clip = newClip;
        musicSource.Play();

        // 새로운 음악 페이드 인
        for (float t = 0; t < transitionTime; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0, startVolume, t / transitionTime);
            yield return null;
        }

        // 볼륨을 원래대로 설정
        musicSource.volume = startVolume;
        background = newClip;
    }
}