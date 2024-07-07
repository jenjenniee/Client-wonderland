using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class optionMenu : MonoBehaviour
{
    // �ٸ� ��ũ��Ʈ���� ���� ������ �����ϵ��� static
    public static bool GameIsPaused = false;
    public GameObject pauseMenuCanvas;
    public GameObject image;
    public GameObject SoundSet;
    //public Player player;
    //AudioManager audiomanager;

    private void Awake()
    {
        //player = player.GetComponent<Player>();
        //audiomanager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    if (GameIsPaused&& pauseMenuCanvas.activeSelf)
        //    {
        //        Resume();
        //    }
        //    else if(!GameIsPaused)
        //    {
        //        Pause();
        //    }
        //}
    }

    public void Resume()
    {
        //audiomanager.PlaySFX(audiomanager.click);
        pauseMenuCanvas.SetActive(false);
        image.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenuCanvas.SetActive(true);
        image.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void ToSettingMenu()
    {
        //audiomanager.PlaySFX(audiomanager.click);
        SoundSet.SetActive(true);
        pauseMenuCanvas.SetActive(false);
    }
    public void BackBtn()
    {
        //audiomanager.PlaySFX(audiomanager.click);
        SoundSet.SetActive(false);
        pauseMenuCanvas.SetActive(true);
    }

    public void ToMain()
    {
        //audiomanager.PlaySFX(audiomanager.click);
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main");
    }

    public void QuitGame()
    {
        //audiomanager.PlaySFX(audiomanager.click);
        Application.Quit();
    }
}
