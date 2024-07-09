using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class QuestionData
{
    public int questionId;
    public string content;
}

[System.Serializable]
public class QuestionResponseData
{
    public int status;
    public bool success;
    public string message;
    public QuestionData[] data;
}

[System.Serializable]
public class AnswerData
{
    public int questionId;
    public string userId;
    public string answer;
}

public class ProblemController : MonoBehaviour
{
    //private int problemType = 1;    //���� ���� (1: ������, 2: �ְ���(OCR))

    [SerializeField]
    private CanvasGroup[] choiceButton = new CanvasGroup[4];  // ���� ��ư
    [SerializeField]
    private GameObject[] animals = new GameObject[4];       // �ش� ����
    [SerializeField]
    private TextMeshProUGUI[] textAnswer;

    [SerializeField]
    private TextMeshProUGUI textProblemNumber;
    [SerializeField]
    private TextMeshProUGUI textProblem;

    private int problemNumber = 0;

    private TimerController timerController;
    [SerializeField]
    private SentenceStageManager sentenceStageManager;
    //private bool solvingProblem = false;

    [SerializeField]
    private string theme;
    [SerializeField]
    private string stage;

    private string getProblemUrl;           // server url
    private string postAnswerUrl;
    QuestionData[] problemData;             // problem data from server. 10 problems get into this variable.
    public TextMeshProUGUI tmp;

    private void Start()
    {
        getProblemUrl = $"https://worderland.kro.kr/api/question/{theme}?stage={stage}";
        postAnswerUrl = "https://worderland.kro.kr/api/answer";
        StartCoroutine(GetRequest(getProblemUrl));

        timerController = GameObject.Find("Gauge Front").GetComponent<TimerController>();
        timerController.onTimer = false;
        
        foreach (GameObject animal in animals)
        {
            animal.SetActive(false);
        }
        setHeart();
    }

    private void setHeart() 
    {
        tmp.text = "" + BackendGameData.Instance.UserGameData.heart;
    }

    private void checkAnswer(string jsonResponse) 
    {
        QuestionResponseData responseData = JsonUtility.FromJson<QuestionResponseData>(jsonResponse);
        string response = responseData.message;
        Debug.Log(response);
        if (response == "정답 입니다")
        {
            Debug.Log("진입");
            BackendGameData.Instance.IncreaseHeart(50);
            setHeart();
        }
        else if (response == "오답 입니다")
        {
            BackendGameData.Instance.DecreaseHeart(50);
            setHeart();
        }
    }
    /// <summary>
    /// ������ ������ ���� ���� �Լ�
    /// </summary>
    /// <param name="chosenNumber"></param>
    public void Choice(int chosenNumber)
    {
        timerController.onTimer = false;
        // ���� ���� ����
        AnswerData data = new AnswerData
        {
            questionId = problemData[problemNumber - 1].questionId,
            userId = "user123",
            answer = textAnswer[chosenNumber].text,
        };

        // JSON 문자열로 변환
        string jsonData = JsonUtility.ToJson(data);

        // POST 요청 시작
        StartCoroutine(PostRequest(postAnswerUrl, jsonData));

        // ���� �� �۾�
        for (int i = 0; i < 4; i++)
        {
            if (i != chosenNumber)
            {
                choiceButton[i].alpha = 0f;
                animals[i].SetActive(false);
            }
        }
        // ���� �� ��ü�� 3�� ���� �������� �˷��ִ� �ִϸ��̼�
        StartCoroutine(ChoiseAnimation(chosenNumber));
    }

    public IEnumerator ChoiseAnimation(int chosenNumber)
    {
        yield return new WaitForSecondsRealtime(3f);
        choiceButton[chosenNumber].alpha = 0f;
        animals[chosenNumber].SetActive(false);

        StartCoroutine(SetNewProblem(0f));
    }

    public IEnumerator SetNewProblem(float delayTime)
    {
        yield return new WaitForSecondsRealtime(delayTime);

        timerController.NewProblemTimer(20f);
        timerController.onTimer = false;

        // ���� ��ȣ ����
        problemNumber++;

        // �������� 00�� �����ϸ� ���� ����������
        if (problemNumber == 11)
        {
            sentenceStageManager.NextStage();
        }
        else
        {
            textProblemNumber.text = $"문제{problemNumber}.";
            string[] words = problemData[problemNumber - 1].content.Split(',');

            // set problem's answers
            for (int i = 0; i < 4; i++)
            {
                textAnswer[i].text = words[i];
            }

            //Debug.Log("A New Problem Set.");
            for (int i = 0; i < 4; i++)
            {
                StartCoroutine(AnimalAppear(Random.Range(0f, 1f), animals[i]));
            }
            StartCoroutine(StartProblem(3f));
        }
    }

    public IEnumerator StartProblem(float delayTime)
    {
        yield return new WaitForSecondsRealtime(delayTime);
        timerController.onTimer = true;
    }

    public IEnumerator AnimalAppear(float delayTime, GameObject obj)
    {
        yield return new WaitForSecondsRealtime(delayTime);
        obj.SetActive(true);
    }

    /// <summary>
    /// Get Problem Set from server.
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            // 요청 보내기
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
                QuestionResponseData responseData = JsonUtility.FromJson<QuestionResponseData>(jsonResponse);

                // 데이터 접근
                if (responseData.success)
                {
                    problemData = responseData.data.Clone() as QuestionData[];
                    // 잘 복사 됐는지 보는 로고
                    foreach (QuestionData questionData in problemData)
                    {
                        int questionId = questionData.questionId;
                        string content = questionData.content;
                        Debug.Log("Question ID: " + questionId);
                        Debug.Log("Content: " + content);
                    }
                    StartCoroutine(SetNewProblem(5f));
                }
                else
                {
                    Debug.LogError("Request failed with message: " + responseData.message);
                }
            }
        }
    }
    IEnumerator PostRequest(string uri, string json)
    {
        var request = new UnityWebRequest(uri, "POST");
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 요청 보내기
        yield return request.SendWebRequest();

        // 오류 처리
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;
            checkAnswer(jsonResponse);
        }
    }
}
