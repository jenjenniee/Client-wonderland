using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Question3ResponseData
{
    public int status;
    public bool success;
    public string message;
    public QuestionData data;
}

public class SentenceStageManager : MonoBehaviour
{
    [SerializeField]
    private GameObject wordStage;
    [SerializeField]
    private CanvasGroup wordStageCanvas;
    [SerializeField]
    private Renderer blackBoxRenderer;

    [SerializeField]
    private GameObject polaroid;
    [SerializeField]
    private GameObject polariodFilm;
    [SerializeField] 
    private CanvasGroup uiGroup;
    [SerializeField]
    private GameObject background;


    [SerializeField]
    private TextMeshProUGUI sentence;
    private string[] splittedSentence;  // 잘린 문자, 정답 단어, 변형된 단어
    public GameObject wordPanel;
    public GameObject tmpPrefab;        // 단어 text 프리팹
    private float panelWidth;
    private float currentX = 0f;
    private float currentY = 0f;
    private float maxHeightInRow = 0f;

    private TextMeshProUGUI selectedText;
    [SerializeField] private TextMeshProUGUI selectedWord; // 화면에 보여지는 텍스트
    [SerializeField] private GameObject correct;
    [SerializeField] private GameObject wrong;

    // 별 애니메이션을 처리할 오브젝트
    public GameObject starAnimation;

    private QuestionData problemData;

    private string postAnswerUrl;
    private string stage3Url;

    // 게임 끝나고 게임 정보 업데이트
    public GameObject getMapInformation;

    private void Start()
    {
        postAnswerUrl = "https://worderland.kro.kr/api/answer";
        // 패널의 너비를 가져옵니다.
        RectTransform panelRectTransform = wordPanel.GetComponent<RectTransform>();
        panelWidth = panelRectTransform.rect.width;
        problemData = ProblemData.instance.problem3Data;

        splittedSentence = problemData.content.Split('+');
        sentence.text = "";

        foreach (string word in splittedSentence[0].Split(' '))
        {
            if (word == splittedSentence[2])
            {
                sentence.text += splittedSentence[1] + " ";
            }
            else
            {
                sentence.text += word + " ";
            }
            CreateWord(word);
        }

        //stage3Url = $"https://worderland.kro.kr/api/question/{SceneTheme.theme}?stage=3";
        //StartCoroutine(GetRequest(stage3Url));
    }

    public void NextStage()
    {
        // ���� ���������� �Ѿ�� wordStage�� ��Ȱ��ȭ.
        if (wordStage != null) 
        {
            StartCoroutine(FadeOutCanvasGroup(wordStageCanvas, blackBoxRenderer, 3f));
        }
    }

    private void CreateWord(string word)
    {
        GameObject tmpObject = Instantiate(tmpPrefab, wordPanel.transform);

        TextMeshProUGUI tmpText = tmpObject.GetComponent<TextMeshProUGUI>();
        tmpText.text = word;

        // 텍스트에 맞춰 RectTransform의 크기 조절
        RectTransform rectTransform = tmpObject.GetComponent<RectTransform>();
        float preferredWidth = tmpText.preferredWidth;
        float preferredHeight = tmpText.preferredHeight;
        rectTransform.sizeDelta = new Vector2(preferredWidth, preferredHeight);

        // 현재 X 위치 + 텍스트의 너비가 패널의 너비를 넘으면 다음 줄로 이동
        if (currentX + preferredWidth > panelWidth)
        {
            currentX = 0f;
            currentY -= maxHeightInRow + 10f; // 줄 바꿈과 간격 설정
            maxHeightInRow = 0f;
        }

        // 단어 위치 설정
        rectTransform.anchoredPosition = new Vector2(currentX, currentY);
        currentX += preferredWidth + 10f; // 다음 단어 위치로 이동

        // 현재 줄에서 가장 높은 단어의 높이를 저장
        if (preferredHeight > maxHeightInRow)
        {
            maxHeightInRow = preferredHeight;
        }
        //Debug.Log($"Current X: {currentX}, Current Y: {currentY}");

        Button button = tmpObject.AddComponent<Button>();
        // TextWordHighlighter 추가
        //TextWordHighlighter highlighter = tmpObject.AddComponent<TextWordHighlighter>();
        //highlighter.originalColor = tmpText.color; // 원래 색상을 저장
        button.onClick.AddListener(() => OnWordClick(tmpText));
    }

    void OnWordClick(TextMeshProUGUI word)
    {
        if (selectedText != null)
        {
            selectedText.color = word.color;
        }
        Debug.Log($"Clicked on word: {word.text}");
        if (word.text == splittedSentence[2])
        {
            Debug.Log("Correct word is selected.");
        }
        word.color = Color.red;
        selectedText = word; 
        selectedWord.text = word.text;
    }

    public void SubmitSelection()
    {
        AnswerData data = new AnswerData
        {
            questionId = problemData.questionId,
            userId = UserInfo.Data.gamerId,
            answer = selectedWord.text,
        };

        if (selectedWord.text == splittedSentence[2])
        {
            //correct.
            selectedWord.text = splittedSentence[1];
            BackendGameData.Instance.IncreaseHeart(50);
            correct.SetActive(true);
            NumberOfCorrect.numberOfCorrect++;
            if (NumberOfCorrect.numberOfCorrect == 11)
            {
                if (SceneTheme.theme == "carousel")
                {
                    starAnimation.GetComponent<GainStarManager>().GainStar(1, 2);
                }
                else if (SceneTheme.theme == "ferris_wheel")
                {
                    starAnimation.GetComponent<GainStarManager>().GainStar(2, 2);
                }
                else
                {
                    starAnimation.GetComponent<GainStarManager>().GainStar(3, 2);
                }
            }
        }
        else
        {
            //wrong.
            wrong.SetActive(true);
        }

        // JSON 문자열로 변환
        string jsonData = JsonUtility.ToJson(data);
        StartCoroutine(PostRequest(postAnswerUrl, jsonData));
    }

    IEnumerator BackToMap()
    {
        getMapInformation.SetActive(true);
        yield return new WaitForSecondsRealtime(3f);
        Utils.LoadScene("Map");
    }

    private IEnumerator FadeOutCanvasGroup(CanvasGroup canvasGroup, Renderer blackBoxRenderer, float duration)
    {
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, time / duration);
            float canvasDuration = duration / 6f;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, time / canvasDuration);
            if (blackBoxRenderer != null)
            {
                foreach (Material mat in blackBoxRenderer.materials)
                {
                    Color color = mat.color;
                    color.a = alpha;
                    mat.color = color;
                }
            }
            yield return null;
        }

        canvasGroup.alpha = 0f;
        if (blackBoxRenderer != null)
        {
            foreach (Material mat in blackBoxRenderer.materials)
            {
                Color color = mat.color;
                color.a = 0f;
                mat.color = color;
            }
        }
        wordStage.SetActive(false);
        polaroid.SetActive(true);
        StartCoroutine(UIFadeIn(2f, 0.3f));
    }

    private IEnumerator UIFadeIn(float delay, float duration)
    {
        yield return new WaitForSecondsRealtime(delay);

        polariodFilm.SetActive(true);
        background.SetActive(true);

        yield return new WaitForSecondsRealtime(1f);

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            uiGroup.alpha = Mathf.Lerp(0f, 1f, time / duration);
            yield return null;
        }
        uiGroup.alpha = 1f;
        uiGroup.interactable = true;
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
                    problemData = responseData.data;
                    splittedSentence = problemData.content.Split('+');
                    sentence.text = "";

                    foreach (string word in splittedSentence[0].Split(' '))
                    {
                        if (word == splittedSentence[2])
                        {
                            sentence.text += splittedSentence[1] + " ";
                        }
                        else
                        {
                            sentence.text += word + " ";
                        }
                        CreateWord(word);
                    } 

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
                    Debug.LogError("Request failed with message: " + responseData.message);
                }
            }
        }
    }

    IEnumerator PostRequest(string uri, string json)
    {
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
            }
        }
        StartCoroutine(BackToMap());
    }
}


public class TextWordHighlighter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI tmpText;
    public Color highlightColor = Color.yellow; // 하이라이트 색상
    public Color originalColor; // 원래 색상

    void Start()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
    }

    // 마우스가 텍스트 위로 올라갔을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        tmpText.color = highlightColor;
    }

    // 마우스가 텍스트에서 벗어났을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        tmpText.color = originalColor;
    }

    // 단어를 클릭했을 때 하이라이트 처리
    public void HighlightWord(TextMeshProUGUI text)
    {
        if (tmpText != null)
        {
            tmpText.color = highlightColor;
        }
    }
}
