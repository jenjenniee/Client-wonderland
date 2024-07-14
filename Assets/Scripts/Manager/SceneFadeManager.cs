using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneFadeManager : MonoBehaviour
{
    public CanvasGroup sceneFadeOut;
    public GameObject sceneFadeOutObj;

    public void FadeOut()
    {
        sceneFadeOutObj.SetActive(true);
        StartCoroutine(FadeUI(0.2f, 1f, sceneFadeOut));
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
        ui.interactable = true;
    }
}
