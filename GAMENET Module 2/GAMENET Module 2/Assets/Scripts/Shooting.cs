using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.Events;

public class Shooting : MonoBehaviourPunCallbacks
{
    public Camera Camera;
    public GameObject HitEffectPrefab;

    public Health Health;
    private float damage = 25f;
    private bool isDead = false;

    private Animator animator;
    public int KillCount { get; private set; } = 0;
    public int MaxKills { get; private set; } = 10;
    public GameObject KillPanelPrefab;
    public GameObject WinPanelPrefab;

    public UnityEvent<string> EvtOnWin = new UnityEvent<string>();
    public UnityEvent<string, string> EvtOnKill = new UnityEvent<string, string>();

    private Unit initiator;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    public void Fire()
    {
        RaycastHit hit;
        Ray ray = Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out hit, 200))
        {
            Debug.Log(hit.collider.gameObject.name);
            photonView.RPC("CreateHitEffects", RpcTarget.All, hit.point);   // "All" b/c the players just entering dont need to see the particle effects
        }

        // ensure we are hitting a player and not ourselves
        if(hit.collider.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
        {
            // Victim calls the take damage fn
            //hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, damage);    // all current and future players get rpc call
            Attack(hit.collider.gameObject, initiator);
        }
    }

    private void Attack(GameObject target, Unit initiator)
    {
        Debug.Log("Initiator = " + initiator.unitName);
        target.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, damage);
        if(target.GetComponent<Shooting>().isDead)
        {
            photonView.RPC("OnKill", RpcTarget.AllBuffered, initiator.unitName, target.GetComponent<Unit>().unitName);
        }
        target.GetComponent<Shooting>().isDead = false;
    }

    [PunRPC]
    public void TakeDamage(float damage, PhotonMessageInfo info)
    {
        this.Health.CurrentHealth -= damage;
        this.Health.SetHealthBarFillAmount();

        if(this.Health.CurrentHealth <= 0)
        {
            Die();
            //OnKill(info.Sender.NickName, info.photonView.Owner.NickName, killer);
            isDead = true;
            Debug.Log(info.Sender.NickName + " killed " + info.photonView.Owner.NickName);
        }
    }

    [PunRPC]
    public void CreateHitEffects(Vector3 position)
    {
        GameObject hitEffect = Instantiate(HitEffectPrefab, position, Quaternion.identity);
        Destroy(hitEffect, 0.2f);
    }

    public void Die()
    {
        if(photonView.IsMine)
        {
            animator.SetBool("isDead", true);
            StartCoroutine(RespawnCountdown());
        }
    }

    private IEnumerator RespawnCountdown()
    {
        GameObject respawnText = GameObject.Find("RespawnText");
        float respawnTime = 5f;

        while(respawnTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime--;

            transform.GetComponent<PlayerMovementController>().enabled = false;
            respawnText.GetComponent<Text>().text = "You are killed. Respawning in " + respawnTime.ToString(".00");
        }

        animator.SetBool("isDead", false);
        respawnText.GetComponent<Text>().text = "";

        this.transform.position = SpawnPointManager.instance.GetRandomSpawnPt();
        transform.GetComponent<PlayerMovementController>().enabled = true;

        photonView.RPC("ResetHealth", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void ResetHealth()
    {
        Health.CurrentHealth = Health.StartingHealth;
        Health.SetHealthBarFillAmount();
    }

    [PunRPC]
    private void OnKill(string killerName, string victimName)
    {
        initiator.KillCount++;
        Debug.Log("Killer: " + initiator.unitName + " kill count = " + initiator.KillCount);
        // instantiate kill panel
        EvtOnKill.Invoke(killerName, victimName);

        if (initiator.KillCount >= MaxKills)
        {
            EvtOnWin.Invoke(killerName);
        }
    }
}