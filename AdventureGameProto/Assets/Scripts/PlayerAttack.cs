using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    public static PlayerAttack instance;
    Animator animator;

    public int attackPower = 5;
    bool pressedAttack = false;
    [HideInInspector]
    public bool canCancelAttack = false;

    [HideInInspector]
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
       
        if(Input.GetButtonDown("Attack"))
        {
            pressedAttack = true;
        }
    }

    void FixedUpdate()
    {        
        if(pressedAttack)
        {
            bool canAttack = PlayerMovement.instance.isIdle || (PlayerMovement.instance.isRolling && PlayerMovement.instance.canCancelRoll);
            if(canAttack)
            {
                //End rolling method if the roll was cancelled
                if(PlayerMovement.instance.isRolling)
                    PlayerMovement.instance.EndRoll();

                Attack();
            }
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
        canCancelAttack = false;
    }


    public void CanCancelAttack()
    {
        canCancelAttack = true;
    }
}
