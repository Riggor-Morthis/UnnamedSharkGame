using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    //privates
    private Vector3 direction;
    private float speed;
    private float lifeExpetancy;
    private int damages;

    public void Initialize(Vector3 p, Vector3 di, float s, float lE, int da)
    {
        direction = di;
        speed = s;
        lifeExpetancy = lE;
        damages = da;

        transform.position = p + 2 * (direction * speed * Time.fixedDeltaTime);
    }

    private void FixedUpdate()
    {
        lifeExpetancy -= Time.fixedDeltaTime;
        if (lifeExpetancy > 0) transform.position += direction * speed * Time.fixedDeltaTime;
        else Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.GetComponent<IDamageable>() != null) other.GetComponent<IDamageable>().TakeDamage(damages);
        else if (other.GetComponent<IThrowable>() != null) other.GetComponent<IThrowable>().getThrown(transform.position.x, speed);
        Destroy(gameObject);
    }
}
