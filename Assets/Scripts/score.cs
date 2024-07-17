using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Networking;

public class score : MonoBehaviour
{
    private string userId;
    private const string API_URL = "https://worderland.kro.kr/api/result/return?userId=";

    [Serializable]
    public class ThemeResult
    {
        public string theme;
        public float firstStageBestRecord;
        public float firstStageAverageRecord;
        public float secondStageBestRecord;
        public float secondStageAverageRecord;
        public float thirdStageBestRecord;
        public float thirdStageAverageRecord;
        public float totalBestRecord;
        public float totalAverageRecord;
        public float totalPlayRecord;
    }

    [Serializable]
    public class DailyResult
    {
        public string date;
        public List<ThemeResult> result;
    }

    [Serializable]
    public class ApiResponse
    {
        public int status;
        public bool success;
        public string message;
        public List<DailyResult> data;
    }

    private void Start()
    {
        userId = UserInfo.Data.gamerId;
    }

    public void GetUserGameReport()
    {
        StartCoroutine(FetchGameReport());
    }

    private IEnumerator FetchGameReport()
    {
        string url = API_URL + userId;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                ProcessGameReport(jsonResponse);
            }
        }
    }

    private void ProcessGameReport(string jsonResponse)
    {
        ApiResponse response = JsonUtility.FromJson<ApiResponse>(jsonResponse);

        if (response.status == 200 && response.success)
        {
            DisplayGameReport(response.data);
        }
        else
        {
            Debug.LogError("API 요청 실패: " + response.message);
        }
    }

    private void DisplayGameReport(List<DailyResult> dailyResults)
    {
        Debug.Log("======= Game Report =======");
        foreach (var dailyResult in dailyResults)
        {
            Debug.Log($"Date: {dailyResult.date}");

            foreach (var themeResult in dailyResult.result)
            {
                Debug.Log($"  Theme: {themeResult.theme}");
                Debug.Log($"    First Stage:");
                Debug.Log($"      Best Record: {themeResult.firstStageBestRecord}");
                Debug.Log($"      Average Record: {themeResult.firstStageAverageRecord}");
                Debug.Log($"    Second Stage:");
                Debug.Log($"      Best Record: {themeResult.secondStageBestRecord}");
                Debug.Log($"      Average Record: {themeResult.secondStageAverageRecord}");
                Debug.Log($"    Third Stage:");
                Debug.Log($"      Best Record: {themeResult.thirdStageBestRecord}");
                Debug.Log($"      Average Record: {themeResult.thirdStageAverageRecord}");
                Debug.Log($"    Total:");
                Debug.Log($"      Best Record: {themeResult.totalBestRecord}");
                Debug.Log($"      Average Record: {themeResult.totalAverageRecord}");
                Debug.Log($"      Play Record: {themeResult.totalPlayRecord}");
                Debug.Log("  -------------------------");
            }
        }
        Debug.Log("============================");

        // 여기에서 UI 업데이트나 다른 게임 로직을 구현할 수 있습니다.
    }
}
