using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
  Animator animator;
  AudioSource audioSource;
  public AudioClip openDoorSFX, closeDoorSFX;
  private bool isTriggered;

  void Start()

  {
    animator = GetComponent<Animator>();
    audioSource = GetComponent<AudioSource>();
    isTriggered = false;
  }

  private void OnTriggerEnter(Collider other)
  {
    if (!isTriggered)
    {
      audioSource.PlayOneShot(openDoorSFX);
    }
    animator.SetBool("character_nearby", true);
    isTriggered = true;
  }

  private void OnTriggerExit(Collider other)
  {
    if (isTriggered)
    {
      audioSource.PlayOneShot(closeDoorSFX);
    }
    animator.SetBool("character_nearby", false);
    audioSource.PlayOneShot(closeDoorSFX);
    isTriggered = false;
  }
}
