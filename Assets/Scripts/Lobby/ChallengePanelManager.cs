using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengePanelManager : MonoBehaviour
{
    public GameObject challengePanel;

    // Start is called before the first frame update
    void Start()
    {
        challengePanel.SetActive(false);
    }

    public void ToggleChallengePanel()
    {
        challengePanel.SetActive(!challengePanel.activeSelf);
    }
}
