using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    const float BLINK_RATE = 0.15f;

    SpriteRenderer spriteRenderer;

    public int maxHealth = 10;
    [HideInInspector]
    public int currentHealth;
    public int armor = 0;

    void Awake()
    {
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
    }


    void Start()
    {
        currentHealth = maxHealth;
    }


    void Update()
    {
        if(currentHealth <= 0)
        {
            Die();
        }
    }


    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(Blink());
    }


    void Die()
    {
        Destroy(gameObject);
    }


    IEnumerator Blink()
    {
        spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
        yield return new WaitForSeconds(BLINK_RATE);
        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    } 
}
