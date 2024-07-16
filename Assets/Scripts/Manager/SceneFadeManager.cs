using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneFadeManager : MonoBehaviour
{
    public CanvasGroup sceneFadeOut;
    public GameObject sceneFadeOutObj;

    private void Start()
    {
        //sceneFadeOutObj.SetActive(true);
        StartCoroutine(FadeUI(2f, 0f, sceneFadeOut));
    }

    IEnumerator FadeUI(float duration, float targetAlpha, CanvasGroup ui)
    {
        float time = 0f;
        float currentAlpha = ui.alpha;
        while (time < duration)
        {
            time += Time.deltaTime;
            ui.alpha = Mathf.Lerp(currentAlpha, targetAlpha, time / duration);
            yield return null;
        }
        ui.alpha = targetAlpha;
        ui.interactable = false;
        sceneFadeOutObj.SetActive(false);
    }
}
