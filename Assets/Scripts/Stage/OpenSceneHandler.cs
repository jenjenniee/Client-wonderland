using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSceneHandler : MonoBehaviour
{
    public CanvasGroup[] objectsToActive;

    public void OnAnimationEnd()
    {
        foreach (CanvasGroup obj in objectsToActive)
        {
            StartCoroutine(FadeInCanvasGroup(obj, 1f, 0.5f));
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (CanvasGroup obj in objectsToActive)
        {
            obj.alpha = 0f;
        }
    }

    private IEnumerator FadeInCanvasGroup(CanvasGroup canvasGroup, float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }


}
