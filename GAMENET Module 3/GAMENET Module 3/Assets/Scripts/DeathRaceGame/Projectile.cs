using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    private Rigidbody rigidbody;
    private float speed = 50f;
    public bool HasHitOtherPlayer = false;
    public bool HasCollided = false;
    public GameObject Victim;

    public UnityEvent<Projectile> EvtOnCollision = new UnityEvent<Projectile>();

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.forward * speed;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            HasHitOtherPlayer = true;
            Victim = collision.gameObject;
            EvtOnCollision.Invoke(this);
        }
        HasCollided = true;
        Destroy(this.gameObject);
    }
}
