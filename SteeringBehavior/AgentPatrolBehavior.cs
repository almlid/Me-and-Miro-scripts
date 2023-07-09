using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class AgentPatrolBehavior : MonoBehaviour, IAgentLocomotionBehavior
{
  public List<Transform> waypoints;
  public int currentWaypointIndex = 0;
  public float waypointThreshold = 1f;
  public float randomOffsetRange = 2f;
  private NavMeshAgent navMeshAgent;
  private AgentStateMachine stateMachine;

  void Awake()
  {
    stateMachine = GetComponent<AgentStateMachine>();
    navMeshAgent = GetComponent<NavMeshAgent>();
    if (waypoints.Count > 0)
    {
      navMeshAgent.SetDestination(GetRandomOffsetWaypoint(waypoints[currentWaypointIndex].position));
    }
  }

  public void Move()
  {
    if (Vector3.Distance(transform.position, navMeshAgent.destination) <= waypointThreshold)
    {
      GoToNextWaypoint();
    }
  }

  void GoToNextWaypoint()
  {
    if (waypoints.Count > 0 && waypoints != null)
    {
      currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
      navMeshAgent.SetDestination(GetRandomOffsetWaypoint(waypoints[currentWaypointIndex].position));
    }
  }

  Vector3 GetRandomOffsetWaypoint(Vector3 originalWaypoint)
  {
    Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-randomOffsetRange, randomOffsetRange), 0, UnityEngine.Random.Range(-randomOffsetRange, randomOffsetRange));
    Vector3 newWaypoint = originalWaypoint + randomOffset;
    stateMachine.homePosition = newWaypoint;
    NavMesh.SamplePosition(newWaypoint, out NavMeshHit hit, randomOffsetRange, NavMesh.AllAreas);
    return hit.position;
  }
}
