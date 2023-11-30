using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class SoyBomjAi : MonoBehaviour
{
    private NavMeshAgent _agent;
    [SerializeField] private Transform _hips;

    [SerializeField] private float maxAgentToHipsDistance;

    //Health and death
    [SerializeField] private bool _isDead = false;
    [SerializeField] private float _maxHealth;
    public float health;
    [SerializeField] private ConfigurableJoint _hipsJoint;
    [SerializeField] private List<ConfigurableJoint> _joints;
    [SerializeField] private float _destroyAfterDeathTime;

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
    [SerializeField] private float maxWalkTime;
    private float walkTime;

    //Attacking
    [SerializeField] public float sightRange;
    [SerializeField] public float attackRange;
    [SerializeField] public float attackTime;
    [SerializeField] public float attacKickFactor;
    private bool isAttacing;
    [SerializeField] public float attackDamage;

    // Flipping
    [SerializeField] public float flipTime;
    [SerializeField] public float flipUpperFactor;
    [SerializeField] public float flipImpulseFactor;
    [SerializeField] public float flipDistance;
    [SerializeField] public List<LayerMask> whatIsFlippableList;
    private LayerMask whatIsFlippable;
    private bool isFlipping;

    //Chasing
    private bool isChasing;

    //States
    public bool playerInSightRange, playerInAttackRange;

    [SerializeField] public event Action onDeath;

    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();

        foreach (LayerMask layer in whatIsFlippableList)
            whatIsFlippable += layer;
        isFlipping = false;
        isIdle = true;
        isChasing = false;

        health = _maxHealth;
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
        if (!_isDead)
        {
            //replace navmesh agent if it's too far
            if ((transform.position - _hips.transform.position).magnitude > maxAgentToHipsDistance)
            {
                transform.position = new Vector3(_hips.transform.position.x, 0f, _hips.transform.position.z);
                _hips.localPosition = new Vector3(0f, _hips.transform.localPosition.y, 0f);
            }

            bool playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            bool playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (!playerInSightRange && !playerInAttackRange)
                StartCoroutine(PatrolingAsync());
            if (playerInSightRange && !playerInAttackRange)
                Chase();
            else
                isChasing = false;
            if (playerInAttackRange && playerInSightRange && !isAttacing)
                StartCoroutine(AttackAsync());

            if (!playerInAttackRange)
                StartCoroutine(FlipIfBlocksAsync());

            walkTime += Time.fixedDeltaTime;
        }
    }

    private IEnumerator PatrolingAsync()
    {

        if (!walkPointSet && isIdle) 
            SearchWalkPoint();
        else
        {
            Vector3 distanceToWalkPoint = transform.position - walkPoint;
            //Walkpoint reached
            if ((distanceToWalkPoint.magnitude < 2f || walkTime >= maxWalkTime) && !isIdle)
            {
                if (_animator.GetBool("IsWalking"))
                    _animator.SetBool("IsWalking", false);

                float idleTime = Random.Range(minIdleTime, maxIdleTime);
                _agent.ResetPath();
                //isIdle = true;
                isIdle = true;
                yield return new WaitForSeconds(idleTime);

                walkPointSet = false;
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

        _agent.SetDestination(walkPoint);
        isIdle = false;
        if (!_animator.GetBool("IsWalking"))
            _animator.SetBool("IsWalking", true);
        walkTime = 0f;
    }

    private IEnumerator AttackAsync()
    {
        isAttacing = true;

        _animator.SetBool("IsHitting", true);
        _animator.SetLayerWeight(_animator.GetLayerIndex("UpperBody"), 1);

        RaycastHit hit;
        Physics.Linecast(_hips.position + 0.2f * _hips.up, player.position, out hit);
        Debug.DrawLine(_hips.position + 0.2f * _hips.up, hit.point, Color.red);
        player.GetComponent<Rigidbody>().AddForceAtPosition(attacKickFactor * (player.position - _hips.transform.position).normalized, hit.point, ForceMode.Acceleration);
        player.GetComponent<PlayerHealth>().health -= attackDamage;
        player.GetComponent<PlayerHealth>().UpdateHealth();
        if (player.GetComponent<PlayerHealth>().health <= 0f)
            player.GetComponent<PlayerHealth>().TriggerDeath();

        yield return new WaitForSeconds(1f);

        _animator.SetBool("IsHitting", false);
        _animator.SetLayerWeight(_animator.GetLayerIndex("UpperBody"), 0);
        yield return new WaitForSeconds(attackTime - 1f);

        isAttacing = false;
    }

    private void Chase()
    {
        if (!isChasing)
        {
            isChasing = true;
            if (!_animator.GetBool("IsWalking"))
                _animator.SetBool("IsWalking", true);
            _animator.SetLayerWeight(_animator.GetLayerIndex("UpperBody"), 0);
        }
        _agent.SetDestination(player.position);
    }

    private IEnumerator FlipIfBlocksAsync()
    {
        if (!isFlipping)
        {
            RaycastHit hit;
            RaycastHit blockHit;           
            if (Physics.SphereCast(_hips.position + 0.2f * _hips.up, 1f, _hips.transform.forward, out hit, 0.1f, whatIsFlippable))
            {
                //Physics.Raycast(_hips.position + 0.2f * _hips.up, hit.point - 0.001f * (hit.point - _hips.position).normalized, out blockHit);
                // if nothing blocks
                //if (whatIsFlippable == (whatIsFlippable | (1 << hit.transform.gameObject.layer)))
                //Debug.DrawLine(_hips.position + 0.2f * _hips.up, hit.point, Color.red);
                Debug.DrawLine(_hips.position + 0.2f * _hips.up, hit.point, Color.red);
                Debug.DrawRay(_hips.position + 0.2f * _hips.up, _hips.transform.forward, Color.blue);
                //if (true)
                if (!Physics.Linecast(_hips.position + 0.2f * _hips.up, hit.point - 0.001f * (hit.point - _hips.position).normalized, out blockHit))
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
        Gizmos.DrawWireSphere(_hips.position + 0.2f * _hips.up, 1f);
    }

    public void TriggerDeath()
    {
        _isDead = true;

        JointDrive zeroDrive = new JointDrive();
        zeroDrive.positionSpring = 0;

        _hipsJoint.angularYMotion = ConfigurableJointMotion.Free;
        _hipsJoint.xDrive = zeroDrive;
        _hipsJoint.yDrive = zeroDrive;
        _hipsJoint.zDrive = zeroDrive;

        _hipsJoint.angularXDrive = zeroDrive;
        _hipsJoint.angularYZDrive = zeroDrive;

        foreach (ConfigurableJoint joint in _joints)
        {
            joint.xDrive = zeroDrive;
            joint.yDrive = zeroDrive;
            joint.zDrive = zeroDrive;

            joint.angularXDrive = zeroDrive;
            joint.angularYZDrive = zeroDrive;
        }

        _agent.enabled = false;

        Destroy(gameObject, _destroyAfterDeathTime);

        onDeath?.Invoke();
    }
}
