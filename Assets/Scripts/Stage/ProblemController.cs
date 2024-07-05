using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProblemController : MonoBehaviour
{
    //private int problemType = 1;    //문제 유형 (1: 객관식, 2: 주관식(OCR))

    [SerializeField]
    private GameObject[] choiceButton = new GameObject[4];  // 선택 버튼
    [SerializeField]
    private GameObject[] animals = new GameObject[4];       // 해당 동물

    private TimerController timerController;

    private void Start()
    {
        timerController = GameObject.Find("Gauge Front").GetComponent<TimerController>();
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
                choiceButton[i].SetActive(false);
                animals[i].SetActive(false);
            }
        }
        // 선택 된 객체는 3초 동안 정답임을 알려주는 애니메이션
        StartCoroutine(SetNewProblem(chosenNumber));
    }

    public IEnumerator SetNewProblem(int chosenNumber)
    {
        yield return new WaitForSecondsRealtime(3f);
        choiceButton[chosenNumber].SetActive(false);
        animals[chosenNumber].SetActive(false);
        timerController.NewProblemTimer(20f);
    }
}
