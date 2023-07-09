using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour, IHealth
{
  public static PlayerHealth Instance { get; private set; }
  public float maxHealth = 100f;
  public float currentHealth = 100f;
  public event Action<float> OnHealthChange;
  PlayerAttributes attributes;
  Player player;
  public AudioClip restoreHealthSFX;
  public AudioSource audioSource;

  private FlashEffect onHitFlashEffect;

  public void Start()
  {
    attributes = GetComponent<PlayerAttributes>();
    maxHealth = attributes.maxHealth;
    currentHealth = attributes.currentHealth;
    player = GetComponent<Player>();
    onHitFlashEffect = gameObject.AddComponent<FlashEffect>();
    audioSource = GetComponent<AudioSource>();
  }

  public void UpdateHealth(float newHealthAmount)
  {
    currentHealth = newHealthAmount;
    if (currentHealth > maxHealth)
    {
      currentHealth = maxHealth;
    }

    if (newHealthAmount <= 0)
    {
      player.Die();
    }
    OnHealthChange?.Invoke(newHealthAmount);
  }

  public void TakeDamage(float damage)
  {
    UpdateHealth(currentHealth -= damage);
    Player.Instance.UpdateInCombatMode(true);
    onHitFlashEffect.OnHitFlash();
  }

  public void RestoreHealth(float amount)
  {
    UpdateHealth(currentHealth += amount);
    audioSource.PlayOneShot(restoreHealthSFX, 1f);
  }

  public float GetCurrentHealth()
  {
    return currentHealth;
  }
  public float GetMaxHealth()
  {
    return maxHealth;
  }

  public void ResetHealth()
  {
    currentHealth = maxHealth;
  }
}
