using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pause : MonoBehaviour
{
    // 다른 스크립트에서 쉽게 접근이 가능하도록 static
    public static bool GameIsPaused = false;
    public GameObject pauseMenuCanvas;
    public GameObject panel;
    public GameObject SoundSet;
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    if (GameIsPaused && pauseMenuCanvas.activeSelf)
        //    {
        //        Resume();
        //    }
        //    else if (!GameIsPaused)
        //    {
        //        Pause();
        //    }
        //}
    }

    public void Resume()
    {
        pauseMenuCanvas.SetActive(false);
        panel.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenuCanvas.SetActive(true);
        panel.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void ToSettingMenu()
    {
        SoundSet.SetActive(true);
        pauseMenuCanvas.SetActive(false);
    }
    public void BackBtn()
    {
        SoundSet.SetActive(false);
        pauseMenuCanvas.SetActive(true);
    }

    public void ToMain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
