using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThemeToggleColor : MonoBehaviour
{
    public void OnToggleChange(bool isOn)
    {
        if (isOn)
        {
            GetComponent<Image>().color = new Color(241 / 255f, 241 / 255f, 241 / 255f);
        }
        else
        {
            GetComponent<Image>().color = Color.white;
        }
    }
}
