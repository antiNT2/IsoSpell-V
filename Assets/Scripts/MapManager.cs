using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MapManager : MonoBehaviour
{
    //public MapDatabase mapDatabase;
    public List<MapData> allMaps = new List<MapData>();
    public static MapManager instance;
    [SerializeField]
    GameObject allMapButtonParent;
    [SerializeField]
    GameObject mapButtonPrefab;
    [SerializeField]
    GameObject mapSelectionPanel;
    public bool isInMapSelection = true;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < allMaps.Count; i++)
        {
            GameObject spawnedMapButton = Instantiate(mapButtonPrefab, allMapButtonParent.transform);
            spawnedMapButton.GetComponent<MapSelectButtonDisplay>().SetMapDisplay(i);

            if (i == 0)
                EventSystem.current.SetSelectedGameObject(spawnedMapButton);
        }
    }

    public void CloseMapSelectionPanel()
    {
        isInMapSelection = false;
        mapSelectionPanel.SetActive(false);
        if (GameManager.instance.isInOnlineMultiplayer == false)
            GetComponent<PlayerInputManager>().joinBehavior = PlayerJoinBehavior.JoinPlayersWhenButtonIsPressed;

        if (GameManager.instance.isInOnlineMultiplayer == false)
            GameManager.instance.weaponSelectionMenu.SetActive(true);
        else
            NetworkManagerMenu.instance.networkManagerPanel.SetActive(true);
    }
}

[CreateAssetMenu()]
public class MapDatabase : ScriptableObject
{
    public List<MapData> allMaps = new List<MapData>();
}



