using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class DeathRaceGameManager : MonoBehaviour
{
    public static DeathRaceGameManager Instance = null;

    public GameObject[] VehiclePrefabs;
    public Transform[] StartingPositions;
    public TextMeshProUGUI TimerTMP;

    public GameObject[] DeathTextUI;
    public int PlayerCount = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            object playerSelectionNumber;

            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                Debug.Log((int)playerSelectionNumber);
                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                Vector3 startingPosition = StartingPositions[actorNumber - 1].position;
                PhotonNetwork.Instantiate(VehiclePrefabs[(int)playerSelectionNumber].name, startingPosition, Quaternion.identity);
                PlayerCount = (int)playerSelectionNumber;
            }
        }

        foreach (GameObject go in DeathTextUI)
        {
            go.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
