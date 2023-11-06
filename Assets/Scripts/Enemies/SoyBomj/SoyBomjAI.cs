using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class SoyBomjAi : MonoBehaviour
{
    [SerializeField] private float _health;

    private NavMeshAgent _agent;
    [SerializeField] private Transform _hips;

    [SerializeField] private float maxAgentToHipsDistance;

    //Animation
    private Animator _animator;

    [SerializeField] public Transform player;
    [SerializeField] public LayerMask whatIsWalkable, whatIsPlayer;

    //Patroling
    private Vector3 walkPoint;
    private bool walkPointSet;
    private bool isIdle;
    [SerializeField] public float walkPointRange;
    [SerializeField] public float minIdleTime;
    [SerializeField] public float maxIdleTime;

    //Attacking
    [SerializeField] public float timeBetweenAttacks;
    private bool alreadyAttacked;

    // Flipping
    [SerializeField] public float flipTime;
    [SerializeField] public float flipUpperFactor;
    [SerializeField] public float flipImpulseFactor;
    [SerializeField] public float flipDistance;
    [SerializeField] public List<LayerMask> whatIsFlippableList;
    private LayerMask whatIsFlippable;
    private bool isFlipping;

    //States
    public float sightRange, attackRange;
    [SerializeField] public bool playerInSightRange, playerInAttackRange;

    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();

        foreach (LayerMask layer in whatIsFlippableList)
            whatIsFlippable += layer;
        isFlipping = false;
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

        
    }

    private void FixedUpdate()
    {
        //replace navmesh agent if it's too far
        if ((transform.position - _hips.transform.position).magnitude > maxAgentToHipsDistance)
        {
            transform.position = new Vector3(_hips.transform.position.x, 0f, _hips.transform.position.z);
            _hips.localPosition = new Vector3(0f, _hips.transform.localPosition.y, 0f);
        }

        StartCoroutine(Patroling());
        StartCoroutine(FlipIfBlocks());
    }

    private IEnumerator Patroling()
    {
        if (!isIdle)
        {
            if (!walkPointSet) SearchWalkPoint();

            if (walkPointSet)
            {
                _agent.SetDestination(walkPoint);
                isIdle = false;
                if (!_animator.GetBool("IsWalking"))
                    _animator.SetBool("IsWalking", true);

            }

            Vector3 distanceToWalkPoint = transform.position - walkPoint;

            //Walkpoint reached
            if (distanceToWalkPoint.magnitude < 2f)
            {
                walkPointSet = false;
                if (_animator.GetBool("IsWalking"))
                    _animator.SetBool("IsWalking", false);

                float idleTime = Random.Range(minIdleTime, maxIdleTime);
                isIdle = true;
                yield return new WaitForSeconds(idleTime);
                isIdle = false;
            }
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

    private IEnumerator FlipIfBlocks()
    {
        if (!isFlipping)
        {
            RaycastHit hit;           
            //RaycastHit blockHit;           
            if (Physics.SphereCast(_hips.position + 0.5f * _hips.forward + 0.2f * _hips.up, 0.6f, _hips.transform.forward, out hit, 0.1f))
            {
                //Physics.Linecast(_hips.position, hit.point + 0.001f * (hit.point - _hips.position).normalized, out blockHit);
                // if nothing blocks
                //if (blockHit.transform.gameObject == hit.transform.gameObject)
                if (whatIsFlippable == (whatIsFlippable | (1 << hit.transform.gameObject.layer)))
                {
                    Rigidbody flipRb = hit.transform.GetComponent<Rigidbody>();
                    Vector3 flipDirection = (hit.point - transform.position).normalized;
                    Vector3 flipImpulse = new Vector3(flipDirection.x, flipUpperFactor, flipDirection.z);
                    flipImpulse *= flipImpulseFactor;
                    flipRb.AddForceAtPosition(flipImpulse, hit.point, ForceMode.Acceleration);

                    isFlipping = true;
                    _animator.SetBool("IsFlipping", true);
                    _animator.SetLayerWeight(_animator.GetLayerIndex("UpperBody"), 1);
                    yield return new WaitForSeconds(flipTime);
                    isFlipping = false;
                    _animator.SetBool("IsFlipping", false);
                    _animator.SetLayerWeight(_animator.GetLayerIndex("UpperBody"), 0);

                }

            }

        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_hips.position + 0.5f * _hips.forward + 0.2f * _hips.up, 0.6f);
    }
}
