using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ProjectileShooting : Shooting
{
    public GameObject ProjectilePrefab;

    protected override void Start()
    {
        base.Start();
        fireRate = 2f;
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetKey(KeyCode.Space) && photonView.IsMine)
        {
            if(fireCountdown <= 0f)
            {
                Fire();
                fireCountdown = 1f / fireRate;
            }
            fireCountdown -= Time.deltaTime;
        }
    }

    protected override void Fire()
    {
        GameObject spawnedProjectile = PhotonNetwork.Instantiate(ProjectilePrefab.name, GunPoint.position, GunPoint.rotation);
        Projectile projectile = spawnedProjectile.GetComponent<Projectile>();
        projectile.EvtOnCollision.AddListener(OnCollision);
        Destroy(spawnedProjectile, 2f);
        Destroy(projectile, 2f);
    }

    private void OnCollision(Projectile projectile)
    {
        if (projectile.HasHitOtherPlayer && !projectile.Victim.GetComponent<PhotonView>().IsMine)
        {
            GameObject target = projectile.Victim;
            Debug.Log("Target = " + target.name);
            Attack(target, initiator);
            Debug.Log("Attacking = " + target.name);
        }
    }
}
