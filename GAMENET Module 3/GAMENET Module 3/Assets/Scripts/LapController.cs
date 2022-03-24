using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;

public class LapController : MonoBehaviourPunCallbacks
{
    public List<GameObject> LapTriggers = new List<GameObject>();
    public enum RaiseEventsCode
    {
        WhoFinishedEventCode = 0
    }

    private int finishOrder = 0;

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code == (byte)RaiseEventsCode.WhoFinishedEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;

            string nicknameOfFinishedPlayer = (string)data[0];
            finishOrder = (int)data[1];
            int viewId = (int)data[2];

            Debug.Log(nicknameOfFinishedPlayer + " " + finishOrder);

            GameObject orderUiText = RacingGameManager.instance.FinisherTextUi[finishOrder - 1];
            orderUiText.SetActive(true);

            if(viewId == photonView.ViewID) // this is you
            {
                orderUiText.GetComponent<TextMeshProUGUI>().text = finishOrder + " " + nicknameOfFinishedPlayer + "(YOU)";
                orderUiText.GetComponent<TextMeshProUGUI>().color = Color.green;
            }
            else
            {
                orderUiText.GetComponent<TextMeshProUGUI>().text = finishOrder + " " + nicknameOfFinishedPlayer;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject go in RacingGameManager.instance.LapTriggers)
        {
            LapTriggers.Add(go);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(LapTriggers.Contains(other.gameObject))
        {
            int indexOfTrigger = LapTriggers.IndexOf(other.gameObject); // get the index of the gameobject player collided with
            LapTriggers[indexOfTrigger].SetActive(false);   // set that lap trigger false
        }

        if(other.gameObject.tag == "FinishTrigger")
        {
            GameFinish();
        }
    }

    public void GameFinish()
    {
        GetComponent<PlayerSetup>().Camera.transform.parent = null;
        GetComponent<VehicleMovement>().enabled = false;

        finishOrder++;
        string nickname = photonView.Owner.NickName;
        int viewID = photonView.ViewID;     // so we can see our ranking on Canvas

        // a list of data that will be passed to the event
        object[] data = new object[] { nickname, finishOrder, viewID };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = false
        };

        PhotonNetwork.RaiseEvent((byte)RaiseEventsCode.WhoFinishedEventCode, data, raiseEventOptions, sendOptions);
    }
}
