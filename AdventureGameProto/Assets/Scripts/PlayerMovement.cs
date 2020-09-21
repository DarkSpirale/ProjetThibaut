using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
	public static PlayerMovement instance;

	public float moveSpeed = 15f;
	public float rollSpeed = 5f;
	public float rollCooldown = 1f;
	float rollSpeedApplied = 1f;
	
	public Rigidbody2D rb;
	public Animator animator;
	public SpriteRenderer spriteRenderer;

	Vector2 movement;

	[HideInInspector]
	public bool isIdle = true;
	bool isRollAvailable = true;
	bool isRolling = false;


	void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Il y a plus d'une instance de PlayerMovement existante");
            return;
        }
        instance = this;
    }


    void Update()
    {
		movement.x = Input.GetAxis("Horizontal") * moveSpeed * Time.fixedDeltaTime;
		movement.y = Input.GetAxis("Vertical") * moveSpeed * Time.fixedDeltaTime;

		if (movement != Vector2.zero)
		{
			animator.SetFloat("HorizontalSpeed", movement.x);
			animator.SetFloat("VerticalSpeed", movement.y);	
		}	
		animator.SetFloat("Speed", movement.sqrMagnitude);

		if(Input.GetButton("Dash") && isRollAvailable && movement.sqrMagnitude > 0.01f)
		{
			StartCoroutine(Roll());
		}

		//Application de l'offset de velocity en cas de roulade
		rollSpeedApplied = isRolling ? rollSpeed : 1f;

		isIdle = !isRolling;
    }


	void FixedUpdate() 
	{
		MovePlayer(movement);
	}

	
	void MovePlayer(Vector2 _movement)
	{	
		rb.MovePosition(rb.position + _movement * moveSpeed * Time.fixedDeltaTime * rollSpeedApplied);	
	}


	public void EndRoll()
	{
		isRolling = false;
		PlayerHealth.instance.isInvincible = false;
	}


	IEnumerator Roll()
	{
		isRollAvailable = false;
		isRolling = true;
		PlayerHealth.instance.isInvincible = true;
		animator.SetTrigger("isRolling");
		
		yield return new WaitForSeconds(rollCooldown);
		isRollAvailable = true;
	}
}
