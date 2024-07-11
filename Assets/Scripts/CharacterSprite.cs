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
            if (BackendGameData.Instance.UserGameData.equipHead == 1)
            {
                GetComponent<Image>().sprite = equipSprite;
            }
            else
            {
                GetComponent<Image>().sprite = unequipSprite;
            }
        }
    }

    public void EquipItemTemporary()
    {
        GetComponent<Image>().sprite = equipSprite;
    }
    public void ToggleItemEquipment()
    {
        if (BackendGameData.Instance.UserGameData.equipHead == 0)
        {
            BackendGameData.Instance.EquipItem(1);
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
