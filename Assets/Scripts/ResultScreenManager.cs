using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultScreenManager : MonoBehaviour
{
    [SerializeField]
    List<PlayerResultDisplay> playersResultDisplay;
    [SerializeField]
    GameObject resultPanel;

    /// <summary>
    /// A list of the players ID that died in chronological order (first in the list died first)
    /// </summary>
    public List<int> playersWhoDiedOrder = new List<int>();

    public static ResultScreenManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void ShowResults()
    {
        resultPanel.SetActive(true);
        Time.timeScale = 0;

        for (int i = 0; i < GameManager.instance.connectedPlayers.Count; i++)
        {
            ConnectedPlayer player = GameManager.instance.connectedPlayers[i];
            playersResultDisplay[i].gameObject.SetActive(true);
            playersResultDisplay[i].SetStatsDisplay(player.numberOfKills, player.numberOfDeaths, GetRank(i));
        }
    }

    int GetRank(int playerId)
    {
        for (int i = 0; i < playersWhoDiedOrder.Count; i++)
        {
            if (playersWhoDiedOrder[i] == playerId)
            {
                return GameManager.instance.connectedPlayers.Count - i;
            }
        }

        return -1;
    }
}
