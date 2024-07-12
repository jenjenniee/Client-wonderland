using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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


public class MapInfoManager : MonoBehaviour
{
    private string userId;
    [SerializeField]
    private string theme;

    // 텍스트
    [SerializeField]
    private TextMeshProUGUI textCorrectness;
    [SerializeField]
    private TextMeshProUGUI textStageInfo;
    [SerializeField]
    private TextMeshProUGUI textNoInfo;

    // 텍스트 오브젝트
    [SerializeField]
    private GameObject objCorrectness;
    [SerializeField]
    private GameObject objStageInfo;
    [SerializeField]
    private GameObject objNoInfo;

    // 별과 관련된 변수들
    [SerializeField]
    private Image[] spriteRenderer = new Image[3];
    [SerializeField]
    private Sprite yellowStar;

    private StageData[] stageData;
    private string url;

    void Start()
    {
        userId = UserInfo.Data.gamerId;
        url = $"https://worderland.kro.kr/api/result/{userId}?theme={theme}";
        StartCoroutine(GetRequest(url));
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            // 요청 보내기
            yield return request.SendWebRequest();

            // 오류 처리
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                objCorrectness.SetActive(false);
                objStageInfo.SetActive(false);
                objNoInfo.SetActive(true);
                textNoInfo.text = "No record";
                Debug.LogError(request.error);
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
                    stageData = responseData.data.Clone() as StageData[];
                    foreach (var userData in responseData.data)
                    {
                        Debug.Log("User ID: " + userData.userId);
                        Debug.Log("Theme: " + userData.theme);
                        Debug.Log("Stage: " + userData.stage);
                        Debug.Log("Correct Answers: " + userData.correctAnswers);
                    }
                    int correct = 0;
                    for(int i = 0; i < stageData.Length - 1; i++)
                    {
                        correct += stageData[i].correctAnswers;
                    }
                    // UI 업데이트
                    objCorrectness.SetActive(true);
                    objStageInfo.SetActive(true);
                    objNoInfo.SetActive(false);

                    float correctness = Mathf.Floor(correct / 11f * 100f);
                    textCorrectness.text = "Correctness";
                    textStageInfo.text = $"{correctness}%";
                    // 별 UI 업데이트
                    if (correctness > 0f)
                        spriteRenderer[0].sprite = yellowStar;
                    if (correctness >= 50f)
                        spriteRenderer[1].sprite = yellowStar;
                    if (correctness == 100f)
                        spriteRenderer[2].sprite = yellowStar;
                }
                else
                {
                    objCorrectness.SetActive(false);
                    objStageInfo.SetActive(false);
                    objNoInfo.SetActive(true);
                    textNoInfo.text = "No record";
                    Debug.LogError("Request failed with message: " + responseData.message);
                }
            }
        }
    }
}
