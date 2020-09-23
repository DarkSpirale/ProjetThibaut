using UnityEngine;

public class AttackDamage : MonoBehaviour
{
    public bool targetIsPlayer;
    public bool targetIsEnemy;


    void Start()
    {
        if(targetIsPlayer && targetIsEnemy)
        {
            Debug.LogWarning("Inconsistent configuration");
        }
    }
    

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.transform.CompareTag("Enemy") && targetIsEnemy)
        {   
            EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>();
            Debug.Log("Snake got hit");  
            enemyHealth.TakeDamage(PlayerAttack.instance.attackPower - enemyHealth.armor);
        }

        if(collider.transform.CompareTag("Player") && targetIsPlayer)
        {
            Debug.Log("Player got hit");
            EnemyAttack enemyAttack = collider.GetComponent<EnemyAttack>();
            PlayerHealth.instance.TakeDamage(enemyAttack.attackPower - PlayerHealth.instance.armor);
        }
    }
}
