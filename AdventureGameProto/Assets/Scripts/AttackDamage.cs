using UnityEngine;

public enum TargetsToAttack {Enemy, Player}

public class AttackDamage : MonoBehaviour
{
    public int knockBackFactor = 10;

    public GameObject enemy;

    public TargetsToAttack targetToAttack;  

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
        EnemyControl enemyControl;

        //La cible est un ennemi
        if(collider.transform.CompareTag("Enemy") && targetIsEnemy)
        {   
            //Récupération des components
            EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>();
            EnemyMovement enemyMovement = collider.GetComponent<EnemyMovement>();
            enemyControl = collider.GetComponent<EnemyControl>(); 

            //Inflige les dégâts à l'ennemi
            if(!enemyHealth.isInvincible)
                enemyHealth.TakeDamage(PlayerAttack.instance.attackPower - enemyControl.data.armor);

            //Inflige le knockback à l'ennemi
            if(enemyHealth.currentHealth > 0)
            {
                knockBackDir = (enemyMovement.transform.position - PlayerMovement.instance.transform.position).normalized;
                enemyMovement.KnockBack(knockBackDir, knockBackFactor);
            }
            
        }

        //La cible est le joueur
        if(collider.transform.CompareTag("Player") && targetIsPlayer)
        {
            //Récupération des components
            enemyControl = transform.GetComponent<EnemyControl>(); 

            //Inflige les dégâts au joueur
            if(!PlayerHealth.instance.isInvincible)
            {
                PlayerHealth.instance.TakeDamage(enemyControl.data.attackPower - PlayerHealth.instance.armor);

                //Inflige le knockback au joueur
                if(PlayerHealth.instance.currentHealth > 0)
                {
                    knockBackDir = (PlayerMovement.instance.transform.position - enemy.transform.position).normalized;
                    PlayerMovement.instance.KnockBack(knockBackDir, knockBackFactor);
                }
            }            
        }
    }
}
