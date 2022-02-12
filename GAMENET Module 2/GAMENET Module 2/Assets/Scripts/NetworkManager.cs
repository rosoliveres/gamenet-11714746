using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Connection Status Panel")]
    public Text ConnectionStatusText;

    [Header("Login UI Panel")]
    public InputField PlayerNameInput;
    public GameObject LoginUiPanel;

    [Header("Game Options Panel")]
    public GameObject GameOptionsPanel;

    [Header("Create Room Panel")]
    public GameObject CreateRoomPanel;
    public InputField roomNameInputField;
    public InputField playerCountInputField;

    [Header("Join Random Room Panel")]
    public GameObject JoinRandomRoomPanel;

    [Header("Show Room List Panel")]
    public GameObject ShowRoomListPanel;

    [Header("Inside Room Panel")]
    public GameObject InsideRoomPanel;

    #region Unity Functions
    // Start is called before the first frame update
    void Start()
    {
        ActivatePanel(LoginUiPanel);
    }

    // Update is called once per frame
    void Update()
    {
        ConnectionStatusText.text = "Connection Status: " + PhotonNetwork.NetworkClientState;   // provides info on how we are connected to photon server
    }
    #endregion

    #region UI Callbacks
    public void OnLoginButtonClicked()
    {
        string playerName = PlayerNameInput.text;

        if(string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("Player name is invalid!");
        }
        else
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void OnCreateRoomButtonClicked()
    {
        string roomName = roomNameInputField.text;
        if(string.IsNullOrEmpty(roomName))
        {
            roomName = "Room " + Random.Range(1000, 10000);
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)int.Parse(playerCountInputField.text);

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }
    #endregion

    #region PUN Callbacks
    public override void OnConnected()
    {
        Debug.Log("Connected to the internet.");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has connected to Photon Servers");
        ActivatePanel(GameOptionsPanel);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " created.");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has joined " + PhotonNetwork.CurrentRoom.Name);
        ActivatePanel(InsideRoomPanel);
    }
    #endregion

    #region Public Methods

    public void ActivatePanel(GameObject panelToBeActivated)
    {
        LoginUiPanel.gameObject.SetActive(panelToBeActivated.Equals(LoginUiPanel));
        GameOptionsPanel.gameObject.SetActive(panelToBeActivated.Equals(GameOptionsPanel));
        CreateRoomPanel.gameObject.SetActive(panelToBeActivated.Equals(CreateRoomPanel));
        JoinRandomRoomPanel.gameObject.SetActive(panelToBeActivated.Equals(JoinRandomRoomPanel));
        ShowRoomListPanel.gameObject.SetActive(panelToBeActivated.Equals(ShowRoomListPanel));
        InsideRoomPanel.gameObject.SetActive(panelToBeActivated.Equals(InsideRoomPanel));
    }
    #endregion
}
