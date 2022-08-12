using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectWeapon : MonoBehaviour
{
    private GunSystem activeGun;
    public List<GunSystem> guns = new List<GunSystem>();
    public int currentGunIndex;

    public List<GunSystem> unlockableGuns = new List<GunSystem>();


    // Start is called before the first frame update
    void Start()
    {
        foreach(GunSystem gun in guns)
        {
            gun.gameObject.SetActive(false);
        }

        activeGun = guns[currentGunIndex];
        activeGun.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            SwitchGun();
    }

    void SwitchGun()
    {
        activeGun.gameObject.SetActive(false);
        currentGunIndex++;

        if(currentGunIndex >= guns.Count)
            currentGunIndex = 0;

        AudioManager.instance.PlaySFX(6);

        activeGun = guns[currentGunIndex];
        activeGun.gameObject.SetActive(true);
    }

    public void UnlockGun(string gunName)
    {
        bool unlocked = false;

        if(unlockableGuns.Count > 0)
        {
            for(int i = 0; i < unlockableGuns.Count; i++)
            {
                if(unlockableGuns[i].name == gunName)
                {
                    guns.Add(unlockableGuns[i]);
                    unlockableGuns.RemoveAt(i);

                    i = unlockableGuns.Count;

                    unlocked = true;
                }
            }
        }

        if (unlocked)
        {
            currentGunIndex = guns.Count - 2;
            SwitchGun();
        }
    }
}
