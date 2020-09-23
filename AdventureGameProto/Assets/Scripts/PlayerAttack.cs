using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    public static PlayerAttack instance;
    Animator animator;

    public int attackPower = 5;

    //[HideInInspector]
    public bool isAttacking = false;


    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Il y a plus d'une instance de PlayerAttack existante");
            return;
        }
        instance = this;

        animator = PlayerMovement.instance.animator;
    }


    void Update()
    {


        if(Input.GetButton("Attack") && PlayerMovement.instance.isIdle)
        {
            Attack();
        }
    
    }

    
    void Attack()
    {
        isAttacking = true;
        PlayerMovement.instance.rb.velocity = Vector2.zero;

        animator.SetTrigger("isAttacking");
    }


    public void EndAttack()
    {
        isAttacking = false;
    }
}
