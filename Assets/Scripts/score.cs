using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class score : MonoBehaviour
{
    private string userId;
    private const string API_URL = "https://worderland.kro.kr/api/result/return?userId=";
    public GraphDrawer graphDrawer; // Inspector에서 할당
    public RectTransform bestGraphPanel; // totalBestRecord 그래프를 그릴 패널
    public RectTransform averageGraphPanel; // totalAverageRecord 그래프를 그릴 패널

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
                Debug.Log("Raw API Response: " + jsonResponse);
                ProcessGameReport(jsonResponse);
            }
        }
    }

    private void ProcessGameReport(string jsonResponse)
    {
        ApiResponse response = JsonUtility.FromJson<ApiResponse>(jsonResponse);
        if (response.status == 200 && response.success)
        {
            DrawGraphs(response.data);
        }
        else
        {
            Debug.LogError("API 요청 실패: " + response.message);
        }
    }

    private void DrawGraphs(List<DailyResult> dailyResults)
    {
        Debug.Log("DrawGraphs called");
        Dictionary<string, List<Vector2>> themeBestData = new Dictionary<string, List<Vector2>>();
        Dictionary<string, List<Vector2>> themeAverageData = new Dictionary<string, List<Vector2>>();

        // Initialize with a starting point (0, 0)
        foreach (var dailyResult in dailyResults)
        {
            double dayIndex = System.DateTime.Parse(dailyResult.date).ToOADate();
            foreach (var themeResult in dailyResult.result)
            {
                if (!themeBestData.ContainsKey(themeResult.theme))
                {
                    themeBestData[themeResult.theme] = new List<Vector2>();
                    themeAverageData[themeResult.theme] = new List<Vector2>();
                    // Add the starting point (0, 0)
                    themeBestData[themeResult.theme].Add(new Vector2(0, 0));
                    themeAverageData[themeResult.theme].Add(new Vector2(0, 0));
                }
                themeBestData[themeResult.theme].Add(new Vector2((float)dayIndex, themeResult.totalBestRecord));
                themeAverageData[themeResult.theme].Add(new Vector2((float)dayIndex, themeResult.totalAverageRecord));
                Debug.Log($"Added point for theme {themeResult.theme} (Best): {dayIndex}, {themeResult.totalBestRecord}");
                Debug.Log($"Added point for theme {themeResult.theme} (Average): {dayIndex}, {themeResult.totalAverageRecord}");
            }
        }

        Debug.Log($"Total themes to draw (Best): {themeBestData.Count}");
        Debug.Log($"Total themes to draw (Average): {themeAverageData.Count}");
        graphDrawer.DrawGraphs(themeBestData, themeAverageData, bestGraphPanel, averageGraphPanel);
    }

    private void DisplayThemeResult(string date, ThemeResult themeResult)
    {
        Debug.Log($"Date: {date}, Theme: {themeResult.theme}");
        Debug.Log($"  First Stage - Best: {themeResult.firstStageBestRecord}, Avg: {themeResult.firstStageAverageRecord}");
        Debug.Log($"  Second Stage - Best: {themeResult.secondStageBestRecord}, Avg: {themeResult.secondStageAverageRecord}");
        Debug.Log($"  Third Stage - Best: {themeResult.thirdStageBestRecord}, Avg: {themeResult.thirdStageAverageRecord}");
        Debug.Log($"  Total - Best: {themeResult.totalBestRecord}, Avg: {themeResult.totalAverageRecord}, Plays: {themeResult.totalPlayRecord}");
        Debug.Log("  -------------------------");
    }
}
