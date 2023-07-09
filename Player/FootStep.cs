using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStep : MonoBehaviour
{
  [SerializeField] private AudioSource audioSource;
  [SerializeField] private AudioClip walkOnFloor;
  [SerializeField] private AudioClip[] walkOnGround;

  private void Awake()
  {
    audioSource = GetComponent<AudioSource>();
  }

  private void OnCollisionEnter(Collision collision)
  {
    switch (collision.gameObject.layer)
    {
      case 3:
        if (walkOnGround.Length > 0)
        {
          audioSource.PlayOneShot(walkOnGround[Random.Range(0, walkOnGround.Length)]);
        }
        break;
      case 8:
        audioSource.PlayOneShot(walkOnFloor);
        break;
    }
  }
}