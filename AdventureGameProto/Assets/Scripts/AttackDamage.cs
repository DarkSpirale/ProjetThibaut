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
        Vector2 knockBackDir = Vector2.zero;

        if(collider.transform.CompareTag("Enemy") && targetIsEnemy)
        {   
            EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>();
            SnakePatrol snakePatrol = collider.GetComponent<SnakePatrol>();
            enemyHealth.TakeDamage(PlayerAttack.instance.attackPower - enemyHealth.armor);

            knockBackDir = (snakePatrol.transform.position - PlayerMovement.instance.transform.position).normalized;
            snakePatrol.KnockBack(knockBackDir, knockBackFactor);
        }

        if(collider.transform.CompareTag("Player") && targetIsPlayer)
        {
            EnemyAttack enemyAttack = enemy.GetComponent<EnemyAttack>();
            PlayerHealth.instance.TakeDamage(enemyAttack.attackPower - PlayerHealth.instance.armor);

            knockBackDir = (PlayerMovement.instance.transform.position - enemy.transform.position).normalized;
            PlayerMovement.instance.KnockBack(knockBackDir, knockBackFactor);
        }
    }
}
