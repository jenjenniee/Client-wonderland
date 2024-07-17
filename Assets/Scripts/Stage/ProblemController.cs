using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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
    private Sprite[] animalSprites = new Sprite[4];
    [SerializeField]
    private TextMeshProUGUI[] textAnswer;

    [SerializeField]
    private GameObject[] correctAnimator = new GameObject[4];
    [SerializeField]
    private GameObject[] wrongAnimator = new GameObject[4];

    [SerializeField]
    private GameObject heartMove;

    [SerializeField]
    private TextMeshProUGUI textProblemNumber;
    [SerializeField]
    private TextMeshProUGUI textProblem;
    [SerializeField]
    private TextMeshProUGUI textFollowingWord;

    private int problemNumber = 0;
    private int stage1Number = 0;
    private int stage2Number = 0;
    private bool timeout = false;

    // 별 애니메이션을 처리할 오브젝트
    public GameObject starAnimation;

    private TimerController timerController;
    [SerializeField]
    private SentenceStageManager sentenceStageManager;
    //private bool solvingProblem = false;
    private int problemStyle;

    private string getProblemUrl;           // server url
    private string postAnswerUrl;
    QuestionData[][] problemData = new QuestionData[2][];             // problem data from server. 10 problems get into this variable.
    public TextMeshProUGUI tmp;
    public GameObject ocrPanel;
    public CanvasGroup ocrCanvasGroup;

    private List<Sprite> spriteFromServer;      // 서버에서 받아온 이미지를 스프라이트 형태로 저장하는 배열
    public GameObject problemImage;             // 이미지를 보여줄 오브젝트 -> Image.sprite
    public GameObject ttsButton;                  // TTS로 들려줄 텍스트를 보여줄 오브젝트, Alpha == 0f
    public GameObject ttsText;                  // TTS로 들려줄 텍스트를 보여줄 오브젝트, Alpha == 0f

    private void Start()
    {
        // 맞은 개수 초기화
        NumberOfCorrect.Reset();

        postAnswerUrl = "https://worderland.kro.kr/api/answer";
        /*
        getProblemUrl = $"https://worderland.kro.kr/api/question/{SceneTheme.theme}?stage=1";
        StartCoroutine(GetRequest(getProblemUrl, 1));
        getProblemUrl = $"https://worderland.kro.kr/api/question/{SceneTheme.theme}?stage=2";
        StartCoroutine(GetRequest(getProblemUrl, 2));
        */
        problemData = ProblemData.instance.problemData.Clone() as QuestionData[][];
        spriteFromServer = ProblemData.instance.spriteFromServer;

        timerController = GameObject.Find("Gauge Front").GetComponent<TimerController>();
        timerController.onTimer = false;
        
        foreach (GameObject animal in animals)
        {
            animal.SetActive(false);
        }
        SetHeart();
        StartCoroutine(SetNewProblem(5f));
    }

    private void Update()
    {
        if (!timeout && timerController.integerTimer == 0)
        {
            timeout = true;
            StartCoroutine(TimeOut());
        }
    }

    private void SetHeart() 
    {
        tmp.text = "" + BackendGameData.Instance.UserGameData.heart;
        Debug.Log($"userId: {UserInfo.Data.gamerId}");
    }

    private void CheckAnswer(string jsonResponse, int idx) 
    {
        QuestionResponseData responseData = JsonUtility.FromJson<QuestionResponseData>(jsonResponse);
        string response = responseData.message;
        Debug.Log(response);
        if (response == "정답 입니다")
        {
            Debug.Log("진입");
            StartCoroutine(AfterHeartMove());
            correctAnimator[idx].SetActive(true);
            heartMove.SetActive(true);
            NumberOfCorrect.numberOfCorrect++;
            if (NumberOfCorrect.numberOfCorrect == 1)
            {
                if (SceneTheme.theme == "carousel")
                {
                    starAnimation.GetComponent<GainStarManager>().GainStar(1, 0);
                } 
                else if (SceneTheme.theme == "ferris_wheel")
                {
                    starAnimation.GetComponent<GainStarManager>().GainStar(2, 0);
                }
                else
                {
                    starAnimation.GetComponent<GainStarManager>().GainStar(3, 0);
                }
            }
            if (NumberOfCorrect.numberOfCorrect == 6)
            {
                if (SceneTheme.theme == "carousel")
                {
                    starAnimation.GetComponent<GainStarManager>().GainStar(1, 1);
                }
                else if (SceneTheme.theme == "ferris_wheel")
                {
                    starAnimation.GetComponent<GainStarManager>().GainStar(2, 1);
                }
                else
                {
                    starAnimation.GetComponent<GainStarManager>().GainStar(3, 1);
                }
            }

        }
        else if (response == "오답 입니다")
        {
            //BackendGameData.Instance.DecreaseHeart(50);
            //SetHeart();
            wrongAnimator[idx].SetActive(true);
        }
        StartCoroutine(ChoiseAnimation(idx));
    }
    private IEnumerator AfterHeartMove()
    {
        yield return new WaitForSecondsRealtime(1f);
        BackendGameData.Instance.IncreaseHeart(10);
        SetHeart();
    }
    /// <summary>
    /// ������ ������ ���� ���� �Լ�
    /// </summary>
    /// <param name="chosenNumber"></param>
    private void Choice(int chosenNumber)
    {
        timerController.onTimer = false;
        // ���� ���� ����
        AnswerData data = new AnswerData
        {
            questionId = problemData[0][stage1Number++].questionId,
            userId = UserInfo.Data.gamerId,
            answer = textAnswer[chosenNumber].text,
        };

        // JSON 문자열로 변환
        string jsonData = JsonUtility.ToJson(data);

        // POST 요청 시작
        StartCoroutine(PostRequest(postAnswerUrl, jsonData, chosenNumber));

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
        //StartCoroutine(ChoiseAnimation(chosenNumber));
    }


    public void OnSubmitOCR(string textOCR)
    {
        //ocrCanvasGroup.interactable = false;
        timerController.onTimer = false;
        // ���� ���� ����
        Debug.Log($"questionID: {problemData[1][stage2Number].questionId}, textOCR: {textOCR}, answer: {problemData[1][stage2Number].content}");
        AnswerData data = new AnswerData
        {
            questionId = problemData[1][stage2Number++].questionId,
            userId = UserInfo.Data.gamerId,
            answer = textOCR,
        };


        // JSON 문자열로 변환
        string jsonData = JsonUtility.ToJson(data);

        // POST 요청 시작
        StartCoroutine(PostRequest(postAnswerUrl, jsonData, 4));
        //StartCoroutine(SetNewProblem(3f));
    }


    private IEnumerator ChoiseAnimation(int chosenNumber)
    {
        yield return new WaitForSecondsRealtime(3f);
        if (chosenNumber < 4)
        {
            choiceButton[chosenNumber].alpha = 0f;
            animals[chosenNumber].SetActive(false);
        }
        correctAnimator[chosenNumber].SetActive(false);
        wrongAnimator[chosenNumber].SetActive(false);
        ocrPanel.SetActive(false);
        problemImage.SetActive(false);
        textFollowingWord.text = "";
        ttsButton.SetActive(false);

        StartCoroutine(SetNewProblem(0f));
    }

    private IEnumerator TimeOut()
    {
        foreach (CanvasGroup btn in choiceButton)
        {
            btn.alpha = 0f;
        }
        AnswerData data = new AnswerData
        {
            questionId = problemData[problemStyle][problemStyle == 0 ? stage1Number++ : stage2Number++].questionId,
            userId = UserInfo.Data.gamerId,
            answer = ".",
        };


        // JSON 문자열로 변환
        string jsonData = JsonUtility.ToJson(data);

        // POST 요청 시작
        StartCoroutine(PostRequest(postAnswerUrl, jsonData, 4));

        yield return new WaitForSecondsRealtime(3f);
        foreach (GameObject obj in animals)
        {
            obj.SetActive(false);
        }
        ocrPanel.SetActive(false);
        problemImage.SetActive(false);
        textFollowingWord.text = "";

        //StartCoroutine(SetNewProblem(0f));
    }

    private IEnumerator SetNewProblem(float delayTime)
    {
        yield return new WaitForSecondsRealtime(delayTime);

        problemStyle = Random.Range(0, 2);

        // stage 1 다 끝났는데 problemStyle == 0 이면, 1로 바꿈
        if (stage1Number == 5 && problemStyle == 0) problemStyle = 1;
        // stage 2 다 끝났는데 problemStyle == 1 이면, 0으로 바꿈
        if (stage2Number == 5 && problemStyle == 1) problemStyle = 0;

        timerController.NewProblemTimer(problemStyle == 0 ? 10f : 20f);
        timerController.onTimer = false;
        heartMove.SetActive(false);

        // ���� ��ȣ ����
        problemNumber++;

        // �������� 00�� �����ϸ� ���� ����������
        if (problemNumber == 11)
        {
            sentenceStageManager.NextStage();
        }
        else
        {
            textProblemNumber.text = $"Q{problemNumber}.";


            if (problemStyle == 0)
            {
                // 선택형
                // "ProblemTitle,Word[4]" 형식으로 보내주므로, Split하고 index 0이 문제 내용임. 나머지는 단어 선택지.
                string[] words = problemData[0][stage1Number].content.Split(',');

                // Problem Title
                textProblem.text = words[0];

                // set problem's answers
                for (int i = 0; i < 4; i++)
                {
                    // words[1] ~ words[4]가 선택지 단어이므로 매핑. Mapping all words to each answer text.
                    textAnswer[i].text = words[i + 1];
                    choiceButton[i].interactable = true;
                }

                //Debug.Log("A New Problem Set.");
                for (int i = 0; i < 4; i++)
                {
                    StartCoroutine(AnimalAppear(Random.Range(0f, 1f), animals[i]));
                }
                StartCoroutine(StartProblem(3f));
            }
            else
            {
                // 주관식
                // "http..."로 시작하면 사진형 문제.
                if (problemData[1][stage2Number].content.StartsWith("http"))
                {
                    textProblem.text = $"What's in the picture below?";
                    problemImage.SetActive(true);
                    // 현재 이미지 문제는 하나이므로 0으로 두었음
                    problemImage.GetComponent<Image>().sprite = spriteFromServer[0];
                }
                // 아니라면 채우기 문제
                // 이때 tts 필요
                else
                {
                    string[] words = problemData[1][stage2Number].content.Split(",");
                    textProblem.text = $"Listen and fill in the blanks:";
                    textFollowingWord.text = $"{words[0]}";
                    // words[1]은 TTS용
                    ttsText.GetComponent<TextMeshProUGUI>().text = words[1];
                    ttsButton.SetActive(true);
                }
                ocrPanel.SetActive(true);
                StartCoroutine(StartProblem(2f));
            }
        }
    }

    private IEnumerator StartProblem(float delayTime)
    {
        yield return new WaitForSecondsRealtime(delayTime);
        timerController.onTimer = true;
        timeout = false;
        ocrCanvasGroup.interactable = true;
        ocrPanel.GetComponent<DrawScript>().CanDrawing();
    }

    private IEnumerator AnimalAppear(float delayTime, GameObject obj)
    {
        yield return new WaitForSecondsRealtime(delayTime);
        int index = Random.Range(0, 4);
        obj.GetComponent<SpriteRenderer>().sprite = animalSprites[index];
        obj.SetActive(true);
        obj.GetComponent<Animator>().SetInteger("Index", index);
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
                    // 잘 복사 됐는지 보는 로고
                    /*
                    foreach (QuestionData questionData in problemData[0])
                    {
                        int questionId = questionData.questionId;
                        string content = questionData.content;
                        //Debug.Log("Question ID: " + questionId);
                        //Debug.Log("Content: " + content);
                    }
                    */
                    Debug.Log($"Response: {problemData[stage - 1]}");

                    // 2 스테이지 문제라면, 미리 이미지 다운로드.
                    if (stage == 2)
                    {
                        CheckIsImageProblem();
                    }
                }
                else
                {
                    Debug.LogError("Request failed with message: " + responseData.message);
                }
            }
        }
    }
    IEnumerator PostRequest(string uri, string json, int idx)
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
            CheckAnswer(jsonResponse, idx);
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
        }
    }


    private void CheckIsImageProblem()
    {
        foreach(var content in problemData[1])
        {
            if (content.content.StartsWith("http"))
            {
                StartCoroutine(DownloadImage(content.content));
            }
        }
    }
}


static public class NumberOfCorrect
{
    static public int numberOfCorrect = 0;

    static public void Reset()
    {
        numberOfCorrect = 0;
    }
}