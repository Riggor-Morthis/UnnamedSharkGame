using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AThrowable : MonoBehaviour, IThrowable
{
    //Constante
    private const float PokeLength = 0.6f;

    //Privates
    protected bool thrown; //Indique si on est actuellement en mouvement ou pas
    protected int direction; //Dans quel sens on va
    protected float throwSpeed; //A quelle vitesse on va
    protected float speedMultiplier; //Demultiplicateur de vitesse, dépend du type de throwable
    protected int damages; //Dégâts infligés par un poke
    protected Rigidbody2D throwableRigidbody; //Le rigidbody de l'objet, pour le déplacer
    protected RaycastHit2D pokedObject; //Pour stocker les objets dans lesquels on rebondi
    protected int score;

    private InterfaceScript ui;

    //Initialisation
    protected void Awake()
    {
        thrown = false;
        direction = 0;
        throwSpeed = 0f;

        throwableRigidbody = gameObject.GetComponent<Rigidbody2D>();
        throwableRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;

        ui = GameObject.Find("Canvas").GetComponent<InterfaceScript>();
    }

    private void FixedUpdate()
    {
        if (thrown) ThrownMover();
    }

    private void ThrownMover()
    {
        throwableRigidbody.velocity = new Vector2(direction * throwSpeed * Time.fixedDeltaTime, throwableRigidbody.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (thrown)
        {
            if (collision.GetComponent<IDamageable>() != null) collision.GetComponent<IDamageable>().TakeDamage(damages);
            else if (collision.GetComponent<IThrowable>() != null) collision.GetComponent<IThrowable>().getThrown(throwableRigidbody.position.x, throwSpeed);
            OnPoke();
        }
    }

    public void getThrown(float xThrower, float throwerSpeed)
    {
        if(!thrown)
        {
            thrown = true;
            direction = Math.Sign(throwableRigidbody.position.x - xThrower);
            throwSpeed = throwerSpeed * speedMultiplier;

            throwableRigidbody.constraints = RigidbodyConstraints2D.None;
            throwableRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

            //On déplace tout de suite de 3 frames pour l'éloigner de ce qui vient d'activer sa collision
            ThrownMover();
            ThrownMover();
            ThrownMover();

            ui.AddScore(score);
        }
    }

    protected abstract void OnPoke();
}
