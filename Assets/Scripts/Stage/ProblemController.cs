using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProblemController : MonoBehaviour
{
    //private int problemType = 1;    //문제 유형 (1: 객관식, 2: 주관식(OCR))

    [SerializeField]
    private CanvasGroup[] choiceButton = new CanvasGroup[4];  // 선택 버튼
    [SerializeField]
    private GameObject[] animals = new GameObject[4];       // 해당 동물

    [SerializeField]
    private TextMeshProUGUI textProblemNumber;
    [SerializeField]
    private TextMeshProUGUI textProblem;

    private int problemNumber = 0;

    private TimerController timerController;
    //private bool solvingProblem = false;

    private void Start()
    {
        timerController = GameObject.Find("Gauge Front").GetComponent<TimerController>();
        timerController.onTimer = false;
        StartCoroutine(SetNewProblem(7f));
        
        foreach (GameObject animal in animals)
        {
            animal.SetActive(false);
        }
        
    }

    /// <summary>
    /// 객관식 문제에 대한 선택 함수
    /// </summary>
    /// <param name="chosenNumber"></param>
    public void Choice(int chosenNumber)
    {
        timerController.onTimer = false;
        // 정답 대조 로직

        // 선택 후 작업
        for (int i = 0; i < 4; i++)
        {
            if (i != chosenNumber)
            {
                choiceButton[i].alpha = 0f;
                animals[i].SetActive(false);
            }
        }
        // 선택 된 객체는 3초 동안 정답임을 알려주는 애니메이션
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
        problemNumber++;
        textProblemNumber.text = $"문제{problemNumber}.";

        //Debug.Log("A New Problem Set.");
        for (int i = 0; i < 4; i++)
        {
            StartCoroutine(AnimalAppear(i * 0.3f, animals[i]));
        }
        StartCoroutine(StartProblem(3f));
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
}
