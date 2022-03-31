using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TurretControl : MonoBehaviourPunCallbacks
{
    public Vector2 Turn;
    public bool IsControlEnabled;

    // Start is called before the first frame update
    void Start()
    {
        IsControlEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsControlEnabled) return;
        if (!photonView.IsMine) return;
        Turn.x += Input.GetAxis("Mouse X");
        Turn.y += Input.GetAxis("Mouse Y");
        transform.localRotation = Quaternion.Euler(-Turn.y, Turn.x, 0);
    }
}
