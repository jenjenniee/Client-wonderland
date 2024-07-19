using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScenario : MonoBehaviour
{

    [SerializeField]
    private UserInfo user;

    //  ε   Ϸ           UI
    public GameObject[] circles = new GameObject[3];
    public GameObject logoPanel;
    //  ε   Ϸ            Object
    public GameObject sceneGroup;
    public GameObject[] set = new GameObject[3];

    private void Update()
    {
        //  ε   Ϸ      
        if (!Loading.isLoading && !Loading.mapLoading)
        {
            foreach (GameObject circle in circles)
            {
                circle.SetActive(false);
            }
            StartCoroutine(FadeUI(0.5f, 0f, logoPanel.GetComponent<CanvasGroup>()));
        }
    }

    private void Awake()
    {
        user.GetUserInfoFromBackend();
    }


    private void Start()
    {
        BackendGameData.Instance.GameDataLoad();
        if (PlayerPrefs.GetInt("isLoading") != 1)
        {
            foreach (GameObject s in set)
            {
                s.SetActive(false);
            }
            logoPanel.SetActive(false);
            sceneGroup.SetActive(true);
        }
        else
        {
            //StartCoroutine(AfterLoading());
            PlayerPrefs.SetInt("isLoading", 0);
            Loading.mapLoading = true;
        }
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
    IEnumerator AfterFade()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        sceneGroup.SetActive(true);
    }
}
