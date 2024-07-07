using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static mainManager;
using UnityEngine.SceneManagement;

public class BtnType : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Start is called before the first frame update
    public BTNType curBtnType;
    public Transform buttonScale;
    public CanvasGroup mainGroup;
    //public CanvasGroup stageGroup;
    public CanvasGroup soundGroup;
    AudioManager audiomanager;
    //public int stage;
    //public int Map;
    Vector3 defaultScale;
    bool isSound;
    public void Awake()
    {
        audiomanager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        defaultScale = buttonScale.localScale;
       
    }
    public void OnBtnClick()
    {
        //SoundManager.instance.SFXPlay("Click", clip);
        audiomanager.PlaySFX(audiomanager.click);
        switch (curBtnType)
        {
            case BTNType.New:
                SceneManager.LoadScene("");
                break;
            case BTNType.Option:
                CanvasGroupOn(soundGroup);
                CanvasGroupOff(mainGroup);
                break;
            case BTNType.Quit:
                Application.Quit();
                break;
            case BTNType.Back:
                CanvasGroupOn(mainGroup);
                CanvasGroupOff(soundGroup);
                break;

        }
    }

    public void CanvasGroupOn(CanvasGroup cg)
    {
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }
    public void CanvasGroupOff(CanvasGroup cg)
    {
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {

        buttonScale.localScale = defaultScale * 1.2f;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonScale.localScale = defaultScale;
    }
}
