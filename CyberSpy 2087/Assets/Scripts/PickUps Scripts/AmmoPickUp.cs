using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickUp : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponentInChildren<GunSystem>().AddAmmo();
            AudioManager.instance.PlaySFX(0);
            Destroy(gameObject);
        }
    }
}
