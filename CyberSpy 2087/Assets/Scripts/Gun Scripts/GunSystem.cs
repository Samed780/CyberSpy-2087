using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSystem : MonoBehaviour
{
    public Transform cameraHead;
    public Transform firePos;
    public GameObject bullet;
    public GameObject muzzleFlash, bulletImpact, waterLeak, bloodEffect;

    public bool canAutoFire;
    bool isShooting, readyToShoot = true;

    public float fireRate;

    public int bulletsAvailable, totalBullets, magazineSize;

    public float reloadTime;
    private bool isReloading;

    private UICanvasController canvasController;

    //aim
    public Transform aimPos;
    private float aimSpeed = 2f;
    private Vector3 initialPos;
    public float zoomAmount;

    public int damageAmout;

    // Start is called before the first frame update
    void Start()
    {
        totalBullets -= magazineSize;
        bulletsAvailable = magazineSize;
        canvasController = FindObjectOfType<UICanvasController>();
        initialPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Shoot();
        GunManager();
        UpdateAmmoText();
    }

    private void GunManager()
    {
        if (Input.GetKeyDown(KeyCode.R) && bulletsAvailable < magazineSize && !isReloading)
            Reload();
        if (Input.GetMouseButton(1))
            transform.position = Vector3.MoveTowards(transform.position, aimPos.position, aimSpeed * Time.deltaTime);
        else
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, initialPos, aimSpeed * Time.deltaTime);
        if (Input.GetMouseButtonDown(1))
            FindObjectOfType<CameraMovement>().ZoomIn(zoomAmount);
        if (Input.GetMouseButtonUp(1))
            FindObjectOfType<CameraMovement>().ZoomOut();
    }

    void Shoot()
    {
        if(canAutoFire)
            isShooting = Input.GetMouseButton(0);
        else
            isShooting = Input.GetMouseButtonDown(0);

        if (isShooting && readyToShoot && bulletsAvailable > 0 && !isReloading)
        {
            readyToShoot = false;

            RaycastHit hit;

            if (Physics.Raycast(cameraHead.position, cameraHead.forward, out hit, 100f))
            {
                if (Vector3.Distance(cameraHead.position, hit.point) > 2f)
                {
                    firePos.LookAt(hit.point);
                    if (hit.collider.tag == "Shootable")
                        Instantiate(bulletImpact, hit.point, Quaternion.LookRotation(hit.normal));
                    if (hit.collider.tag == "Water Leak")
                        Instantiate(waterLeak, hit.point, Quaternion.LookRotation(hit.normal));
                }

                if (hit.collider.tag == "Enemy")
                {
                    hit.collider.GetComponent<EnemyHealthSystem>().TakeDamage(damageAmout);
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                }

            }
            else
                firePos.LookAt(cameraHead.position + (cameraHead.forward * 50f));

            bulletsAvailable--;

            Instantiate(muzzleFlash, firePos.position, firePos.rotation, firePos);
            Instantiate(bullet, firePos.position, firePos.rotation, firePos);

            StartCoroutine(ResetShooting());

        }
    }

    IEnumerator ResetShooting()
    {
        yield return new WaitForSeconds(fireRate);
        readyToShoot = true;
    }

    private void Reload()
    {
        int bulletsToAdd = magazineSize - bulletsAvailable;
        if (totalBullets > bulletsToAdd)
        {
            totalBullets -= bulletsToAdd;
            bulletsAvailable = magazineSize;
        }
        else
        {
            bulletsAvailable += totalBullets;
            totalBullets = 0;
        }

        isReloading = true;

        StartCoroutine(ReloadTime());
    }

    IEnumerator ReloadTime()
    {
        yield return new WaitForSeconds(reloadTime);
        isReloading = false;
    }

    private void UpdateAmmoText()
    {
        canvasController.ammoText.SetText(bulletsAvailable + "/" + magazineSize);
        canvasController.totalAmmoText.SetText(totalBullets.ToString());
    }
}
