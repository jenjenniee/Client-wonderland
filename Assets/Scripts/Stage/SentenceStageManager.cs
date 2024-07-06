using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentenceStageManager : MonoBehaviour
{
    [SerializeField]
    private GameObject wordStage;
    [SerializeField]
    private CanvasGroup wordStageCanvas;
    [SerializeField]
    private Renderer blackBoxRenderer;

    [SerializeField]
    private GameObject polaroid;


    public void NextStage()
    {
        // 다음 스테이지로 넘어가면 wordStage는 비활성화.
        if (wordStage != null) 
        {
            StartCoroutine(FadeOutCanvasGroup(wordStageCanvas, blackBoxRenderer, 3f));
        }
    }

    private IEnumerator FadeOutCanvasGroup(CanvasGroup canvasGroup, Renderer blackBoxRenderer, float duration)
    {
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, time / duration);
            float canvasDuration = duration / 6f;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, time / canvasDuration);
            if (blackBoxRenderer != null)
            {
                foreach (Material mat in blackBoxRenderer.materials)
                {
                    Color color = mat.color;
                    color.a = alpha;
                    mat.color = color;
                }
            }
            yield return null;
        }

        canvasGroup.alpha = 0f;
        if (blackBoxRenderer != null)
        {
            foreach (Material mat in blackBoxRenderer.materials)
            {
                Color color = mat.color;
                color.a = 0f;
                mat.color = color;
            }
        }
        wordStage.SetActive(false);
        polaroid.SetActive(true);
    }

}
