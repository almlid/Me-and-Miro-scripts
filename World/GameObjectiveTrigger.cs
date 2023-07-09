using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectiveTrigger : MonoBehaviour
{
  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.tag == "Player")
    {
      Player player = other.GetComponent<Player>();
      player.UpdateOnObjectiveChange(true);
      Destroy(this.gameObject);
    }
  }
}
