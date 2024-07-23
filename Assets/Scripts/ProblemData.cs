using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class ProblemData : MonoBehaviour
{
    public static ProblemData instance;
    public QuestionData[][] problemData = new QuestionData[2][];
    public QuestionData problem3Data;
    public List<Sprite> spriteFromServer;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 이 객체를 파괴하지 않음
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnLoadStageData(string theme)
    {
        string getProblemUrl;
        getProblemUrl = $"https://worderland.kro.kr/api/question/{theme}?stage=1";
        StartCoroutine(GetRequest(getProblemUrl, 1));
        getProblemUrl = $"https://worderland.kro.kr/api/question/{theme}?stage=2";
        StartCoroutine(GetRequest(getProblemUrl, 2));
        getProblemUrl = $"https://worderland.kro.kr/api/question/{theme}?stage=3";
        StartCoroutine(GetRequestStage3(getProblemUrl));
    }

    /// <summary>
    /// Get Problem Set from server.
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    IEnumerator GetRequest(string uri, int stage)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            // 요청 보내기
            yield return request.SendWebRequest();

            // 오류 처리
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Loading.OnError();
                Debug.LogError(request.error);
            }
            else
            {
                // 응답 받기
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Response: " + jsonResponse);

                // JSON 파싱
                QuestionResponseData responseData = JsonUtility.FromJson<QuestionResponseData>(jsonResponse);

                // 데이터 접근
                if (responseData.success)
                {
                    problemData[stage - 1] = responseData.data.Clone() as QuestionData[];
                    Debug.Log($"Response: {problemData[stage - 1]}");
                    // 2 스테이지 문제라면, 미리 이미지 다운로드.
                    if (stage == 2 && SceneTheme.theme != "carousel")
                    {
                        CheckIsImageProblem();
                    }
                    else
                    {

                        Loading.sceneLoadedCount++;
                    }
                }
                else
                {
                    Loading.OnError();
                    Debug.LogError("Request failed with message: " + responseData.message);
                }
            }
        }
    }
    IEnumerator GetRequestStage3(string uri)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            // 요청 보내기
            yield return request.SendWebRequest();

            // 오류 처리
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Loading.OnError();
                Debug.LogError(request.error);
            }
            else
            {
                // 응답 받기
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Response: " + jsonResponse);

                // JSON 파싱
                Question3ResponseData responseData = JsonUtility.FromJson<Question3ResponseData>(jsonResponse);

                // 데이터 접근
                if (responseData.success)
                {
                    problem3Data = responseData.data;
                    Loading.sceneLoadedCount++;

                    // 잘 복사 됐는지 보는 로고
                    /*
                    int questionId = problemData.questionId;
                    string content = problemData.content;
                    Debug.Log("Question ID: " + questionId);
                    Debug.Log("Content: " + content);
                    */
                }
                else
                {
                    Loading.OnError();
                    Debug.LogError("Request failed with message: " + responseData.message);
                }
            }
        }
    }


    /// <summary>
    /// 이미지 url에서 이미지를 다운로드 받아 Sprite로 등록
    /// </summary>
    /// <param name="url">이미지 url</param>
    /// <returns></returns>
    IEnumerator DownloadImage(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            spriteFromServer.Add(sprite);
            Loading.sceneLoadedCount++;
        }
    }


    private void CheckIsImageProblem()
    {
        foreach (var content in problemData[1])
        {
            if (content.content.StartsWith("http"))
            {
                StartCoroutine(DownloadImage(content.content));
            }
        }
    }
}
