using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishGameTrigger : MonoBehaviour
{
  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.tag == "Player")
    {
      Player player = other.GetComponent<Player>();
      if (player.gotObjective == true)
      {
        GameManager.Instance.UpdateGameState(GameManager.GameState.Victory);
      }
    }
  }
}
