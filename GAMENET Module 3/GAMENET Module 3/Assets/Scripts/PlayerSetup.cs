using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public Camera Camera;
    public TextMeshProUGUI PlayerNameTMP;

    // Start is called before the first frame update
    void Start()
    {
        this.Camera = transform.Find("Camera").GetComponent<Camera>();
        if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
        {
            GetComponent<VehicleMovement>().enabled = photonView.IsMine;        // only enable movement if that car is yours
            GetComponent<LapController>().enabled = photonView.IsMine;
            Camera.enabled = photonView.IsMine;
        }
        else if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
        {
            GetComponent<VehicleMovement>().enabled = photonView.IsMine;        // only enable movement if that car is yours
            PlayerNameTMP.text = photonView.Owner.NickName;
            Camera.enabled = photonView.IsMine;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
