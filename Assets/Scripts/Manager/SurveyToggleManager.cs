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
    public GameObject surveyResultUI;
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

        using (UnityWebRequest request = new UnityWebRequest(uri, "POST"))
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
                SurveyResponse responseData = JsonUtility.FromJson<SurveyResponse>(jsonResponse);

                Debug.Log($"Response: {jsonResponse}");

                // Ensure the responseData is not null and has valid data
                if (responseData != null && responseData.data != null)
                {
                    surveyResultUI.SetActive(true);
                    SurveyResultManager resultManager = surveyResultUI.GetComponent<SurveyResultManager>();
                    if (resultManager != null)
                    {
                        resultManager.SetText(responseData.data);
                        Debug.Log("SetText executed successfully.");
                    }
                    else
                    {
                        Debug.LogError("SurveyResultManager component not found on surveyResultUI.");
                    }
                    surveyUI.SetActive(false);
                }
                else
                {
                    Debug.LogError("Response data is null or invalid.");
                }
            }
        }
    }
}
[System.Serializable]
public class SurveyData
{
    public string userId;
    public int scoreResult;
}
[System.Serializable]
public class SurveyResponse
{
    public int status;
    public bool success;
    public string message;
    public SurveyResponseData data;
}
[System.Serializable]
public class SurveyResponseData
{
    public string result;
    public string detailResult;
}