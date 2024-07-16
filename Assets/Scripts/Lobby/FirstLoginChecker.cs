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

            // ���� ó��
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                // ���� �ޱ�
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Response: " + jsonResponse);

                // JSON �Ľ�
                MemberCheckResponseData responseData = JsonUtility.FromJson<MemberCheckResponseData>(jsonResponse);

                // ������ ����
                if (responseData.success)
                {
                    if (responseData.data.result)
                    {
                        // ������ ó�� �α����� ��Ȳ -> ���� �׽�Ʈ On
                    }else
                    {
                        // �̹� �����׽�Ʈ �� ��Ȳ -> ����
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
