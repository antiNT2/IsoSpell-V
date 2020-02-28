using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultScreenManager : MonoBehaviour
{
    [SerializeField]
    List<PlayerResultDisplay> playersResultDisplay;
    [SerializeField]
    GameObject resultPanel;

    public static ResultScreenManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void ShowResults()
    {
        resultPanel.SetActive(true);

        for (int i = 0; i < GameManager.instance.connectedPlayers.Count; i++)
        {
            ConnectedPlayer player = GameManager.instance.connectedPlayers[i];
            playersResultDisplay[i].gameObject.SetActive(true);
            playersResultDisplay[i].SetStatsDisplay(player.numberOfKills, player.numberOfDeaths);
        }
    }
}
