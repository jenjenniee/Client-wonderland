using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageInfoManager : MonoBehaviour
{
    public GameObject infoUI;           // Ŭ�� �� ������� ����â
    public string targetStage;          // �� �� �� Ŭ�� �� �Ѿ�� stage
    private CanvasGroup canvasGroup;    // alpha ������ ����
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
        }
    }
    void changeScene(string theme)
    {
        // �� �� �� Ŭ���ϸ� �������� ����.
        SceneTheme.theme = theme;
        Utils.LoadScene(targetStage);
    }
    public void CloseInfo()
    {
        canvasGroup.alpha = 0;
        clickCount = 0;
        infoUI.SetActive(false);
    }
}
