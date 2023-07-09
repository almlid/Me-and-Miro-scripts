
using UnityEngine;

public class TargetEnemyInLineOfSight : MonoBehaviour
{
  public LayerMask enemyLayerMask;
  public float viewDistance = 100f;
  private GameObject currentTarget;
  private Outline currentTargetOutline;
  private Camera mainCamera;

  private void Start()
  {
    mainCamera = Camera.main;
  }

  private void Update()
  {
    Ray ray;
    Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0f);
    ray = mainCamera.ScreenPointToRay(screenCenter);

    RaycastHit hit;

    if (Physics.Raycast(ray, out hit, viewDistance))
    {
      if (hit.collider.gameObject != currentTarget)
      {
        if (currentTargetOutline != null)
        {
          currentTargetOutline.enabled = false;
        }
        currentTarget = hit.collider.gameObject;
      }

      currentTargetOutline = hit.collider.gameObject.GetComponent<Outline>();
      if (currentTargetOutline != null)
      {
        currentTargetOutline.enabled = true;
      }
    }
    else
    {
      if (currentTargetOutline != null)
      {
        currentTargetOutline.enabled = false;
      }
      currentTarget = null;
    }
  }
}