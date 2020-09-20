using UnityEngine;
using System.Collections;

public class SnakePatrol : MonoBehaviour
{
    public Animator animator;

    public BoxCollider2D myCollider;

    public int damageOnCollision;

    public float movementRadius = 3f;
    public int moveSpeed = 2;
    public float movementDelay = 4f;

    private Vector3 targetPosition;
    bool canMove = true;
    Vector3 dir;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        //Autorise le serpent à bouger seulement si le joueur est à proximité
        if(Vector2.Distance(transform.position, PlayerMovement.instance.transform.position) < 20)
        {
            dir = targetPosition - transform.position;
            dir = dir.normalized;
                    
            //Si le serpent se rend à sa destination                            Nico : modif 0.3f
            if(canMove && Vector3.Distance(transform.position, targetPosition) > 0.03f)
            {
                //Déplacement du serpent
                transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);

                animator.SetFloat("HorizontalSpeed", dir.x);
                animator.SetFloat("VerticalSpeed", dir.y);
            }

            //Si le serpent vient d'arriver à sa destination                    Nico : modif 0.3f
            if(canMove && Vector3.Distance(transform.position, targetPosition) <= 0.03f)
            {
                canMove = false;
                StartCoroutine(CalculateNewTarget());
            }

            animator.SetBool("IsMoving", canMove);
            }
    }

    void OnCollisionEnter2D(Collision2D collision)
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
        int maxTry = 100;
        
        yield return new WaitForSeconds(movementDelay); //Attente jusqu'à la prochaine possibilité de mouvement
        canMove = true;
        
        while(!isTargetOk && maxTry > 0) //Execute tant que la nouvelle position à atteindre n'est pas valide
        {
            //Calcul de la nouvelle position à atteindre
            targetPosition = (Random.insideUnitCircle * movementRadius);
            dir = targetPosition;
            targetPosition += transform.position;

            //Nico : test Boxcast
            //hit = Physics2D.Raycast(transform.position, dir.normalized, Vector2.Distance(transform.position, targetPosition), obstacleLayer);
            hit = Physics2D.BoxCast(transform.position, myCollider.size, 0f, dir.normalized,
                Vector2.Distance(transform.position, targetPosition), obstacleLayer);
            
            if (hit) //Vérifie qu'il n'y a pas d'obstacle sur le chemin
            {
                Debug.DrawRay(transform.position, dir.normalized * hit.distance, Color.red, 2f);
                maxTry--;
                if (maxTry == 0) Debug.Log("100 try !");
            }else
            {
                isTargetOk = true; //Nouvelle position à atteindre valide
                Debug.DrawRay(transform.position, dir.normalized * Vector2.Distance(transform.position, targetPosition), Color.green, 2f);
            }
        } 
    }

    //Nico : trace cube
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + dir.normalized * Vector2.Distance(transform.position, targetPosition), myCollider.size);
    }
}
