using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentAttributes : MonoBehaviour
{
  public Vector3 originalPosition;
  public float maxDistanceFromHome = 10;
  public float maxHealth = 80;
  public float walkSpeed = 2.0f;
  public float runSpeed = 4.0f;
  public float detectionRange = 10f;
  public float attackRange = 1f;
  public float attackDamage = 10f;
  public float respawnTime;
  public float chaseDistance;
  public float stoppingDistance;
  void Awake()
  {
    originalPosition = transform.localPosition;
  }
}
