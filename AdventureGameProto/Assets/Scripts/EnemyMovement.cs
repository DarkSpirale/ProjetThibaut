using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour
{
    //Variables de récupération des components
    private Animator animator;
    [HideInInspector]
    public Rigidbody2D rb;
    private BoxCollider2D myCollider;
    private EnemyControl enemyControl;
    private EnemyHealth enemyHealth;

    //Variables pour les déplacements aléatoires
    public bool wanderAround = true;   
    public float movementRadius = 3f;
    public float movementDelay = 3f;

    //Variables pour la limitation de zone de déplacement
    public bool limitedArea = false;
    public Collider2D movementArea;

    //Variables de déplacement pour le rigidbody
    private Vector3 targetPosition;
    Vector2 dir;
    private Vector2 velocity;
    bool canMove = true;
    bool isMoving = false;

    //Variables de poursuite du joueur
    float distanceWithPlayer;
    bool playerInSight = false;
    bool gotOutOfSight = false;
    List<Vector3> pathPositions = new List<Vector3>();
    bool recordPosition = true;
    const float DELAY_BETWEEN_POSITION_RECORDS = 0.25f;
    bool isBackToStartPosition = true;

    //Variables de détection d'obstacles
    RaycastHit2D hit;
    int obstacleLayer = 1 << 8;
    bool canDrawRay = true;

    Vector2 knockBack = Vector2.zero;    

    void Awake()
    {
        animator = transform.GetComponent<Animator>();
        rb = transform.GetComponent<Rigidbody2D>();
        myCollider = transform.GetComponent<BoxCollider2D>(); 
        enemyControl = transform.GetComponent<EnemyControl>();
        enemyHealth = transform.GetComponent<EnemyHealth>();

        if(limitedArea && movementArea == null)
            Debug.LogError("Missing movement area affectaction");
    }


    void Start()
    {
        targetPosition = transform.position;

        if(!limitedArea && movementArea != null)
            movementArea.enabled = false;
    }
    

    void Update()
    {   
        distanceWithPlayer = Vector2.Distance(transform.position, PlayerMovement.instance.transform.position);

        //Si l'ennemi est configuré pour cibler le joueur
        if(enemyControl.data.targetsPlayer)
        {     
            //Si le joueur est dans le rayon de détection de l'ennemi       
            if(distanceWithPlayer <= enemyControl.data.detectionRadius)
            {
                hit = Physics2D.BoxCast(transform.position, myCollider.size, 0f, (PlayerMovement.instance.transform.position - transform.position).normalized,
                    Vector2.Distance(transform.position, PlayerMovement.instance.transform.position), obstacleLayer);

                //Vérifie qu'il n'y a pas d'obstacle entre l'ennemi et le joueur
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
            if (gotOutOfSight || (playerInSight && distanceWithPlayer > enemyControl.data.detectionRadius))
            {
                //Le joueur n'étant plus en vue, l'ennemi se rend à la dernière position cible enregistrée
                if(Vector2.Distance(transform.position, targetPosition) <= 0.1f)
                {
                    playerInSight = false;
                    gotOutOfSight = false;
                    isMoving = false;
                    canMove = true;
                    targetPosition = transform.position;
                    dir = Vector2.zero;  

                    if(limitedArea)
                        isBackToStartPosition = false;
                }          
            }  

            //Si l'ennemi a une zone de restriction de déplacement configurée et qu'il poursuit le joueur,
            //alors on enregistre ses déplacements pour lui permettre de retourner dans sa zone lorsqu'il perd le joueur de vue
            if(limitedArea && playerInSight)
            {
                RecordPath();
            }
            //Si l'ennemi a une zone de restriction de déplacement configurée et qu'il perd de vue le joueur,
            //alors l'ennemi rebrousse chemin jusqu'à sa position de départ
            else if(limitedArea && !playerInSight && !isBackToStartPosition)
            {
                isMoving = true;
                UnrecordPath();
                dir = (targetPosition - transform.position).normalized;
            }                    
        }         

        

        //Si l'ennemi est autorisé à se déplacer aléatoirement et l'ennemi est dans le champs de la caméra
        //et le joueur n'est pas en vue (seulement si l'ennemi est configuré pour cibler le joueur)
        //et l'ennemi est dans sa zone de déplacement (seulement si l'ennemi est configuré pour cibler le joueur et a une zone de déplacement limitée)
        if(wanderAround && distanceWithPlayer < 20f && !playerInSight && isBackToStartPosition) 
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
            rb.velocity = dir * enemyControl.data.moveSpeed * Time.fixedDeltaTime;
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

        int maxTry = 100;
        while(!isTargetOk && maxTry > 0) //Execute tant que la nouvelle position à atteindre n'est pas valide
        {
            //Calcul de la nouvelle position à atteindre
            targetPosition = (Random.insideUnitCircle * movementRadius);
            dir = targetPosition;
            targetPosition += transform.position;

            hit = Physics2D.BoxCast(transform.position, myCollider.size, 0f, dir.normalized,
                Vector2.Distance(transform.position, targetPosition), obstacleLayer);

            //Vérifie qu'il n'y a pas d'obstacle sur le chemin et que targetPosition est dans la mouvementArea si la restriction de mouvement est activée
            if(!hit)
            {
                if(!limitedArea)
                {
                    isTargetOk = true; //Nouvelle position à atteindre valide
                    Debug.DrawRay(transform.position, dir.normalized * Vector2.Distance(transform.position, targetPosition), Color.green, movementDelay - 0.5f);
                }
                else if(movementArea.OverlapPoint(targetPosition))
                {
                    isTargetOk = true; //Nouvelle position à atteindre valide
                    Debug.DrawRay(transform.position, dir.normalized * Vector2.Distance(transform.position, targetPosition), Color.green, movementDelay - 0.5f);
                }              
            }
            else //En cas d'obstacle sur le chemin, dessine un raycast dans la scène
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


    IEnumerator DelayBetweenPositionRecords()
    {
        yield return new WaitForSeconds(DELAY_BETWEEN_POSITION_RECORDS);
        recordPosition = true;
    }


    void RecordPath()
    {
        //Initialisation de la 'List' de poursuite avec la position de départ
        if(pathPositions.Count == 0)
        {
            pathPositions.Add(transform.position);
        }

        //Enregistrement
        if(recordPosition)
        {
            recordPosition = false;

            if(pathPositions[pathPositions.Count - 1] != transform.position)
                pathPositions.Add(transform.position);

            StartCoroutine("DelayBetweenPositionRecords");
        }  
    }


    void UnrecordPath()
    {
        if(pathPositions.Count > 0)
        {
            targetPosition = pathPositions[pathPositions.Count - 1];

            //Si la dernière position est atteinte, alors affectation de la précédente si elle existe
            while(Vector2.Distance(targetPosition, transform.position) < 0.1f && pathPositions.Count > 0)
            {
                pathPositions.RemoveAt(pathPositions.Count - 1);
                if(pathPositions.Count > 0)
                    targetPosition = pathPositions[pathPositions.Count - 1];
            }
        }
        else
        {
            targetPosition = transform.position;
            isBackToStartPosition = true;
        }
    }


    public void KnockBack(Vector2 knockBackDir, int knockBackPower)
    {
		knockBack = knockBackDir * knockBackPower;
    }
}
