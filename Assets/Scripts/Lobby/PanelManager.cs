using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    public GameObject panel;
    private bool isPanel = false;

    // Start is called before the first frame update
    void Start()
    {
        panel.SetActive(isPanel);
    }

    public void TogglePanel()
    {
        Debug.Log($"isPanel : {isPanel}");
        isPanel = !isPanel;
        panel.SetActive(isPanel);
    }
}
