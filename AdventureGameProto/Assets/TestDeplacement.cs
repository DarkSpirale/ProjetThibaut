using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestDeplacement : MonoBehaviour
{
    const float DELAY_BETWEEN_REGISTERS = 0.25f;

    Rigidbody2D rb;

    Vector3 targetPosition;
    Vector2 dir;

    public List<Vector3> pathPositions = new List<Vector3>();


    bool registerMove = true;
    public bool isFollowingPlayer = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = transform.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        if(isFollowingPlayer)
        {
            targetPosition = PlayerMovement.instance.transform.position;

            //Création de la List
            if(pathPositions.Count == 0)
            {
                pathPositions.Add(transform.position);
            }

            if(registerMove)
            {
                registerMove = false;

                if(pathPositions[pathPositions.Count - 1] != transform.position)
                    pathPositions.Add(transform.position);

                StartCoroutine("DelayBetweenMovements");
            }  
        }
        else //Rebrousse chemin
        {
            if(pathPositions.Count > 0)
            {
                targetPosition = pathPositions[pathPositions.Count - 1];

                while(Vector2.Distance(targetPosition, transform.position) < 0.1f && pathPositions.Count > 0) //Destination atteinte
                {
                    pathPositions.RemoveAt(pathPositions.Count - 1);
                    if(pathPositions.Count > 0)
                        targetPosition = pathPositions[pathPositions.Count - 1];
                }
            }
            else
            {
                targetPosition = transform.position;
            }
        }
           
        if(Input.GetKeyDown(KeyCode.T))
        {
            isFollowingPlayer = !isFollowingPlayer;
        }

        dir = (targetPosition - transform.position).normalized;
        
    }

    void FixedUpdate() 
    {
        rb.velocity = dir * 100 * Time.fixedDeltaTime;
    }


    IEnumerator DelayBetweenMovements()
    {
        yield return new WaitForSeconds(DELAY_BETWEEN_REGISTERS);
        registerMove = true;
    }
}
