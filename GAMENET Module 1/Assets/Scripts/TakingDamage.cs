using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class TakingDamage : MonoBehaviourPunCallbacks
{
    private float startHealth = 100f;
    public float health;

    [SerializeField]
    private Image healthBar;

    // Start is called before the first frame update
    void Start()
    {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
    }

    [PunRPC] // RPC fns are calls that get broadcasted to the server
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Health " + health);
        healthBar.fillAmount = health / startHealth;

        if (health <= 0)
            Die();
    }

    private void Die()
    {
        // Ensure that it is your player that leaves room when you die
        if(photonView.IsMine)
            GameManager.instance.LeaveRoom();
    }
}
