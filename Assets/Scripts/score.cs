using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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

    [SerializeField] private Graph graphManager;
    [SerializeField] private RectTransform graphPanel;
    [SerializeField] private ThemeInfoDisplay themeInfoDisplay;

    [SerializeField] private ToggleGroup themeToggleGroup;

    private void PrepareAndDrawGraphs(List<DailyResult> dailyResults)
    {
        string[] toggleName = { "GraphTab", "GraphTabCarousel", "GraphTabFerrisWheel", "GraphTabRollerCoaster" };
        int theme = 0;  // 0: All, 1: Carousel, 2: Ferris-wheel, 3: Roller coaster
        if (themeToggleGroup.ActiveToggles().Any())
        {
            for (int i = 0; i < 4; i++)
            {
                if (themeToggleGroup.ActiveToggles().FirstOrDefault().name.Equals(toggleName[i]))
                {
                    theme = i;
                }
            }
        }

        Debug.Log($"Toggle Theme: {toggleName[theme]}");
        if (theme == 0)
        {
            List<float> dailyBestList = new List<float>();
            List<float> dailyAverageList = new List<float>();

            foreach (var dailyResult in dailyResults)
            {
                float dailyBest = float.MinValue;
                float dailySum = 0;

                foreach (var themeResult in dailyResult.result)
                {
                    dailyBest = Mathf.Max(dailyBest, themeResult.totalBestRecord);
                    dailySum += themeResult.totalAverageRecord;
                }

                dailyBestList.Add(dailyBest);
                dailyAverageList.Add(dailySum / dailyResult.result.Count);
            }

            graphManager.DrawGraphs(dailyAverageList, dailyBestList, graphPanel);
        }
        else
        {
            List<float> themeBestList = new List<float>();
            List<float> themeAverageList = new List<float>();
            foreach (var dailyResult in dailyResults)
            {
                themeBestList.Add(dailyResult.result[theme - 1].totalBestRecord);
                themeAverageList.Add(dailyResult.result[theme - 1].totalAverageRecord);
            }
            graphManager.DrawGraphs(themeAverageList, themeBestList, graphPanel);
        }
        //themeInfoDisplay.DisplayThemeInfo(dailyResults[dailyResults.Count - 1].result);  // 가장 최근 날짜의 테마 정보 표시
        /*  데이터 가공  */
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
        /*
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
        */
        PrepareAndDrawGraphs(dailyResults);
    }
}
