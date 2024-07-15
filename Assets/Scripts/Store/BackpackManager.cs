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
    public string itemId;

    public GameObject item;

    void Start()
    {
        UpdateState();
        if (!BackendGameData.Instance.UserGameData.hasItem[itemId])
        {
            item.SetActive(false);
        }
    }

    public void UpdateState()
    {
        if (BackendGameData.Instance.UserGameData.equipHead == itemId)
        {
            equipText.text = "equipped";
            equipButtonText.text = "UNEQUIP";
        }
        else
        {
            equipText.text = "";
            equipButtonText.text = "EQUIP";
        }
    }

}
