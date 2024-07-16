using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSprite : MonoBehaviour
{ 
    public Sprite unequipSprite;
    public Sprite []equipSprite;
    void Start()
    {
        UpdateCharacterSprite();
    }

    public void UpdateCharacterSprite()
    {
        if (BackendGameData.Instance != null)
        {
            if (BackendGameData.Instance.UserGameData.equipHead!="i001")
            {
                int num = int.Parse(BackendGameData.Instance.UserGameData.equipHead.Substring(1));
                GetComponent<Image>().sprite = equipSprite[num - 101];
            }
            else
                GetComponent<Image>().sprite = unequipSprite;
        }
    }

    public void EquipItemTemporary(string itemId)
    {

        int num = int.Parse(itemId.Substring(1));

        if (!BackendGameData.Instance.UserGameData.hasItem[itemId])
            GetComponent<Image>().sprite = equipSprite[num-101];
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