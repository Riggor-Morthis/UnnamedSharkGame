using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AWeapon : MonoBehaviour
{
    //protected
    protected float shootDelay;
    protected float bulletSpeed;
    protected int bulletDirection;
    protected int damages;
    protected PlayerCombatScript playerCombat;
    protected GameObject bulletPrefab;
    protected GameObject currentBullet;
    protected AudioManagerScript audioManager;

    //private
    private float shootTimer;
    bool shootInput;

    //PRIVATE//

    private void FixedUpdate()
    {
        if (shootTimer > 0) shootTimer -= Time.fixedDeltaTime;
        else if (shootInput)
        {
            Shoot();
            playerCombat.Recoil();
            shootTimer = shootDelay;
            audioManager.Play("Gunshot");
        }
    }

    //PROTECTED//

    protected void Init()
    {
        shootTimer = 0f;
        shootInput = false;
        bulletDirection = 0;
        shootInput = false;
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
    }

    protected abstract void Shoot();

    public void ReceiveInputs(bool fI, int lX)
    {
        shootInput = fI;
        bulletDirection = lX;
    }
}
