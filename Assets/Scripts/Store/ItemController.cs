using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemController : MonoBehaviour
{
    public GameObject clickedUI;
    public CanvasGroup errorMessageUI;
    public TextMeshProUGUI errorText;
    public TextMeshProUGUI heartText;

    void Start()
    {
        heartText.text = $"{BackendGameData.Instance.UserGameData.heart}";
    }

    public void Buy()
    {
        if(BackendGameData.Instance.BuyItem(0, 100))
        {
            GameObject.Find("Item").GetComponent<LoadItems>().CheckSoldOut();
            heartText.text = BackendGameData.Instance.UserGameData.heart.ToString();
            clickedUI.SetActive(false);
        }
        else
        {
            StartCoroutine(ErrorMessage("하트가 모자랍니다!"));
        }
    }

    public void ClickItem()
    {
        clickedUI.SetActive(true);
    }

    public void Unequip()
    {
        clickedUI.SetActive(false);
    }

    private IEnumerator ErrorMessage(string message)
    {
        errorText.text = message;
        StartCoroutine(FadeInCanvasGroup(errorMessageUI, 1f, 0.3f));
        yield return new WaitForSecondsRealtime(2f);
        StartCoroutine(FadeInCanvasGroup(errorMessageUI, 0f, 0.3f));
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
