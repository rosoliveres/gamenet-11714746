using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TurretControl : MonoBehaviourPunCallbacks
{
    public Vector2 Turn;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;
        Turn.x += Input.GetAxis("Mouse X");
        Turn.y += Input.GetAxis("Mouse Y");
        transform.localRotation = Quaternion.Euler(-Turn.y, Turn.x, 0);
    }
}
