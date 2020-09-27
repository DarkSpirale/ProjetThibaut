using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    private Animator animator;
    [HideInInspector]
    public Rigidbody2D rb;
    private BoxCollider2D myCollider;
    private EnemyControl enemyControl;
    private EnemyHealth enemyHealth;

    public bool wanderAround = true;
    public bool limitedArea = false;
    public Collider2D movementArea;
    public float movementRadius = 3f;
    public float movementDelay = 3f;

    private Vector3 targetPosition;
    Vector3 lastPosition;
    Vector3 dir;
    private Vector2 velocity;
    bool canMove = true;

    Vector2 knockBack = Vector2.zero;    

    void Awake()
    {
        animator = transform.GetComponent<Animator>();
        rb = transform.GetComponent<Rigidbody2D>();
        myCollider = transform.GetComponent<BoxCollider2D>(); 
        enemyControl = transform.GetComponent<EnemyControl>();
        enemyHealth = transform.GetComponent<EnemyHealth>();
    }


    void Start()
    {
        if(wanderAround)
            targetPosition = transform.position;

        if(!limitedArea)
        {
            movementArea.enabled = false;
        }
    }
    

    void Update()
    {   
        //Si l'ennemi cible le joueur
        if(enemyControl.data.targetsPlayer)
        {
            if(Vector3.Distance(transform.position, PlayerMovement.instance.transform.position) < enemyControl.data.detectionRadius)
            {

            }
        }
        else
        {
            if(wanderAround)
            {
                //Autorise le serpent à bouger seulement si le joueur est à proximité
                if(Vector2.Distance(transform.position, PlayerMovement.instance.transform.position) < 20f)
                {
                    dir = (targetPosition - transform.position).normalized;
                            
                    //Si le serpent se rend à sa destination
                    if(canMove && Vector3.Distance(transform.position, targetPosition) > 0.3f)
                    {              
                        animator.SetFloat("HorizontalSpeed", dir.x);
                        animator.SetFloat("VerticalSpeed", dir.y);
                    }

                    //Si le serpent vient d'arriver à sa destination
                    if(canMove && Vector3.Distance(transform.position, targetPosition) <= 0.3f)
                    {
                        canMove = false;
                        StartCoroutine(CalculateNewTarget());
                    }

                    animator.SetBool("IsMoving", canMove);
                }
            }      
        }
        //Si l'ennemi est autorisé à se déplacer aléatoirement
        
    }


    void FixedUpdate()
    {
        if(wanderAround)
        {
            //Déplacement du serpent
            if(canMove)
            {
                //Déplacement du serpent
                rb.velocity = dir * enemyControl.data.moveSpeed * Time.fixedDeltaTime;
            }
            else   
                rb.velocity = Vector2.zero;
        }        

        rb.velocity += knockBack;

        //Rampe le knockback jusqu'à 0
		if(knockBack.sqrMagnitude > 0.01f)
		{
			knockBack *= 0.75f;
		}
		else
			knockBack = Vector2.zero;
    }


    public IEnumerator CalculateNewTarget()
    {
        bool isTargetOk = false;
        int obstacleLayer = 1 << 8;
        RaycastHit2D hit;

        yield return new WaitForSeconds(movementDelay); //Attente jusqu'à la prochaine possibilité de mouvement
        canMove = true;

        int maxTry = 20;
        while(!isTargetOk && maxTry > 0) //Execute tant que la nouvelle position à atteindre n'est pas valide
        {
            //Calcul de la nouvelle position à atteindre
            targetPosition = (Random.insideUnitCircle * movementRadius);
            dir = targetPosition;
            targetPosition += transform.position;

            hit = Physics2D.BoxCast(transform.position, myCollider.size, 0f, dir.normalized,
                Vector2.Distance(transform.position, targetPosition), obstacleLayer);

            //Vérifie qu'il n'y a pas d'obstacle sur le chemin et que targetPosition est dans la mouvementArea si la restriction de mouvement est activée
            if(!hit && (movementArea.OverlapPoint(targetPosition) || !limitedArea))
            {
                isTargetOk = true; //Nouvelle position à atteindre valide
                Debug.DrawRay(transform.position, dir.normalized * Vector2.Distance(transform.position, targetPosition), Color.green, movementDelay - 0.5f);
            }
            else if (hit) //En cas d'obstacle sur le chemin, dessine un raycast dans la scène
            {
                Debug.DrawRay(transform.position, dir.normalized * hit.distance, Color.red, movementDelay - 0.5f);
            }

            maxTry--;
            if(maxTry == 0)
                Debug.LogWarning("Enemy is stuck");
        }
    }


    public void KnockBack(Vector2 knockBackDir, int knockBackPower)
    {
		knockBack = knockBackDir * knockBackPower;
    }
}
