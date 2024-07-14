using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class MemberCheckResponseData
{
    public int status;
    public bool success;
    public string message;
    public MemberCheckData data;
}

[System.Serializable]
public class MemberCheckData
{
    public bool result;
}

public class FirstLoginChecker : MonoBehaviour
{
    private string getUri;

    private void Start()
    {
        getUri = $"https://worderland.kro.kr/api/member_check?userId={UserInfo.Data.gamerId}";
        StartCoroutine(GetMemberCheck(getUri));
    }

    IEnumerator GetMemberCheck(string uri)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();

            // 오류 처리
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                // 응답 받기
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Response: " + jsonResponse);

                // JSON 파싱
                MemberCheckResponseData responseData = JsonUtility.FromJson<MemberCheckResponseData>(jsonResponse);

                // 데이터 접근
                if (responseData.success)
                {
                    if (responseData.data.result)
                    {
                        // 유저가 처음 로그인한 상황 -> 진단 테스트 On
                    }else
                    {
                        // 이미 진단테스트 본 상황 -> 무시
                    }
                }
                else
                {
                    Debug.LogError("Request failed with message: " + responseData.message);
                }
            }
        }
    }
}
