using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;

public abstract class Shooting : MonoBehaviourPunCallbacks
{
    public enum RaiseEventsCode
    {
        WhoDiedEventCode = 0
    }

    public bool CanShoot;
    public Camera Camera;
    public Health Health;
    public Transform GunPoint;
    protected float damage = 20f;
    protected float fireRate = 2f;
    protected float fireCountdown = 0f;
    private bool isDead = false;
    [SerializeField] protected Unit initiator;

    private int dieOrder = 0; // needed?

    protected void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnDiedEvent;
    }

    protected void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnDiedEvent;
    }

    protected void OnDiedEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)RaiseEventsCode.WhoDiedEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;

            string nicknameOfFinishedPlayer = (string)data[0];
            dieOrder = (int)data[1];
            int viewId = (int)data[2];

            Debug.Log(nicknameOfFinishedPlayer + " " + dieOrder);

            GameObject orderUiText = DeathRaceGameManager.Instance.DeathTextUI[dieOrder - 1];
            orderUiText.SetActive(true);

            if (viewId == photonView.ViewID) // this is you
            {
                orderUiText.GetComponent<TextMeshProUGUI>().text = dieOrder + " " + nicknameOfFinishedPlayer;
            }
            else
            {
                orderUiText.GetComponent<TextMeshProUGUI>().text = dieOrder + " " + nicknameOfFinishedPlayer + "(YOU)"; // the one who calls "OnDie" is the target, not you
                orderUiText.GetComponent<TextMeshProUGUI>().color = Color.red;
            }

            DeathRaceGameManager.Instance.PlayerCount--;

            if (DeathRaceGameManager.Instance.PlayerCount <= 0)
            {
                DeathRaceGameManager.Instance.DeathTextUI[2].SetActive(true);
                DeathRaceGameManager.Instance.DeathTextUI[2].GetComponent<TextMeshProUGUI>().text = "Winner: " + initiator.UnitName;
            }

        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Health = this.GetComponent<Health>();
        initiator = this.GetComponent<Unit>();
        CanShoot = false;
    }

    protected virtual void Update()
    {
        if (!CanShoot) return;
        
    }

    protected abstract void Fire();

    protected void Attack(GameObject target, Unit initiator)
    {
        Debug.Log("Initiator = " + initiator.UnitName);
        target.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, damage);
        if (target.GetComponent<Shooting>().isDead)
        {
            //photonView.RPC("OnKill", RpcTarget.AllBuffered, initiator.UnitName, target.GetComponent<Unit>().UnitName);
        }
        target.GetComponent<Shooting>().isDead = false;
    }

    [PunRPC]
    protected void TakeDamage(float damage, PhotonMessageInfo info)
    {
        Health.CurrentHealth -= damage;
        Health.SetHealthBarFillAmount();

        if (Health.CurrentHealth <= 0)
        {
            Die();
            isDead = true;
            Debug.Log(info.Sender.NickName + " killed " + info.photonView.Owner.NickName);

        }
    }

    protected void Die()
    {
        if (photonView.IsMine)
        {
            OnDie();
        }
    }

    public void OnDie()
    {
        GetComponent<PlayerSetup>().Camera.transform.parent = null;
        GetComponent<VehicleMovement>().enabled = false;

        dieOrder++;
        string nickName = photonView.Owner.NickName;
        int viewID = photonView.ViewID;

        object[] data = new object[] { nickName, dieOrder, viewID };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = false
        };

        PhotonNetwork.RaiseEvent((byte)RaiseEventsCode.WhoDiedEventCode, data, raiseEventOptions, sendOptions);
    }
}
