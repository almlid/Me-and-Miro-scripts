using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Weapon : MonoBehaviour
{
  public event Action OnPrimaryAttack;
  public float damage = 10f;
  public float range = 100f;
  public WeaponType weaponType;
  protected float attackCooldown = 1f;

  public Vector3 position_drawn;
  public Vector3 rotationEuler_drawn;
  public Vector3 position_sheathed;
  public Vector3 rotationEuler_sheathed;

  protected bool canAttack = true;

  public virtual void Use()
  {
    OnPrimaryAttack?.Invoke();
  }

  public abstract void EndAttack();
  public virtual void SetAim(Transform transform) { }

  public WeaponType GetWeaponType()
  {
    return weaponType;
  }
}
