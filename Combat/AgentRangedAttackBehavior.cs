using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentRangedAttackBehavior : MonoBehaviour, IAgentAttackBehavior
{
  public float primaryAttackDuration = 1f;
  public float secondaryAttackDuration = 1.5f;
  public float secondaryAttackCooldown = 8f;
  public Weapon[] primaryAttackWeapons;
  public Weapon[] secondaryAttackWeapons;
  public bool canAttack = true;
  float lastTimeRun;
  public Animator animator;

  void Awake()
  {
    animator = GetComponent<Animator>();
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
    foreach (var weapon in primaryAttackWeapons)
    {
      weapon.Use();
    }
    StartCoroutine(EndAttack(primaryAttackDuration, primaryAttackWeapons));
  }

  public void SecondaryAttack()
  {
    animator.SetTrigger("Attack");
    animator.SetFloat("AttackType", 2);
    foreach (var weapon in secondaryAttackWeapons)
    {
      weapon.Use();
    }
    StartCoroutine(EndAttack(secondaryAttackDuration, secondaryAttackWeapons));
  }

  private void ResetAttack()
  {
    canAttack = true;
  }

  IEnumerator EndAttack(float delay, Weapon[] weapons)
  {
    canAttack = false;
    yield return new WaitForSeconds(delay);
    animator.ResetTrigger("Attack");
    animator.SetFloat("AttackType", 0);
    ResetAttack();
  }

  public bool CanAttack()
  {
    return canAttack;
  }
}
