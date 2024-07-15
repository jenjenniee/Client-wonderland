using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemController : MonoBehaviour
{
    public GameObject soldOut;
    public GameObject clickedUI;
    public CanvasGroup errorMessageUI;
    public TextMeshProUGUI errorText;
    public TextMeshProUGUI heartText;
    private bool isSoldout;
    public string itemId;

    void Start()
    {
        heartText.text = $"{BackendGameData.Instance.UserGameData.heart}";
        CheckSoldOut();
    }

    /// <summary>
    /// 상점 진입시, 이미 구매한 Item은 SoldOut 처리.
    /// </summary>
    public void CheckSoldOut()
    {
        Debug.Log($"hasItem: {BackendGameData.Instance.UserGameData.hasItem}");
        if (BackendGameData.Instance.UserGameData.hasItem[itemId])
        {
            soldOut.SetActive(true);
            isSoldout = true;
        }
    }

    public void Buy()
    {
        if(BackendGameData.Instance.BuyItem(itemId, 100))
        {
            CheckSoldOut();
            heartText.text = BackendGameData.Instance.UserGameData.heart.ToString();
            clickedUI.SetActive(false);
            isSoldout = true;
        }
        else
        {
            StartCoroutine(ErrorMessage("하트가 모자랍니다!"));
        }
    }

    public void ClickItem()
    {
        if (isSoldout) return;
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
