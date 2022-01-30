using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    public void Update()
    {
        if (Input.GetKey(KeyCode.P) || Input.GetKey(KeyCode.Escape))
        {
            Pause();
        }
        if (Input.GetKey(KeyCode.Space))
        {
            Resume();
        }
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    // public void Home(int sceneID)
    // {
    //     Time.timeScale = 1f;
    //     SceneManager.LoadScene(sceneID);
    // }
}
