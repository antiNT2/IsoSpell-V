using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<ConnectedPlayer> connectedPlayers = new List<ConnectedPlayer>();

    public bool isInWeaponSelection = true;
    [SerializeField]
    GameObject[] playerWeaponSelectUI;
    [SerializeField]
    public GameObject[] playerHealthUI;
    [SerializeField]
    GameObject[] playerWeaponSelectIndicators;
    [SerializeField]
    GameObject weaponSelectionMenu;
    [SerializeField]
    Transform weaponEquipButtonsParent;
    [SerializeField]
    GameObject readyToStartGameobject;
    GameObject firstWeaponSelected;

    PlayerInputManager playerInputManager;
    [SerializeField]
    public WeaponDatabase weaponDatabase;
    [SerializeField]
    GameObject weaponEquipButtonPrefab;
    [SerializeField]
    InputActionAsset playerInputActions;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Confined;
        playerInputManager = GetComponent<PlayerInputManager>();
        SpawnAllWeaponButtons();
    }

    private void Update()
    {
        if (isInWeaponSelection)
        {
            for (int i = 0; i < connectedPlayers.Count; i++)
            {
                if (connectedPlayers[i].eventSystem.currentSelectedGameObject != null)
                    playerWeaponSelectIndicators[i].transform.position = Vector2.Lerp(playerWeaponSelectIndicators[i].transform.position, connectedPlayers[i].eventSystem.currentSelectedGameObject.transform.position, 15f * Time.deltaTime);
                //playerWeaponSelectIndicators[i].transform.position = connectedPlayers[i].eventSystem.currentSelectedGameObject.transform.position;
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

        if (Input.GetKeyDown(KeyCode.O))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void OnPlayerJoined(PlayerInput _input)
    {
        playerWeaponSelectUI[connectedPlayers.Count].SetActive(true);
        playerHealthUI[connectedPlayers.Count].SetActive(true);
        playerWeaponSelectIndicators[connectedPlayers.Count].SetActive(true);
        connectedPlayers.Add(new ConnectedPlayer(_input.gameObject));
        _input.gameObject.GetComponentInChildren<MultiplayerEventSystem>().SetSelectedGameObject(firstWeaponSelected);
        SetPlayerColor(_input.gameObject);
    }

    void SpawnAllWeaponButtons()
    {
        for (int i = 1; i < weaponDatabase.allWeapons.Count; i++)
        {
            EquipWeaponSlotDisplay slotDisplay = Instantiate(weaponEquipButtonPrefab, weaponEquipButtonsParent).GetComponent<EquipWeaponSlotDisplay>();
            slotDisplay.SetWeaponId(i);
            if (i == 1)
                firstWeaponSelected = slotDisplay.gameObject;
        }
    }

    public void SetPlayerEquipWeaponDisplay(int playerId, int weaponId)
    {
        playerWeaponSelectUI[playerId].transform.GetChild(1).GetComponent<Image>().sprite = weaponDatabase.allWeapons[weaponId].weaponIcon;
        playerWeaponSelectUI[playerId].transform.GetChild(2).GetComponent<Text>().text = weaponDatabase.allWeapons[weaponId].weaponName;
        playerHealthUI[playerId].transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = weaponDatabase.allWeapons[weaponId].weaponIcon;
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

        string statuts = "Press Enter When Ready";
        if (connectedPlayers[playerId].ready)
            statuts = "<color=#49B581>Ready</color>";
        playerWeaponSelectUI[playerId].transform.GetChild(3).GetComponent<Text>().text = statuts;
    }

    void CloseWeaponSelectionMenu()
    {
        if (isInWeaponSelection)
        {
            isInWeaponSelection = false;
            weaponSelectionMenu.SetActive(false);
        }
    }

    void SetPlayerColor(GameObject player)
    {
        int playerId = GetPlayerId(player);
        player.GetComponent<SpriteRenderer>().material.SetColor("NewColor1", CustomFunctions.PlayerIdToColor(playerId));
        //player.GetComponent<SpriteRenderer>().material.SetColor("NewColor1", Color.white);
        player.GetComponent<SpriteRenderer>().material.SetColor("NewColor2", CustomFunctions.DarkColor(CustomFunctions.PlayerIdToColor(playerId)));
    }
}

public class ConnectedPlayer
{
    public GameObject playerObject;
    public MultiplayerEventSystem eventSystem;
    public bool ready = false;

    public ConnectedPlayer(GameObject playerObject)
    {
        this.playerObject = playerObject;
        this.eventSystem = playerObject.GetComponentInChildren<MultiplayerEventSystem>();
    }
}
