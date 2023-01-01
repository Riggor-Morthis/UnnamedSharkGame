using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBox : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<IDamageable>() != null) collision.GetComponent<IDamageable>().TakeDamage(1000000);
        else Destroy(collision.gameObject);
    }
}
