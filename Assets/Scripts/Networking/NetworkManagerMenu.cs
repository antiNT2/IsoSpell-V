using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;
using TMPro;

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
    GameObject loadingIcon;
    [SerializeField]
    GameObject closeWaitPanelButton;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        serverIp.text = PlayerPrefs.GetString("ServerIp");
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
