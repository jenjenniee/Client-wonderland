using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SurveyResultManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultTitle;
    [SerializeField] private TextMeshProUGUI resultDetail;

    public void SetText(SurveyResponseData responseData)
    {
        Debug.Log($"responseData : {responseData.result}, {responseData.detailResult}");
        resultTitle.text = responseData.result;
        // 위험도에 따라 텍스트 색상 변경
        if (responseData.result.Equals("Moderate Risk")) { resultTitle.color = new Color(249f/255f, 132f/255f, 4f/255f, 1f); }
        else if (responseData.result.Equals("Significant Risk")) { resultTitle.color = Color.red; }
        else { resultTitle.color = Color.yellow; }

        resultDetail.text = responseData.detailResult;
    }
}
