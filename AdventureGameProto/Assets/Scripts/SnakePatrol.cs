using UnityEngine;
using System.Collections;

public class SnakePatrol : MonoBehaviour
{
    public Animator animator;
    public BoxCollider2D myCollider;
    public BoxCollider2D movementArea;

    public int damageOnCollision;

    public float movementRadius = 3f;
    public int moveSpeed = 2;
    public float movementDelay = 4f;
    public bool limitedArea = false;

    private Vector3 targetPosition;
    Vector3 lastPosition;
    Vector3 dir;
    bool canMove = true;
    bool invalidPosition = false;
    

    void Start()
    {
        targetPosition = transform.position;

        if(!limitedArea)
        {
            movementArea.enabled = false;
        }
    }
    

    void Update()
    {
        //Autorise le serpent à bouger seulement si le joueur est à proximité
        if(Vector2.Distance(transform.position, PlayerMovement.instance.transform.position) < 20)
        {
            dir = targetPosition - transform.position;
            dir = dir.normalized;
                    
            //Si le serpent se rend à sa destination
            if(canMove && Vector3.Distance(transform.position, targetPosition) > 0.3f)
            {
                //Déplacement du serpent
                transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);

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


    void OnCollisionStay2D(Collision2D collision)
    {
        //Dégâts au joueur
        if(collision.transform.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.transform.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(damageOnCollision);
        }
    }


    public IEnumerator CalculateNewTarget()
    {
        bool isTargetOk = false;
        int obstacleLayer = 1 << 8;
        RaycastHit2D hit;

        yield return new WaitForSeconds(movementDelay); //Attente jusqu'à la prochaine possibilité de mouvement
        canMove = true;

        while(!isTargetOk) //Execute tant que la nouvelle position à atteindre n'est pas valide
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
        }
    }
}
