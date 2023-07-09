using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleSitBehavior : StateMachineBehaviour
{
  private float timer = 0f;
  public float sitTime = 15f;
  private string blendValue = "SitAnimation";

  override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    if (animator.GetFloat("Speed") <= 0.1f)
    {
      timer += Time.deltaTime;

      if (timer >= sitTime)
      {
        animator.SetFloat(blendValue, 1.0f);
        timer = 0f;
      }
    }
    else
    {
      timer = 0f;
      animator.SetFloat(blendValue, 0f);
    }
  }
}
