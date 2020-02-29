using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerResultDisplay : MonoBehaviour
{
    [SerializeField]
    Text killCountDisplay;
    [SerializeField]
    Text deathAmountDisplay;
    [SerializeField]
    TextMeshProUGUI rankDisplay;

    public void SetStatsDisplay(int killCount, int deathCount, int rank)
    {
        killCountDisplay.text = killCount + " kills";
        deathAmountDisplay.text = deathCount + " deaths";
        if (rank != -1)
            rankDisplay.text = "#" + rank;
        else if (rank == -1)
        {
            rankDisplay.text = "WINNER";
            rankDisplay.color = Color.yellow;
        }
    }
}
