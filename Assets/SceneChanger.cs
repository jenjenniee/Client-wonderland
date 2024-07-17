using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    string targetStage;
    public void waitScene(string target)
    {
        targetStage = target;
        Invoke("changeScene", 6f);
    }
    void changeScene()
    {
        Utils.LoadScene(targetStage);
    }
}
