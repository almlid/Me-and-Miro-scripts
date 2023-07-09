using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IHealth
{
  abstract float GetCurrentHealth();
  abstract float GetMaxHealth();
  void TakeDamage(float damage);
  void RestoreHealth(float amount);
  public event Action<float> OnHealthChange;
}
