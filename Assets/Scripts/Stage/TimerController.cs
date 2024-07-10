using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    public bool onTimer = true;     // 타이머 on/off

    public GameObject timer;        // "Gauge Front" : 타이머 바
    public float duration;          // 타이머 시간
    public TextMeshProUGUI text;    // 남은 시간 출력하는 텍스트

    public int integerTimer = 20;

    private float elapsedTime;      // 경과시간
    private Vector3 initialScale;   // 초기 타이머 바 가로 길이 저장용

    // Start is called before the first frame update
    void Start()
    {
        if (timer == null)
        {
            timer = this.gameObject;
        }
        elapsedTime = 0f;
        initialScale = timer.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (onTimer)
        {
            if (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float newWidth = Mathf.Lerp(initialScale.x, 0, elapsedTime / duration);
                timer.transform.localScale = new Vector3(newWidth, initialScale.y, initialScale.z);
                text.text = $"{Mathf.Ceil(duration - elapsedTime)}초";
                integerTimer = (int)Mathf.Ceil(duration - elapsedTime);
            }
        }
    }

    /// <summary>
    /// 새로운 문제 시간 재기
    /// </summary>
    public void NewProblemTimer(float duration)
    {
        this.duration = duration;
        elapsedTime = 0f;
        timer.transform.localScale = initialScale;
        text.text = $"{Mathf.Ceil(duration - elapsedTime)}초";
        onTimer = true;
    }
}
