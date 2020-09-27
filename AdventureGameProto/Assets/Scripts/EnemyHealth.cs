using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    const float BLINK_RATE = 0.15f;

    SpriteRenderer spriteRenderer;
    Animator animator;
    Collider2D myCollider;
    EnemyMovement enemyMovement;
    EnemyControl enemyControl;

    [HideInInspector]
    public int currentHealth;

    [HideInInspector]
    public bool isInvincible = false;
    public float invincibilityDuration = 1f;

    void Awake()
    {
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
        animator = transform.GetComponent<Animator>();
        myCollider = transform.GetComponent<Collider2D>();
        enemyMovement = transform.GetComponent<EnemyMovement>();
        enemyControl = transform.GetComponent<EnemyControl>();
    }


    void Start()
    {
        currentHealth = enemyControl.data.health;
    }


    public void TakeDamage(int damage)
    {
        if(!isInvincible)
        {
            currentHealth -= damage;

            if(currentHealth <= 0)
                Die();
            else
            {
                isInvincible = true;
                StartCoroutine(Blink());
                StartCoroutine(InvincibilityTime());
            }                         
        }

    }


    void Die()
    {   
        //Désactivation des components ne devant plus faire effet à la mort
        myCollider.enabled = false;
        enemyMovement.rb.velocity = Vector2.zero;
        enemyMovement.enabled = false;

        animator.SetTrigger("isDead"); //Animation de mort
    }


    public void Destroy()
    {
        Destroy(gameObject);
    }


    IEnumerator Blink()
    {
        while(isInvincible)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
            yield return new WaitForSeconds(BLINK_RATE);
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(BLINK_RATE);
        }      
    } 


    IEnumerator InvincibilityTime()
    {
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }
}
