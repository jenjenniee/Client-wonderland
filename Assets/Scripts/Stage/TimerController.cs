using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    public bool onTimer = true;

    public GameObject timer;
    public float duration;
    public TextMeshProUGUI text;

    private float elapsedTime;
    private Vector3 initialScale;

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
        onTimer = true;
    }
}
