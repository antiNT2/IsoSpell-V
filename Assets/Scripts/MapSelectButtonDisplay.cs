using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MapSelectButtonDisplay : MonoBehaviour
{
    [SerializeField]
    Image mapIcon;
    [SerializeField]
    TextMeshProUGUI mapNameDisplay;
    public int mapId;

    public void SetMapDisplay(int id)
    {
        mapId = id;
        MapData mapToDisplay = MapManager.instance.allMaps[id];

        mapIcon.sprite = mapToDisplay.mapIcon;
        mapNameDisplay.text = mapToDisplay.mapName;
    }

    public void LoadMap()
    {
        MapData currentDisplayedMap = MapManager.instance.allMaps[mapId];
        SceneManager.LoadScene(currentDisplayedMap.mapScene, LoadSceneMode.Additive);
        MapManager.instance.CloseMapSelectionPanel();
    }
}
