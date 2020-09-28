using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    EnemyControl enemyControl;

    public int knockBackOnCollision = 10;

    void Awake()
    {
        enemyControl  = transform.GetComponent<EnemyControl>();
    }


    void OnCollisionStay2D(Collision2D collision)
    {
        //Dégâts au joueur
        if(collision.transform.CompareTag("Player"))
        {
            if(!PlayerHealth.instance.isInvincible)
            {
                PlayerHealth.instance.TakeDamage(enemyControl.data.damageOnCollision);

                //Inflige le knockback au joueur
                if(PlayerHealth.instance.currentHealth > 0)
                {
                    Vector2 knockBackDir = (PlayerMovement.instance.transform.position - transform.position).normalized;
                    PlayerMovement.instance.KnockBack(knockBackDir, knockBackOnCollision);
                }
            }
        }
    }
}
