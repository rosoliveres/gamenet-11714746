using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Unit : MonoBehaviourPunCallbacks
{
    public string unitName = "";
    public GameObject WinPanel;
    public GameObject KillPanel;
    public RectTransform KillPanelParent;

    public int KillCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        unitName = photonView.Owner.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
