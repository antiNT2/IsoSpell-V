using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleScreen : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI descriptionDisplay;

    public void SetDescription(int id)
    {
        if(id == -1)
        {
            descriptionDisplay.text = "";
        }else if (id == 0)
        {
            descriptionDisplay.text = "Fight against your imaginary friends in local multiplayer !";
        }
        else if (id == 1)
        {
            descriptionDisplay.text = "Not implemented yet. :)";
        }
        else if (id == 2)
        {
            descriptionDisplay.text = "Don't leave me alone";
        }
    }

    public void LoadPvp()
    {
        PlayerPrefs.SetString("mode", "pvp");
        PlayerPrefs.SetString("online", "false");
        SceneManager.LoadScene(1);
    }

    public void LoadOnlinePvp()
    {
        PlayerPrefs.SetString("mode", "pvp");
        PlayerPrefs.SetString("online", "true");
        SceneManager.LoadScene(1);
    }

    public void LoadZombie()
    {
        PlayerPrefs.SetString("mode", "zombie");
        PlayerPrefs.SetString("online", "false");
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
