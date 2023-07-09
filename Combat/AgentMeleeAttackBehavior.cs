using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AgentMeleeAttackBehavior : MonoBehaviour, IAgentAttackBehavior
{
  public float primaryAttackDuration = 1f;
  public float secondaryAttackDuration = 1.5f;
  public float secondaryAttackCooldown = 5f;
  public Collider[] primaryAttackColliders;
  public Collider[] secondaryAttackColliders;
  public bool canAttack = true;
  float lastTimeRun;
  public Animator animator;
  public AudioSource audioSource;

  void Awake()
  {
    animator = GetComponent<Animator>();
    audioSource = GetComponent<AudioSource>();
    EnableColliders(false, primaryAttackColliders);
  }

  public void Attack()
  {
    if (canAttack)
      Debug.Log("Can Attack"); // Dont remove! Bugs the animator??
    {
      if (HasElapsed(secondaryAttackCooldown))
      {
        SecondaryAttack();
      }
      else
      {
        PrimaryAttack();
      }
    }
  }

  private bool HasElapsed(float seconds)
  {
    if (Time.time - lastTimeRun >= seconds)
    {
      lastTimeRun = Time.time;
      return true;
    }
    return false;
  }


  public void PrimaryAttack()
  {
    animator.SetTrigger("Attack");
    animator.SetFloat("AttackType", 1);
    EnableColliders(true, primaryAttackColliders);
    StartCoroutine(EndAttack(primaryAttackDuration, primaryAttackColliders));
  }

  public void SecondaryAttack()
  {
    animator.SetTrigger("Attack");
    animator.SetFloat("AttackType", 2);
    EnableColliders(true, secondaryAttackColliders);
    StartCoroutine(EndAttack(secondaryAttackDuration, secondaryAttackColliders));
  }


  private void EnableColliders(bool flag, Collider[] colliders)
  {
    foreach (var collider in colliders)
    {
      collider.enabled = flag;
    }
  }

  private void ResetAttack()
  {
    canAttack = true;
  }

  IEnumerator EndAttack(float delay, Collider[] colliders)
  {
    canAttack = false;
    yield return new WaitForSeconds(delay);
    EnableColliders(false, colliders);
    animator.ResetTrigger("Attack");
    animator.SetFloat("AttackType", 0);
    ResetAttack();
  }
  public bool CanAttack()
  {
    return canAttack;
  }
}