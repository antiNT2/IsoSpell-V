using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;

    [SerializeField]
    GameObject pausePanel;
    [SerializeField]
    GameObject firstButtonSelected;
    bool isPause = false;

    private void Awake()
    {
        instance = this;
    }

    public void Pause()
    {
        isPause = !isPause;

        pausePanel.SetActive(isPause);
        if (isPause)
        {
            if (GameManager.instance.isInOnlineMultiplayer == false)
                Time.timeScale = 0f;
            EventSystem.current.SetSelectedGameObject(firstButtonSelected);
        }
        else
            Time.timeScale = 1f;
    }

    public void LoadTitleScreen()
    {
        Time.timeScale = 1f;
        if (NetworkManagerMenu.instance.isNetworkActive)
            NetworkManagerMenu.instance.StopHost();
        Destroy(NetworkManagerMenu.instance.gameObject);
        SceneManager.LoadScene(0);
    }

    public void Restarting()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }
}
