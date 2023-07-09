using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
  public Transform target;
  public Image foregroundImage;
  public Image backgroundImage;
  public Vector3 offset;
  public IHealth health;
  public float maxHealth = 100;
  public Camera mainCamera;


  void Awake()
  {
    health = target.GetComponentInChildren<IHealth>();
    maxHealth = health.GetMaxHealth();
    health.OnHealthChange += UpdateHealthBar;
    mainCamera = Camera.main;
  }

  void Destroy()
  {
    health.OnHealthChange -= UpdateHealthBar;
  }

  void LateUpdate()
  {
    RaycastHit hit;
    Vector3 direction = (target.position - mainCamera.transform.position).normalized;
    if (Physics.Raycast(mainCamera.transform.position, direction, out hit))
    {
      if (hit.collider.gameObject == target.gameObject)
      {
        bool isBehind = Vector3.Dot(direction, mainCamera.transform.forward) <= 0.5f;
        foregroundImage.enabled = !isBehind;
        backgroundImage.enabled = !isBehind;
        transform.position = mainCamera.WorldToScreenPoint(target.position + offset);
      }
      else
      {
        foregroundImage.enabled = false;
        backgroundImage.enabled = false;
      }
    }
    else
    {
      bool isBehind = Vector3.Dot(direction, mainCamera.transform.forward) <= 0.5f;
      foregroundImage.enabled = !isBehind;
      backgroundImage.enabled = !isBehind;
      transform.position = mainCamera.WorldToScreenPoint(target.position + offset);
    }
  }

  void UpdateHealthBar(float value)
  {
    float newHealth = health.GetCurrentHealth() / maxHealth;
    SetHealthBarPercentage(newHealth);
  }

  public void SetHealthBarPercentage(float percentage)
  {
    float parentWidth = GetComponent<RectTransform>().rect.width;
    float width = parentWidth * percentage;
    foregroundImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
  }

}
