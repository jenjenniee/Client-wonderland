using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class LoadItems : MonoBehaviour
{
    // 데모 버전에서는 아이템이 하나라 추후 아이템 추가시 수정 필요.
    //public GameObject[] items;
    public GameObject soldOut;

    void Start()
    {
        CheckSoldOut();
    }

    public void CheckSoldOut()
    {
        Debug.Log($"hasItem: {BackendGameData.Instance.UserGameData.hasItem}");
        if (BackendGameData.Instance.UserGameData.hasItem)
        {
            soldOut.SetActive(true);
        }
    }
}
