using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BackpackManager : MonoBehaviour
{
    public TextMeshProUGUI equipText;
    public TextMeshProUGUI equipButtonText;
    public GameObject clickedUI;

    void Start()
    {
        UpdateState();
    }

    public void UpdateState()
    {
        if (BackendGameData.Instance.UserGameData.equipHead == 0)
        {
            equipText.text = "";
            equipButtonText.text = "EQUIP";
        }
        else
        {
            equipText.text = "equipped";
            equipButtonText.text = "UNEQUIP";
        }
    }
}
