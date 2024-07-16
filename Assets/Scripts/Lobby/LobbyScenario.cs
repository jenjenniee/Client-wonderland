using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScenario : MonoBehaviour
{
    
    [SerializeField]
    private UserInfo user;

    // �ε� �Ϸ� �� ����� UI
    public GameObject[] circles = new GameObject[3];
    public GameObject logoPanel;
    // �ε� �Ϸ� �� ������ Object
    public GameObject sceneGroup;

    private void Update()
    {
        // �ε� �Ϸ� ����
        if (!Loading.isLoading)
        {
            // �ε� �Ϸ�Ǹ� UI ��������ϱ�
            StartCoroutine(AfterLoading());
        }
    }

    private void Awake()
    {
        user.GetUserInfoFromBackend();
    }


    private void Start()
    {
        BackendGameData.Instance.GameDataLoad();
        
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
        logoPanel.SetActive(false);

        StartCoroutine(AfterFade());
    }

    IEnumerator AfterLoading()
    {
        yield return new WaitForSecondsRealtime(2f);
        foreach (GameObject circle in circles)
        {
            circle.SetActive(false);
        }
        StartCoroutine(FadeUI(0.5f, 0f, logoPanel.GetComponent<CanvasGroup>()));
    }
    IEnumerator AfterFade()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        sceneGroup.SetActive(true);
    }
}
