using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LaserShooting : MonoBehaviourPunCallbacks
{
    public Camera Camera;
    public Health Health;
    public Transform GunPoint;
    private float damage = 20f;
    private LineRenderer lineRenderer;
    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        Health = this.GetComponent<Health>();
        lineRenderer = this.GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
    }

    private void LateUpdate()
    {
        if(Input.GetKey(KeyCode.Space) && photonView.IsMine)
        {
            Fire();
        }
    }

    public void Fire()
    {
        lineRenderer.SetPosition(0, GunPoint.position);
        RaycastHit hit;

        Ray ray = Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if(Physics.Raycast(ray, out hit, 200))
        {
            if (hit.collider)
            {
                Debug.Log(hit.collider.gameObject.name);
                lineRenderer.SetPosition(1, hit.point);
            }
            else lineRenderer.SetPosition(1, transform.forward * 5000);

            if (hit.collider.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                // Victim calls the take damage fn
                //hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, damage);    // all current and future players get rpc call
                Attack(hit.collider.gameObject, initiator);
            }
        }
    }

    private void Attack(GameObject target, Unit initiator)
    {
        Debug.Log("Initiator = " + initiator.unitName);
        target.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, damage);
        if (target.GetComponent<LaserShooting>().isDead)
        {
            photonView.RPC("OnKill", RpcTarget.AllBuffered, initiator.unitName, target.GetComponent<Unit>().unitName);
        }
        target.GetComponent<LaserShooting>().isDead = false;
    }

    [PunRPC]
    public void TakeDamage(float damage, PhotonMessageInfo info)
    {
        this.Health.CurrentHealth -= damage;
        this.Health.SetHealthBarFillAmount();

        if (this.Health.CurrentHealth <= 0)
        {
            Die();
            //OnKill(info.Sender.NickName, info.photonView.Owner.NickName, killer);
            isDead = true;
            Debug.Log(info.Sender.NickName + " killed " + info.photonView.Owner.NickName);
        }
    }

    public void Die()
    {
        if (photonView.IsMine)
        {
            //StartCoroutine(RespawnCountdown());
            Destroy(this.gameObject);
        }
    }
}
