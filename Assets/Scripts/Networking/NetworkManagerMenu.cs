using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;
using TMPro;
using Mirror.FizzySteam;
using Steamworks;

public class NetworkManagerMenu : NetworkManager
{
    public static NetworkManagerMenu instance;

    public GameObject networkManagerPanel;
    public GameObject waitPanel;
    [SerializeField]
    TextMeshProUGUI waitStatuts;

    [SerializeField]
    InputField serverIp;
    [SerializeField]
    Dropdown transportSelect;
    [SerializeField]
    GameObject loadingIcon;
    [SerializeField]
    GameObject closeWaitPanelButton;

    [SerializeField]
    FizzySteamyMirror steamTransport;
    [SerializeField]
    TelepathyTransport telepathyTransport;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        serverIp.text = PlayerPrefs.GetString("ServerIp");
    }

    void PrintAllFriends()
    {
        for (int i = 0; i < SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagAll); i++)
        {
            print(SteamFriends.GetFriendPersonaName(SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagAll)) + " " + SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagAll).m_SteamID);
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
            PrintAllFriends();
    }

    public void ConnectToServer()
    {
        SetAdressAndShowWaitPanel();
        StartClient();
    }

    public void HostServer()
    {
        SetAdressAndShowWaitPanel();
        StartHost();
    }

    public void SetTransport()
    {
        int id = transportSelect.value;

        if (id == 0)
            SetTransportToTelepathy();
        else if (id == 1)
            SetTransportToSteam();
    }

    void SetTransportToSteam()
    {
        steamTransport.enabled = true;
        telepathyTransport.enabled = false;
        transport = steamTransport;

        serverIp.text = steamTransport.SteamUserID.ToString();
    }

    void SetTransportToTelepathy()
    {
        telepathyTransport.enabled = true;
        steamTransport.enabled = false;
        transport = telepathyTransport;

        serverIp.text = PlayerPrefs.GetString("ServerIp");
    }

    void SetAdressAndShowWaitPanel()
    {
        networkAddress = serverIp.text;
        waitPanel.SetActive(true);
        waitStatuts.text = "Connecting to " + networkAddress + " ...";
        ToggleLoadingPanelButton(false);

        PlayerPrefs.SetString("ServerIp", serverIp.text);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        waitPanel.SetActive(false);
        networkManagerPanel.SetActive(false);
        GameManager.instance.weaponSelectionMenu.SetActive(true);
    }

    /*public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        base.OnClientError(conn, errorCode);
        waitStatuts.text = "Error " + errorCode + " ...";
        ToggleLoadingPanelButton(true);
    }*/

    void ToggleLoadingPanelButton(bool show)
    {
        closeWaitPanelButton.SetActive(show);
        loadingIcon.SetActive(!show);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        Destroy(this.gameObject);
    }

}
