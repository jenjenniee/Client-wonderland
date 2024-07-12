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

    public GameObject item;

    void Start()
    {
        UpdateState();
        if (!BackendGameData.Instance.UserGameData.hasItem)
        {
            item.SetActive(false);
        }
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
