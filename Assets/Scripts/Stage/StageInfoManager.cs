using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class StageInfoManager : MonoBehaviour
{
    public GameObject infoUI;           // Ŭ�� �� ������� ����â
    public string targetStage;          // �� �� �� Ŭ�� �� �Ѿ�� stage
    private CanvasGroup canvasGroup;    // alpha ������ ����
    private int clickCount = 0;
    public SceneChanger sceneChanger;
    public GameObject playSet;
    public GameObject mapSet;
    Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = infoUI.GetComponent<CanvasGroup>();
        infoUI.SetActive(false);
        mainCamera = Camera.main;
    }

    public void ShowInfo(string theme)
    {
        infoUI.SetActive(true);
        canvasGroup.alpha = 1;
        clickCount++;
        if (clickCount == 2) {
            playSet.SetActive(true);
            SceneTheme.theme = theme;
            sceneChanger.waitScene(targetStage);
            if (mainCamera != null)
                mainCamera.backgroundColor = Color.black;
            mapSet.SetActive(false);
            ProblemData.instance.OnLoadStageData(theme);
        }
    }
    
    public void CloseInfo()
    {
        canvasGroup.alpha = 0;
        clickCount = 0;
        infoUI.SetActive(false);
    }
}