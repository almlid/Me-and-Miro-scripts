using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Claws : MonoBehaviour
{
  private string collisionTag = "Player";
  private AgentAttributes attributes;
  private float damage;
  public AudioSource audioSource;
  public GameObject onHitPrefab;
  void Awake()
  {
    attributes = GetComponentInParent<AgentAttributes>();
    audioSource = GetComponent<AudioSource>();
    if (attributes)
    {
      damage = attributes.attackDamage;
    }
  }

  void OnTriggerEnter(Collider collider)
  {
    if (collider.gameObject.tag == collisionTag && collider != null)
    {
      collider.gameObject.GetComponent<IHealth>().TakeDamage(damage);
      GameObject newOnHit = Instantiate(onHitPrefab, this.transform.position, this.transform.rotation);
      newOnHit.transform.parent = collider.transform;
      if (audioSource)
      {
        audioSource.Play();
      }
    }
  }
}
