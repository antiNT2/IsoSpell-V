using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerResultDisplay : MonoBehaviour
{
    [SerializeField]
    Text killCountDisplay;
    [SerializeField]
    Text deathAmountDisplay;

    public void SetStatsDisplay(int killCount, int deathCount)
    {
        killCountDisplay.text = killCount + " kills";
        deathAmountDisplay.text = deathCount + " deaths";
    }
}
