using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleScreen : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI descriptionDisplay;
    [SerializeField]
    Text versionDisplay;

    private void Start()
    {
        versionDisplay.text = "v" + Application.version;
    }

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
        else if (id == 3)
        {
            descriptionDisplay.text = "Yeah online haha";
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
