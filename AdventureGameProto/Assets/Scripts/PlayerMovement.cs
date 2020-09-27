using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
	public static PlayerMovement instance;

	public float moveSpeed = 15f;
	public float rollSpeed = 5f;
	public float rollCooldown = 1f;
	float rollSpeedApplied = 1f;
	
	[HideInInspector]
	public Rigidbody2D rb;
	[HideInInspector]
	public Animator animator;
	SpriteRenderer spriteRenderer;

	Vector2 movement;

	[HideInInspector]
	public bool isIdle = true;
	bool isRollAvailable = true;
	[HideInInspector]
	public bool isRolling = false;
	bool pressedRoll = false;
	[HideInInspector]
	public bool canCancelRoll = false;
	bool isAttacking = false;

	Vector2 knockBack = Vector2.zero;

	void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Il y a plus d'une instance de PlayerMovement existante");
            return;
        }
        instance = this;

		rb = transform.GetComponent<Rigidbody2D>();
		animator = transform.GetComponent<Animator>();
		spriteRenderer = transform.GetComponent<SpriteRenderer>();
    }

	void Update()
	{
		if(Input.GetButtonDown("Roll")) //Register roll input
			pressedRoll = true;
	}

	void FixedUpdate() 
	{
		isAttacking = PlayerAttack.instance.isAttacking;

		if(!isAttacking)
		{
			movement.x = Input.GetAxis("Horizontal") * moveSpeed * Time.fixedDeltaTime;
			movement.y = Input.GetAxis("Vertical") * moveSpeed * Time.fixedDeltaTime;
		} else
			movement = Vector2.zero;		

		if (movement != Vector2.zero)
		{
			if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
            {
				if(Mathf.Abs(movement.x) < 0.1f)
					movement.x = movement.x < 0 ? -0.1f : 0.1f;

                animator.SetFloat("HorizontalSpeed", movement.x);
                animator.SetFloat("VerticalSpeed", 0);    
            }
            else
            {
				if(Mathf.Abs(movement.y) < 0.1f)
					movement.y = movement.y < 0 ? -0.1f : 0.1f;
		
                animator.SetFloat("HorizontalSpeed", 0);
                animator.SetFloat("VerticalSpeed", movement.y);    
            }  
		}	
		animator.SetFloat("Speed", movement.sqrMagnitude);

		//Lancement de la roulade
		if(pressedRoll)
		{
			bool canRoll = isRollAvailable && (isIdle || (isAttacking && PlayerAttack.instance.canCancelAttack));
			if(canRoll)
			{
				//End attacking method if the attack was cancelled
				if(isAttacking)
					PlayerAttack.instance.EndAttack();

				Roll();
			}
			pressedRoll = false;
		}

		//Application de l'offset de velocity en cas de roulade
		rollSpeedApplied = isRolling ? rollSpeed : 1f;

		isIdle = !isRolling && !isAttacking;

		MovePlayer(movement);

		//Rampe le knockback jusqu'à 0
		if(knockBack.sqrMagnitude > 0.01f)
		{
			knockBack *= 0.75f;
		}
		else
			knockBack = Vector2.zero;
	}

	
	void MovePlayer(Vector2 _movement)
	{	
		if(!isAttacking)
		{
			rb.velocity = _movement.normalized * moveSpeed * rollSpeedApplied * Time.fixedDeltaTime;	
			rb.velocity += knockBack;
		}	
		else
			rb.velocity = Vector2.zero;
	}


	void Roll()
	{
		isRollAvailable = false;
		isRolling = true;
		PlayerHealth.instance.isInvincible = true;
		animator.SetTrigger("isRolling");
	}

	public void EndRoll()
	{
		isRolling = false;
		PlayerHealth.instance.isInvincible = false;
		canCancelRoll = false;

		StartCoroutine(RollCooldown());
	}	

	public void CanCancelRoll()
	{
		canCancelRoll = true;
	}

	IEnumerator RollCooldown()
	{
		yield return new WaitForSeconds(rollCooldown);
		isRollAvailable = true;
	}


    public void KnockBack(Vector2 knockBackDir, int knockBackPower)
    {
		knockBack = knockBackDir * knockBackPower;
    }
}
