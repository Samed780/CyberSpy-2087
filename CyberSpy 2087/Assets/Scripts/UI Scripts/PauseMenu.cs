using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
    public GameObject pauseScreen;

    public static bool isPaused = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    void Pause()
    {
        pauseScreen.SetActive(true);
        isPaused = true;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
    }

    void Resume() 
    {
        pauseScreen.SetActive(false);
        isPaused = false;

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
