using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikyVines : MonoBehaviour
{
  public float damage = 100;
  private void OnTriggerStay(Collider other)
  {
    if (other.gameObject.tag == "Player")
    {
      other.GetComponent<IHealth>().TakeDamage(damage);
    }
  }
}
