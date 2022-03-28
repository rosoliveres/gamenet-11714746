using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class CountdownManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI TimerTMP;
    public float TimeToStartRace = 5.0f; 

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
        {
            TimerTMP = RacingGameManager.instance.TimerTMP;
        }
        else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
        {
            TimerTMP = DeathRaceGameManager.Instance.TimerTMP;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            if (TimeToStartRace > 0)
            {
                TimeToStartRace -= Time.deltaTime;
                photonView.RPC("SetTime", RpcTarget.AllBuffered, TimeToStartRace);
            }
            else if (TimeToStartRace < 0)
            {
                photonView.RPC("StartRace", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    public void SetTime(float time)
    {
        if(time > 0)
        {
            TimerTMP.text = time.ToString("F1");
        }
        else
        {
            TimerTMP.text = "";
        }
    }

    [PunRPC]
    public void StartRace()
    {
        GetComponent<VehicleMovement>().IsControlEnabled = true;
        this.enabled = false;
    }
}
