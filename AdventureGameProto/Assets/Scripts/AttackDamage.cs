using UnityEngine;

public class AttackDamage : MonoBehaviour
{
    public int knockBackFactor = 10;
    
    public enum Targets {Enemy, Player}
    [Space(10)]
    [Tooltip("Collider to be affected by the attack")]
    public Targets target;

    [Tooltip("To be selected only if target is enemy")]
    public GameObject enemy;

    bool targetIsPlayer;
    bool targetIsEnemy;

    void Start()
    {
        targetIsEnemy = target == Targets.Enemy;
        targetIsPlayer = target == Targets.Player;
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
            Debug.Log("Player got hit");
            EnemyAttack enemyAttack = enemy.GetComponent<EnemyAttack>();
            PlayerHealth.instance.TakeDamage(enemyAttack.attackPower - PlayerHealth.instance.armor);

            Vector2 knockBackDir = (PlayerMovement.instance.transform.position - enemy.transform.position).normalized;
            PlayerMovement.instance.KnockBack(knockBackDir, enemyAttack.attackPower * knockBackFactor);
        }
    }
}
