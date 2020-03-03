using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using Mirror;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<ConnectedPlayer> connectedPlayers = new List<ConnectedPlayer>();
    List<GameObject> spawnedEquipButtons = new List<GameObject>();

    public bool isInWeaponSelection = true;
    public bool isInOnlineMultiplayer = false;

    [SerializeField]
    GameObject[] playerWeaponSelectUI;
    [SerializeField]
    public GameObject[] playerHealthUI;
    [SerializeField]
    GameObject[] playerWeaponSelectIndicators;
    [SerializeField]
    public GameObject weaponSelectionMenu;
    [SerializeField]
    Transform weaponEquipButtonsParent;
    [SerializeField]
    GameObject readyToStartGameobject;
    GameObject firstWeaponSelected;
    [SerializeField]
    Text numberOfLivesThisGameDisplay;

    PlayerInputManager playerInputManager;
    [SerializeField]
    public WeaponDatabase weaponDatabase;
    [SerializeField]
    GameObject weaponEquipButtonPrefab;
    [SerializeField]
    InputActionAsset playerInputActions;
    public int numberOfLivesThisGame = 3;

    private void Awake()
    {
        instance = this;


        if (PlayerPrefs.GetString("online") == "true")
            isInOnlineMultiplayer = true;
        else
            isInOnlineMultiplayer = false;
    }

    private void Start()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        SpawnAllWeaponButtons();

        if (PlayerPrefs.HasKey("NumberOfLives"))
            numberOfLivesThisGame = PlayerPrefs.GetInt("NumberOfLives");
        numberOfLivesThisGameDisplay.text = numberOfLivesThisGame.ToString();
    }

    private void Update()
    {
        if (isInWeaponSelection)
        {
            for (int i = 0; i < connectedPlayers.Count; i++)
            {
                if (connectedPlayers[i].playerObject.GetComponentInChildren<MultiplayerEventSystem>().currentSelectedGameObject != null)
                {
                    playerWeaponSelectIndicators[i].transform.position = Vector2.Lerp(playerWeaponSelectIndicators[i].transform.position, connectedPlayers[i].playerObject.GetComponentInChildren<MultiplayerEventSystem>().currentSelectedGameObject.transform.position, 15f * Time.deltaTime);
                    playerWeaponSelectIndicators[i].transform.localScale = connectedPlayers[i].playerObject.GetComponentInChildren<MultiplayerEventSystem>().currentSelectedGameObject.transform.localScale;
                }
            }

            if (AllPlayersReady())
            {
                readyToStartGameobject.SetActive(true);
                if (playerInputActions.FindAction("START").phase == InputActionPhase.Started)
                    CloseWeaponSelectionMenu();
            }
            else if (readyToStartGameobject.activeSelf == true && !AllPlayersReady())
                readyToStartGameobject.SetActive(false);
        }
    }

    void OnPlayerJoined(PlayerInput _input)
    {
        playerWeaponSelectUI[connectedPlayers.Count].SetActive(true);
        playerHealthUI[connectedPlayers.Count].SetActive(true);
        playerWeaponSelectIndicators[connectedPlayers.Count].SetActive(true);

        ConnectedPlayer newPlayer = new ConnectedPlayer();
        newPlayer.playerObject = _input.gameObject;

        connectedPlayers.Add(newPlayer);

        newPlayer.playerObject.name = "Player " + connectedPlayers.Count;
        _input.gameObject.GetComponentInChildren<MultiplayerEventSystem>().SetSelectedGameObject(firstWeaponSelected);
        SetPlayerColor(_input.gameObject);
        _input.transform.position = SpawnPointsManager.instance.GetPlayerRespawnPosition(connectedPlayers.Count - 1);
        _input.gameObject.GetComponent<PlayerWeapon>().EquipWeapon(1);
    }

    void SpawnAllWeaponButtons()
    {
        for (int i = 1; i < weaponDatabase.allWeapons.Count; i++)
        {
            EquipWeaponSlotDisplay slotDisplay = Instantiate(weaponEquipButtonPrefab, weaponEquipButtonsParent).GetComponent<EquipWeaponSlotDisplay>();
            slotDisplay.SetWeaponId(i);
            spawnedEquipButtons.Add(slotDisplay.gameObject);
            if (i == 1)
                firstWeaponSelected = slotDisplay.gameObject;
        }
    }

    public void SetPlayerEquipWeaponDisplay(int playerId, int weaponId)
    {
        playerWeaponSelectUI[playerId].transform.GetChild(1).GetComponent<Image>().sprite = weaponDatabase.allWeapons[weaponId].weaponIcon;
        playerWeaponSelectUI[playerId].transform.GetChild(2).GetComponent<Text>().text = weaponDatabase.allWeapons[weaponId].weaponName;
        playerHealthUI[playerId].transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = weaponDatabase.allWeapons[weaponId].weaponIcon;

        connectedPlayers[playerId].equipedWeaponId = weaponId;
        if (connectedPlayers[playerId].playerObject.GetComponentInChildren<MultiplayerEventSystem>().alreadySelecting == false)
            connectedPlayers[playerId].playerObject.GetComponentInChildren<MultiplayerEventSystem>().SetSelectedGameObject(spawnedEquipButtons[weaponId - 1]);
    }

    public int GetPlayerId(GameObject playerObject)
    {
        for (int i = 0; i < connectedPlayers.Count; i++)
        {
            if (connectedPlayers[i].playerObject == playerObject)
                return i;
        }

        return -1;
    }

    bool AllPlayersReady()
    {
        bool output = true;

        for (int i = 0; i < connectedPlayers.Count; i++)
        {
            if (connectedPlayers[i].ready == false)
                output = false;
        }

        if (connectedPlayers.Count == 0)
            output = false;

        return output;
    }

    public void ChangePlayerReadyState(GameObject player, bool forceUnready = false)
    {
        int playerId = GetPlayerId(player);
        if (forceUnready == false)
            connectedPlayers[playerId].ready = !connectedPlayers[playerId].ready;
        else
            connectedPlayers[playerId].ready = false;

        RefreshReadyStateDisplay(player);

        if (isInOnlineMultiplayer)
            NetworkPlayer.localPlayer.SetSyncListElementToServer(connectedPlayers[playerId], playerId);
    }

    public void RefreshReadyStateDisplay(GameObject player)
    {
        int playerId = GetPlayerId(player);

        string statuts = "Press Enter When Ready";
        if (connectedPlayers[playerId].ready)
            statuts = "<color=#49B581>Ready</color>";
        playerWeaponSelectUI[playerId].transform.GetChild(3).GetComponent<Text>().text = statuts;
    }

    /* void RefreshCurrentSelectedUiPos(int playerId)
     {
         connectedPlayers[playerId].currentSelectedUIElementPos = connectedPlayers[playerId].playerObject.GetComponentInChildren<MultiplayerEventSystem>().currentSelectedGameObject.transform.position;
     }*/

    public void CloseWeaponSelectionMenu()
    {
        if (isInWeaponSelection)
        {
            isInWeaponSelection = false;
            weaponSelectionMenu.SetActive(false);
            Spawner.instance.BeginSpawning();
            GetComponent<PlayerInputManager>().joinBehavior = PlayerJoinBehavior.JoinPlayersManually;

            for (int i = 0; i < connectedPlayers.Count; i++)
            {
                connectedPlayers[i].playerObject.GetComponent<PlayerHealth>().SetNumberOfLives();
                playerWeaponSelectIndicators[i].SetActive(false);
            }

            if (isInOnlineMultiplayer)
                NetworkPlayer.localPlayer.CmdStartMatch();
        }
    }

    void SetPlayerColor(GameObject player)
    {
        int playerId = GetPlayerId(player);
        player.GetComponent<SpriteRenderer>().material.SetColor("NewColor1", CustomFunctions.PlayerIdToColor(playerId));
        //player.GetComponent<SpriteRenderer>().material.SetColor("NewColor1", Color.white);
        player.GetComponent<SpriteRenderer>().material.SetColor("NewColor2", CustomFunctions.DarkColor(CustomFunctions.PlayerIdToColor(playerId)));
    }

    public WeaponInfo GetPlayerWeapon(GameObject _player)
    {
        return weaponDatabase.allWeapons[_player.GetComponent<PlayerWeapon>().currentWeapon];
    }

    public void ChangeNumberOfLivesThisGame(int add)
    {
        if ((add > 0 && numberOfLivesThisGame == 9) || (add < 0 && numberOfLivesThisGame == 1))
            return;

        numberOfLivesThisGame += add;
        RefreshNumberOfLivesDisplay();
    }

    public void RefreshNumberOfLivesDisplay()
    {
        numberOfLivesThisGameDisplay.text = numberOfLivesThisGame.ToString();
        PlayerPrefs.SetInt("NumberOfLives", numberOfLivesThisGame);
    }

    bool AllPlayersAreDead()
    {
        bool output = true;

        for (int i = 0; i < connectedPlayers.Count; i++)
        {
            if (connectedPlayers[i].playerObject.activeSelf == true)
                output = false;
        }

        return output;
    }

    float NumberOfDeadPlayers()
    {
        float output = 0;

        for (int i = 0; i < connectedPlayers.Count; i++)
        {
            if (connectedPlayers[i].playerObject.activeSelf == false)
                output++;
        }

        return output;
    }

    public void ShowResultScreenIfNecessary()
    {
        if ((AllPlayersAreDead() && Spawner.instance.isInZombieMode == true) || (NumberOfDeadPlayers() == connectedPlayers.Count - 1 && Spawner.instance.isInZombieMode == false && connectedPlayers.Count > 1))
        {
            for (int i = 0; i < connectedPlayers.Count; i++)
            {
                connectedPlayers[i].playerObject.SetActive(false);
            }
            ResultScreenManager.instance.ShowResults();
        }
    }

    public void SetAllPlayerRespawnPosition()
    {
        for (int i = 0; i < connectedPlayers.Count; i++)
        {
            connectedPlayers[i].playerObject.transform.position = SpawnPointsManager.instance.GetPlayerRespawnPosition(i);
        }
    }
}

[System.Serializable]
public class ConnectedPlayer
{
    public GameObject playerObject;

    public bool ready = false;

    public int numberOfDeaths;
    public int numberOfKills;

    public int equipedWeaponId;

    /*public ConnectedPlayer(GameObject playerObject)
    {
        this.playerObject = playerObject;
        //this.eventSystem = playerObject.GetComponentInChildren<MultiplayerEventSystem>();
    }*/
}
