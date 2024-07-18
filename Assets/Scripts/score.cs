using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class score : MonoBehaviour
{
    private string userId;
    private const string API_URL = "https://worderland.kro.kr/api/result/return?userId=";
    public RectTransform panelTransform1; // 첫 번째 패널의 RectTransform
    public RectTransform panelTransform2; // 두 번째 패널의 RectTransform
    public LineRenderer totalAverageLineRenderer; // Total Average 값을 그릴 LineRenderer
    public LineRenderer totalBestLineRenderer; // Total Best 값을 그릴 LineRenderer
    public Canvas canvas; // UI 캔버스

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
        InitializeLineRenderers();
        GetUserGameReport();
    }

    private void InitializeLineRenderers()
    {
        // LineRenderer 초기 설정
        totalAverageLineRenderer.positionCount = 0;
        totalAverageLineRenderer.startWidth = 0.1f;
        totalAverageLineRenderer.endWidth = 0.1f;
        totalAverageLineRenderer.useWorldSpace = false; // 패널의 자식으로 설정되어 있을 경우
        totalAverageLineRenderer.sortingLayerName = "UI"; // UI 레이어로 설정
        totalAverageLineRenderer.sortingOrder = 10; // 다른 UI 요소들 위에 그려지도록 설정

        totalBestLineRenderer.positionCount = 0;
        totalBestLineRenderer.startWidth = 0.1f;
        totalBestLineRenderer.endWidth = 0.1f;
        totalBestLineRenderer.useWorldSpace = false; // 패널의 자식으로 설정되어 있을 경우
        totalBestLineRenderer.sortingLayerName = "UI"; // UI 레이어로 설정
        totalBestLineRenderer.sortingOrder = 10; // 다른 UI 요소들 위에 그려지도록 설정
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

        UpdateGraph(dailyResults);
    }

    private void UpdateGraph(List<DailyResult> dailyResults)
    {
        List<Vector3> totalAveragePoints = new List<Vector3>();
        List<Vector3> totalBestPoints = new List<Vector3>();

        float panelWidth1 = panelTransform1.rect.width;
        float panelHeight1 = panelTransform1.rect.height;
        float panelWidth2 = panelTransform2.rect.width;
        float panelHeight2 = panelTransform2.rect.height;
        float xInterval1 = panelWidth1 / (dailyResults.Count - 1);
        float xInterval2 = panelWidth2 / (dailyResults.Count - 1);
        float maxValue = GetMaxValue(dailyResults);

        Debug.Log($"Panel1 Width: {panelWidth1}, Height: {panelHeight1}");
        Debug.Log($"Panel2 Width: {panelWidth2}, Height: {panelHeight2}");
        Debug.Log($"Max Value: {maxValue}");

        for (int i = 0; i < dailyResults.Count; i++)
        {
            var dailyResult = dailyResults[i];
            float totalAverage = 0f;
            float totalBest = 0f;

            foreach (var themeResult in dailyResult.result)
            {
                totalAverage += themeResult.totalAverageRecord;
                totalBest += themeResult.totalBestRecord;
            }

            float normalizedTotalAverage = (totalAverage / maxValue) * panelHeight1;
            float normalizedTotalBest = (totalBest / maxValue) * panelHeight2;

            Vector2 localPoint1 = new Vector2(i * xInterval1, normalizedTotalAverage);
            Vector2 localPoint2 = new Vector2(i * xInterval2, normalizedTotalBest);

            totalAveragePoints.Add(panelTransform1.TransformPoint(localPoint1));
            totalBestPoints.Add(panelTransform2.TransformPoint(localPoint2));

            Debug.Log($"Local Point1: {localPoint1}, Transformed Point1: {panelTransform1.TransformPoint(localPoint1)}");
            Debug.Log($"Local Point2: {localPoint2}, Transformed Point2: {panelTransform2.TransformPoint(localPoint2)}");
        }

        totalAverageLineRenderer.positionCount = totalAveragePoints.Count;
        totalBestLineRenderer.positionCount = totalBestPoints.Count;

        totalAverageLineRenderer.SetPositions(totalAveragePoints.ToArray());
        totalBestLineRenderer.SetPositions(totalBestPoints.ToArray());
    }

    private float GetMaxValue(List<DailyResult> dailyResults)
    {
        float maxValue = 0f;

        foreach (var dailyResult in dailyResults)
        {
            foreach (var themeResult in dailyResult.result)
            {
                if (themeResult.totalAverageRecord > maxValue)
                    maxValue = themeResult.totalAverageRecord;
                if (themeResult.totalBestRecord > maxValue)
                    maxValue = themeResult.totalBestRecord;
            }
        }

        return maxValue;
    }
}
