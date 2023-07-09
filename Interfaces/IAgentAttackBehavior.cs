using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IAgentAttackBehavior
{
  void Attack();
  void PrimaryAttack();
  void SecondaryAttack();
  bool CanAttack();
}
