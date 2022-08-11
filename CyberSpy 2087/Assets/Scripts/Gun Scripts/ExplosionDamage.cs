using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{
    public int explosionDamage;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyExplosion());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
            other.GetComponent<EnemyHealthSystem>().TakeDamage(explosionDamage);
        if (other.CompareTag("Player"))
            other.GetComponent<PlayerHealthSystem>().TakeDamage(explosionDamage);
    }

    IEnumerator DestroyExplosion()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
