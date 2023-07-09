using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private RectTransform healthBarFillRectTransform;

    private void Update()
    {
        float currentHealth = playerHealth.GetCurrentHealth();
        float maxHealth = playerHealth.GetMaxHealth();
        UpdateHealthBar(currentHealth, maxHealth);
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        float fillPercentage = currentHealth / maxHealth;
        healthBarFillRectTransform.anchorMax = new Vector2(fillPercentage, 1);
    }
}
