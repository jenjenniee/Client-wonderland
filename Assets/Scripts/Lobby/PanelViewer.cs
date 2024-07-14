using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelViewer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textHeart;

    private void Awake()
    {
        // BackendGameData.Instance.onGameDataLoadEvent.AddListener(UpdateGameData);
        UpdateGameData();
    }

    public void UpdateGameData()
    {
        textHeart.text = $"{BackendGameData.Instance.UserGameData.heart}";
    }
}
