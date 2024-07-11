using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageInfoManager : MonoBehaviour
{
    public GameObject infoUI;           // 클릭 시 띄워지는 정보창
    public string targetStage;          // 한 번 더 클릭 시 넘어가는 stage
    private CanvasGroup canvasGroup;    // alpha 조정을 위함
    private int clickCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = infoUI.GetComponent<CanvasGroup>();
        infoUI.SetActive(false);
    }

    public void ShowInfo(string theme)
    {
        infoUI.SetActive(true);
        canvasGroup.alpha = 1;
        clickCount++;
        if (clickCount == 2) {
            // 한 번 더 클릭하면 스테이지 진입.
            SceneTheme.theme = theme;
            Utils.LoadScene(targetStage);
        }
    }

    public void CloseInfo()
    {
        canvasGroup.alpha = 0;
        clickCount = 0;
        infoUI.SetActive(false);
    }
}
