using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    public static PlayerAttack instance;
    Animator animator;

    public int attackPower = 5;
    bool pressedAttack = false;

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
        if(Input.GetButtonDown("Attack") && PlayerMovement.instance.isIdle)
        {
            pressedAttack = true;
        }
    }

    void FixedUpdate()
    {
        if(pressedAttack)
        {
            Attack();
            pressedAttack = false;
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
