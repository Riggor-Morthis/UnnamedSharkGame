using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class APlayer : MonoBehaviour
{
    //Constantes
    protected const float CheckingRadius = .1f; //Rayon du cercle dans lequel on cherche des collisions avec le sol

    //Protected
    protected Collider2D[] groundColliders, bonkColliders; //Les colliders que le joueur touche (ou non) avec ses pieds, touche (ou non) avec sa tete, touche (ou non) avec son corps
    protected bool isGrounded; //Indique si le joueur est au sol ou non
    protected bool isBonked; //Indique la tete du joueur est en collision avec le sol ou non
    protected bool isDashing; //Indique si le joueur est en train de dasher ou non
    protected bool isJumping; //Indique si le joueur est actuellement en train de sauter ou non
    protected float xInput; //Input A/D
    protected int lastXInput; //Memorise le dernier input horizontal, si celui-ci est different de 0
    protected ArmeChanger animScript;
    protected AudioManagerScript audioManager;

    //Publics
    public Transform playerBackFeet, playerFrontFeet; //Bas du sprite, pour check contact avec le sol. On en a 2, un pour l'avant et l'autre pour l'arriere
    public Transform playerBackHead, playerFrontHead; //Comme au dessus mais dans l'autre sens. 2 pour les meme raisons qu'au dessus
    public LayerMask groundLayer; //Le layer utilise par le sol, pour check les collisions avec les plateformes

    //Initialisation des variables
    protected void Awake()
    {
        isGrounded = false;
        isBonked = false;
        isDashing = false;
        isJumping = false;
        xInput = 0f;
        lastXInput = 1;
        animScript = GetComponentInChildren<ArmeChanger>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
    }

    /// <summary>
    /// Pour savoir si le joueur est au sol ou pas
    /// </summary>
    protected void GroundChecker()
    {
        //On regarde si on a des collisions avec le sol sous le premier pied...
        groundColliders = Physics2D.OverlapCircleAll(playerBackFeet.position, CheckingRadius, groundLayer);
        if (groundColliders.Length == 0)
        {
            //...puis sous le deuxième si on a rien trouvé
            groundColliders = Physics2D.OverlapCircleAll(playerFrontFeet.position, CheckingRadius, groundLayer);
            if (groundColliders.Length == 0) isGrounded = false;
            else isGrounded = true;
        }
        else isGrounded = true;
    }

    /// <summary>
    /// Pour savoir si la tete du joueur est dans le plafond ou pas
    /// </summary>
    protected void BonkChecker()
    {
        //On regarde si on a des collisions avec le plafond à l'avant de la tête...
        bonkColliders = Physics2D.OverlapCircleAll(playerBackHead.position, CheckingRadius, groundLayer);
        if (bonkColliders.Length == 0)
        {
            //... puis on vérifie l'arrière de la tête si on a rien trouvé
            bonkColliders = Physics2D.OverlapCircleAll(playerFrontHead.position, CheckingRadius, groundLayer);
            if (bonkColliders.Length == 0) isBonked = false;
            else isBonked = true;
        }
        else isBonked = true;
    }

    /// <summary>
    /// Mémorise le dernier input horizontal qui a été donné au joueur
    /// </summary>
    protected void LastXGetter()
    {
        if (!isDashing && xInput != 0) lastXInput = Math.Sign(xInput);
    }
}
