using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Networking;


[System.Serializable]
public class StageData
{
    public string userId;
    public string theme;
    public int stage;
    public int correctAnswers;
}

[System.Serializable]
public class StageResponseData
{
    public int status;
    public bool success;
    public string message;
    public StageData[] data;
}

public class GetMapInformation : MonoBehaviour
{
    private string userId;
    private string url;
    private int[] correctness = new int[3];

    void Start()
    {
        string[] themes = { "carousel", "ferris_wheel", "roller_coaster" };
        userId = UserInfo.Data.gamerId;
        for (int i = 0; i < themes.Length; i++)
        {
            url = $"https://worderland.kro.kr/api/result/{userId}?theme={themes[i]}";
            StartCoroutine(GetRequest(url, i));
        }
    }


    IEnumerator GetRequest(string uri, int idx)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            // 요청 보내기
            yield return request.SendWebRequest();

            // 오류 처리
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                Star.SetStar(idx + 1, -1);
            }
            else
            {
                // 응답 받기
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Response: " + jsonResponse);

                // JSON 파싱
                StageResponseData responseData = JsonUtility.FromJson<StageResponseData>(jsonResponse);

                // 데이터 접근
                if (responseData.success)
                {
                    int correct = 0;
                    foreach(StageData stageData in responseData.data)
                    {
                        correct += stageData.correctAnswers;
                    }
                    Star.SetStar(idx + 1, correct);
                }
                else
                {
                    Star.SetStar(idx + 1, -1);
                    Debug.LogError("Request failed with message: " + responseData.message);
                }
            }
        }
    }
}
