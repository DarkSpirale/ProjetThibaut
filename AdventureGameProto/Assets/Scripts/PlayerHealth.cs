using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;
    SpriteRenderer spriteRenderer;

    public int maxHealth = 100;
    [HideInInspector]
    public int currentHealth;
    public int armor;

    [HideInInspector]
    public bool isInvincible = false;
    public float invincibilityDuration = 1f;
    public float blinkRate;


    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Il y a plus d'une instance de PlayerHealth existante");
            return;
        }
        instance = this;

        spriteRenderer = transform.GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        armor = 0;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(20);
        }
    }


    public void TakeDamage(int damage)
    {
        if (!isInvincible)
        {
            currentHealth -= damage;
            isInvincible = true;
            StartCoroutine(Blink());
            StartCoroutine(InvincibilityTime());
        } 
    }


    IEnumerator Blink()
    {
        while(isInvincible)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
            yield return new WaitForSeconds(blinkRate);
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(blinkRate);
        }
    }


    IEnumerator InvincibilityTime()
    {
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }
}
