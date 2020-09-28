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
    Vector2 dir;
    private Vector2 velocity;
    bool canMove = true;
    bool isMoving = false;
    bool playerInSight = false;
    bool gotOutOfSight = false;

    RaycastHit2D hit;
    int obstacleLayer = 1 << 8;
    bool canDrawRay = true;

    Vector2 knockBack = Vector2.zero;    

    IEnumerator coroutine;

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
        targetPosition = transform.position;

        if(!limitedArea)
        {
            //movementArea.enabled = false;
        }
    }
    

    void Update()
    {   
        float distanceWithPlayer = Vector2.Distance(transform.position, PlayerMovement.instance.transform.position);

        //Si l'ennemi est configuré pour cibler le joueur, et le joueur est dans le rayon de détection
        if(enemyControl.data.targetsPlayer && distanceWithPlayer <= enemyControl.data.detectionRadius)
        {            
            hit = Physics2D.BoxCast(transform.position, myCollider.size, 0f, (PlayerMovement.instance.transform.position - transform.position).normalized,
                Vector2.Distance(transform.position, PlayerMovement.instance.transform.position), obstacleLayer);

            //Vérifie qu'il n'y a pas d'obstacle sur le chemin
            if(!hit)
            {
                StopCoroutine("CalculateNewTarget");
                playerInSight = true;
                gotOutOfSight = false;
                isMoving = true;
                targetPosition = PlayerMovement.instance.transform.position;   

                dir = (targetPosition - transform.position).normalized;

                animator.SetFloat("HorizontalSpeed", dir.x);
                animator.SetFloat("VerticalSpeed", dir.y);    

                if(canDrawRay) //Affichage du Ray limité à une fois toutes les 0.5s
                    StartCoroutine(DrawRayDelay(0.5f, Color.green));          
                
            } else
            {
                if(playerInSight)
                    gotOutOfSight = true;

                if(canDrawRay)
                    StartCoroutine(DrawRayDelay(0.5f, Color.red));
            } 
                             
        }        

        //Si le joueur sort du champs de vision de l'ennemi
        if (gotOutOfSight || (playerInSight && enemyControl.data.targetsPlayer && distanceWithPlayer > enemyControl.data.detectionRadius))
        {
            //Le joueur n'étant plus en vue, l'ennemi se rend à la dernière position cible enregistrée
            if(Vector2.Distance(transform.position, targetPosition) <= 0.3f)
            {
                playerInSight = false;
                gotOutOfSight = false;
                isMoving = false;
                canMove = true;
                targetPosition = transform.position;
                dir = Vector2.zero;  
            }          
        }      

        //Si l'ennemi est autorisé à se déplacer aléatoirement et l'ennemi est dans le champs de la caméra
        //et le joueur n'est pas en vue (seulement si l'ennemi est configuré pour cibler le joueur)
        if(wanderAround && distanceWithPlayer < 20f && !playerInSight) 
        {
            dir = (targetPosition - transform.position).normalized;

            //Si l'ennemi' se rend à sa destination
            if(canMove && Vector2.Distance(transform.position, targetPosition) > 0.3f)
            {     
                isMoving = true;         
                animator.SetFloat("HorizontalSpeed", dir.x);
                animator.SetFloat("VerticalSpeed", dir.y);
            }

            //Si l'ennemi vient d'arriver à sa destination
            if(canMove && Vector2.Distance(transform.position, targetPosition) <= 0.3f)
            {
                isMoving = false;
                canMove = false;
                StartCoroutine("CalculateNewTarget");
            }          
        }
    animator.SetBool("IsMoving", isMoving);    
    }


    void FixedUpdate()
    {
        //Déplacement de l'ennemi
        if(isMoving)
        {
            //Déplacement de l'ennemi
            rb.velocity = dir * enemyControl.data.moveSpeed * Time.fixedDeltaTime;
        }
        else  
            rb.velocity = Vector2.zero;
                
        rb.velocity += knockBack;

        //Rampe le knockback jusqu'à 0
		if(knockBack.sqrMagnitude > 0.01f)
		{
			knockBack *= 0.75f;
		}
		else
			knockBack = Vector2.zero;
    }


    IEnumerator CalculateNewTarget()
    {
        bool isTargetOk = false;
        
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


    IEnumerator DrawRayDelay(float delay, Color rayColor)
    {
        canDrawRay = false;
        Debug.DrawRay(transform.position, PlayerMovement.instance.transform.position - transform.position, rayColor, delay);
        yield return new WaitForSeconds(delay);
        canDrawRay = true;
    }


    public void KnockBack(Vector2 knockBackDir, int knockBackPower)
    {
		knockBack = knockBackDir * knockBackPower;
    }
}
