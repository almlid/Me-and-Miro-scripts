using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentAnimationStateController : MonoBehaviour
{
  private Animator animator;
  private AgentStateMachine stateMachine;
  private float movementSpeed;
  private float animSmoothingSpeed = 3f;
  private float horizontalSpeed, verticalSpeed;
  private NavMeshAgent agent;
  private string p_speed = "Speed";

  void Awake()
  {
    agent = GetComponent<NavMeshAgent>();
    stateMachine = GetComponent<AgentStateMachine>();
    stateMachine.OnStateChange += AnimateOnStateChange;
    animator = GetComponent<Animator>();
  }

  private void OnDestroy()
  {
    stateMachine.OnStateChange -= AnimateOnStateChange;
  }

  void Update()
  {
    movementSpeed = agent.velocity.magnitude;

    if (movementSpeed > 0.1f)
    {
      animator.SetFloat(p_speed, Mathf.Lerp(animator.GetFloat(p_speed), movementSpeed, Time.deltaTime * animSmoothingSpeed));
    }
    else
    {
      animator.SetFloat(p_speed, Mathf.Lerp(animator.GetFloat(p_speed), 0.1f, Time.deltaTime * animSmoothingSpeed));
    }
  }

  public void AnimateOnStateChange(AgentStateMachine.AgentState state)
  {
    if (state == AgentStateMachine.AgentState.Dead)
    {
      animator.SetTrigger("Die");
    }
  }
}
