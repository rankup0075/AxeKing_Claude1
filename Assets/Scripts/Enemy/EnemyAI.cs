//using UnityEngine;
//using UnityEngine.AI;

//public class EnemyAI : MonoBehaviour
//{
//    [Header("AI Settings")]
//    public float detectionRange = 8f;
//    public float attackRange = 2f;
//    public float moveSpeed = 3f;
//    public float rotationSpeed = 5f;

//    [Header("Patrol Settings")]
//    public bool usePatrol = true;
//    public Transform[] patrolPoints;
//    public float patrolWaitTime = 2f;
//    public float patrolSpeed = 2f;

//    [Header("Combat Settings")]
//    public float attackCooldown = 2f;
//    public float loseTargetTime = 5f;

//    [Header("References")]
//    public Transform player;
//    public NavMeshAgent navAgent;
//    public Animator animator;
//    //public EnemyController enemyController;

//    // AI ����
//    public enum AIState
//    {
//        Idle,
//        Patrol,
//        Chase,
//        Attack,
//        Hit,
//        Dead
//    }

//    [Header("Debug")]
//    public AIState currentState = AIState.Idle;

//    // ���� ������
//    private float lastAttackTime;
//    private float loseTargetTimer;
//    private int currentPatrolIndex = 0;
//    private float patrolTimer;
//    private Vector3 lastKnownPlayerPosition;
//    private bool hasTarget = false;

//    // �ִϸ��̼� �ؽð���
//    private int walkHash = Animator.StringToHash("Walk");
//    private int attackHash = Animator.StringToHash("Attack");
//    private int hitHash = Animator.StringToHash("Hit");
//    private int deathHash = Animator.StringToHash("Death");

//    void Start()
//    {
//        InitializeComponents();
//        InitializeAI();
//    }

//    void InitializeComponents()
//    {
//        // ������Ʈ �ڵ� �Ҵ�
//        if (navAgent == null)
//            navAgent = GetComponent<NavMeshAgent>();
//        if (animator == null)
//            animator = GetComponentInChildren<Animator>();
//        if (enemyController == null)
//            enemyController = GetComponent<EnemyController>();

//        // �÷��̾� ã��
//        if (player == null)
//        {
//            GameObject playerObj = GameObject.FindWithTag("Player");
//            if (playerObj != null)
//                player = playerObj.transform;
//        }
//    }

//    void InitializeAI()
//    {
//        if (navAgent != null)
//        {
//            navAgent.speed = moveSpeed;
//            navAgent.stoppingDistance = attackRange * 0.8f;
//            navAgent.acceleration = 12f;
//        }

//        // ��Ʈ�� ����Ʈ�� ������ ��Ȱ��ȭ
//        if (patrolPoints == null || patrolPoints.Length == 0)
//        {
//            usePatrol = false;
//        }

//        // �ʱ� ���� ����
//        ChangeState(usePatrol ? AIState.Patrol : AIState.Idle);
//    }

//    void Update()
//    {
//        if (enemyController != null && enemyController.CurrentHealth <= 0)
//        {
//            ChangeState(AIState.Dead);
//            return;
//        }

//        // ���º� ������Ʈ
//        switch (currentState)
//        {
//            case AIState.Idle:
//                UpdateIdle();
//                break;
//            case AIState.Patrol:
//                UpdatePatrol();
//                break;
//            case AIState.Chase:
//                UpdateChase();
//                break;
//            case AIState.Attack:
//                UpdateAttack();
//                break;
//            case AIState.Hit:
//                UpdateHit();
//                break;
//            case AIState.Dead:
//                UpdateDead();
//                break;
//        }

//        // �÷��̾� ���� Ȯ�� (Dead ���°� �ƴ� ����)
//        if (currentState != AIState.Dead && currentState != AIState.Hit)
//        {
//            CheckPlayerDetection();
//        }
//    }

//    void CheckPlayerDetection()
//    {
//        if (player == null) return;

//        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

//        // �÷��̾ ���� ���� �ȿ� �ִ��� Ȯ��
//        if (distanceToPlayer <= detectionRange)
//        {
//            // �þ� Ȯ�� (����ĳ��Ʈ)
//            if (CanSeePlayer())
//            {
//                hasTarget = true;
//                lastKnownPlayerPosition = player.position;
//                loseTargetTimer = 0f;

//                // ���� ���� �ȿ� ������ ����, �ƴϸ� ����
//                if (distanceToPlayer <= attackRange && currentState != AIState.Attack)
//                {
//                    ChangeState(AIState.Attack);
//                }
//                else if (currentState != AIState.Chase && currentState != AIState.Attack)
//                {
//                    ChangeState(AIState.Chase);
//                }
//            }
//        }
//        else if (hasTarget)
//        {
//            // Ÿ���� �Ҿ�� ��
//            loseTargetTimer += Time.deltaTime;
//            if (loseTargetTimer >= loseTargetTime)
//            {
//                hasTarget = false;
//                ChangeState(usePatrol ? AIState.Patrol : AIState.Idle);
//            }
//        }
//    }

//    bool CanSeePlayer()
//    {
//        if (player == null) return false;

//        Vector3 directionToPlayer = (player.position - transform.position).normalized;
//        RaycastHit hit;

//        // ����ĳ��Ʈ�� ��ֹ� Ȯ��
//        if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out hit, detectionRange))
//        {
//            return hit.collider.CompareTag("Player");
//        }

//        return false;
//    }

//    void ChangeState(AIState newState)
//    {
//        if (currentState == newState) return;

//        // ���� ���� ó��
//        ExitState(currentState);

//        // �� ���� ����
//        currentState = newState;
//        EnterState(newState);
//    }

//    void EnterState(AIState state)
//    {
//        switch (state)
//        {
//            case AIState.Idle:
//                if (navAgent != null) navAgent.isStopped = true;
//                if (animator != null) animator.SetBool(walkHash, false);
//                break;

//            case AIState.Patrol:
//                if (navAgent != null)
//                {
//                    navAgent.isStopped = false;
//                    navAgent.speed = patrolSpeed;
//                }
//                SetPatrolDestination();
//                break;

//            case AIState.Chase:
//                if (navAgent != null)
//                {
//                    navAgent.isStopped = false;
//                    navAgent.speed = moveSpeed;
//                }
//                if (animator != null) animator.SetBool(walkHash, true);
//                break;

//            case AIState.Attack:
//                if (navAgent != null) navAgent.isStopped = true;
//                if (animator != null) animator.SetBool(walkHash, false);
//                break;

//            case AIState.Hit:
//                if (navAgent != null) navAgent.isStopped = true;
//                if (animator != null)
//                {
//                    animator.SetBool(walkHash, false);
//                    animator.SetTrigger(hitHash);
//                }
//                break;

//            case AIState.Dead:
//                if (navAgent != null) navAgent.isStopped = true;
//                if (animator != null)
//                {
//                    animator.SetBool(walkHash, false);
//                    animator.SetTrigger(deathHash);
//                }
//                break;
//        }
//    }

//    void ExitState(AIState state)
//    {
//        // �ʿ��� ��� ���� ���� ó��
//    }

//    void UpdateIdle()
//    {
//        // ��� ���� - Ư���� ó�� ����
//    }

//    void UpdatePatrol()
//    {
//        if (!usePatrol || patrolPoints.Length == 0) return;

//        if (navAgent != null && !navAgent.pathPending)
//        {
//            if (navAgent.remainingDistance < 0.5f)
//            {
//                patrolTimer += Time.deltaTime;
//                if (patrolTimer >= patrolWaitTime)
//                {
//                    patrolTimer = 0f;
//                    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
//                    SetPatrolDestination();
//                }
//            }
//        }

//        // �ִϸ��̼� ������Ʈ
//        if (animator != null)
//        {
//            bool isMoving = navAgent.velocity.magnitude > 0.1f;
//            animator.SetBool(walkHash, isMoving);
//        }
//    }

//    void SetPatrolDestination()
//    {
//        if (patrolPoints.Length > 0 && navAgent != null)
//        {
//            navAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
//        }
//    }

//    void UpdateChase()
//    {
//        if (player == null || navAgent == null) return;

//        // �÷��̾� ��ġ�� �̵�
//        Vector3 targetPosition = hasTarget ? player.position : lastKnownPlayerPosition;
//        navAgent.SetDestination(targetPosition);

//        // ���� ���� Ȯ��
//        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
//        if (distanceToPlayer <= attackRange && hasTarget)
//        {
//            ChangeState(AIState.Attack);
//        }

//        // �ִϸ��̼� ������Ʈ
//        if (animator != null)
//        {
//            bool isMoving = navAgent.velocity.magnitude > 0.1f;
//            animator.SetBool(walkHash, isMoving);
//        }
//    }

//    void UpdateAttack()
//    {
//        if (player == null) return;

//        // �÷��̾ ���� ȸ��
//        Vector3 directionToPlayer = (player.position - transform.position).normalized;
//        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
//        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

//        // ���� ��ٿ� Ȯ��
//        if (Time.time >= lastAttackTime + attackCooldown)
//        {
//            PerformAttack();
//        }

//        // ���� ������ ������� Ȯ��
//        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
//        if (distanceToPlayer > attackRange * 1.2f) // �ణ�� ������ ��
//        {
//            ChangeState(AIState.Chase);
//        }
//    }

//    void PerformAttack()
//    {
//        lastAttackTime = Time.time;

//        if (animator != null)
//        {
//            animator.SetTrigger(attackHash);
//        }

//        // EnemyController�� ���� �޼��� ȣ��
//        if (enemyController != null)
//        {
//            enemyController.AttackPlayer();
//        }
//    }

//    void UpdateHit()
//    {
//        // �ǰ� ���� - �ִϸ��̼��� ���� ������ ���
//        // �ִϸ��̼� �̺�Ʈ�� ���� ��ȯ�� ����
//    }

//    void UpdateDead()
//    {
//        // ��� ���� - �ƹ��͵� ���� ����
//    }

//    // �ܺο��� ȣ���� �� �ִ� �޼����
//    public void OnHit()
//    {
//        if (currentState != AIState.Dead)
//        {
//            ChangeState(AIState.Hit);

//            // �÷��̾ Ÿ������ ���� (�ݰ�)
//            hasTarget = true;
//            loseTargetTimer = 0f;
//        }
//    }

//    public void OnDeath()
//    {
//        ChangeState(AIState.Dead);
//    }

//    // �ִϸ��̼� �̺�Ʈ�� ȣ��� �޼����
//    public void OnHitAnimationEnd()
//    {
//        if (hasTarget)
//            ChangeState(AIState.Chase);
//        else
//            ChangeState(usePatrol ? AIState.Patrol : AIState.Idle);
//    }

//    public void OnAttackAnimationEnd()
//    {
//        // ���� �ִϸ��̼� �Ϸ� �� ���� ����
//        if (hasTarget)
//        {
//            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
//            if (distanceToPlayer <= attackRange)
//                ChangeState(AIState.Attack); // ��� ����
//            else
//                ChangeState(AIState.Chase); // ����
//        }
//        else
//        {
//            ChangeState(usePatrol ? AIState.Patrol : AIState.Idle);
//        }
//    }

//    // ����׿� �����
//    void OnDrawGizmosSelected()
//    {
//        // ���� ����
//        Gizmos.color = Color.yellow;
//        Gizmos.DrawWireSphere(transform.position, detectionRange);

//        // ���� ����
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(transform.position, attackRange);

//        // ��Ʈ�� ���
//        if (usePatrol && patrolPoints != null && patrolPoints.Length > 1)
//        {
//            Gizmos.color = Color.blue;
//            for (int i = 0; i < patrolPoints.Length; i++)
//            {
//                if (patrolPoints[i] != null)
//                {
//                    Gizmos.DrawSphere(patrolPoints[i].position, 0.3f);

//                    // ���� ����Ʈ�� ���ἱ
//                    int nextIndex = (i + 1) % patrolPoints.Length;
//                    if (patrolPoints[nextIndex] != null)
//                    {
//                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[nextIndex].position);
//                    }
//                }
//            }
//        }
//    }
//}