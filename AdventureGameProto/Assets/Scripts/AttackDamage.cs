using UnityEngine;

public enum TargetsToAttack {Enemy, Player}

public class AttackDamage : MonoBehaviour
{
    public int knockBackFactor = 10;
    
    public TargetsToAttack targetToAttack;

    public GameObject enemy;

    bool targetIsPlayer;
    bool targetIsEnemy;

    void Start()
    {
        targetIsEnemy = targetToAttack == TargetsToAttack.Enemy;
        targetIsPlayer = targetToAttack == TargetsToAttack.Player;
    }
    

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.transform.CompareTag("Enemy") && targetIsEnemy)
        {   
            EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>();
            enemyHealth.TakeDamage(PlayerAttack.instance.attackPower - enemyHealth.armor);
        }

        if(collider.transform.CompareTag("Player") && targetIsPlayer)
        {
            EnemyAttack enemyAttack = enemy.GetComponent<EnemyAttack>();
            PlayerHealth.instance.TakeDamage(enemyAttack.attackPower - PlayerHealth.instance.armor);

            Vector2 knockBackDir = (PlayerMovement.instance.transform.position - enemy.transform.position).normalized;
            PlayerMovement.instance.KnockBack(knockBackDir, enemyAttack.attackPower * knockBackFactor);
        }
    }
}
