using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class AgentStateMachine : MonoBehaviour
{
  public bool isAlive = true;
  public enum AgentState
  {
    Idle,
    Move,
    Chase,
    Attack,
    Dead
  }
  public enum MovementState
  {
    Walk,
    Run,
  }
  public AgentState state = AgentState.Idle;
  public MovementState movementState = MovementState.Walk;
  public event Action<AgentState> OnStateChange;
  public GameObject[] inventory;
  private bool isHandlingState = false;


  public void UpdateState(AgentState newState)
  {
    state = newState;

    switch (newState)
    {
      case AgentState.Idle:
        HandleIdleState();
        break;
      case AgentState.Move:
        HandleMoveState();
        SetMovementSpeed(MovementState.Walk);
        break;
      case AgentState.Chase:
        HandleChaseState();
        SetMovementSpeed(MovementState.Run);
        break;
      case AgentState.Attack:
        HandleAttackState();
        break;
      case AgentState.Dead:
        HandleDeadState();
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
    }
    OnStateChange?.Invoke(newState);
  }

  private Transform playerTransform;
  private AgentHealth health;
  private NavMeshAgent agent;
  private Animator animator;
  private AgentAttributes attributes;
  private IAgentAttackBehavior attackBehavior;
  private IAgentLocomotionBehavior locomotionBehavior;
  AudioSource audioSource;


  protected bool canAttack = true;
  public Vector3 homePosition;

  private void Start()
  {
    playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    health = GetComponent<AgentHealth>();
    agent = GetComponent<NavMeshAgent>();
    animator = GetComponent<Animator>();
    attributes = GetComponent<AgentAttributes>();
    isAlive = true;
    locomotionBehavior = GetComponent<IAgentLocomotionBehavior>();
    attackBehavior = gameObject.GetComponent<IAgentAttackBehavior>();
    homePosition = attributes.originalPosition;
    audioSource = GetComponent<AudioSource>();

    UpdateState(AgentState.Idle);
    SetMovementSpeed(MovementState.Walk);
  }

  private void Update()
  {
    if (isAlive)
    {
      UpdateState(state);
    }
  }

  public void HandleIdleState()
  {
    if (isHandlingState)
    {
      return;
    }
    isHandlingState = true;

    if (CanSeePlayer(attributes.detectionRange))
    {
      UpdateState(AgentState.Chase);
    }
    else
    {
      UpdateState(AgentState.Move);
    }
    isHandlingState = false;
  }

  public void HandleMoveState()
  {
    if (isHandlingState)
    {
      return;
    }
    isHandlingState = true;

    if (CanSeePlayer(attributes.detectionRange))
    {
      DecideToChaseOrGiveUp();
    }

    else if (agent.velocity.magnitude < 0.1)
    {
      locomotionBehavior.Move();
    }
    isHandlingState = false;
  }

  public void DecideToChaseOrGiveUp()
  {
    float distanceFromHome = Vector3.Distance(transform.position, homePosition);
    float distanceToTarget = Vector3.Distance(agent.destination, transform.position);

    if (distanceFromHome > attributes.maxDistanceFromHome && distanceToTarget > attributes.chaseDistance)
    {
      agent.SetDestination(homePosition);
    }
    else
    {
      UpdateState(AgentState.Chase);
    }
  }

  public void HandleChaseState()
  {
    if (isHandlingState)
    {
      return;
    }
    isHandlingState = true;
    if (CanSeePlayer(attributes.attackRange))
    {
      UpdateState(AgentState.Attack);
    }
    else if (!CanSeePlayer(attributes.detectionRange))
    {
      UpdateState(AgentState.Move);
    }
    else
    {
      ChasePlayer();
    }
    isHandlingState = false;
  }

  public void HandleAttackState()
  {
    if (isHandlingState)
    {
      return;
    }
    isHandlingState = true;
    if (CanSeePlayer(attributes.attackRange))
    {
      Vector3 directionToPlayer = playerTransform.position - transform.position;
      Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
      transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2);
      AttackPlayer();
    }
    else
    {
      isHandlingState = false;
      UpdateState(AgentState.Chase);
    }
    isHandlingState = false;
  }

  public void HandleDeadState()
  {
    isAlive = false;
    StartCoroutine(DropLoot(inventory));
    agent.enabled = (false);
    animator.SetTrigger("Die");
    GameManager.Instance.EnemyDied(gameObject);
    audioSource.Play();
  }

  IEnumerator DropLoot(GameObject[] loot)
  {
    yield return new WaitForSeconds(2);
    foreach (var item in loot)
    {
      Instantiate(item, agent.transform.position + item.transform.position, Quaternion.identity, null);
    }
  }

  private bool CanSeePlayer(float distance)
  {
    RaycastHit hit;
    Vector3 rayDirection = playerTransform.position - transform.position;
    Vector3 rayStart = transform.position + Vector3.up * 0.5f;

    if (Physics.Raycast(rayStart, rayDirection, out hit, distance))
    {
      if (hit.transform == playerTransform)
      {
        return true;
      }
    }
    return false;
  }

  public void ChasePlayer()
  {
    agent.destination = playerTransform.position;
    StartCoroutine(StartDecidingToChaseOrGiveUp());
  }

  IEnumerator StartDecidingToChaseOrGiveUp()
  {
    yield return new WaitForSeconds(4);
    DecideToChaseOrGiveUp();
  }

  private void AttackPlayer()
  {
    attackBehavior.Attack();
  }

  public void SetMovementSpeed(MovementState newState)
  {
    switch (newState)
    {
      case MovementState.Walk:
        agent.speed = attributes.walkSpeed;
        break;
      case MovementState.Run:
        agent.speed = attributes.runSpeed;
        break;
    }
  }
}
