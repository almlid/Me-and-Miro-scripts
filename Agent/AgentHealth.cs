
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class AgentHealth : MonoBehaviour, IHealth
{
  private float maxHealth = 100f;
  private float currentHealth = 100f;
  private AgentStateMachine agentStateMachine;
  AgentAttributes agentAttributes;
  private FlashEffect onHitFlashEffect;
  public event Action<float> OnHealthChange;
  AudioSource audioSource;
  public AudioClip onHitSFX;

  public void Awake()
  {
    agentStateMachine = GetComponent<AgentStateMachine>();
    agentAttributes = GetComponent<AgentAttributes>();
    maxHealth = agentAttributes.maxHealth;
    currentHealth = agentAttributes.maxHealth;
    onHitFlashEffect = gameObject.AddComponent<FlashEffect>();
    audioSource = GetComponent<AudioSource>();
  }

  public void TakeDamage(float damage)
  {
    currentHealth -= damage;
    onHitFlashEffect.OnHitFlash();
    OnHealthChange?.Invoke(currentHealth);
    audioSource.PlayOneShot(onHitSFX);
    if (currentHealth <= 0 && agentStateMachine != null && agentStateMachine.isAlive)
    {
      Die();
    }
    if (agentStateMachine.state == AgentStateMachine.AgentState.Idle || agentStateMachine.state == AgentStateMachine.AgentState.Move)
    {
      agentStateMachine.ChasePlayer();
    }
  }

  public float GetCurrentHealth()
  {
    return currentHealth;
  }
  public float GetMaxHealth()
  {
    return maxHealth;
  }

  protected void Die()
  {
    if (agentStateMachine != null)
    {
      agentStateMachine.UpdateState(AgentStateMachine.AgentState.Dead);
    }
  }

  public void RestoreHealth(float amount)
  {
    currentHealth += amount;
  }
}
