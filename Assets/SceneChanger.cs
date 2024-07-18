using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    string targetStage;
    bool isLoading = false;

    private void Update()
    {
        if (Loading.sceneLoadedCount == 3)
        {
            Loading.CompleteLoad();
        }
        if (isLoading && !Loading.isLoading)
        {
            ChangeScene();
        }
    }

    public void waitScene(string target)
    {
        targetStage = target;
        Loading.isLoading = true;
        isLoading = true;
        //Invoke("changeScene", 6f);
    }
    void ChangeScene()
    {
        Utils.LoadScene(targetStage);
    }
}
