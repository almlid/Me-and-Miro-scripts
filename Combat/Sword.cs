using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sword : Weapon
{
  public Collider swordCollider;
  public string collisionTag;
  public GameObject onHitPrefab;

  private void Start()
  {
    weaponType = WeaponType.Sword;
    swordCollider.enabled = false;
  }

  public override void Use()
  {
    if (canAttack)
    {
      swordCollider.enabled = true;
      base.Use();
      Invoke("EndAttack", attackCooldown / 2);
    }
  }

  public override void EndAttack()
  {
    swordCollider.enabled = false;
    canAttack = false;
    Invoke("ResetAttack", attackCooldown);
  }

  private void ResetAttack()
  {
    canAttack = true;
  }
  public override void SetAim(Transform transform) { }

  void OnTriggerEnter(Collider collider)
  {
    if (collider.gameObject.tag == collisionTag && collider != null)
    {
      collider.gameObject.GetComponent<IHealth>().TakeDamage(base.damage);
      Instantiate(onHitPrefab, this.transform.position, this.transform.rotation);
    }
  }
}
