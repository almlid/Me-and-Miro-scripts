using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
  public float healthAmount;

  private void OnTriggerEnter(Collider collider)
  {
    if (collider.CompareTag("Player"))
    {
      PlayerHealth playerHealth = collider.GetComponent<PlayerHealth>();
      if (playerHealth != null)
      {
        playerHealth.RestoreHealth(healthAmount);
        Destroy(gameObject);
      }

    }
  }
}
