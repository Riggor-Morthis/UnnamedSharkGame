using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : APlayer
{
    //Constantes
    private const float Speed = 535f; //Vitesse des mouvements du personnage
    private const float DashForce = 1775f; //"Puissance" du dash du personnage
    private const float JumpForce = 575f; //"Puissance" du saut du personnage
    private const float GravityOn = 6.6f, GravityOff = 0f; //La force de la gravite, en dehors d'un saut et durant un saut respectivement
    private const float InputLength = 0.1f; //Dur�e courte pour une action, standardisee
    private const float DashTotalCooldown = 1.25f; //Il faut attendre au moins ce temps la entre 2 dash

    //Privates
    private Rigidbody2D playerRigidbody; //Le rigidbody de notre personnage, pour le faire bouger
    private bool jumpInput; //Input de saut instantane (la frame ou le bouton est appuye)
    private bool dashInput; //Input de dash, lui aussi instantanne
    private bool jumpToken; //Indique si le joueur est en capacite de sauter (est au sol ou a toucher le sol depuis son dernier saut)
    private bool dashToken; //Indique si le joueur est en etat de dash (cooldown ecoule)
    private bool dashForgiveness; //Indique si le joueur est dans l'algo d'acceptance ou non
    private float dashTimer, dashCooldown, dashForgivenessTimer; //Utilise pour chronometrer le dash en cours, faire revenir le dash et faire la tolerance du dash, respectivement
    private bool jumpForgiveness; //Indique si le joueur est dans l'algo d'acceptance
    private float jumpTimer, jumpForgivenessTimer; //Pour decompter le temps passe en saut, et le temps d'acceptation
    GameObject body;

    //On awake avec nos initialisations pour eviter les erreurs
    private new void Awake()
    {
        base.Awake();

        playerRigidbody = GetComponent<Rigidbody2D>();
        playerRigidbody.gravityScale = GravityOn;

        jumpInput = false;
        jumpToken = false;
        dashToken = false;
        dashForgiveness = false;
        dashTimer = 0f; dashCooldown = DashTotalCooldown; dashForgivenessTimer = 0f;
        isJumping = false;
        jumpForgiveness = false;
        jumpTimer = 0f; jumpForgivenessTimer = 0f;
        body = GameObject.Find("/Player/Body");
    }

    //On recupere les infos physiques, puis on s'en sert pour faire des actions
    private void FixedUpdate()
    {
        //On check les differentes variables utiles
        GroundChecker();
        if (!isGrounded) animScript.SetSaut(true);
        else animScript.SetSaut(false);
        BonkChecker();
        LastXGetter();
        body.transform.localScale = new Vector3(lastXInput * 0.55f, 0.55f, 1f);
        DashTokenizer();
        JumpTokenizer();

        //On applique les inputs qui ont ete recus
        DashMover();
        GroundMover();
        JumpMover();
    }

    /// <summary>
    /// R�initialise le jeton de dash en g�rant le temps rechargement
    /// </summary>
    void DashTokenizer()
    {
        if (!dashToken && !isDashing)
        {
            if (dashCooldown > DashTotalCooldown) dashToken = true;
            else dashCooldown += Time.fixedDeltaTime;
        }
    }

    /// <summary>
    /// Retourne le jeton de saut une fois qu'on est au sol
    /// </summary>
    void JumpTokenizer()
    {
        if (!jumpToken && !isJumping && isGrounded) jumpToken = true;
    }

    /// <summary>
    /// La fonction qui g�re le dash du personnage
    /// </summary>
    void DashMover()
    {
        //Algo de tol�rance :
        //Si le joueur a donn� un input � un moment invalide, celui-ci va �tre gard� en m�moire pendant un court instant des fois qu'il devienne valide par la suite
        if (dashForgiveness)
        {
            dashForgivenessTimer += Time.fixedDeltaTime;
            //Ce jeton peut servir d'input lorsque le jeton d'input est pr�sent
            if (dashToken && !isDashing)
            {
                DashModeOn();
                dashForgiveness = false;
            }
            //On garde le jeton pendant une courte dur�e
            else if (dashForgivenessTimer > InputLength) dashForgiveness = false;
        }

        //Si on a un input...
        if (dashInput)
        {
            //...il faut le reset (posez pas de questions)
            dashInput = false;
            //...on lance le dash si toutes les conditions sont pr�sentes...
            if (dashToken && !isDashing) DashModeOn();
            else
            {
                //...sinon on lance l'algo de tol�rance
                dashForgiveness = true;
                dashForgivenessTimer = 0f;
            }
        }

        //On g�re le d�placement quand on est en train de dasher
        if (isDashing)
        {
            animScript.SetAdvance(true);
            //Le dash ne dure pas longtemps...
            if (dashTimer > InputLength) DashModeOff();
            else
            {
                //...mais d�place le joueur vite
                playerRigidbody.velocity = new Vector2(lastXInput * Time.fixedDeltaTime * DashForce, playerRigidbody.velocity.y);
                dashTimer += Time.fixedDeltaTime;
            }
        }
    }

    /// <summary>
    /// Initialiser les variables pour bien rentrer dans un dash
    /// </summary>
    void DashModeOn()
    {
        if (isJumping) JumpModeOff();
        isDashing = true;
        playerRigidbody.gravityScale = GravityOff;
        playerRigidbody.velocity = Vector2.zero;
        dashTimer = 0f;
        dashToken = false;
        audioManager.Play("Step");
    }

    /// <summary>
    /// Initialiser les variables pour bien quitter un dash
    /// </summary>
    void DashModeOff()
    {
        isDashing = false;
        playerRigidbody.gravityScale = GravityOn;
        dashCooldown = 0f;
    }

    /// <summary>
    /// Bouger le personnage horizontalement
    /// </summary>
    void GroundMover()
    {
        //On peut pas bouger si on est en train de dasher
        if(!isDashing) playerRigidbody.velocity = new Vector2(xInput * Time.fixedDeltaTime * Speed, playerRigidbody.velocity.y);
        if (xInput != 0) animScript.SetAdvance(true);
        else animScript.SetAdvance(false);
    }
    
    /// <summary>
    /// Pour g�rer les sauts du personnage
    /// </summary>
    void JumpMover()
    {
        //Algo de tol�rance :
        //Si le joueur a donn� un input � un moment invalide, celui-ci va �tre gard� en m�moire pendant un court instant des fois qu'il devienne valide par la suite
        if (jumpForgiveness)
        {
            jumpForgivenessTimer += Time.fixedDeltaTime;
            //Ce jeton peut servir d'input lorsque le jeton d'input est pr�sent
            if (jumpToken && !isJumping && !isDashing)
            {
                JumpModeOn();
                jumpForgiveness = false;
            }
            //On garde le jeton pendant une courte dur�e
            else if (jumpForgivenessTimer > InputLength) jumpForgiveness = false;
        }

        //Si on a un input...
        if (jumpInput)
        {
            //...il faut le reset (posez pas de questions)
            jumpInput = false;
            //...on lance le saut si toutes les conditions sont pr�sentes...
            if (jumpToken && !isJumping && !isDashing) JumpModeOn();
            //...sinon on lance l'algo de tol�rance
            else
            {
                jumpForgiveness = true;
                jumpForgivenessTimer = 0f;
            }
        }
        
        //On g�re l'ascension du joueur si on est en train de sauter
        if(isJumping)
        {
            //On s'arr�te quand on se prend le plafond
            if (isBonked) JumpModeOff();
            //O� lorsqu'on a pass� assez longtemps � sauter
            else if (jumpTimer > InputLength) JumpModeOff();
            //Sinon on monte
            else
            {
                playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, JumpForce * Time.fixedDeltaTime);
                jumpTimer += Time.fixedDeltaTime;
            }
        }
    }

    /// <summary>
    /// Initialiser les variables pour bien sauter
    /// </summary>
    void JumpModeOn()
    {
        playerRigidbody.gravityScale = GravityOff;
        playerRigidbody.velocity = Vector2.right * playerRigidbody.velocity.x;
        jumpTimer = 0f;
        isJumping = true;
        if(!isGrounded) jumpToken = false;
        animScript.SetSaut(true);
        audioManager.Play("Step");
    }

    /// <summary>
    /// Initialiser les variables pour bien arr�ter de saut
    /// </summary>
    void JumpModeOff()
    {
        playerRigidbody.gravityScale = GravityOn;
        isJumping = false;
    }

    /// <summary>
    /// Recoit les inputs du script d'input
    /// </summary>
    /// <param name="xMovement">Le mouvement horizontal du joueur (Q ou D)</param>
    /// <param name="jump_i">Instruction de saut</param>
    /// <param name="dash_i">Instruction de dash</param>
    public void ReceiveInputs(float xMovement, bool jump_i, bool dash_i)
    {
        xInput = xMovement;
        if (jump_i) jumpInput = true;
        if (dash_i) dashInput = true;
    }

    /// <summary>
    /// Peut �tre utilis� par les autres scripts pour forcer le personnage � sauter
    /// </summary>
    public void ForceJump()
    {
        JumpModeOn();
    }

    /// <summary>
    /// Ask the movement script how fast the character is going
    /// </summary>
    /// <returns>La vitesse actuelle du personnage</returns>
    public float getCurrentSpeed()
    {
        if (isDashing) return DashForce;
        else return Speed;
    }
}
