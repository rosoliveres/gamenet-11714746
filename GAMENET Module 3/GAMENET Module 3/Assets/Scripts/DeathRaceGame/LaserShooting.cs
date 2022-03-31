using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LaserShooting : Shooting
{
    private LineRenderer lineRenderer;
    [SerializeField] private GameObject laserPrefab;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        lineRenderer = this.GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
        damage = 20f;
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Space) && photonView.IsMine)
        {
            Fire();
        }
    }

    protected override void Fire()
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, GunPoint.position);
        RaycastHit hit;

        Ray ray = Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if(Physics.Raycast(ray, out hit, 200))
        {
            Debug.Log(hit.collider.gameObject.name);
            lineRenderer.SetPosition(1, hit.point);

            if (hit.collider.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                // Victim calls the take damage fn
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, damage);    // all current and future players get rpc call
                Attack(hit.collider.gameObject, initiator);
            }
            StartCoroutine(WaitForDisableLineRenderer());
        }
    }

    private IEnumerator WaitForDisableLineRenderer()
    {
        float waitTime = 1f;

        while(waitTime > 0)
        {
            yield return new WaitForSeconds(1f);
            waitTime--;
        }

        lineRenderer.enabled = false;
    }
}
