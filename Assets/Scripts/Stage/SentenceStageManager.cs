using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField]
    private GameObject polariodFilm;
    [SerializeField] 
    private CanvasGroup uiGroup;
    [SerializeField]
    private GameObject background;


    [SerializeField]
    private TextMeshProUGUI sentence;
    

    
    public void NextStage()
    {
        // ���� ���������� �Ѿ�� wordStage�� ��Ȱ��ȭ.
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
        StartCoroutine(UIFadeIn(2f, 0.3f));
    }

    private IEnumerator UIFadeIn(float delay, float duration)
    {
        yield return new WaitForSecondsRealtime(delay);

        polariodFilm.SetActive(true);
        background.SetActive(true);

        yield return new WaitForSecondsRealtime(1f);

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            uiGroup.alpha = Mathf.Lerp(0f, 1f, time / duration);
            yield return null;
        }
        uiGroup.alpha = 1f;
        uiGroup.interactable = true;
    }
}
