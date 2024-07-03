using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public string sceneName;

    public void OnClickButton()
    {
        Utils.LoadScene(sceneName);
    }
}
