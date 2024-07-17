using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SurveyToggleManager : MonoBehaviour
{
    public ToggleGroup[] surveyToggleGroup = new ToggleGroup[6];
    public GameObject surveyUI;
    private int[] scores = new int[6];
    public void PressSubmitButton()
    {
        string[] toggleName = { "Never", "Rarely", "Sometimes", "Frequently", "Always" };
        int totalScore = 0;
        foreach (ToggleGroup toggleGroup in  surveyToggleGroup)
        {
            if (toggleGroup.ActiveToggles().Any() )
            {
                for (int i=0;i<5;i++)
                {
                    if (toggleGroup.ActiveToggles().FirstOrDefault().name.Equals(toggleName[i]))
                    {
                        totalScore += i;
                    }
                }
            }
        }
        StartCoroutine(PostRequest(totalScore));
        surveyUI.SetActive(false);
    }

    IEnumerator PostRequest(int totalScore)
    {
        SurveyData data = new SurveyData
        {
            userId = UserInfo.Data.gamerId,
            scoreResult = totalScore,
        };
        string uri = "https://worderland.kro.kr/api/survey/add";
        string json = JsonUtility.ToJson(data);
        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(uri, json))
        {
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Send the request
            yield return request.SendWebRequest();

            // Error handling
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {request.error}\nResponse Code: {request.responseCode}\nResponse: {request.downloadHandler.text}");
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log($"Response: {jsonResponse}");
                // response 데이터를 사용할 일 있으면 추가하기
            }
        }
    }
}

public class SurveyData
{
    public string userId;
    public int scoreResult;
}

public class SurveyResponse
{
    public int status;
    public bool success;
    public string message;
    public SurveyResponseData data;
}

public class SurveyResponseData
{
    public string result;
    public string detailResult;
}