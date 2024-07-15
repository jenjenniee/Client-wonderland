using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSprite : MonoBehaviour
{
    public Sprite equipSprite;
    public Sprite unequipSprite;

    void Start()
    {
        UpdateCharacterSprite();
    }

    public void UpdateCharacterSprite()
    {
        if (BackendGameData.Instance != null)
        {
            if (BackendGameData.Instance.UserGameData.equipHead == "i101")
            {
                GetComponent<Image>().sprite = equipSprite;
            }
            else
            {
                GetComponent<Image>().sprite = unequipSprite;
            }
        }
    }

    public void EquipItemTemporary(string itemId)
    {
        if (!BackendGameData.Instance.UserGameData.hasItem[itemId])
            GetComponent<Image>().sprite = equipSprite;
    }
    public void SetItemEquipment(string itemId)
    {
        if (BackendGameData.Instance.UserGameData.equipHead != itemId)
        {
            BackendGameData.Instance.EquipItem(itemId);
        }
        else
        {
            BackendGameData.Instance.UnequipItem();
        }
        UpdateCharacterSprite();
    }
    public void UnequipItem()
    {
        GetComponent<Image>().sprite = unequipSprite;
    }

}
