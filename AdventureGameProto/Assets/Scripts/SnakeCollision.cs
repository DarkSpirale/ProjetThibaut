using UnityEngine;

public class SnakeCollision : MonoBehaviour
{
    EnemyControl enemyControl;


    void Awake()
    {
        enemyControl  = transform.GetComponent<EnemyControl>();
    }


    void OnCollisionStay2D(Collision2D collision)
    {
        //Dégâts au joueur
        if(collision.transform.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.transform.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(enemyControl.data.damageOnCollision);
        }
    }
}
