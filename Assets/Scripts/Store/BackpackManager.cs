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

    public void OnEquip(int idx)
    {
        if (BackendGameData.Instance.UserGameData.equipHead == 0)
            BackendGameData.Instance.EquipItem(idx);
        else
            BackendGameData.Instance.UnequipItem();
        clickedUI.SetActive(false);
    }
}
