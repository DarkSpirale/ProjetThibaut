using UnityEngine;
using System.Collections;

public class SnakePatrol : MonoBehaviour
{
    public int damageOnCollision;

    public float movementRadius = 3f;
    public int moveSpeed = 2;
    public float movementDelay = 5f;

    private Vector3 targetPosition;
    bool canMove = true;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        Vector3 dir = targetPosition - transform.position;
        
        //Si le serpent se rend à sa destination
        if(canMove && Vector3.Distance(transform.position, targetPosition) > 0.3f)
        {
            //Déplacement du serpent
            transform.Translate(dir.normalized * moveSpeed * Time.deltaTime, Space.World);
        }
        
        //Si le serpent vient d'arriver à sa destination
        if(Vector3.Distance(transform.position, targetPosition) <= 0.3f && canMove)
        {
            canMove = false;
            StartCoroutine(CalculateNewTarget());
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
        yield return new WaitForSeconds(movementDelay); //Attente jusqu'à la prochaine possibilité de mouvement
        canMove = true;
        //Calcul de la nouvelle position à atteindre
        targetPosition = (Random.insideUnitCircle * movementRadius);
        targetPosition += transform.position;
    }
}
