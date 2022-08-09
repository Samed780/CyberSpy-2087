using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileController : MonoBehaviour
{
    Rigidbody body;
    public float upForce, forwardForce;

    public int damage = 2;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();

        GrenadeThrow();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GrenadeThrow()
    {
        body.AddForce(transform.forward * forwardForce, ForceMode.Impulse);
        body.AddForce(transform.up * upForce, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
            other.gameObject.GetComponent<PlayerHealthSystem>().TakeDamage(damage);
    }
}
