using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Projectile : MonoBehaviour
{
  public int speed;
  public float damage = 5f;
  public float lifespan = 1000f;
  private float timer = 0f;
  public string collisionTag;
  public Vector3 hitPoint;
  public GameObject dirt;
  public GameObject blood;

  void Start()
  {
    this.GetComponent<Rigidbody>().AddForce((this.transform.forward).normalized * speed);
    StartCoroutine(DestroyAfterTime());
  }

  void OnCollisionEnter(Collision collision)
  {
    if (collision.gameObject.tag == collisionTag)
    {
      collision.gameObject.GetComponent<IHealth>().TakeDamage(damage);
      GameObject newBlood = Instantiate(blood, this.transform.position, this.transform.rotation);
      newBlood.transform.parent = collision.transform;
      Destroy(this.gameObject);
    }
    else
    {
      Instantiate(dirt, this.transform.position, this.transform.rotation);
      Destroy(this.gameObject);
    }

    Destroy(this.gameObject);
  }

  IEnumerator DestroyAfterTime()
  {
    yield return new WaitForSeconds(10f);
    Destroy(this.gameObject);
  }


}