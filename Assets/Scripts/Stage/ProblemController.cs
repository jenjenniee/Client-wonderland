using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProblemController : MonoBehaviour
{
    //private int problemType = 1;    //���� ���� (1: ������, 2: �ְ���(OCR))

    [SerializeField]
    private CanvasGroup[] choiceButton = new CanvasGroup[4];  // ���� ��ư
    [SerializeField]
    private GameObject[] animals = new GameObject[4];       // �ش� ����

    [SerializeField]
    private TextMeshProUGUI textProblemNumber;
    [SerializeField]
    private TextMeshProUGUI textProblem;

    private int problemNumber = 0;

    private TimerController timerController;
    [SerializeField]
    private SentenceStageManager sentenceStageManager;
    //private bool solvingProblem = false;

    private void Start()
    {
        timerController = GameObject.Find("Gauge Front").GetComponent<TimerController>();
        timerController.onTimer = false;
        StartCoroutine(SetNewProblem(5f));
        
        foreach (GameObject animal in animals)
        {
            animal.SetActive(false);
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
            textProblemNumber.text = $"����{problemNumber}.";

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
}
