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

    [Header("Room List Panel")]
    public GameObject RoomListPanel;
    public GameObject RoomItemPrefab;
    public GameObject RoomListParent;

    private Dictionary<string, RoomInfo> cachedRoomList;        // key is the string (room name) and the value stored is the room info

    #region Unity Functions
    // Start is called before the first frame update
    void Start()
    {
        cachedRoomList = new Dictionary<string, RoomInfo>();
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

    public void OnCancelButtonClicked()
    {
        ActivatePanel(GameOptionsPanel);
    }

    public void OnShowRoomListButtonClicked()
    {
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();
        ActivatePanel(ShowRoomListPanel);
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

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(RoomInfo info in roomList)
        {
            Debug.Log(info.Name);

            // Check room info first...
            if(!info.IsOpen || !info.IsVisible || info.RemovedFromList)     // if room info is not open, not visible or removed from list (b/c it's full)
            {
                if(cachedRoomList.ContainsKey(info.Name))   // if dictionary already contains the info
                {
                    cachedRoomList.Remove(info.Name);       // remove from dictionary
                }
            }
            else
            {
                //Update existing room info if it already exists:
                if(cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList[info.Name] = info;
                }
                else
                {
                    cachedRoomList.Add(info.Name, info);
                }
            }
        }

        // Cached room list should now have updated room list
        foreach(RoomInfo info in cachedRoomList.Values)
        {
            GameObject listItem = Instantiate(RoomItemPrefab);          // instantiates list item prefab
            listItem.transform.SetParent(RoomListParent.transform);     // sets it as a child of the list parent
            listItem.transform.localScale = Vector3.one;                // ensures no scaling issues
            listItem.transform.Find("RoomNameText").GetComponent<Text>().text = info.Name;      // set room name text
            listItem.transform.Find("RoomPlayersText").GetComponent<Text>().text = "Player count: " + info.PlayerCount + "/" + info.MaxPlayers;
        }
    }
    #endregion

    #region Private Methods

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
        RoomListPanel.gameObject.SetActive(panelToBeActivated.Equals(RoomListPanel));
    }
    #endregion
}
