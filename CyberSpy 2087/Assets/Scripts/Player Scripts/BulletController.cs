using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed, bulletLife;
    public Rigidbody rb;

    public ParticleSystem explosion;
    public bool rocket;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FireBullet();

        bulletLife -= Time.deltaTime;
        if(bulletLife <= 0)
            Destroy(gameObject);
    }

    void FireBullet()
    {
        rb.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (rocket)
            Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
