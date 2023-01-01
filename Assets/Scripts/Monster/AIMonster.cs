using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMonster : MonoBehaviour
{
    public Transform bottomArea;
    public GameObject frontColliderObject;
    public bool checkBottom = true;
    public LayerMask groundLayer;
    public LayerMask collisionLayer;
    public LayerMask playerLayers;

    Rigidbody2D m_Rigidbody;
    float m_Speed = 3.0f;
    Vector2 sens = new Vector2(1.0f, 0);
    bool isMoving = true;
    bool isAttacking;
    float countdownTimer, attackTimer;
    Collider2D target;
    SpriteRenderer sprite;

    private void Awake()
    {
        isAttacking = false;
        countdownTimer = 0f;
        attackTimer = 1f;
        target = null;
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        return ((layerMask.value & (1 << obj.layer)) > 0);
    }


    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    private void flipMob()
    {
        sens *= -1;
        Vector3 localScale = frontColliderObject.transform.localPosition;
        localScale.x *= -1;
        frontColliderObject.transform.localPosition = localScale;
        localScale = bottomArea.localPosition;
        localScale.x *= -1;
        bottomArea.localPosition = localScale;
        sprite.flipX = !sprite.flipX;
    }
    // Update is called once per frame
    void Update()
    {
        if (!Physics2D.OverlapCircle(bottomArea.position, 0.1f,groundLayer))
        {
            flipMob();
        }
        if (isAttacking)
        {
            m_Rigidbody.velocity = Vector2.zero;
            if (countdownTimer <= 0)
            {
                isAttacking = false;
                Attack();
            }
            else countdownTimer -= Time.deltaTime;
        }
        else if (isMoving)
        {
            m_Rigidbody.velocity = sens * m_Speed;
        }

    }

    void Attack()
    {
        target = Physics2D.OverlapCircle(frontColliderObject.transform.position, 1.5f, playerLayers);
        if (target != null)
        {
            target.GetComponent<PlayerCombatScript>().TakeDamage(1);
            isAttacking = true;
            countdownTimer = attackTimer;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (IsInLayerMask(col.gameObject, collisionLayer)) flipMob();
        else if (IsInLayerMask(col.gameObject, playerLayers))
        {
            isMoving = false;
            isAttacking = true;
            countdownTimer = attackTimer;
        }
        m_Rigidbody.velocity = new Vector2(0.0f,0.0f);

    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (IsInLayerMask(col.gameObject, playerLayers)) isMoving = true;

    }
}
