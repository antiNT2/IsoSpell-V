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
    GameObject joinSteamFriendButton;
    [SerializeField]
    GameObject joinFriendMenu;
    [SerializeField]
    GameObject friendDisplayParent;

    [SerializeField]
    FizzySteamyMirror steamTransport;
    [SerializeField]
    TelepathyTransport telepathyTransport;
    [SerializeField]
    GameObject friendDisplayPrefab;

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

    public void JoinFriend(string steamId)
    {
        serverIp.text = steamId;
        ConnectToServer();
        ToggleJoinFriendMenu(false);
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
        joinSteamFriendButton.SetActive(true);

        serverIp.text = steamTransport.SteamUserID.ToString();
    }

    void SetTransportToTelepathy()
    {
        telepathyTransport.enabled = true;
        steamTransport.enabled = false;
        transport = telepathyTransport;
        joinSteamFriendButton.SetActive(false);

        serverIp.text = PlayerPrefs.GetString("ServerIp");
    }

    public void ToggleJoinFriendMenu(bool show)
    {
        joinFriendMenu.SetActive(show);
        if (show)
            PopulateFriendList();
    }

    void PopulateFriendList()
    {
        for (int i = 0; i < friendDisplayParent.transform.childCount; i++)
        {
            Destroy(friendDisplayParent.transform.GetChild(i).gameObject);
        }

        EFriendFlags flag = EFriendFlags.k_EFriendFlagAll;

        for (int i = 0; i < SteamFriends.GetFriendCount(flag); i++)
        {
            if (SteamFriends.GetFriendPersonaState(SteamFriends.GetFriendByIndex(i, flag)) != EPersonaState.k_EPersonaStateOffline)
            {
                GameObject friendButton = Instantiate(friendDisplayPrefab, friendDisplayParent.transform);
                friendButton.GetComponent<FriendDisplay>().SetFriendName(SteamFriends.GetFriendPersonaName(SteamFriends.GetFriendByIndex(i, flag)), SteamFriends.GetFriendByIndex(i, flag).m_SteamID.ToString());
                //print(SteamFriends.GetFriendPersonaName(SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagAll)) + " " + SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagAll).m_SteamID);
            }
        }
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

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        base.OnClientError(conn, errorCode);
        waitStatuts.text = "Error " + errorCode + " ...";
        ToggleLoadingPanelButton(true);
    }

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
