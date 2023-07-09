using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashEffect : MonoBehaviour
{
  public float blinkIntensity = 20f;
  public float blinkDuration = 0.15f;
  float blinkTimer;
  float intensity;
  Renderer[] renderers;
  Renderer[] originalRenderers;

  void Awake()
  {
    renderers = GetComponentsInChildren<Renderer>();
    originalRenderers = renderers;
  }

  public void OnHitFlash()
  {
    blinkTimer = blinkDuration;
    StartCoroutine(FlashWhite(blinkTimer));
    renderers = originalRenderers;
  }

  private IEnumerator FlashWhite(float duration)
  {
    while (blinkTimer > Time.deltaTime)
    {
      blinkTimer -= Time.deltaTime;
      float lerp = Mathf.Clamp01(blinkTimer / blinkDuration);
      intensity = (lerp * blinkIntensity) + 1.0f;

      foreach (var r in renderers)
      {
        foreach (Material m in r.materials)
        {
          m.color = Color.white * intensity;
        }
      }
      yield return null;
    }
  }
}
