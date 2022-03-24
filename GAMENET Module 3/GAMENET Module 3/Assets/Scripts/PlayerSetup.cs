using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public Camera Camera;

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

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
