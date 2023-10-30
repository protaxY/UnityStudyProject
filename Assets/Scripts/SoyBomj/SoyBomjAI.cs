using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoyBomjAi : MonoBehaviour
{
    private NavMeshAgent agent;

    //Animation
    private Animator animator;

    [SerializeField] public Transform player;

    [SerializeField] public LayerMask whatIsWalkable, whatIsPlayer;

    //Patroling
    private Vector3 walkPoint;
    private bool walkPointSet;
    [SerializeField] public float walkPointRange;

    //Attacking
    [SerializeField] public float timeBetweenAttacks;
    private bool alreadyAttacked;

    // Flipping
    
    //States
    public float sightRange, attackRange;
    [SerializeField] public bool playerInSightRange, playerInAttackRange;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        //Debug.Log(animator.parameters);
    }

    // Update is called once per frame
    void Update()
    {
        //Check for sight and attack range
        //playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        //playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        //if (!playerInSightRange && !playerInAttackRange) Patroling();
        //if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        //if (playerInAttackRange && playerInSightRange) AttackPlayer();
        Patroling();
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
            if (!animator.GetBool("IsWalking"))
                animator.SetBool("IsWalking", true);

        }


        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
            if (animator.GetBool("IsWalking"))
                animator.SetBool("IsWalking", false);
        }
    }

    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsWalkable))
            walkPointSet = true;
    }
}
