using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class CompanionStateMachine : MonoBehaviour
{

  public enum CompanionState
  {
    Idle,
    Follow,
    Chase,
    Attack,
    Dead
  }

  public enum CompanionCommandState
  {
    Passive,
    Aggressive,
    Stay,
  }

  public CompanionState state;
  public static event Action<CompanionState> OnStateChange;

  public void UpdateState(CompanionState newState)
  {
    state = newState;
    switch (newState)
    {
      case CompanionState.Idle:
        HandleIdleState();
        break;
      case CompanionState.Follow:
        HandleFollowState();
        break;
      case CompanionState.Chase:
        HandleChaseState();
        break;
      case CompanionState.Attack:
        HandleAttackState();
        break;
      case CompanionState.Dead:
        HandleDeadState();
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
    }
    OnStateChange?.Invoke(newState);
  }


  public Transform playerTransform;
  public float maxTime = 1.0f;
  public float maxDistance = 1.0f;
  public float slowDownDistance = 5.0f;
  public float stoppingDistance = 2.0f;
  float timer = 0.0f;
  public AgentAttributes attributes;
  private NavMeshAgent agent;
  private Animator animator;
  private bool isAlive;

  private Transform enemyTransform;

  public Weapon weapon;

  void Awake()
  {
    GameObject playerObject = GameObject.Find("Player");
    if (playerObject != null)
    {
      playerTransform = playerObject.transform;
    }
    weapon = GetComponentInChildren<Weapon>();
  }
  void Start()
  {
    attributes = GetComponent<AgentAttributes>();
    agent = GetComponent<NavMeshAgent>();
    animator = GetComponent<Animator>();
    isAlive = true;
    UpdateState(CompanionState.Idle);
  }

  void Update()
  {
    if (isAlive && state != null)
    {
      UpdateState(state);
    }
  }

  public void HandleIdleState()
  {
    float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
    if (distanceToPlayer > stoppingDistance * 2)
    {
      UpdateState(CompanionState.Follow);
    }
    if (CanSeeEnemy(attributes.detectionRange))
    {
      UpdateState(CompanionState.Chase);
    }
  }

  public float CheckDistanceToPlayer()
  {
    float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
    return distanceToPlayer;
  }

  public void HandleFollowState()
  {
    timer -= Time.deltaTime;
    if (timer < 0.0f)
    {
      float sqDistance = (playerTransform.position - agent.destination).sqrMagnitude;
      if (CheckDistanceToPlayer() > maxDistance * maxDistance)
      {
        agent.SetDestination(playerTransform.position);
      }
      timer = maxTime;
    }
    animator.SetFloat("Speed", agent.velocity.magnitude);

    if (CheckDistanceToPlayer() > slowDownDistance)
    {
      agent.speed = attributes.runSpeed;
    }
    else
    {
      agent.speed = attributes.walkSpeed;
    }

    if (agent.velocity.magnitude <= 0.1 && CheckDistanceToPlayer() <= stoppingDistance * 1.2)
    {
      UpdateState(CompanionState.Idle);
    }
  }

  public void HandleChaseState()
  {
    if (CanSeeEnemy(attributes.attackRange))
    {
      UpdateState(CompanionState.Attack);
    }
    else if (!CanSeeEnemy(attributes.detectionRange))
    {
      UpdateState(CompanionState.Follow);
    }
    else
    {
      ChaseEnemy();
    }
  }

  public void HandleAttackState()
  {
    if (CheckDistanceToPlayer() > attributes.chaseDistance)
    {
      UpdateState(CompanionState.Follow);
    }
    else if (CanSeeEnemy(attributes.attackRange) && enemyTransform)
    {
      Vector3 directionToEnemy = enemyTransform.position - transform.position;
      Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy);
      transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2);
      AttackEnemy();
    }
    else
    {
      UpdateState(CompanionState.Chase);
    }
  }

  public void HandleDeadState()
  {
    isAlive = false;
  }

  public bool CanSeeEnemy(float distance)
  {
    Collider[] hitColliders = Physics.OverlapSphere(transform.position, attributes.detectionRange);
    foreach (Collider collider in hitColliders)
    {
      Vector3 directionToEnemy = collider.transform.position - transform.position;
      if (collider.tag == "Enemy" && Vector3.Distance(collider.transform.position, transform.position) <= attributes.chaseDistance)
      {
        enemyTransform = collider.transform;
        return true;
      }
    }
    enemyTransform = null;
    return false;
  }

  public void ChaseEnemy()
  {
    if (Vector3.Distance(playerTransform.position, transform.position) > attributes.chaseDistance)
    {
      UpdateState(CompanionState.Follow);
    }
    else if (enemyTransform)
    {
      agent.destination = enemyTransform.position;
    }
  }

  public void AttackEnemy()
  {
    weapon.Use();
  }
}


