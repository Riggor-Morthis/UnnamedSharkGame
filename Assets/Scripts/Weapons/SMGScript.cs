using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMGScript : AWeapon
{
    int Ammo;

    private void Awake()
    {
        Init();
        shootDelay = 0.2f;
        bulletSpeed = 25f;
        damages = 1;
        playerCombat = GetComponent<PlayerCombatScript>();
        bulletPrefab = playerCombat.getBulletPrefab();

        Ammo = 30;
        playerCombat.SetAmmo(Ammo);
    }

    protected override void Shoot()
    {
        currentBullet = Instantiate(bulletPrefab);
        currentBullet.GetComponent<BulletScript>().Initialize(transform.position, Vector3.right * bulletDirection, bulletSpeed, 0.4f, damages);

        Ammo--;
        playerCombat.SetAmmo(Ammo);
        if (Ammo == 0) playerCombat.LoseGun();
    }
}
