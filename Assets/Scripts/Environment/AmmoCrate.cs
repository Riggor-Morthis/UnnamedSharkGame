using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoCrate : MonoBehaviour
{
    public LayerMask playerLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if((playerLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            collision.GetComponent<PlayerCombatScript>().SwitchGun(Random.Range(1, 3));
            Destroy(gameObject);
        }
    }
}
