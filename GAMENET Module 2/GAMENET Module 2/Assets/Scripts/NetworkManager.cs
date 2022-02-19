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
    public Text RoomInfoText;
    public GameObject PlayerListItemPrefab;
    public GameObject PlayerListViewParent;
    public GameObject StartGameButton;

    [Header("Room List Panel")]
    public GameObject RoomListPanel;
    public GameObject RoomItemPrefab;
    public GameObject RoomListParent;

    private Dictionary<string, RoomInfo> cachedRoomList;        // key is the string (room name) and the value stored is the room info
    private Dictionary<string, GameObject> roomListGameObjects;
    private Dictionary<int, GameObject> playerListGameObjects;

    #region Unity Functions
    // Start is called before the first frame update
    void Start()
    {
        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListGameObjects = new Dictionary<string, GameObject>();
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

    public void OnBackButtonClicked()
    {
        if(PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        ActivatePanel(GameOptionsPanel);
    }

    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();  // all assigned callbacks will be called (in this case, our OnLeftRoom override will be called)
    }

    public void OnJoinRandomRoomClicked()
    {
        ActivatePanel(JoinRandomRoomPanel);
        PhotonNetwork.JoinRandomRoom();
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

        RoomInfoText.text = $"Room name: {PhotonNetwork.CurrentRoom.Name} Current Player Count: {PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}";
        
        // Initialize player GO dict here
        if(playerListGameObjects == null)
        {
            playerListGameObjects = new Dictionary<int, GameObject>();
        }

        // "Player" is defined by Photon
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerItem = Instantiate(PlayerListItemPrefab);
            playerItem.transform.SetParent(PlayerListViewParent.transform);
            playerItem.transform.localScale = Vector3.one;
            playerItem.transform.Find("PlayerNameText").GetComponent<Text>().text = player.NickName;
            
            playerItem.transform.Find("PlayerIndicator").gameObject.SetActive(player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);

            playerListGameObjects.Add(player.ActorNumber, playerItem);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListGameObjects();             // removes every GO and entry in the room list GO dictionary to prep for adding new ones
        StartGameButton.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient); // start button only activates if you are the host (master client)
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
            listItem.transform.Find("JoinRoomButton").GetComponent<Button>().onClick.AddListener(() => OnJoinedRoomClicked(info.Name));

            roomListGameObjects.Add(info.Name, listItem);
        }
    }

    public override void OnLeftLobby()
    {
        ClearRoomListGameObjects();
        cachedRoomList.Clear();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Update player count on room info when player enters
        RoomInfoText.text = $"Room name: {PhotonNetwork.CurrentRoom.Name} Current Player Count: {PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}";

        // Updates player list for all players currently in the room (even the ones that entered earlier)
        GameObject playerItem = Instantiate(PlayerListItemPrefab);
        playerItem.transform.SetParent(PlayerListViewParent.transform);
        playerItem.transform.localScale = Vector3.one;
        playerItem.transform.Find("PlayerNameText").GetComponent<Text>().text = newPlayer.NickName;

        playerItem.transform.Find("PlayerIndicator").gameObject.SetActive(newPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);

        playerListGameObjects.Add(newPlayer.ActorNumber, playerItem);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // When host leaves, Photon assigns next player as host - start button becomes active for that player
        StartGameButton.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient);
        // Update player count when player left room
        RoomInfoText.text = $"Room name: {PhotonNetwork.CurrentRoom.Name} Current Player Count: {PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}";

        Destroy(playerListGameObjects[otherPlayer.ActorNumber]);        // destroys prefab
        playerListGameObjects.Remove(otherPlayer.ActorNumber);          // Removes entry in dict
    }

    public override void OnLeftRoom()
    {
        foreach(var gameObject in playerListGameObjects.Values)
        {
            Destroy(gameObject);
        }
        playerListGameObjects.Clear();
        playerListGameObjects = null;
        ActivatePanel(GameOptionsPanel);
    }
    
    // Gets called when we try to join a random room but there's no rooms available
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning(message);

        // Create a random room
        string roomName = "Room " + Random.Range(1000, 10000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }
    #endregion

    #region Private Methods

    // Since subscribing this to OnClickListener,
    // the corresponding button in the inspector must have an empty list or no null references for On Click() for this to work
    private void OnJoinedRoomClicked(string roomName)
    {
        if(PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        PhotonNetwork.JoinRoom(roomName);
    }

    private void ClearRoomListGameObjects()
    {
        foreach(var item in roomListGameObjects.Values)
        {
            Destroy(item);
        }
        roomListGameObjects.Clear();
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
        RoomListPanel.gameObject.SetActive(panelToBeActivated.Equals(RoomListPanel));
    }
    #endregion
}
