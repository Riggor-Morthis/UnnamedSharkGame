using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDummy : MonoBehaviour, IDamageable
{
    //Privates
    private int healthPoints = 3;
    private GameObject theDummy;
    private InterfaceScript ui;
    bool dead;

    private void Awake()
    {
        theDummy = gameObject;
        ui = GameObject.Find("Canvas").GetComponent<InterfaceScript>();
        dead = false;
    }

    public void TakeDamage(int damages)
    {
        healthPoints -= damages;
        if (healthPoints <= 0) OnDeath();
    }

    void OnDeath()
    {
        if (!dead)
        {
            dead = true;
            ui.AddScore(50);
            Destroy(gameObject);
        }   
    }
}
