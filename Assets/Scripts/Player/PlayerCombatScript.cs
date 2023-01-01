using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCombatScript : APlayer, IDamageable
{
    //Constantes
    private const int BootDamage = 5; //D�g�ts inflig�s lorsqu'on aterrit sur un ennemi

    //Privates
    private bool fireInput;
    private Rigidbody2D playerRigidbody; //Le rigidbody de notre personnage, pour le faire bouger
    private PlayerMovementScript playerMovement; //Le script de mouvement pour forcer le personnage � bouger
    private Collider2D baddieHit; //Pour stocker les ennemis touches par les bottes
    private Collider2D throwableHit; //Pour stocker les throwables qu'on touche
    private IDamageable baddieBody; //Pour stocker le component IDamageable du truc touche
    private IThrowable throwableBody; //Pour stocker le component IThrowable du truc touche
    private AWeapon currentGun;
    private bool bulletProofVest;
    private InterfaceScript ui;

    //Publique
    public LayerMask baddiesLayer; //Le layer utilis� pour les gens sur lesquels on peut taper
    public LayerMask throwableLayer; //Le layer utilis� par les objets jetables dans l'environnement
    public GameObject bulletPrefab;

    //Premi�re m�thode lanc�
    private new void Awake()
    {
        base.Awake();

        fireInput = false;
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovementScript>();
        currentGun = GetComponent<AWeapon>();
        bulletProofVest = false;
        ui = GameObject.Find("Canvas").GetComponent<InterfaceScript>();
    }

    //Fait pour les calculs physiques
    private void FixedUpdate()
    {
        LastXGetter();
        BootOnHead();
        ThrowingStuff();
        currentGun.ReceiveInputs(fireInput, lastXInput);
    }

    /// <summary>
    /// Utilise pour infliger des d�g�ts aux ennemis lorsque le joueur atterit dessus
    /// </summary>
    private void BootOnHead()
    {
        //On doit �tre en chute libre
        if(playerRigidbody.velocity.y < 0)
        {
            //On (essaye de) r�cup�re(r) ce qu'on touche avec nos bottes
            baddieHit = Physics2D.OverlapArea(playerBackFeet.position, playerFrontFeet.position + Vector3.down * CheckingRadius, baddiesLayer);
            if(baddieHit != null)
            {
                //Si le truc r�cup�r� impl�mente l'interface IDamageable, on lui fait des d�g�ts
                baddieBody = baddieHit.gameObject.GetComponent<IDamageable>();
                if (baddieBody != null)
                {
                    baddieBody.TakeDamage(BootDamage);
                    playerMovement.ForceJump();
                    audioManager.Play("Step");
                }
            }
        }
    }

    /// <summary>
    /// Utilise pour detecter et lancer les elements de l'environnement
    /// </summary>
    private void ThrowingStuff()
    {
        //On cherche les jetables autour de nous
        throwableHit = Physics2D.OverlapArea(playerBackFeet.position + (Vector3.left + Vector3.down) * CheckingRadius, playerFrontHead.position + (Vector3.up + Vector3.right) * CheckingRadius, throwableLayer);
        //Si on en trouve un, on le lance !
        if (throwableHit != null)
        {
            //Si l'objet a la bonne interface...
            throwableBody = throwableHit.gameObject.GetComponent<IThrowable>();
            if(throwableBody != null)
            {
                //... on peut le jeter
                if (playerRigidbody.velocity.y < 0) playerMovement.ForceJump();
                throwableBody.getThrown(playerRigidbody.position.x, playerMovement.getCurrentSpeed());
                audioManager.Play("Step");
            }
            
        }
    }

    /// <summary>
    /// Recoit les inputs du script d'input
    /// </summary>
    /// <param name="fire">Input de tir</param>
    public void ReceiveInputs(bool fire, float xI)
    {
        fireInput = fire;
        xInput = xI;
    }

    /// <summary>
    /// Comment infliger des d�g�ts � notre personnage
    /// </summary>
    /// <param name="damages">Quantit� de d�g�ts � infliger</param>
    public void TakeDamage(int damages)
    {
        if (!isDashing)
        {
            if (bulletProofVest)
            {
                bulletProofVest = false;
                LoseGun();

                ui.AddScore(-10);
                audioManager.Play("Block");
            }
            else OnDeath();
        }
    }

    public GameObject getBulletPrefab() => bulletPrefab;

    /// <summary>
    /// Quoi faire quand le personnage meurt
    /// </summary>
    private void OnDeath()
    {
        audioManager.Play("PlayerKilled");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SwitchGun(int guntype)
    {
        bulletProofVest = true;
        Destroy(GetComponent<AWeapon>());
        if (guntype == 1)
        {
            currentGun = gameObject.AddComponent<SMGScript>();
            animScript.SetArme(guntype);
        }
        else
        {
            currentGun = gameObject.AddComponent<ShotgunScript>();
            animScript.SetArme(guntype);
        }

        ui.AddScore(20);
        audioManager.Play("PickUp");
    }

    public void LoseGun()
    {
        Destroy(GetComponent<AWeapon>());
        currentGun = gameObject.AddComponent<PistolScript>();
        animScript.SetArme(0);
        audioManager.Play("PickUp");
    }

    public void SetAmmo(int ammo)
    {
        ui.SetMunition(ammo);
    }

    public void Recoil()
    {
        animScript.SetAttack(true);
    }
}
