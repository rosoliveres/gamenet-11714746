using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Unit : MonoBehaviourPunCallbacks
{
    public string UnitName;
    // Start is called before the first frame update
    void Start()
    {
        UnitName = photonView.Owner.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
