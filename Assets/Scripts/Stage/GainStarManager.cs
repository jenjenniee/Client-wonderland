using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class GainStarManager : MonoBehaviour
{
    public GameObject starPanel;
    public CanvasGroup greatText;
    public TextMeshProUGUI text;
    AudioManager audiomanager;
    public AudioClip []clip;

    private string[] achiveText = { "GOOD!", "GREAT!!", "PERFECT!!!" };

    // Start is called before the first frame update
    void Start()
    {
        audiomanager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    public void GainStar(int stage, int starNumber)
    {
        Debug.Log(starNumber);
        Debug.Log(clip.Length);
        // ���� ȹ���� �� �ִٸ�,
        if (!Star.GetStar(stage, starNumber))
        {
            // �� ȹ�� �ִϸ��̼�
            
            starPanel.SetActive(true);
            text.text = achiveText[starNumber];
            audiomanager.PlaySFX(clip[starNumber]);
        }

    }

    public void SetActiveFalsePanel()
    {
        starPanel.SetActive(false);
    }

    public void FadeInGreatText()
    {
        StartCoroutine(FadeUI(0.3f, 1f, greatText));
    }
    public void FadeOutGreatText()
    {
        StartCoroutine(FadeUI(0.3f, 0f, greatText));
    }

    private IEnumerator FadeUI(float duration, float targetAlpha, CanvasGroup ui)
    {
        float time = 0f;
        float currentAlpha = ui.alpha;
        while (time < duration)
        {
            time += Time.deltaTime;
            ui.alpha = Mathf.Lerp(currentAlpha, targetAlpha, time / duration);
            yield return null;
        }
        ui.alpha = targetAlpha;
        ui.interactable = true;
    }

}


static public class Star
{
    static public bool[] stageStar = new bool[9];
    static public int[] correctness = new int[3];
    static public bool[] noRecord = new bool[3];

    /*
    static public void Reset(int[] stageCorrect)
    {
        for (int i = 0; i < 3; i++)
        {
            if (stageCorrect[i] == -1)
            {
                noRecord[i] = true;
            }
            else
            {
                correctness[i] = stageCorrect[i] * 100 / 11;
                SetStar(i + 1, correctness[i]);
            }
        }
    }
    */

    static public void SetStar(int stage, int stageCorrect)
    {
        int idx = 3 * (stage - 1);
        if (stageCorrect == -1)
        {
            noRecord[stage - 1] = true;
        }
        else
        {
            noRecord[stage - 1] = false;
            correctness[stage - 1] = stageCorrect * 100 / 11;
            stageStar[idx++] = correctness[stage - 1] > 0;
            stageStar[idx++] = correctness[stage - 1] >= 50;
            stageStar[idx] = correctness[stage - 1] == 100;
        }
    }
    static public bool GetStar(int stage, int starNumber)
    {
        return stageStar[3 * (stage - 1) + starNumber];
    }
}