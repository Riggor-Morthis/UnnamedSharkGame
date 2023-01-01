using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolScript : AWeapon
{ 
    private void Awake()
    {
        Init();
        shootDelay = 0.8f;
        bulletSpeed = 25f;
        damages = 2;
        playerCombat = GetComponent<PlayerCombatScript>();
        bulletPrefab = playerCombat.getBulletPrefab();
    }

    private void Start()
    {
        playerCombat.SetAmmo(0);
    }

    protected override void Shoot()
    {
        currentBullet = Instantiate(bulletPrefab);
        currentBullet.GetComponent<BulletScript>().Initialize(transform.position, Vector3.right * bulletDirection, bulletSpeed, 0.5f, damages);
    }
}
